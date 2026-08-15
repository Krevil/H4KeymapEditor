using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H4KeymapEditor.Models
{
    public enum Theme
    {
        Light,
        Dark,
    }

    public class Themes
    {
        public static Dictionary<Theme, string> ThemeDictionary = new Dictionary<Theme, string>
        {
            { Theme.Light, "Themes/LightTheme.xaml" },
            { Theme.Dark, "Themes/DarkTheme.xaml" },
        };
    }
}
