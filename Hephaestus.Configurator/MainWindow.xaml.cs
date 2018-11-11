using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Hephaestus.Configurator.Utilities;
using Hephaestus.Common.Utilities;
using Hephaestus.Common.Classes;

namespace Hephaestus.Configurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void comboBox_projectDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Equals(ComboBoxProjectDirectory.SelectedItem, ComboBoxProjectDirectoryAddNew))
            {
                AddProjectDirectoryToComboBox(DialogUtility.OpenFileDialogToSelectFolder());
            }
        }

        private void button_projectDirectory_Click(object sender, RoutedEventArgs e)
        {
            AddProjectDirectoryToComboBox(DialogUtility.OpenFileDialogToSelectFolder());
            
            LoadAllFields();
        }

        private void button_sourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSourceDirectory.Text = DialogUtility.OpenFileDialogToSelectFolder();
        }

        private void button_targetDirectory_Click(object sender, RoutedEventArgs e)
        {
            TextBoxTargetDirectory.Text = DialogUtility.OpenFileDialogToSelectFolder();
        }

        private void button_addonBuilderFile_Click(object sender, RoutedEventArgs e)
        {
            string defaultAddonBuilderPath = RegistryUtility.GetKey(@"SOFTWARE\WOW6432Node\Bohemia Interactive\addonbuilder", "path");

            TextBoxAddonBuilderFile.Text = DialogUtility.OpenFileDialogToSelectFile(defaultAddonBuilderPath, "AddonBuilder.exe", "Addon Builder|AddonBuilder.exe");
        }

        private void button_privateKeyFile_Click(object sender, RoutedEventArgs e)
        {
            TextBoxPrivateKeyFile.Text = DialogUtility.OpenFileDialogToSelectFile("", ".biprivatekey", "BI Private Key (*.biprivatekey)|*.biprivatekey");
        }

        private void button_privateKeyFileCreateNew_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add support to create a new private key
        }

        private void button_gameExecutable_Click(object sender, RoutedEventArgs e)
        {
            string defaultArma3Path = RegistryUtility.GetKey(@"SOFTWARE\WOW6432Node\Bohemia Interactive\arma 3", "main");

            TextBoxGameExecutable.Text = DialogUtility.OpenFileDialogToSelectFile(defaultArma3Path);
        }

        private void button_saveSettings_Click(object sender, RoutedEventArgs e)
        {
            string path = ComboBoxProjectDirectory.Text;

            Project project = new Project(
                path,
                TextBoxSourceDirectory.Text,
                TextBoxTargetDirectory.Text,
                TextBoxAddonBuilderFile.Text,
                TextBoxProjectPrefix.Text,
                TextBoxPrivateKeyFile.Text,
                new Game(TextBoxGameExecutable.Text, TextBoxGameExecutableArguments.Text), 
                CheckboxShutdownGameBeforeBuilding.IsChecked ?? false,
                CheckboxStartGameAfterBuilding.IsChecked ?? false);

            project.Save();

            MessageBox.Show("Hephaestus Project Saved!", "Hephaestus Configurator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddProjectDirectoryToComboBox(string path)
        {
            if (path.Length <= 0)
            {
                return;
            }
            
            int indexToInsert = ComboBoxProjectDirectory.Items.Count - 1;

            ComboBoxProjectDirectory.Items.Insert(ComboBoxProjectDirectory.Items.Count - 1, new ComboBoxItem { Content = path });
            ComboBoxProjectDirectory.SelectedIndex = indexToInsert;
        }

        private void ComboBoxProjectDirectory_OnLostFocus(object sender, RoutedEventArgs e)
        {
            LoadAllFields();
        }
        
        private void ComboBoxProjectDirectory_OnKeyUp(object sender, RoutedEventArgs e)
        {
            LoadAllFields();
        }

        private void LoadAllFields()
        {
            if (Equals(ComboBoxProjectDirectory.SelectedItem, ComboBoxProjectDirectoryAddNew))
            {
                return;
            }
            
            string path = ComboBoxProjectDirectory.Text;
            if (! Directory.Exists(path))
            {
                return;
            }

            if (! ProjectUtility.ProjectExists(path))
            {
                return;
            }
            
            Project project = ProjectUtility.GetProject(path);
            
            TextBoxSourceDirectory.Text = project.SourceDirectory;
            TextBoxTargetDirectory.Text = project.TargetDirectory;
            TextBoxAddonBuilderFile.Text = project.AddonBuilderFile;
            TextBoxProjectPrefix.Text = project.ProjectPrefix;
            TextBoxPrivateKeyFile.Text = project.PrivateKeyFile;
            TextBoxGameExecutable.Text = project.Game.GameExecutable;
            TextBoxGameExecutableArguments.Text = project.Game.GameExecutableArguments;
            CheckboxShutdownGameBeforeBuilding.IsChecked = project.ShutdownGameBeforeBuilding;
            CheckboxStartGameAfterBuilding.IsChecked = project.StartGameAfterBuilding;
        }

        private void ComboBoxProjectDirectory_OnLoaded(object sender, RoutedEventArgs e)
        {
            string[] arguments = Environment.GetCommandLineArgs();

            if (arguments.Length <= 1)
            {
                return;
            }

            if (! Directory.Exists(arguments[1]))
            {
                return;
            }
            
            ComboBoxProjectDirectory.Text = arguments[1];
                
            LoadAllFields();
        }
    }
}
