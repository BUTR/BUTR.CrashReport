using BUTR.CrashReport.ImGui;
using BUTR.CrashReport.ImGui.Enums;
using BUTR.CrashReport.ImGui.Extensions;
using BUTR.CrashReport.Memory.Utils;

using System.Buffers;
using System.Numerics;

using Utf8StringInterpolation;

namespace BUTR.CrashReport.Renderer.ImGui.Components;

internal class FilePicker : IDisposable
{
    private static readonly char[] SearchFilterSplit = new char['|'];
    private static readonly Vector2 Child400 = new(400, 400);

    private static readonly Vector2 Zero2 = Vector2.Zero;
    private static readonly Vector4 Zero4 = Vector4.Zero;

    private static readonly Dictionary<int, FilePicker> _filePickers = new();

    public static FilePicker GetPicker<T>(T o, IImGui cmGui, FilePickerMode mode, string startingPath, string? searchFilter = null)
        where T : notnull
    {
        if (File.Exists(startingPath))
        {
            startingPath = Path.GetDirectoryName(startingPath)!;
        }
        else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
        {
            startingPath = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(startingPath))
                startingPath = AppContext.BaseDirectory;
        }

        var hashCode = o.GetHashCode();
        if (!_filePickers.TryGetValue(hashCode, out var fp))
        {
            var allowedExtensions = searchFilter?.Split(SearchFilterSplit, StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];
            fp = new FilePicker(cmGui, startingPath, mode, allowedExtensions);


            _filePickers.Add(hashCode, fp);
        }

