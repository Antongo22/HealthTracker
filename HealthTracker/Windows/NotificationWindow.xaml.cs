using System;
using System.Windows;

namespace HealthTracker.Windows
{
    public partial class NotificationWindow : Window
    {
        public bool IsPostponed { get; private set; } = false;

        public NotificationWindow(string text, int time = 5)
        {
            InitializeComponent();
            TextBlockNotification.Text = text;
            ButtonLater.Content = $"Отложить ({time} мин)";
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonLater_Click(object sender, RoutedEventArgs e)
        {
            IsPostponed = true;
            Close();
        }
    }
}