using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                LoadPersonalSchedule(dateTimePicker1.Value.ToString("dd.MM.yyyy"));
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
                LoadPersonalSchedule(selectedDate);
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

        //Ввімкнення/вимкнення сповіщень
        private void сповіщенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Змінна для зберігання стану пункту меню
            bool notificationsEnabled = сповіщенняToolStripMenuItem.Checked;

            // Якщо сповіщення вмикнуті, встановити інтервал для таймера та запустити його
            if (notificationsEnabled)
            {
                // Код для перевірки та відправлення сповіщення
                SendNotification("Повідомлення", "Це тестове повідомлення.");

                timer1.Interval = 60000; // Інтервал в мілісекундах (60000 мс = 1 хвилина)
                timer1.Start();
            }
            else // Якщо сповіщення вимкнуті, зупинити таймер
            {
                timer1.Stop();
            }
        }
        // Обробник події для перевірки та відправлення сповіщення
        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckAndSendNotification();
        }
        private void CheckAndSendNotification()
        {
            // Отримання поточного часу
            DateTime currentTime = DateTime.Now;

            // Перевірка часу початку пари
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                string timeBeginHeader = GetTimeBeginHeader(i);
                DateTime timeBegin = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);

                // Якщо поточний час співпадає з часом початку пари, відправити сповіщення
                if (currentTime.TimeOfDay == timeBegin.TimeOfDay)
                {
                    SendNotification($"Початок {timeBeginHeader} пари", $"Початок {timeBeginHeader} пари згідно вашого розкладу.");
                    break;
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

        //Налаштування кольорових тем
        private void стандартнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void чорнаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
