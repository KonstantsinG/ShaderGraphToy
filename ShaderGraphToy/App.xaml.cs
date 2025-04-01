using ShaderGraphToy.Resources;
using ShaderGraphToy.Utilities.Common;
using System.Globalization;
using System.Windows;

namespace ShaderGraphToy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly List<CultureInfo> _languages = [ new("en-US"), new("ru-RU") ];
        public static List<CultureInfo> Languages
        {
            get => _languages;
        }

        public static event EventHandler LanguageChanged = delegate { };
        public static CultureInfo Language
        {
            get => Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (value == null) ArgumentNullException.ThrowIfNull("value");
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                Thread.CurrentThread.CurrentUICulture = value!;
                ResourceDictionary dict = ResourceManager.GetLocalizationDictionaryFromResources(value!.Name);
                ResourceManager.SwitchLocalizationDictionaries(dict);

                LanguageChanged(Application.Current, new());
                Settings.Default.DefaultLanguage = Language;
                Settings.Default.Save();
            }
        }


        public App()
        {
            InitializeComponent();
            Language = Settings.Default.DefaultLanguage;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RenderingDeviceManager.EnableNvapi();
            Exit += AppExit;
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            RenderingDeviceManager.DisableNvapi();
        }
    }
}