        return fp;
    }

    public static void RemovePicker<T>(T o)
        where T : notnull
    {
        var hashCode = o.GetHashCode();

        var picker = _filePickers[hashCode];
        _filePickers.Remove(hashCode);
        picker.Dispose();
    }


    private readonly IImGui _imgui;
    private readonly FilePickerMode _mode;
    private readonly List<string> _allowedExtensions;

    private string _rootFolder;
    private string? _currentParentFolder;
    private string _currentFolder;

    private byte[] _currentFolderPathUtf8 = [];
    private List<(string, byte[])> _currentFiles = new();
    private List<(string, byte[])> _currentFolders = new();

    private byte[] _newNameUtf8;
    private string? _selectedFile;

    public string? SelectedPath { get; private set; }


    private FilePicker(IImGui imgui, string startingPath, FilePickerMode mode, List<string> allowedExtensions)
    {
        _imgui = imgui;
        _mode = mode;
        _allowedExtensions = allowedExtensions;

        _newNameUtf8 = ArrayPool<byte>.Shared.Rent(256);

        _rootFolder = default!;
        _currentFolder = default!;
        UpdateCurrentFolder(startingPath);
    }

    private void UpdateCurrentFolder(string path)
    {
        _rootFolder = Path.GetPathRoot(path)!;
        _currentParentFolder = Path.GetDirectoryName(path);
        _currentFolder = path;

        _currentFolderPathUtf8 = Utf8String.Format($"Current Folder: {Path.GetFileName(_rootFolder)}{_currentFolder.Replace(_rootFolder, "")}\0");

        _selectedFile = null;

        Reload();
    }

    private void Reload()
    {
        _currentFolders = Directory.EnumerateDirectories(_currentFolder)
            .Select(x => (x, Utf8String.Format($"{Path.GetFileName(x)}/")))
            .ToList();

        if (_mode is FilePickerMode.CreateFile or FilePickerMode.OpenFile)
        {
            _currentFiles = Directory.EnumerateFiles(_currentFolder)
                .Where(x => _allowedExtensions.Count == 0 || _allowedExtensions.Contains(Path.GetExtension(x)))
                .Select(x => (x, Utf8String.Format($"{Path.GetFileName(x)}\0")))
                .ToList();
        }
    }

    public bool Draw()
    {
        _imgui.Text(_currentFolderPathUtf8);
        var result = false;

        if (_imgui.BeginChild("###Folder\0"u8, in Child400, in Zero4, ImGuiChildFlags.None, ImGuiWindowFlags.None))
        {
            if (!string.IsNullOrEmpty(_currentParentFolder) && _currentFolder != _rootFolder)
            {
                //_imgui.PushStyleColor(ImGuiCol.Text, in Yellow);
                var isSelected = false;
                if (_imgui.Selectable("../\0"u8, ref isSelected, ImGuiSelectableFlags.NoAutoClosePopups))
                    UpdateCurrentFolder(_currentParentFolder!);
                //_imgui.PopStyleColor();
            }

            for (var i = 0; i < _currentFolders.Count; i++)
            {
                var (folderPath, folderNameUtf8) = _currentFolders[i];

                //_imgui.PushStyleColor(ImGuiCol.Text, in Yellow);
                var isSelected = false;
                if (_imgui.Selectable(folderNameUtf8, ref isSelected, ImGuiSelectableFlags.NoAutoClosePopups))
                    UpdateCurrentFolder(folderPath);
                //_imgui.PopStyleColor();
            }

            if (_mode is FilePickerMode.CreateFile or FilePickerMode.OpenFile)
            {
                for (var i = 0; i < _currentFiles.Count; i++)
                {
                    var (filePath, fileNameUtf8) = _currentFiles[i];

                    var isSelected = string.Equals(_selectedFile, filePath, StringComparison.Ordinal);
                    if (_imgui.Selectable(fileNameUtf8, ref isSelected, ImGuiSelectableFlags.NoAutoClosePopups))
                        _selectedFile = filePath;

                    if (_imgui.IsMouseDoubleClicked(0))
                    {
                        result = true;
                        _imgui.CloseCurrentPopup();
                    }
                }
            }
        }
        _imgui.EndChild();

        if (_mode is FilePickerMode.CreateFile)
        {
            _imgui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            if (_imgui.BeginChild("###FolderSub\0"u8, in Zero2, ImGuiChildFlags.Border | ImGuiChildFlags.AutoResizeY, ImGuiWindowFlags.None))
            {
                _imgui.PopStyleVar();
                _imgui.Text("File Name: \0"u8);
                _imgui.SameLine();
                _imgui.InputText(" "u8, _newNameUtf8, ImGuiInputTextFlags.None);
            }
            else
            {
                _imgui.PopStyleVar();
            }
            _imgui.EndChild();
        }


        if (_imgui.Button("Cancel\0"u8))
        {
            result = false;
            _imgui.CloseCurrentPopup();
        }

        if (_mode is FilePickerMode.CreateFile && _newNameUtf8.Any(x => x != 0))
        {
            _imgui.SameLine();
            _imgui.Text(" \0"u8);
            _imgui.SameLine();
            if (_imgui.Button("Create File\0"u8))
            {
                var idxNull = Array.IndexOf(_newNameUtf8, (byte) 0);
                SelectedPath = Path.Combine(_currentFolder, Utf8Utils.ToString(_newNameUtf8.AsSpan(0, idxNull)).Trim());
                result = true;
                _imgui.CloseCurrentPopup();
            }
        }

        if (_mode is FilePickerMode.OpenFile && !string.IsNullOrEmpty(_selectedFile))
        {
            _imgui.SameLine();
            _imgui.Text(" \0"u8);
            _imgui.SameLine();
            if (_imgui.Button("Open File\0"u8))
            {
                SelectedPath = _selectedFile;
                result = true;
                _imgui.CloseCurrentPopup();
            }
        }

        if (_mode is FilePickerMode.CreateFolder && _newNameUtf8.Any(x => x != 0))
        {
            _imgui.SameLine();
            _imgui.Text(" \0"u8);
            _imgui.SameLine();
            if (_imgui.Button("Create Folder\0"u8))
            {
                SelectedPath = _currentFolder;
                result = true;
                _imgui.CloseCurrentPopup();
            }
        }
        if (_mode is FilePickerMode.OpenFolder)
        {
            _imgui.SameLine();
            _imgui.Text(" \0"u8);
            _imgui.SameLine();
            if (_imgui.Button("Open Current Folder\0"u8))
            {
                SelectedPath = _currentFolder;
                result = true;
                _imgui.CloseCurrentPopup();
            }
        }

        return result;
    }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(_newNameUtf8);
    }
}