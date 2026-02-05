using System;
using System.Windows;
using System.Windows.Media;
using Base;
using Wpf.Ui.Appearance;

namespace OxoBrowser.Services
{
    public static class ThemeService
    {
        private const string ThemeDictionaryPrefix = "Themes/Catppuccin/";

        public static void Apply(CatppuccinTheme theme)
        {
            var normalizedTheme = NormalizeTheme(theme);
            var appTheme = normalizedTheme == CatppuccinTheme.Latte ? ApplicationTheme.Light : ApplicationTheme.Dark;

            ApplicationThemeManager.Apply(appTheme);
            ApplicationAccentColorManager.Apply(GetMauve(normalizedTheme), appTheme, false);

            ApplyResourceDictionary(normalizedTheme);
        }

        private static void ApplyResourceDictionary(CatppuccinTheme theme)
        {
            if (Application.Current == null)
            {
                return;
            }

            var merged = Application.Current.Resources.MergedDictionaries;
            for (int i = merged.Count - 1; i >= 0; i--)
            {
                var source = merged[i].Source?.OriginalString ?? merged[i].Source?.ToString();
                if (source != null && source.IndexOf(ThemeDictionaryPrefix, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    merged.RemoveAt(i);
                }
            }

            var sourceUri = new Uri(
                $"pack://application:,,,/OxoBrowser;component/{ThemeDictionaryPrefix}{GetThemeFile(theme)}",
                UriKind.Absolute);
            merged.Add(new ResourceDictionary { Source = sourceUri });
        }

        private static string GetThemeFile(CatppuccinTheme theme)
        {
            return theme switch
            {
                CatppuccinTheme.Latte => "Latte.xaml",
                CatppuccinTheme.Frappe => "Frappe.xaml",
                _ => "Latte.xaml"
            };
        }

        private static Color GetMauve(CatppuccinTheme theme)
        {
            return theme switch
            {
                CatppuccinTheme.Latte => Color.FromRgb(0x88, 0x39, 0xEF),
                CatppuccinTheme.Frappe => Color.FromRgb(0xCA, 0x9E, 0xE6),
                _ => Color.FromRgb(0x88, 0x39, 0xEF)
            };
        }

        private static CatppuccinTheme NormalizeTheme(CatppuccinTheme theme)
        {
            return theme switch
            {
                CatppuccinTheme.Latte => CatppuccinTheme.Latte,
                CatppuccinTheme.Frappe => CatppuccinTheme.Frappe,
                _ => CatppuccinTheme.Frappe
            };
        }
    }
}
