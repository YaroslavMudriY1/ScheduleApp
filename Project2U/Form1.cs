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

        static bool IsNotificationSent = false; // Прапорець "Чи надіслано сповіщення"
        DateTime currentClassStartTime; // Змінна для зберігання початку поточної пари
        DateTime currentClassEndTime;
        DateTime currentDate;
        //Ввімкнення/вимкнення сповіщень
        private void сповіщенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Отримання поточної дати
            currentDate = dateTimePicker1.Value;//DateTime.Today;
            // Перевірка, чи ввімкнуті сповіщення
            if (сповіщенняToolStripMenuItem.Checked)
            {
                if (!hasScheduleForToday(currentDate)) {
                    SendNotification("Повідомлення", "Це тестове повідомлення. Сьогодні пари відсутні."); //Якщо на сьогодні немає розкладу то виводиться таке повідомлення
                }
                else {
                    // Код для перевірки та відправлення тестового сповіщення
                    SendNotification("Повідомлення", "Це тестове повідомлення.");
                    SetTimerInterval(currentDate);
                    //timer1.Interval = 10000; // Інтервал в мілісекундах (60000 мс = 1 хвилина)
                    timer1.Start();
                    CheckAndSendNotification();
                }

            }
            else // Якщо сповіщення вимкнуті, зупинити таймер
            {
                timer1.Stop();
                // Скидаємо прапорець після кожної зміни параметру
                IsNotificationSent = false;
            }
        }
        // Метод для встановлення інтервалу таймера залежно від початку наступної пари
        private void SetTimerInterval(DateTime currentDate)
        {
            //DateTime currentTime = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

            DateTime nextClassStartTime = GetNextClassStartTime(currentDate);
            DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
            TimeSpan timeUntilNextClass = nextClassStartTime - DateTime.Now;//DateTime.Now;
            timer1.Interval = (int)timeUntilNextClass.TotalMilliseconds;
        }

        

        // Обробник події для перевірки та відправлення сповіщення
        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckTime(currentDate);    // Виклик методу для перевірки часу
           // DateTime currentTime = DateTime.ParseExact("08:00", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

            // Перевірка чи треба відправити сповіщення
            if (!IsNotificationSent)
                //if ( DateTime.Now >= currentClassStartTime && DateTime.Now < currentClassEndTime)
            {
                CheckAndSendNotification();
            }
        }

        // Метод для встановлення часу та перевірки поточного часу
        private void CheckTime(DateTime currentDate)
        {
            DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
            DateTime nextClassStartTime = currentDate.AddDays(1);
            DateTime currentClassStartTime = currentDate.AddHours(-1);

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
            // Встановлюємо час початку та кінця поточної пари
            currentClassEndTime = GetCurrentClassEndTime(currentDate, currentClassStartTime.ToString());

            // Якщо поточний час перевищує час початку наступної пари, оновлюємо дані для наступної пари
            if (DateTime.Now >= nextClassStartTime)
            {
                currentClassStartTime = nextClassStartTime;
                currentClassEndTime = GetCurrentClassEndTime(currentDate, currentClassStartTime.ToString());
                IsNotificationSent = false; // Скидаємо прапорець, оскільки починається нова пара

            }

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
                DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

                if (classStartTime > DateTime.Now && classStartTime < nextClassStartTime)
                {
                    nextClassStartTime = classStartTime;
                }
            }

            return nextClassStartTime;
        }

        // Метод для отримання часу початку поточної пари
        private DateTime GetCurrentClassStartTime(DateTime currentDate)
        {
            DateTime currentClassStartTime = DateTime.MaxValue;

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                string timeBeginHeader = GetTimeBeginHeader(row.Index);
                DateTime classStartTime = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;

                if (classStartTime < DateTime.Now && DateTime.Now < GetNextClassStartTime(currentDate))
                {
                    currentClassStartTime = classStartTime;
                }
            }

            return currentClassStartTime;
        }

        // Метод для отримання часу закінчення пари з бази даних
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
        }
        // Метод для перевірки та відправлення сповіщення
        private void CheckAndSendNotification()
        {
            // Отримання поточної дати
            DateTime currentDate = dateTimePicker1.Value; //DateTime.Today;

                    // Отримання поточного часу
                    DateTime currentTime = DateTime.ParseExact("07:59", "HH:mm", CultureInfo.InvariantCulture); //DateTime.Now;
                // Перевірка часу початку пари
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    // Отримання часу початку пари з першого стовпця
                    string timeBeginHeader = GetTimeBeginHeader(i); // Отримання часу початку пари
                    DateTime timeBegin = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                    DateTime endTime = GetCurrentClassEndTime(currentDate, timeBeginHeader); // Отримання часу закінчення пари
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
                        IsNotificationSent = true;
                        break;
                    }
                }
                    // Якщо поточний час перевищує час закінчення пари, скидаємо прапорець
/*                    if (DateTime.Now.TimeOfDay >= endTime.TimeOfDay)
                    {
                        IsNotificationSent = false;
                    }*/
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


        //Налаштування кольорових тем
        private void стандартнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void чорнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
