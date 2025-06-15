using Microsoft.Win32;

public class FileDialogService : IFileDialogService
{
    public string ShowSaveFileDialog(string filter, string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
