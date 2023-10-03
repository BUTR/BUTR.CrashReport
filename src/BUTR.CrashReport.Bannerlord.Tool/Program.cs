using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BUTR.CrashReport.Models;
using CommandLine;

namespace BUTR.CrashReport.Bannerlord.Tool;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		HtmlOptions? parsedOptions = null;

		try
		{
			var parser = Parser.Default
				.ParseArguments<HtmlOptions>(args);

			parser = await parser
				.WithParsedAsync<HtmlOptions>(async options =>
				{
					parsedOptions = options;

					var stream = Stream.Null;
					if (new Uri(options.ArchiveFile).IsFile)
						stream = File.OpenRead(options.ArchiveFile);
					else
						stream = await new HttpClient().GetStreamAsync(options.ArchiveFile);

					using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);

					await using var jsonStream = archive.GetEntry("crashreport.json")?.Open();
					await using var logsStream = archive.GetEntry("logs.json")?.Open();
					if (jsonStream is null) return;

					using var minidumpMemoryStream = new MemoryStream();
					await using var minidumpZipStream = new GZipStream(minidumpMemoryStream, CompressionMode.Compress, true);
					await using var minidumpStream = archive.GetEntry("minidump.dmp")?.Open();
					if (minidumpStream is not null) await minidumpStream.CopyToAsync(minidumpZipStream);
					var minidump = Convert.ToBase64String(minidumpMemoryStream.ToArray());

					using var saveFileMemoryStream = new MemoryStream();
					await using var saveFileZipStream = new GZipStream(saveFileMemoryStream, CompressionMode.Compress, true);
					await using var saveFileStream = archive.GetEntry("save.sav")?.Open();
					if (saveFileStream is not null) await saveFileStream.CopyToAsync(saveFileZipStream);
					var saveFile = Convert.ToBase64String(saveFileMemoryStream.ToArray());

					using var screenshotMemoryStream = new MemoryStream();
					await using var screenshotStream = archive.GetEntry("screenshot.bmp")?.Open();
					if (screenshotStream is not null) await screenshotStream.CopyToAsync(screenshotMemoryStream);
					var screenshot = Convert.ToBase64String(screenshotMemoryStream.ToArray());

					var crashReportJson = await new StreamReader(jsonStream).ReadToEndAsync();
					var crashReport = JsonSerializer.Deserialize<CrashReportModel>(crashReportJson, new JsonSerializerOptions()
					{
						Converters = { new JsonStringEnumConverter() }
					})!;
					var logs = logsStream is not null ? JsonSerializer.Deserialize<LogSource[]>(logsStream)! : Array.Empty<LogSource>();

					var html = CrashReportHtmlRenderer.AddData(CrashReportHtmlRenderer.Build(crashReport, logs), crashReportJson, minidump, saveFile, screenshot);

					var output = options.OutputFile ?? Path.Combine(Path.GetDirectoryName(options.ArchiveFile)!, $"{Path.GetFileNameWithoutExtension(options.ArchiveFile)}.html");
					await File.WriteAllTextAsync(output, html);
				});

			parser = parser
				.WithNotParsed(e => { Console.Write("INVALID COMMAND"); });

			if (parser.Errors.Any()) return 1;

			return 0;
		}
		catch (FileNotFoundException fex)
		{
			Console.WriteLine("The input file could not be found");
			if(parsedOptions is not null)
				Console.WriteLine($"File: '{parsedOptions.ArchiveFile}'");
			return 1;
		}
		catch (Exception ex)
		{
			Console.WriteLine("An error occurred:");
			Console.WriteLine(ex);
			return 1;
		}
	}
}