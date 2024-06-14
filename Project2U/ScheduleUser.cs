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
using MaterialSkin; // Підключення пакету для покращення інтерфейсу
using MaterialSkin.Controls;
using Windows.Data.Xml.Dom;

namespace ScheduleUser
{
    public partial class Schedule : MaterialForm
    {

        public Schedule()
        {
            InitializeComponent();
            LoadSettings();
            // Завантаження списку нещодавно відкритих файлів при запуску програми
            LoadRecentFilesOnStartup();
            //Налаштування теми форми
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

            if (trayIcon)
            {
                notifyIcon1.Visible = true;
            }

            UpdateDateLabels(DateTime.Now); //Оновлення тексту за поточною датою

            // Перевірка наявності останньої використаної бази даних
            if (recentlyOpenedFiles.Count == 0 || string.IsNullOrEmpty(recentlyOpenedFiles[0]))
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
                        string selectedDatabasePath = openFileDialog.FileName;
                        OpenDatabaseFile(selectedDatabasePath);
                        UpdateConnectionString(); // Оновлюємо значення підключення до бази даних
                    }
                }
            }
            else
            {
                if (autoLastConnected)
                {
                    // Якщо є остання база даних, відкриваємо її автоматично
                    OpenDatabaseFile(recentlyOpenedFiles[0]);
                }
            }
        }
        // Зміна вигляду вкладок
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
            }
        }


        //Виклик вікна для введенях персональних даних користувача
        private void профільКористувачаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для зберігання даних користувача
            Authorization authorization = new Authorization();
            authorization.AuthorizationClosed += Authorization_Closed; // Підписка на подію

            authorization.ShowDialog();
        }

        private void Authorization_Closed()
        {
            LoadPersonalSchedule(dateTimePicker1.Value.ToString("dd.MM.yyyy"), false);
        }

        //Ввімкнення/вимкнення сповіщень
        public void сповіщенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Оновлюємо стан елемента в панелі задач
            toolStripMenuItem4.Checked = сповіщенняToolStripMenuItem.Checked;
            // Отримання поточної дати
            currentDate = DateTime.Today;
            // Перевірка, чи ввімкнуті сповіщення
            if (сповіщенняToolStripMenuItem.Checked)
            {
                if (!hasScheduleForToday(currentDate))
                {
                    SendNotification("Повідомлення", "Це тестове повідомлення. Сьогодні пари відсутні."); //Якщо на сьогодні немає розкладу то виводиться таке повідомлення
                }
                else
                {
                    // Код для перевірки та відправлення тестового сповіщення
                    SendNotification("Повідомлення", "Це тестове повідомлення.");
                    CheckTime(currentDate);
                    timer1.Start();
                }

            }
            else // Якщо сповіщення вимкнуті, зупинити таймер
            {
                timer1.Stop();
            }
        }


        // Обробник події для перевірки та відправлення сповіщення
        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckTime(currentDate);    // Виклик методу для перевірки часу
        }

        //Налаштування кольорових тем
        MaterialSkinManager ThemeManager = MaterialSkinManager.Instance;
        private bool isDarkTheme=false;

        private void стандартнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            стандартнаToolStripMenuItem.Checked = true;
            чорнаToolStripMenuItem.Checked = false;
            isDarkTheme = false;
            // Зміна кольорової теми на стандартну
            ChangeTheme(SystemColors.Control, SystemColors.ControlText);
            ThemeManager.Theme = MaterialSkinManager.Themes.LIGHT;
            this.файлToolStripMenuItem.ForeColor = SystemColors.ControlText;
            this.налаштуванняToolStripMenuItem.ForeColor = SystemColors.ControlText;
            this.довідкаToolStripMenuItem.ForeColor = SystemColors.ControlText;
        }

        private void чорнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            стандартнаToolStripMenuItem.Checked = false;
            чорнаToolStripMenuItem.Checked ^= true;
            isDarkTheme = true;
            // Зміна кольорової теми на темну
            System.Drawing.Color dark = System.Drawing.Color.FromArgb(255, 48, 48, 48);
            ChangeTheme(dark, SystemColors.ControlLightLight);
            ThemeManager.Theme = MaterialSkinManager.Themes.DARK;
            this.файлToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
            this.налаштуванняToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
            this.довідкаToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
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
            this.buttonGetSchedule.BackColor = backgroundColor;

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

        // Зміна кольорів тексту меню при відкритті
        private void dropDownOpenedItemClick1(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
            this.файлToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(255, 48, 48, 48);
        }
        private void dropDownClosedItemClick1(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
                this.файлToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
        }
        private void dropDownOpenedItemClick2(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
                this.налаштуванняToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(255, 48, 48, 48);
        }
        private void dropDownClosedItemClick2(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
                this.налаштуванняToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
        }
        private void dropDownOpenedItemClick3(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
                this.довідкаToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(255, 48, 48, 48);
        }
        private void dropDownClosedItemClick3(object sender, EventArgs e)
        {
            if (чорнаToolStripMenuItem.Checked == true)
                this.довідкаToolStripMenuItem.ForeColor = SystemColors.ControlLightLight;
        }

        //Іконка в треї
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Перевіряємо, чи вікно програми згорнуто
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Розгортаємо вікно
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // Відкриваємо вікно, якщо воно закрито
                this.Show();
            }

            // Активуємо вікно
            this.Activate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи вікно програми згорнуто
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Розгортаємо вікно
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // Відкриваємо вікно, якщо воно закрито
                this.Show();
            }

            // Активуємо вікно
            this.Activate();

            tabControl1.SelectTab(tabPage1);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи вікно програми згорнуто
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Розгортаємо вікно
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // Відкриваємо вікно, якщо воно закрито
                this.Show();
            }

            // Активуємо вікно
            this.Activate();
            tabControl1.SelectTab(tabPage3);
        }

        // Обробник події кнопки "Вихід з програми"
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) {
            // Інвертуємо стан елемента меню
            сповіщенняToolStripMenuItem.Checked = toolStripMenuItem4.Checked;
            if (toolStripMenuItem4.Checked)
            {
                сповіщенняToolStripMenuItem_Click(sender, e);
            }
                
    }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про програму
            AboutInfo aboutProgram = new AboutInfo(isDarkTheme);
            aboutProgram.ShowDialog();
        }

        private void гайдКористуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірити, чи існує вже вікно "Гайд"
            Guide guideInstance = null;

            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Guide)
                {
                    guideInstance = openForm as Guide;
                    break;
                }
            }

            // Якщо вікно не існує, створити нове
            if (guideInstance == null)
            {
                Guide guide = new Guide();
                guide.Show();
            }
            // Інакше, активувати існуюче вікно
            else
            {
                guideInstance.Activate();
            }
        }

        private void змінитиНалаштуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(isDarkTheme);
            settings.FormClosed += SettingsForm_FormClosed;
            settings.ShowDialog();
        }
        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Після закриття форми налаштувань оновлюємо налаштування
            LoadSettings();
        }
        private void Schedule_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Перевірка, чи хоче користувач закрити програму
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Вивести діалогове вікно з підтвердженням
                DialogResult result = MessageBox.Show("Ви впевнені, що хочете закрити програму?", "Закрити програму", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Якщо користувач підтверджує закриття, 
                // збережіть дані або виконайте інші дії перед закриттям
                if (result == DialogResult.Yes)
                {
                    // Якщо встановлене налаштування, видалити файл профілю користувача
                    if (deleteUserProfile)
                    {
                        File.Delete("UserProfile.xml");
                    }
                    // Закрити програму
                    Application.Exit();
                }
                else
                {
                    // Скасувати закриття форми
                    e.Cancel = true;
                }
            }
        }

    };
}
