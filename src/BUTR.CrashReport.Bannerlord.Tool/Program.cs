using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BUTR.CrashReport.Models;
using CommandLine;

namespace BUTR.CrashReport.Bannerlord.Tool;

public class Program
{
	public static int Main(string[] args)
	{
		HtmlOptions? parsedOptions = null;

		try
		{
			var parseResult = Parser.Default.ParseArguments<HtmlOptions>(args)
				.WithParsed<HtmlOptions>(options =>
				{
					parsedOptions = options;

					using var fs = File.OpenRead(options.ArchiveFile);
					var archive = new ZipArchive(fs, ZipArchiveMode.Read);

					using var jsonStream = archive.GetEntry("crashreport.json")?.Open();
					using var logsStream = archive.GetEntry("logs.json")?.Open();
					if (jsonStream is null) return;

					using var minidumpMemoryStream = new MemoryStream();
					using var minidumpZipStream = new GZipStream(minidumpMemoryStream, CompressionMode.Compress, true);
					using var minidumpStream = archive.GetEntry("minidump.dmp")?.Open();
					minidumpStream?.CopyTo(minidumpZipStream);
					var minidump = Convert.ToBase64String(minidumpMemoryStream.ToArray());

					using var saveFileMemoryStream = new MemoryStream();
					using var saveFileZipStream = new GZipStream(saveFileMemoryStream, CompressionMode.Compress, true);
					using var saveFileStream = archive.GetEntry("save.sav")?.Open();
					saveFileStream?.CopyTo(saveFileZipStream);
					var saveFile = Convert.ToBase64String(saveFileMemoryStream.ToArray());

					using var screenshotMemoryStream = new MemoryStream();
					using var screenshotStream = archive.GetEntry("screenshot.bmp")?.Open();
					screenshotStream?.CopyTo(screenshotMemoryStream);
					var screenshot = Convert.ToBase64String(screenshotMemoryStream.ToArray());

					var crashReportJson = new StreamReader(jsonStream).ReadToEnd();
					var crashReport = JsonSerializer.Deserialize<CrashReportModel>(crashReportJson, new JsonSerializerOptions()
					{
						Converters = { new JsonStringEnumConverter() }
					})!;
					var logs = logsStream is not null ? JsonSerializer.Deserialize<LogSource[]>(logsStream)! : Array.Empty<LogSource>();

					var html = CrashReportHtmlRenderer.AddData(CrashReportHtmlRenderer.Build(crashReport, logs), crashReportJson, minidump, saveFile, screenshot);

					var output = options.OutputFile ?? Path.Combine(Path.GetDirectoryName(options.ArchiveFile)!, $"{Path.GetFileNameWithoutExtension(options.ArchiveFile)}.html");
					File.WriteAllText(output, html);
				});

			if (parseResult.Errors.Any()) return 1;

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