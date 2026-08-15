using System.Windows;
using System.IO;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using H4KeymapEditor.ViewModels;
using H4KeymapEditor.Models;

namespace H4KeymapEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open game executable",
                Filter = "Executables (*.exe)|*.exe",
                DefaultExt = ".exe",
                CheckFileExists = true,
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                mainViewModel.OpenFile(ofd.FileName);
            }
        }
    }

    private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.SaveFile();
        }
    }
}