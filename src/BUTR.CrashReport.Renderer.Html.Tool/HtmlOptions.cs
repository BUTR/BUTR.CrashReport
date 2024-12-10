using CommandLine;

namespace BUTR.CrashReport.Renderer.Html.Tool;

[Verb("html", HelpText = "Converts the zip file to the HTML report")]
public class HtmlOptions
{
    [Option('i', "input", Required = true, HelpText = "The full path to the zip file to parse.")]
    public string ArchiveFile { get; set; } = null!;

    [Option('o', "output", Required = false, HelpText = "The file to output the result to. If not provided, will use the input folder aand teh same file name with a different extension")]
    public string? OutputFile { get; set; }

}