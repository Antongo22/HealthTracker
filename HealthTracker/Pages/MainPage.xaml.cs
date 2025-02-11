using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HealthTracker.Windows;
using Newtonsoft.Json;

namespace HealthTracker.Pages
{
    public partial class MainPage : Page
    {
        private UserSettings _userSettings;
        private readonly string _settingsFilePath = "user_settings.json";
        private DispatcherTimer _reminderTimer;

        public MainPage()
        {
            InitializeComponent();
            LoadSettings();
            Dispatcher.InvokeAsync(UpdateUIFromSettings, DispatcherPriority.Background);
            _reminderTimer = new DispatcherTimer();
            _reminderTimer.Tick += ReminderTimer_Tick;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    _userSettings = JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _userSettings = new UserSettings();
                }
            }
            else
            {
                _userSettings = new UserSettings();
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            if (_userSettings == null)
                _userSettings = new UserSettings();

            try
            {
                var json = JsonConvert.SerializeObject(_userSettings, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateUIFromSettings()
        {
            TextBoxMain.Text = _userSettings.MainText;
            TextBoxReminderTime.Text = _userSettings.ReminderTime.ToString();
            TextBoxRepeatTime.Text = _userSettings.RepeatTime.ToString();
        }

        private void UpdateSettingsFromUI()
        {
            _userSettings.MainText = TextBoxMain.Text;
            _userSettings.ReminderTime = int.TryParse(TextBoxReminderTime.Text, out int reminder) ? reminder : 0;
            _userSettings.RepeatTime = int.TryParse(TextBoxRepeatTime.Text, out int repeat) ? repeat : 0;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSettingsFromUI();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void StartReminderTimer()
        {
            _reminderTimer.Interval = TimeSpan.FromMinutes(_userSettings.ReminderTime);
            _reminderTimer.Start();
        }

        private void RestartReminderTimer()
        {
            _reminderTimer.Stop();
            _reminderTimer.Interval = TimeSpan.FromMinutes(_userSettings.ReminderTime);
            _reminderTimer.Start();
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            _reminderTimer.Stop();
            ShowNotificationWindow();
        }

        private void ShowNotificationWindow()
        {
            var notificationWindow = new NotificationWindow(_userSettings.MainText, _userSettings.RepeatTime);
            notificationWindow.Closed += NotificationWindow_Closed;
            notificationWindow.ShowDialog();
        }

        private void NotificationWindow_Closed(object sender, EventArgs e)
        {
            var window = sender as NotificationWindow;
            if (window?.IsPostponed == true)
            {
                _reminderTimer.Interval = TimeSpan.FromMinutes(_userSettings.RepeatTime);
            }
            else
            {
                _reminderTimer.Interval = TimeSpan.FromMinutes(_userSettings.ReminderTime);
            }
            _reminderTimer.Start();
        }

        private void SaveAndRestart_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            RestartReminderTimer();
        }
    }
}
