using BUTR.CrashReport.Models;

using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUTR.CrashReport.Renderer.WinForms;

public class ScriptObject
{
    private static async Task<bool> SetClipboardTextAsync(string text)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var staThread = new Thread(() =>
        {
            try
            {
                var dataObject = new DataObject();
                dataObject.SetText(text, TextDataFormat.Text);
                Clipboard.SetDataObject(dataObject, true, 10, 100);
                completionSource.SetResult(true);
            }
            catch (Exception)
            {
                completionSource.SetResult(false);
            }
        });
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        return await completionSource.Task;
    }

    [UsedImplicitly]
    public bool IncludeMiniDump { get; set; }
    [UsedImplicitly]
    public bool IncludeSaveFile { get; set; }
    [UsedImplicitly]
    public bool IncludeScreenshot { get; set; }

    private readonly Form _form;
    private readonly CrashReportModel _crashReport;
    private readonly ICollection<LogSourceModel> _logSources;
    private readonly ICrashReportRendererUtilities _crashReportRendererUtilities;
    private readonly string _reportInHtml;

    public ScriptObject(Form form, CrashReportModel crashReport, ICollection<LogSourceModel> logSources, ICrashReportRendererUtilities crashReportRendererUtilities, string reportInHtml)
    {
        _form = form;
        _crashReport = crashReport;
        _logSources = logSources;
        _crashReportRendererUtilities = crashReportRendererUtilities;
        _reportInHtml = reportInHtml;
    }

    [UsedImplicitly]
    public void SetIncludeMiniDump(bool value) => IncludeMiniDump = value;
    [UsedImplicitly]
    public void SetIncludeSaveFile(bool value) => IncludeSaveFile = value;
    [UsedImplicitly]
    public void SetIncludeScreenshot(bool value) => IncludeScreenshot = value;

    [UsedImplicitly]
    public async void CopyAsHTML()
    {
        if (!await SetClipboardTextAsync(_reportInHtml))
            MessageBox.Show("Failed to copy the HTML content to the clipboard!", "Error!");
    }

    [UsedImplicitly]
    public void UploadReport() => _crashReportRendererUtilities.Upload(_crashReport, _logSources);

    [UsedImplicitly]
    public void SaveReport() => _crashReportRendererUtilities.SaveAsHtml(_crashReport, _logSources, IncludeMiniDump, IncludeSaveFile, IncludeScreenshot);

    [UsedImplicitly]
    public void Close() => _form.Close();
}

public partial class CrashReportWinForms : Form
{
    // https://gist.github.com/eikes/2299607
    // Copyright: Eike Send http://eike.se/nd
    // License: MIT License
    private const string ScriptText = """
                                      if (!document.getElementsByClassName) {
                                        document.getElementsByClassName = function(search) {
                                          var d = document, elements, pattern, i, results = [];
                                          if (d.querySelectorAll) { // IE8
                                            return d.querySelectorAll("." + search);
                                          }
                                          if (d.evaluate) { // IE6, IE7
                                            pattern = ".//*[contains(concat(' ', @class, ' '), ' " + search + " ')]";
                                            elements = d.evaluate(pattern, d, null, 0, null);
                                            while ((i = elements.iterateNext())) {
                                              results.push(i);
                                            }
                                          } else {
                                            elements = d.getElementsByTagName("*");
                                            pattern = new RegExp("(^|\\s)" + search + "(\\s|$)");
                                            for (i = 0; i < elements.length; i++) {
                                              if ( pattern.test(elements[i].className) ) {
                                                results.push(elements[i]);
                                              }
                                            }
                                          }
                                          return results;
                                        }
                                      }
                                      function handleIncludeMiniDump(cb) {
                                        window.external.SetIncludeMiniDump(cb.checked);
                                      }
                                      function handleIncludeSaveFile(cb) {
                                        window.external.SetIncludeSaveFile(cb.checked);
                                      }
                                      function handleIncludeScreenshot(cb) {
                                        window.external.SetIncludeScreenshot(cb.checked);
                                      }
                                      """;

    private const string TableText = """
                                     <table style='width: 100%;'>
                                      <tbody>
                                      <tr>
                                        <td style='width: 50%;'>
                                          <h1>Intercepted an exception!</h1>
                                        </td>
                                        <td>
                                          <button style='float:right; margin-left:10px;' onclick='window.external.Close()'>Close Report</button>
                                          <button style='float:right; margin-left:10px;' onclick='window.external.UploadReport()'>Upload Report as a Permalink</button>
                                          <button style='float:right; margin-left:10px;' onclick='window.external.SaveReport()'>Save Report</button>
                                          <button style='float:right; margin-left:10px;' onclick='window.external.CopyAsHTML()'>Copy as HTML</button>
                                        </td>
                                      </tr>
                                      <tr>
                                        <td style='width: 50%;'>
                                        </td>
                                        <td>
                                          <input style='float:right;' type='checkbox' onclick='handleIncludeMiniDump(this);'>
                                          <label style='float:right; margin-left:10px;'>Include Mini Dump:</label>
                                          <input style='float:right;' type='checkbox' onclick='handleIncludeSaveFile(this);'>
                                          <label style='float:right; margin-left:10px;'>Include Latest Save File:</label>
                                          <input style='float:right;' type='checkbox' onclick='handleIncludeScreenshot(this);'>
                                          <label style='float:right; margin-left:10px;'>Include Screenshot:</label>
                                        </td>
                                      </tr>
                                      </tbody>
                                     </table>
                                     Clicking 'Close Report' will continue with the Game's error report mechanism.
                                     <hr/>
                                     """;

    private ICrashReportRendererUtilities CrashReportRendererUtilities { get; }
    private CrashReportModel CrashReport { get; }
    private ICollection<LogSourceModel> LogSources { get; }
    private string ReportInHtml { get; }

    public CrashReportWinForms(CrashReportModel crashReport, ICollection<LogSourceModel> logSources, ICrashReportRendererUtilities crashReportRendererUtilities)
    {
        CrashReportRendererUtilities = crashReportRendererUtilities;
        CrashReport = crashReport;
        LogSources = logSources;
        ReportInHtml = crashReportRendererUtilities.CopyAsHtml(crashReport, logSources);

        InitializeComponent();
        HtmlRender.ObjectForScripting = this;
        HtmlRender.DocumentText = ReportInHtml;
        HtmlRender.DocumentCompleted += (_, _) =>
        {
            if (HtmlRender.Document is not { Body: { } body } document) return;

            if (document.CreateElement("script") is { } scriptElement)
            {
                scriptElement.SetAttribute("text", ScriptText);
                body.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, scriptElement);
            }

            if (document.CreateElement("div") is { } tableElement && body.FirstChild is { } firstChild)
            {
                tableElement.InnerHtml = TableText;
                firstChild.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, tableElement);
            }
        };
    }

    protected override void OnLoad(EventArgs e)
    {
        HtmlRender.ObjectForScripting = new ScriptObject(this, CrashReport, LogSources, CrashReportRendererUtilities, ReportInHtml);
        base.OnLoad(e);
    }

    private void HtmlRender_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        if (e.Url.ToString() is { } uri && UriIsValid(uri))
        {
            e.Cancel = true;
            Process.Start(uri);
        }
    }

    private static bool UriIsValid(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}