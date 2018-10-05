using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace HephaestusConfigurator.Utilities
{
    public static class Dialogs
    {
        public static string OpenFileDialogToSelectFolder(string initialDirectory = "")
        {
            string selectedFolder = "";

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedFolder = dialog.FileName;
            }

            return selectedFolder;
        }

        public static string OpenFileDialogToSelectFile(string initialDirectory = "",
            string defaultExtension = ".exe", string allowedExtensions = "Executable Files (*.exe)|*.exe")
        {
            string selectedFile = "";

            OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = allowedExtensions,
                InitialDirectory = initialDirectory
            };

            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = dialog.FileName;
            }

            return selectedFile;
        }
    }
}
