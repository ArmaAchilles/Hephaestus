using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using HephaestusConfigurator.Utilities;

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
            string defaultAddonBuilderPath = ReadRegistryKey("SOFTWARE\\WOW6432Node\\Bohemia Interactive\\addonbuilder", "path");

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
            string defaultArma3Path = ReadRegistryKey("SOFTWARE\\WOW6432Node\\Bohemia Interactive\\arma 3", "main");

            textBox_gameExecutable.Text = Dialogs.OpenFileDialogToSelectFile(defaultArma3Path);
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

        public string ReadRegistryKey(string keyPath, string subKeyName)
        {
            string value = "";

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        Object subKeyObject = key.GetValue(subKeyName);

                        if (subKeyObject != null)
                        {
                            value = subKeyObject as String;
                        }
                    }
                }
            }
            catch (Exception) { }

            return value;
        }
    }
}
