namespace HealthTracker.Pages
{
    public class UserSettings
    {
        public string MainText { get; set; } = "Введите текст";
        public int ReminderTime { get; set; } = 10; // Время напоминания по умолчанию (в минутах)
        public int RepeatTime { get; set; } = 5;    // Время повтора по умолчанию (в минутах)
    }
}