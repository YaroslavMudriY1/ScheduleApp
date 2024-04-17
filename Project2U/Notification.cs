using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project2U
{
    public partial class  Schedule
    {
/*        static bool IsNotificationSent = false; // Прапорець "Чи надіслано сповіщення"
        DateTime currentClassStartTime; // Змінна для зберігання початку поточної пари
        DateTime currentClassEndTime;*/
        DateTime currentDate;

        // Метод для встановлення інтервалу таймера залежно від початку наступної пари
        private void SetTimerInterval(DateTime currentDate)
        {
            //DateTime currentTime = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

            DateTime nextClassStartTime = GetNextClassStartTime(currentDate);
            //DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
            TimeSpan timeUntilNextClass = nextClassStartTime - DateTime.Now;//DateTime.Now;
            timer1.Interval = (int)timeUntilNextClass.TotalMilliseconds;
        }
        // Метод для встановлення часу та перевірки поточного часу
        private void CheckTime(DateTime currentDate)
        {
            //DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
            DateTime nextClassStartTime = currentDate.AddDays(1);
            DateTime currentClassStartTime = currentDate;

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                string timeBeginHeader = GetTimeBeginHeader(row.Index);
                DateTime classStartTime = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);

                // Знаходимо час початку наступної пари
                if (classStartTime > DateTime.Now && classStartTime < nextClassStartTime)
                {
                    nextClassStartTime = classStartTime;
                }
                // Знаходимо час початку поточної пари
                if (classStartTime < DateTime.Now && DateTime.Now < nextClassStartTime)
                {
                    currentClassStartTime = classStartTime;
                }
            }
            TimeSpan timeUntilNextClass = nextClassStartTime - DateTime.Now;
            timer1.Interval = (int)timeUntilNextClass.TotalMilliseconds; //Оновлення інтервалу таймера
            CheckAndSendNotification();
            // Встановлюємо час початку та кінця поточної пари
            //currentClassEndTime = GetCurrentClassEndTime(currentDate, currentClassStartTime.ToString());

/*            // Якщо поточний час перевищує час початку наступної пари, оновлюємо дані для наступної пари
            if (DateTime.Now >= nextClassStartTime)
            {
                currentClassStartTime = nextClassStartTime;
                //currentClassEndTime = GetCurrentClassEndTime(currentDate, currentClassStartTime.ToString());
                IsNotificationSent = false; // Скидаємо прапорець, оскільки починається нова пара

            }*/

            /*            // Встановлюємо час початку поточної пари
                        currentClassStartTime = GetCurrentClassStartTime(currentDate);

                        // Перевірка чи поточний час знаходиться в межах поточної пари
                        if (!IsNotificationSent&&currentTime >= currentClassStartTime && currentTime <= currentClassEndTime)
                        {
                            CheckAndSendNotification(); 
                        }*/
        }

        // Метод для отримання часу початку наступної пари
        private DateTime GetNextClassStartTime(DateTime currentDate)
        {
            DateTime nextClassStartTime = currentDate.AddDays(1);

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                string timeBeginHeader = GetTimeBeginHeader(row.Index);
                DateTime classStartTime = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                //DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

                if (classStartTime > DateTime.Now && classStartTime < nextClassStartTime)
                {
                    nextClassStartTime = classStartTime;
                }
            }

            return nextClassStartTime;
        }

/*        // Метод для отримання часу закінчення пари з бази даних
        private DateTime GetCurrentClassEndTime(DateTime currentDate, string timeBeginHeader)
        {
            DateTime currentClassEndTime = currentDate.AddHours(1);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                string query = "SELECT time_end FROM Schedule WHERE date = @currentDate AND time_start = @timeBeginHeader";

                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentDate", currentDate.ToString("dd.MM.yyyy"));
                command.Parameters.AddWithValue("@timeBeginHeader", timeBeginHeader);

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    currentClassEndTime = DateTime.ParseExact(result.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                }

                connection.Close();
            }

            return currentClassEndTime;
        }*/
        // Метод для перевірки та відправлення сповіщення
        private void CheckAndSendNotification()
        {
            // Отримання поточної дати
            DateTime currentDate = DateTime.Today;

            // Отримання поточного часу
            //DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
                                                                                                        // Перевірка часу початку пари
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                // Отримання часу початку пари з першого стовпця
                string timeBeginHeader = GetTimeBeginHeader(i); // Отримання часу початку пари
                DateTime timeBegin = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                //DateTime endTime = GetCurrentClassEndTime(currentDate, timeBeginHeader); // Отримання часу закінчення пари
                // Якщо поточний час співпадає з часом початку пари, відправити сповіщення
                if (DateTime.Now.AddMinutes(5) >= timeBegin && DateTime.Now <= timeBegin.AddMinutes(5))
                {
                    UserProfile userProfile = GetUserProfile();
                    string notificationTitle = $"{GetTimeHeader(i)}";// Отримання пари за порядком для сповіщення
                    LoadPersonalSchedule(currentDate.ToString("dd.MM.yyyy"), true);
                    if (!String.IsNullOrEmpty(dataGridView3.Rows[i].Cells[1].Value as String))
                    {
                        // Отримання інформації про предмет, викладача та аудиторію з відповідних стовпців dataGridView3
                        string schedule = dataGridView3.Rows[i].Cells[1].Value.ToString();
                        string notificationBody = $"{userProfile.Name}, початок {i + 1} пари.\n{schedule}";
                        SendNotification(notificationTitle, notificationBody);
                        //IsNotificationSent = true;
                        break;
                    }
                }
            }

        }


        // Метод для відправлення сповіщення
        public void SendNotification(string title, string body)
        {
            // Створення вмісту сповіщення
            new ToastContentBuilder()
                .AddText(title)
                .AddText(body)
                .Show();
        }
    }
}
