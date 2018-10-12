using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using HephaestusConfigurator.Utilities;
using HephaestusCommon.Utilities;
using HephaestusCommon.Classes;

namespace HephaestusConfigurator
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
            if (comboBox_projectDirectory.SelectedItem == comboBox_projectDirectory_addNew)
            {
                AddProjectDirectoryToComboBox(Dialogs.OpenFileDialogToSelectFolder());
            }
        }

        private void button_projectDirectory_Click(object sender, RoutedEventArgs e)
        {
            AddProjectDirectoryToComboBox(Dialogs.OpenFileDialogToSelectFolder());
        }

        private void button_sourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            textBox_sourceDirectory.Text = Dialogs.OpenFileDialogToSelectFolder();
        }

        private void button_targetDirectory_Click(object sender, RoutedEventArgs e)
        {
            textBox_targetDirectory.Text = Dialogs.OpenFileDialogToSelectFolder();
        }

        private void button_addonBuilderFile_Click(object sender, RoutedEventArgs e)
        {
            string defaultAddonBuilderPath = RegistryUtility.GetKey(@"SOFTWARE\WOW6432Node\Bohemia Interactive\addonbuilder", "path");

            textBox_addonBuilderFile.Text = Dialogs.OpenFileDialogToSelectFile(defaultAddonBuilderPath, "AddonBuilder.exe", "Addon Builder|AddonBuilder.exe");
        }

        private void button_privateKeyFile_Click(object sender, RoutedEventArgs e)
        {
            textBox_privateKeyFile.Text = Dialogs.OpenFileDialogToSelectFile("", ".biprivatekey", "BI Private Key (*.biprivatekey)|*.biprivatekey");
        }

        private void button_privateKeyFileCreateNew_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void button_gameExecutable_Click(object sender, RoutedEventArgs e)
        {
            string defaultArma3Path = RegistryUtility.GetKey(@"SOFTWARE\WOW6432Node\Bohemia Interactive\arma 3", "main");

            textBox_gameExecutable.Text = Dialogs.OpenFileDialogToSelectFile(defaultArma3Path);
        }

        private void button_saveSettings_Click(object sender, RoutedEventArgs e)
        {
            string path = comboBox_projectDirectory.Text;

            Project project = new Project(
                path,
                textBox_sourceDirectory.Text,
                textBox_targetDirectory.Text,
                textBox_addonBuilderFile.Text,
                textBox_projectPrefix.Text,
                textBox_privateKeyFile.Text,
                textBox_gameExecutable.Text,
                textBox_gameExecutableArguments.Text,
                checkbox_shutdownGameBeforeBuilding.IsChecked ?? false,
                checkbox_startGameAfterBuilding.IsChecked ?? false);

            ProjectUtility.SetProject(path, project);

            MessageBox.Show("Hephaestus Project Saved!", "Hephaestus Configurator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddProjectDirectoryToComboBox(string path)
        {
            if (path.Length > 0)
            {
                int indexToInsert = comboBox_projectDirectory.Items.Count - 1;

                comboBox_projectDirectory.Items.Insert(comboBox_projectDirectory.Items.Count - 1, new ComboBoxItem { Content = path });
                comboBox_projectDirectory.SelectedIndex = indexToInsert;
            }
        }
    }
}
