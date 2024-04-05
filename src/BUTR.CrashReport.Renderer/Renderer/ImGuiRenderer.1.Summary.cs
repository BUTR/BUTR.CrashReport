namespace BUTR.CrashReport.Renderer.Renderer;

partial class ImGuiRenderer
{
    private bool _addScreenshots;
    private bool _addLatestSave;
    private bool _addMiniDump;

    private void RenderSummary()
    {
        if (_imgui.BeginTable("Buttons\0"u8, 2))
        {
            _imgui.TableNextColumn();
            _imgui.SetWindowFontScale(2);
            _imgui.TextWrapped("Intercepted an exception!\0"u8);
            _imgui.SetWindowFontScale(1);
            _imgui.TableNextColumn();

            if (_imgui.Button("Save Report as HTML\0"u8)) _crashReportRendererUtilities.SaveCrashReportAsHtml(_crashReport, _logSources, _addMiniDump, _addLatestSave, _addScreenshots);
            _imgui.SameLine();
            if (_imgui.Button("Save Report as ZIP\0"u8)) _crashReportRendererUtilities.SaveCrashReportAsZip(_crashReport, _logSources);
            _imgui.SameLine();
            if (_imgui.Button("Close Report and Continue\0"u8, in Secondary, in Secondary2, in Secondary3)) _onClose();
            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            if (_imgui.Button("Copy as HTML\0"u8)) _crashReportRendererUtilities.CopyAsHtml(_crashReport, _logSources);
            _imgui.SameLine();
            if (_imgui.Button("Upload Report as Permalink\0"u8)) _crashReportRendererUtilities.Upload(_crashReport, _logSources);
            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            _imgui.Text("Save Report as HTML Options:\0"u8);
            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            _imgui.Checkbox("Include Screenshot\0"u8, ref _addScreenshots);
            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            _imgui.Checkbox("Include Latest Save File\0"u8, ref _addLatestSave);
            _imgui.TableNextColumn();
            _imgui.TableNextColumn();

            _imgui.Checkbox("Include Mini Dump\0"u8, ref _addMiniDump);
            _imgui.EndTable();
        }

        _imgui.Text("Clicking 'Close Report and Continue' will continue with the Game's error report mechanism.\0"u8);

        _imgui.Separator();
        _imgui.NewLine();

        _imgui.SetWindowFontScale(2);
        _imgui.TextSameLine(_crashReport.Metadata.GameName ?? string.Empty);
        _imgui.Text(" has encountered a problem and will close itself!\0"u8);
        _imgui.SetWindowFontScale(1);

        _imgui.NewLine();

        _imgui.Text("This is a community Crash Report. Please save it and use it for reporting the error. Do not provide screenshots, provide the report!\0"u8);

        _imgui.Text("Most likely this error was caused by a custom installed module.\0"u8);

        _imgui.NewLine();

        _imgui.Text("If you were in the middle of something, the progress might be lost.\0"u8);

        _imgui.NewLine();

        _imgui.TextSameLine("Launcher: \0"u8);
        _imgui.TextSameLine(_crashReport.Metadata.LauncherType ?? string.Empty);
        _imgui.TextSameLine(" (\0"u8);
        _imgui.TextSameLine(_crashReport.Metadata.LauncherVersion ?? string.Empty);
        _imgui.Text(")\0"u8);

        _imgui.TextSameLine("Runtime: \0"u8);
        _imgui.Text(_crashReport.Metadata.Runtime ?? string.Empty);
    }
}