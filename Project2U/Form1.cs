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
using MaterialSkin;
using MaterialSkin.Controls;

namespace Project2U
{
    public partial class Schedule : MaterialForm
    {
        // Збереження шляху до останньої використаної бази даних
        private string recentlyOpenedDatabase = "";

        public Schedule()
        {
            InitializeComponent();

            //Налаштування теми форми
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

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

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            // Set Border header  
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(48, 48, 48)), e.Bounds);
            Rectangle paddedBounds = e.Bounds;
            paddedBounds.Inflate(-2, -2);
            e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, this.Font, SystemBrushes.HighlightText, paddedBounds);

            //set  Tabcontrol border  
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.FromArgb(48, 48, 48), 10);
            g.DrawRectangle(p, tabPage1.Bounds);

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
            // Отримання поточної дати
            currentDate = DateTime.Today;
            // Перевірка, чи ввімкнуті сповіщення
            if (сповіщенняToolStripMenuItem.Checked)
            {
                if (!hasScheduleForToday(currentDate)) {
                    SendNotification("Повідомлення", "Це тестове повідомлення. Сьогодні пари відсутні."); //Якщо на сьогодні немає розкладу то виводиться таке повідомлення
                }
                else {
                    // Код для перевірки та відправлення тестового сповіщення
                    SendNotification("Повідомлення", "Це тестове повідомлення.");
                    CheckTime(currentDate);
                    //timer1.Interval = 10000; // Інтервал в мілісекундах (60000 мс = 1 хвилина)
                    timer1.Start();
                    //CheckAndSendNotification();
                }

            }
            else // Якщо сповіщення вимкнуті, зупинити таймер
            {
                timer1.Stop();
                // Скидаємо прапорець після кожної зміни параметру
                //IsNotificationSent = false;
            }
        }
        

        // Обробник події для перевірки та відправлення сповіщення
        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckTime(currentDate);    // Виклик методу для перевірки часу

                
        }

        //Налаштування кольорових тем
        MaterialSkinManager ThemeManager = MaterialSkinManager.Instance;

        private void стандартнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            стандартнаToolStripMenuItem.Checked = true;
            чорнаToolStripMenuItem.Checked = false;
            // Зміна кольорової теми на стандартну
            ChangeTheme(SystemColors.Control, SystemColors.ControlText);
            ThemeManager.Theme = MaterialSkinManager.Themes.LIGHT;
            this.tabControl1.DrawMode = TabDrawMode.Normal;
        }

        private void чорнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            стандартнаToolStripMenuItem.Checked = false;
            чорнаToolStripMenuItem.Checked ^= true;
            // Зміна кольорової теми на темну
           this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            System.Drawing.Color dark = System.Drawing.Color.FromArgb(255, 48, 48, 48);
            ChangeTheme(dark, SystemColors.ControlLightLight);
            ThemeManager.Theme = MaterialSkinManager.Themes.DARK;
        }

        private void ChangeTheme(Color backgroundColor, Color textColor)
        {
            // Зміна кольорів для форми та її компонентів
            this.BackColor = backgroundColor;
            this.ForeColor = textColor;
            this.menuStrip1.BackColor = backgroundColor;
            this.menuStrip1.ForeColor = textColor;
            this.tabControl1.BackColor = backgroundColor;
            this.tabControl1.ForeColor = textColor;

            // Зміна кольорів для кожної вкладки TabControl
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                tabPage.BackColor = backgroundColor;
                tabPage.ForeColor = textColor;
            }

            // Зміна кольорів для кожної таблиці DataGridView
            foreach (DataGridView dataGridView in new DataGridView[] { dataGridView1, dataGridView2, dataGridView3 })
            {
                dataGridView.BackgroundColor = backgroundColor;
                dataGridView.DefaultCellStyle.BackColor = backgroundColor;
                dataGridView.DefaultCellStyle.ForeColor = textColor;
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = backgroundColor;
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
                dataGridView.RowHeadersDefaultCellStyle.BackColor = backgroundColor;
                dataGridView.RowHeadersDefaultCellStyle.ForeColor = textColor;
            }

        }

        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про програму
            AboutInfo aboutProgram = new AboutInfo();
            aboutProgram.ShowDialog();
        }
    }
}
