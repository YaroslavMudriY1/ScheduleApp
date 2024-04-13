using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Globalization.DateTimeFormatting;

namespace Project2U
{
    public partial class Schedule : Form
    {
        // Збереження шляху до останньої використаної бази даних
        private string recentlyOpenedDatabase = "";

        public Schedule()
        {
            InitializeComponent();

            // Завантаження останньої використаної бази даних
            recentlyOpenedDatabase = LoadRecentDatabase();

            UpdateDateLabels(DateTime.Now); //Оновлення тексту за поточною датою

            // Перевірка наявності останньої використаної бази даних
            if (string.IsNullOrEmpty(recentlyOpenedDatabase))
            {
                // Якщо остання база даних відсутня, виводимо повідомлення та пропонуємо користувачеві вибрати файл
                DialogResult result = MessageBox.Show("Відсутній файл останньої використаної бази даних. Будь ласка, оберіть файл для відкриття.", "Повідомлення", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    // Виклик вікна відкриття файла бази даних
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "SQL DB files(*.db)|*.db";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        OpenDatabaseFile(selectedDatabasePath);
                        UpdateConnectionString();// Оновлюємо значення recentlyOpenedDatabase
                        SaveRecentDatabase(recentlyOpenedDatabase); // Зберігаємо шлях до бази даних
                    }
                }

            }
            else
            {
                // Якщо є остання база даних, відкриваємо її автоматично
                OpenDatabaseFile(recentlyOpenedDatabase);
            }
        }
    


