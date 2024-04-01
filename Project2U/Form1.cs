using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            dateTimePicker1.Value = DateTime.Now;

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
                        selectedDatabasePath = openFileDialog.FileName;
                        OpenDatabaseFile(openFileDialog.FileName);
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
        }

        private void dateTimePicker2_ValueChanged_1(object sender, EventArgs e)
        {
            string dateString = dateTimePicker2.Value.ToString("dd MMMM yyyy р.");
            label2.Text = $"Розклад занять за {dateString}";

        }

        //Отримання фільтрованого розкладу
        private void buttonGetSchedule_Click(object sender, EventArgs e)
        {
            // Отримання значення з текстового поля textBoxGroups
            string groupNameFilter = textBoxGroupSearch.Text;

            string teacherFilter = textBoxTeacherSearch.Text;
            string classroomFilter = textBoxClassroomSearch.Text;
            string subjectFilter = textBoxSubjectSearch.Text;

            // Виклик методу для завантаження даних у dataGridView4 з урахуванням фільтрації
            LoadFilteredDataFromDatabase(groupNameFilter, teacherFilter, classroomFilter, subjectFilter);

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

        }

        //Ввімкнення/вимкнення сповіщень
        private void сповіщенняToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
