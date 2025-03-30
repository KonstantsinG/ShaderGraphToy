using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GUI.Resources
{
    internal static class ResourceManager
    {
        internal static bool IsCacheExists(string path)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShaderGraph");
            return File.Exists(Path.Combine(folder, path));
        }

        internal static void SaveBitmapToCache(WriteableBitmap bitmap, string file)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShaderGraph", "bitmaps");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using var stream = new FileStream(Path.Combine(folder, file), FileMode.OpenOrCreate);
            encoder.Save(stream);
        }

        internal static BitmapImage GetBitmapFromCache(string name)
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShaderGraph", "bitmaps", name), UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bitmap.EndInit();

            return bitmap;
        }


        internal static bool IsResourceExists(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            return Array.Exists(resourceNames, rn => rn.EndsWith(name));
        }

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
