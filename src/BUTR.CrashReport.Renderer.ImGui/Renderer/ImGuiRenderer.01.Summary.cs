using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.ImGui.Utils;
using BUTR.CrashReport.Memory;
using BUTR.CrashReport.Renderer.ImGui.Components;
using BUTR.CrashReport.Renderer.ImGui.Extensions;

namespace BUTR.CrashReport.Renderer.ImGui.Renderer;

partial class ImGuiRenderer
{
    // ReSharper disable once HeapView.ObjectAllocation
    protected static readonly LiteralSpan<byte>[] _operatingSystemTypeNames =
    [
        "Unknown\0"u8,         // Unknown
        "Windows\0"u8,         // Windows
        "Linux\0"u8,           // Linux
        "MacOS\0"u8,           // MacOS
        "Windows on Wine\0"u8, // WindowsWine
    ];
}

partial class ImGuiRenderer<TImGuiIORef, TImGuiViewportRef, TImDrawListRef, TImGuiStyleRef, TColorsRangeAccessorRef, TImGuiListClipperRef>
{
    private readonly LiteralSpan<byte> _filePickerHtmlId = "###FilePickerHtml\0"u8;
    private readonly LiteralSpan<byte> _filePickerZiplId = "###FilePickerZip\0"u8;

    private FilePickerOnSelected _onCreateHtmlSelected = default!;
    private FilePickerOnSelected _onCreateZipSelected = default!;

    private bool _addScreenshots;
    private bool _addLatestSave;
    private bool _addMiniDump;

    private void InitializeSummary()
    {
        _onCreateHtmlSelected = OnCreateHtmlSelected;
        _onCreateZipSelected = OnCreateZipSelected;
    }

    private void RenderSummary()
    {
        var capabilities = _crashReportRendererUtilities.Capabilities;

        if (_imgui.BeginTable("Buttons\0"u8, 2))
        {
            _imgui.TableNextColumn();
            _imgui.SetWindowFontScale(2);
            _imgui.TextWrapped("Intercepted an exception!\0"u8);
            _imgui.SetWindowFontScale(1);
            _imgui.TableNextColumn();

            _imgui.CheckboxRound("Dark Mode\0"u8, ref _isDarkMode);

            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            if (_imgui.Button("Save Report as HTML\0"u8))
            {
                if (!capabilities.IsSet(CrashReportRendererCapabilities.Dialogs))
                {
                    _imgui.OpenPopup(_filePickerHtmlId, ImGuiPopupFlags.None);
                }
                else
                {
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "crashreport.html");
                    using var stream = _crashReportRendererUtilities.SaveFileDialog("HTML files|*.html|All files (*.*)|*.*", filePath);
                    if (stream != Stream.Null)
                        _crashReportRendererUtilities.SaveAsHtml(_crashReport, _logSources, _addScreenshots, _addLatestSave, _addMiniDump, stream);
                }

            }
            RenderModalPicker(_filePickerHtmlId, FilePickerMode.CreateFile, _onCreateHtmlSelected);
            _imgui.SameLine(0.0f, -1.0f);

            if (_imgui.Button("Save Report as ZIP\0"u8))
            {
                if (!capabilities.IsSet(CrashReportRendererCapabilities.Dialogs))
                {
                    _imgui.OpenPopup(_filePickerZiplId, ImGuiPopupFlags.None);
                }
                else
                {
                    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "crashreport.zip");
                    using var stream = _crashReportRendererUtilities.SaveFileDialog("ZIP files|*.zip|All files (*.*)|*.*", filePath);
                    if (stream != Stream.Null)
                        _crashReportRendererUtilities.SaveAsZip(_crashReport, _logSources, stream);
                }
            }
            _imgui.SameLine(0.0f, -1.0f);
            RenderModalPicker(_filePickerZiplId, FilePickerMode.CreateFile, _onCreateZipSelected);

