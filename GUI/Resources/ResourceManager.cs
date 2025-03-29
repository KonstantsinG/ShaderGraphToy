using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GUI.Resources
{
    internal static class ResourceManager
    {
        internal static BitmapImage GetIconFromResources(string name)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/GUI;component/Resources/Icons/{name}", UriKind.Absolute));
        }

        internal static Cursor GetCursorFromResources(string name)
        {
            return new Cursor(GetResourceStream($"GUI.Resources.Cursors.{name}"));
        }

        internal static ResourceDictionary GetLocalizationDictionaryFromResources(string name)
        {
            ResourceDictionary dict = new()
            {
                Source = name switch
                {
                    "ru-RU" => new Uri($"Resources/Localization/guiLoc.{name}.xaml", UriKind.Relative),
                    _ => new Uri("Resources/Localization/guiLoc.xaml", UriKind.Relative),
                }
            };

            return dict;
        }

        internal static void SwitchLocalizationDictionaries(ResourceDictionary dict)
        {
            ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                          where d.Source != null && d.Source.OriginalString.StartsWith("Resources/Localization/guiLoc.")
                                          select d).First();
            if (oldDict != null)
            {
                int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
            }
            else
                Application.Current.Resources.MergedDictionaries.Add(dict);
        }


        private static Stream GetResourceStream(string path)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(path)!;
        }
    }
}
