using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using H4KeymapEditor.Models;

namespace H4KeymapEditor.Themes
{
    public class ThemeManager
    {
        public static Theme CurrentTheme { get; set; } = Theme.Light;

        public static void SwapTheme()
        {
            Collection<ResourceDictionary> dict = Application.Current.Resources.MergedDictionaries;
            Theme newTheme = CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            string? removePath;
            string? newPath;
            Models.Themes.ThemeDictionary.TryGetValue(CurrentTheme, out removePath);
            Models.Themes.ThemeDictionary.TryGetValue(newTheme, out newPath);
            if (removePath != null && newPath != null)
            {
                ResourceDictionary? removeDict = dict.FirstOrDefault(d => d.Source?.OriginalString == removePath);
                if (removeDict != null)
                {
                    dict.Remove(removeDict);
                }
                dict.Insert(0, new ResourceDictionary { Source = new Uri(newPath, UriKind.Relative) });
                CurrentTheme = newTheme;
            }
        }
    }
}