            if (capabilities.IsSet(CrashReportRendererCapabilities.CloseAndContinue))
            {
                if (_imgui.Button("Close Report and Continue\0"u8, in Secondary, in Secondary2, in Secondary3)) _onClose();
            }

            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            if (capabilities.IsSet(CrashReportRendererCapabilities.CopyAsHtml))
            {
                if (_imgui.Button("Copy as HTML\0"u8)) _crashReportRendererUtilities.CopyAsHtml(_crashReport, _logSources);
                _imgui.SameLine(0.0f, -1.0f);
            }
            if (capabilities.IsSet(CrashReportRendererCapabilities.Upload))
            {
                if (_imgui.Button("Upload Report as Permalink\0"u8)) _crashReportRendererUtilities.Upload(_crashReport, _logSources);
            }
            if (capabilities.IsSet(CrashReportRendererCapabilities.CopyAsHtml))
            {
                _imgui.TableNextColumn();
                _imgui.TableNextColumn();
            }

            if (capabilities.IsSet(CrashReportRendererCapabilities.HasScreenshots | CrashReportRendererCapabilities.HasSaveFiles | CrashReportRendererCapabilities.HasMiniDump))
            {
                _imgui.Text("Save Report as HTML Options:\0"u8);
                _imgui.TableNextColumn();
                _imgui.TableNextColumn();
            }

            if (capabilities.IsSet(CrashReportRendererCapabilities.HasScreenshots))
            {
                _imgui.CheckboxRound("Include Screenshot\0"u8, ref _addScreenshots);
                _imgui.TableNextColumn();
                _imgui.TableNextColumn();
            }

            if (capabilities.IsSet(CrashReportRendererCapabilities.HasSaveFiles))
            {
                _imgui.CheckboxRound("Include Latest Save File\0"u8, ref _addLatestSave);
                _imgui.TableNextColumn();
                _imgui.TableNextColumn();
            }

            if (capabilities.IsSet(CrashReportRendererCapabilities.HasMiniDump))
            {
                _imgui.CheckboxRound("Include Mini Dump\0"u8, ref _addMiniDump);
            }
            _imgui.EndTable();
        }

        if (capabilities.IsSet(CrashReportRendererCapabilities.CloseAndContinue))
        {
            _imgui.Text("Clicking 'Close Report and Continue' will continue with the Game's error reporting mechanism.\0"u8);
        }

        _imgui.Separator();
        _imgui.NewLine();

        _imgui.SetWindowFontScale(2);
        _imgui.Text(_crashReport.Metadata.GameName ?? string.Empty);
        _imgui.SameLine();
        _imgui.Text(" has encountered a problem!\0"u8);
        _imgui.SetWindowFontScale(1);

        _imgui.NewLine();

        _imgui.TextWrapped("This is a community Crash Report. Please save it and use it for reporting the error. Do not provide screenshots, provide the report!\0"u8);

        _imgui.Text("Most likely this error was caused by a custom installed module.\0"u8);

        _imgui.NewLine();

        _imgui.Text("If you were in the middle of something, the progress might be lost.\0"u8);

        _imgui.NewLine();

        _imgui.Text("Operating System: \0"u8);
        _imgui.SameLine();
        _imgui.Text(_operatingSystemTypeNames[(int) _crashReport.Metadata.OperatingSystemType]);
        _imgui.SameLine();
        _imgui.Text(" (\0"u8);
        _imgui.SameLine();
        _imgui.Text(_crashReport.Metadata.OperatingSystemVersion ?? string.Empty);
        _imgui.SameLine();
        _imgui.Text(")\0"u8);

        _imgui.Text("Launcher: \0"u8);
        _imgui.SameLine();
        _imgui.Text(_crashReport.Metadata.LauncherType ?? string.Empty);
        _imgui.SameLine();
        _imgui.Text(" (\0"u8);
        _imgui.SameLine();
        _imgui.Text(_crashReport.Metadata.LauncherVersion ?? string.Empty);
        _imgui.SameLine();
        _imgui.Text(")\0"u8);

        _imgui.Text("Runtime: \0"u8);
        _imgui.SameLine();
        _imgui.Text(_crashReport.Metadata.Runtime ?? string.Empty);
    }

    private void OnCreateHtmlSelected(string filePath)
    {
        var fs = File.OpenWrite(filePath);
        fs.SetLength(0);
        _crashReportRendererUtilities.SaveAsHtml(_crashReport, _logSources, _addScreenshots, _addLatestSave, _addMiniDump, fs);
    }

    private void OnCreateZipSelected(string filePath)
    {
        var fs = File.OpenWrite(filePath);
        fs.SetLength(0);
        _crashReportRendererUtilities.SaveAsZip(_crashReport, _logSources, fs);
    }
}