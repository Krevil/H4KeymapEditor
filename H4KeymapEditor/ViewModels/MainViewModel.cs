using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.IO;
using H4KeymapEditor.Models;
using H4KeymapEditor.Themes;

namespace H4KeymapEditor.ViewModels
{
    public class MainViewModel
    {
        public List<ExecutableType> Executables { get; }
        public ExecutableType SelectedExecutable { get; set; }
        public ICollectionView VisibleKeyBindings { get; set; }
        public Keycode[] Keycodes { get; }
        public string? currentFile { get; private set; }
        
        private bool useDarkMode;
        public bool UseDarkMode
        {
            get
            {
                return useDarkMode;
            }
            set
            {
                ThemeManager.SwapTheme();
                useDarkMode = value;
            }
        }
        private bool showUnknowns = false;
        public bool ShowUnknowns
        {
            get
            {
                return showUnknowns;
            }
            set
            {
                showUnknowns = value;
                VisibleKeyBindings.Refresh();
            }
        }

        public MainViewModel()
        {
            Executables = new List<ExecutableType> { ExecutableType.Sapien, ExecutableType.TagTest };
            VisibleKeyBindings = CollectionViewSource.GetDefaultView(KeyBinding.KeyBindings);
            VisibleKeyBindings.Filter = KeyBindingFilter;
            Keycodes = Enum.GetValues<Keycode>();
        }

        public void OpenFile(string filePath)
        {
            ExecutableType newExeType;
            if (filePath.Contains("sapien"))
            {
                if (filePath.Contains("sapien_play"))
                {
                    MessageBox.Show("Play executables not currently supported");
                    return;
                }
                newExeType = ExecutableType.Sapien;
            }
            else if (filePath.Contains("tag_test"))
            {
                newExeType = ExecutableType.TagTest;
            }
            else if (filePath.Contains("tag_play"))
            {
                MessageBox.Show("Play executables not currently supported");
                return;
            }
            else
            {
                MessageBox.Show("Executable must be either sapien or tag_test");
                return;
            }

            // If there is current keybindings loaded
            if (KeyBinding.KeyBindings.Count > 0)
            {
                // handle save and close
                var result = MessageBox.Show("Save current keybindings?", "Save keybindings?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            currentFile = filePath;
            Patcher.OpenFile(filePath, newExeType);
            SelectedExecutable = newExeType;
            VisibleKeyBindings.Refresh();
        }

        public void SaveFile()
        {
            if (currentFile == null)
                return;
            ExecutableType exeType;
            if (currentFile.Contains("sapien"))
            {
                if (currentFile.Contains("sapien_play"))
                {
                    MessageBox.Show("Play executables not currently supported");
                    return;
                }
                exeType = ExecutableType.Sapien;
            }
            else if (currentFile.Contains("tag_test"))
            {
                exeType = ExecutableType.TagTest;
            }
            else if (currentFile.Contains("tag_play"))
            {
                MessageBox.Show("Play executables not currently supported");
                return;
            }
            else
            {
                MessageBox.Show("Executable must be either sapien or tag_test");
                return;
            }
            Patcher.SaveFile(currentFile, exeType);
        }

        private bool KeyBindingFilter(object obj)
        {
            if (obj is KeyBinding keyBinding)
            {
                return (!keyBinding.Unknown || ShowUnknowns) && keyBinding.PrimaryKey != Keycode.Invalid;
            }
            return false;
        }
    }
}
