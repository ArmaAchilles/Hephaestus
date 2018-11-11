using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Hephaestus.Configurator.Utilities
{
    public static class DialogUtility
    {        
        public static string OpenFileDialogToSelectFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : "";
        }

        public static string OpenFileDialogToSelectFile(string initialDirectory,
            string defaultExtension = ".exe", string allowedExtensions = "Executable Files (*.exe)|*.exe")
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = allowedExtensions,
                InitialDirectory = initialDirectory
            };

            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                return dialog.FileName;
            }

            return "";
        }
    }
}
