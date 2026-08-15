using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
            VisibleKeyBindings = CollectionViewSource.GetDefaultView(Models.KeyBinding.KeyBindings);
            VisibleKeyBindings.Filter = KeyBindingFilter;
            Keycodes = Enum.GetValues<Keycode>();
        }

        public void OpenFile(string filePath)
        {
            currentFile = filePath;
            Patcher.OpenFile(filePath);
            VisibleKeyBindings.Refresh();
        }

        public void SaveFile()
        {

        }

        private bool KeyBindingFilter(object obj)
        {
            if (obj is Models.KeyBinding keyBinding)
            {
                return (!keyBinding.Unknown || ShowUnknowns) && keyBinding.PrimaryKey != Keycode.Invalid;
            }
            return false;
        }
    }
}
