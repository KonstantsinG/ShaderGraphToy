using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ShaderGraphToy.Representation.Controls
{
    /// <summary>
    /// Логика взаимодействия для NotificationPanel.xaml
    /// </summary>
    public partial class NotificationsPanel : UserControl
    {
        private bool _state = false;


        public NotificationsPanel()
        {
            InitializeComponent();
        }


        public void Toggle(bool state)
        {
            if (_state == state) return;
            _state = state;

            if (_state) AnimatePanel(0, 150);
            else AnimatePanel(150,  0);
        }

        public void ClearText()
        {
            textContainer.Children.Clear();
            bgGrid.Background = null;
        }

        public void AppendMessage(string text)
        {
            Toggle(true);

            TextBlock tb = new()
            {
                Text = text,
                Foreground = (SolidColorBrush)FindResource("MessageText")
            };

            bgGrid.Background = (SolidColorBrush)FindResource("MessageText");
            textContainer.Children.Add(tb);
        }

        public void AppendWarning(string text)
        {
            Toggle(true);

            TextBlock tb = new()
            {
                Text = text,
                Foreground = (SolidColorBrush)FindResource("WarningText")
            };

            bgGrid.Background = (SolidColorBrush)FindResource("WarningText");
            textContainer.Children.Add(tb);
        }

        public void AppendError(string text)
        {
            Toggle(true);

            TextBlock tb = new()
            {
                Text = text,
                Foreground = (SolidColorBrush)FindResource("ErrorText")
            };

            bgGrid.Background = (SolidColorBrush)FindResource("ErrorText");
            textContainer.Children.Add(tb);
        }


        private void AnimatePanel(double from, double to)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            BeginAnimation(HeightProperty, animation);
        }

        private void ClearRect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClearText();
        }

        private void CrossRect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Toggle(false);
        }
    }
}