    //Зміна дати
    private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadScheduleForDate(dateTimePicker1.Value);
            UpdateDateLabels(dateTimePicker1.Value);
            // Перевірка, чи користувач перейшов на вкладку "Мій розклад"
            if (tabControl1.SelectedTab == tabPage3)
            {
                // Виклик методу для завантаження персонального розкладу
                LoadPersonalSchedule(dateTimePicker1.Value.ToString("dd.MM.yyyy"), false);
            }
        }


        //Отримання фільтрованого розкладу
        private void buttonGetSchedule_Click(object sender, EventArgs e)
        {
            string selectedDate = dateTimePicker1.Value.ToString("dd MMMM yyyy р.");
            label2.Text = $"Розклад занять за {selectedDate}";

            string searchDate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
            // Отримання значення з текстового поля textBoxGroups
            string groupNameFilter = textBoxGroupSearch.Text;
            string teacherFilter = textBoxTeacherSearch.Text;
            string classroomFilter = textBoxClassroomSearch.Text;
            string subjectFilter = textBoxSubjectSearch.Text;

            // Очищення dataGridView3 перед відображенням нових даних
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            // Виклик методу для завантаження даних у dataGridView4 з урахуванням фільтрації
            LoadFilteredDataFromDatabase(groupNameFilter, teacherFilter, classroomFilter, subjectFilter, searchDate, dataGridView2);

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Перевірка, чи користувач перейшов на вкладку "Мій розклад"
            if (tabControl1.SelectedTab == tabPage3)
            {
                string selectedDate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                // Виклик методу для завантаження розкладу
                LoadPersonalSchedule(selectedDate, false);
            }
        }

        //Вибір та відкриття файлу бази даних
        private void обратиФайлБазиДанихToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SQL DB files(*.db)|*.db";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                OpenDatabaseFile(selectedFilePath);
                SaveRecentDatabase(selectedFilePath);
            }
        }

        private void нещодавноВідкритіToolStripMenuItem_Click(object sender, EventArgs e)
        {
/*            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string filePath = menuItem.Tag.ToString();
            OpenDatabaseFile(filePath);
            SaveRecentDatabase(filePath);*/
        }

        //Виклик вікна для введенях персональних даних користувача
        private void профільКористувачаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для зберігання даних користувача
            Authorization authorization= new Authorization();
            authorization.ShowDialog();
        }

        static bool IsNotificationSent = false;
        //Ввімкнення/вимкнення сповіщень
        private void сповіщенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Змінна для зберігання стану пункту меню
            bool notificationsEnabled = сповіщенняToolStripMenuItem.Checked;
            // Отримання поточної дати
            DateTime currentDate = dateTimePicker1.Value;//DateTime.Today;
            // Якщо сповіщення ввімкнути, встановити інтервал для таймера та запустити його
            if (notificationsEnabled)
            {
                if (!hasScheduleForToday(currentDate)) {
                    SendNotification("Повідомлення", "Це тестове повідомлення. Сьогодні пари відсутні."); //Якщо на сьогодні немає розкладу то виводиться таке повідомлення
                }
                else {
                    // Код для перевірки та відправлення тестового сповіщення
                    SendNotification("Повідомлення", "Це тестове повідомлення.");
                    timer1.Interval = 10000; // Інтервал в мілісекундах (60000 мс = 1 хвилина)
                    timer1.Start();
                }

            }
            else // Якщо сповіщення вимкнуті, зупинити таймер
            {
                timer1.Stop();
                // Скидаємо прапорець після кожної зміни параметру
                IsNotificationSent = false;
            }
        }
        // Обробник події для перевірки та відправлення сповіщення
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Виклик методу для перевірки та відправлення сповіщення
            if (!IsNotificationSent)
            CheckAndSendNotification();
        }

        // Метод для перевірки та відправлення сповіщення
        private void CheckAndSendNotification()
        {
            // Отримання поточної дати
            DateTime currentDate = dateTimePicker1.Value; //DateTime.Today;

            // Якщо є розклад на сьогоднішній день, перевірити час початку пари
            if (hasScheduleForToday(currentDate))
            {
                // Отримання поточного часу
                DateTime currentTime = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
                // Перевірка часу початку пари
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    // Отримання часу початку пари з першого стовпця
                    string timeBeginHeader = GetTimeBeginHeader(i); // Отримання часу початку пари
                    DateTime timeBegin = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                    string timeHeader = GetTimeHeader(i); // Отримання пари за порядком для сповіщення
                    DateTime endTime = GetEndTime(currentDate, timeBeginHeader); // Отримання часу закінчення пари
                    // Якщо поточний час співпадає з часом початку пари, відправити сповіщення
                    if (currentTime.AddMinutes(1) >= timeBegin && currentTime <= timeBegin.AddMinutes(5))
                    {
                        UserProfile userProfile = GetUserProfile();
                        string notificationTitle = $"{timeHeader}";
                            LoadPersonalSchedule(currentDate.ToString("dd.MM.yyyy"), true);
                        if (!String.IsNullOrEmpty(dataGridView3.Rows[i].Cells[1].Value as String))
                          {
                            // Отримання інформації про предмет, викладача та аудиторію з відповідних стовпців dataGridView3
                            string schedule = dataGridView3.Rows[i].Cells[1].Value.ToString();
                            string notificationBody = $"{userProfile.Name}, початок {i+1} пари.\n{schedule}";
                            SendNotification(notificationTitle, notificationBody);
                            IsNotificationSent = true;
                          }
                    }
                    // Якщо поточний час перевищує час закінчення пари, скидаємо прапорець
                    if (DateTime.Now.TimeOfDay >= endTime.TimeOfDay)
                    {
                        IsNotificationSent = false;
                    }
                }
            }
            else
            {
                IsNotificationSent = false; // Встановлюємо флаг IsNotificationSent в false, оскільки на сьогодні немає розкладу
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

        // Метод для отримання часу закінчення пари з бази даних
        private DateTime GetEndTime(DateTime currentDate, string timeBeginHeader)
        {
            DateTime endTime = DateTime.MinValue;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                string query = "SELECT time_end FROM Schedule " +
                               "WHERE date = @currentDate AND time_start = @timeBeginHeader";

                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentDate", currentDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@timeBeginHeader", timeBeginHeader);

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    endTime = DateTime.ParseExact(result.ToString(), "HH:mm", CultureInfo.InvariantCulture);
                }

                connection.Close();
            }

            return endTime;
        }

        //Налаштування кольорових тем
        private void стандартнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void чорнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
