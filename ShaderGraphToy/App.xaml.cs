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
        #region LOCALIZATION
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
                ArgumentNullException.ThrowIfNull("value");

                // set "." as floating point numbers separator for each culture
                CultureInfo culture = (CultureInfo)value.Clone();
                culture.NumberFormat.NumberDecimalSeparator = ".";
                culture.NumberFormat.CurrencyDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                if (culture == Thread.CurrentThread.CurrentUICulture)
                    return;

                Thread.CurrentThread.CurrentUICulture = culture;
                ResourceDictionary dict = ResourceManager.GetLocalizationDictionaryFromResources(culture.Name);
                ResourceManager.SwitchLocalizationDictionaries(dict);

                LanguageChanged(Application.Current, new());
                Settings.Default.DefaultLanguage = Language;
                Settings.Default.Save();
            }
        }
        #endregion


        public App()
        {
            InitializeComponent();
            Language = Settings.Default.DefaultLanguage;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
#else
            SetupExceptionsHandling(); // Exceptions logger is only for the Release build
#endif
            RenderingDeviceManager.EnableNvapi();
            Exit += AppExit;
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            RenderingDeviceManager.DisableNvapi();
        }


        #region EXCEPTIONS HANDLING
#pragma warning disable IDE0051 // Удалите неиспользуемые закрытые члены
        private void SetupExceptionsHandling()
#pragma warning restore IDE0051 // Удалите неиспользуемые закрытые члены
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogError(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                ExceptionsLogger.LogError(exception, message);
            }
        }
        #endregion
    }
}
