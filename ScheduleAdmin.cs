using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Xml.Serialization; 
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MaterialSkin;
using MaterialSkin.Controls;
using Excel = Microsoft.Office.Interop.Excel; //Для експорту в Excel

namespace ScheduleAdmin
{
    public partial class Schedule : MaterialForm
    {
        public Schedule()
        {
            InitializeComponent();
            //LoadSettings();
            // Завантаження списку нещодавно відкритих файлів при запуску програми
            LoadRecentFilesOnStartup();            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);
           
            if (trayIcon)
            {
                notifyIcon1.Visible = true;
            }
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
                this.Show();
            }

        }

        private void buttonCheckSchedule1_Click(object sender, EventArgs e)
        {
                dateTimePickerAdd.Value = dateTimePicker1.Value;
                LoadDataFromDatabase(dateTimePicker1, dataGridView1); //Використання функції з наданням вихідних об'єктів в аргументів
        }       

        // Подія для зміни значення DateTimePicker1
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            // Отримати нове значення DateTime з dateTimePicker1
            DateTime selectedDate = dateTimePicker1.Value;

            // Виклик методу для синхронізації значень DateTimePicker на інших вкладках
            SyncDateTimePickers(selectedDate);
        }
        private void dateTimePicker2_ValueChanged_1(object sender, EventArgs e)
        {
            // Отримати нове значення DateTime з dateTimePicker1
            DateTime selectedDate = dateTimePicker2.Value;

            // Виклик методу для синхронізації значень DateTimePicker на інших вкладках
            SyncDateTimePickers(selectedDate);
        }
        private void dateTimePicker3_ValueChanged_1(object sender, EventArgs e)
        {
            // Отримати нове значення DateTime з dateTimePicker1
            DateTime selectedDate = dateTimePicker3.Value;

            // Виклик методу для синхронізації значень DateTimePicker на інших вкладках
            SyncDateTimePickers(selectedDate);
        }
        private void dateTimePicker4_ValueChanged_1(object sender, EventArgs e)
        {
            // Отримати нове значення DateTime з dateTimePicker1
            DateTime selectedDate = dateTimePicker4.Value;

            // Виклик методу для синхронізації значень DateTimePicker на інших вкладках
            SyncDateTimePickers(selectedDate);
        }

        //Додавання запису
        private void buttonAddData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxAddGroup.Text == "" || textBoxAddSubject.Text == "" || textBoxAddTeacher.Text == "" || textBoxAddClassroom.Text == "")
            {
                MessageBox.Show("Будь ласка, введіть всі дані для запису в розклад.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var connection = new SQLiteConnection(connectionString);
            // Отримання значень з текстових полів
            string groupName = textBoxAddGroup.Text;
            string subjectName = textBoxAddSubject.Text;
            string teacherName = textBoxAddTeacher.Text;
            string classroom = textBoxAddClassroom.Text;
            string selectedDate = dateTimePickerAdd.Value.ToString("dd.MM.yyyy");
            string pair = comboBoxAddLesson.Text;
            string timeStart = GetTimeStartFromPair(pair);
            string timeEnd = GetTimeEndFromPair(pair);
            int pairIndex = GetPairIndex(pair);

            // Перевірка і додавання нової групи, якщо вона не існує
            CheckAndAddGroup(groupName);

            // Перевірка і додавання нового предмету, якщо він не існує
            CheckAndAddSubject(subjectName);

            // Перевірка і додавання нового викладача, якщо він не існує
            CheckAndAddTeacher(teacherName);

            // Перевірка наявності запису в таблиці Schedule для заданої групи та пари за розкладом
            bool recordExists = CheckIfRecordExists(groupName, selectedDate, pairIndex);

            if (recordExists)
            {
                // Якщо запис уже існує, запитати користувача, чи він бажає перезаписати дані
                DialogResult result = MessageBox.Show("Запис для обраної групи та пари за розкладом вже існує. Бажаєте перезаписати?", "Підтвердження перезапису", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    return;
                }
                // Якщо користувач погоджується, видалити існуючий запис
                string deleteQuery = "DELETE FROM Schedule WHERE group_id = (SELECT group_id FROM Groups WHERE group_name = @groupName) " +
                                     "AND date = @selectedDate AND time_start = @timeStart";
                using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@groupName", groupName);
                    deleteCommand.Parameters.AddWithValue("@selectedDate", selectedDate);
                    deleteCommand.Parameters.AddWithValue("@timeStart", timeStart);
                    connection.Open();
                    deleteCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }

            // Додавання нового рядка до таблиці Schedule
            string query = "INSERT INTO Schedule (group_id, subject_id, teacher_id, classroom, date, time_start, time_end, pair_index) " +
                           "VALUES (@group_id, @subject_id, @teacher_id, @classroom, @date, @time_start, @time_end, @pair_index)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                // Отримання group_id для введеної назви групи
                int groupId = GetGroupId(groupName);

                // Отримання subject_id для введеної назви предмету
                int subjectId = GetSubjectId(subjectName);

                // Отримання teacher_id для введеного імені викладача
                int teacherId = GetTeacherId(teacherName);

                command.Parameters.AddWithValue("@group_id", groupId);
                command.Parameters.AddWithValue("@subject_id", subjectId);
                command.Parameters.AddWithValue("@teacher_id", teacherId);
                command.Parameters.AddWithValue("@classroom", classroom);
                command.Parameters.AddWithValue("@date", selectedDate);
                command.Parameters.AddWithValue("@time_start", timeStart);
                command.Parameters.AddWithValue("@time_end", timeEnd);
                command.Parameters.AddWithValue("@pair_index", pairIndex);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Дані успішно додано до бази даних.");
            LoadDataFromDatabase(dateTimePickerAdd, dataGridView1);
        }


        //Вкладка видалення записів
        private void buttonDeleteEntry_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxDeleteGroup.Text == "" || comboBoxDeleteLesson.Text == "")
            {
                MessageBox.Show("Будь ласка, необхідні дані для видалення запису з розкладу.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Отримання значень з полів вибору
            string groupName = textBoxDeleteGroup.Text;
            string selectedDate = dateTimePicker2.Value.ToString("dd.MM.yyyy");
            int pairIndex = GetPairIndex(comboBoxDeleteLesson.Text);

            // Складання SQL-запиту для видалення записів
            string query = "DELETE FROM Schedule WHERE group_id = (SELECT group_id FROM Groups WHERE group_name = @groupName) " +
                           "AND date = @selectedDate AND pair_index = @pairIndex";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupName", groupName);
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    command.Parameters.AddWithValue("@pairIndex", pairIndex);

                    // Виконання SQL-запиту для видалення записів
                    int rowsAffected = command.ExecuteNonQuery();

                    // Перевірка кількості рядків, які були видалені
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Записи успішно видалено.", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Записи не було знайдено для видалення.", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                connection.Close();
            }
            LoadDataFromDatabase(dateTimePicker2, dataGridView2);
        }

        private void buttonCheckSchedule2_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker2, dataGridView2);
        }

        private void buttonDeleteDayEntry_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedDate = dateTimePicker2.Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі день.місяць.рік
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            string query = "DELETE FROM Schedule WHERE date = @selectedDate";
            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@selectedDate", selectedDate);
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Всі записи за вказаний день успішно видалено з бази даних.");
        }

        private void buttonCheckSchedule3_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker3, dataGridView3);
        }

        private void buttonGetSchedule_Click(object sender, EventArgs e)
        {
            // Отримання значення з текстового поля textBoxGroups
            string groupNameFilter = textBoxGroupSearch.Text;
            string teacherFilter = textBoxTeacherSearch.Text;
            string classroomFilter = textBoxClassroomSearch.Text;
            string subjectFilter = textBoxSubjectSearch.Text;

            // Виклик методу для завантаження даних у dataGridView4 з урахуванням фільтрації
            LoadFilteredDataFromDatabase(groupNameFilter, teacherFilter, classroomFilter, subjectFilter, dataGridView4);
        }

        private void buttonCheckSchedule4_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker4, dataGridView4);
        }

        // Дії кнопок у меню
        // Вибір і відкриття файлу бази даних
        private void обратиФайлБазиДанихToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SQL DB files(*.db)|*.db";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog1.FileName;
                OpenDatabaseFile(selectedFilePath);
            }
        }
        //Зберегти базу даних як окремий файл
        private void зберегтиЯкToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Вибір шляху та імені для збереження бази даних
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL DB files(*.db)|*.db";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Копіювання поточного файлу бази даних за обраний шлях та ім'я
                string sourcePath = selectedDatabasePath;
                string destinationPath = saveFileDialog.FileName;
                File.Copy(sourcePath, destinationPath, true);
                MessageBox.Show("Базу даних успішно збережено.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Створення резервної копії в папці з базою даних
        private void створитиРезервнуКопіюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірка, чи обрано файл бази даних
            if (!string.IsNullOrEmpty(selectedDatabasePath))
            {
                try
                {
                    // Визначення шляху до папки з резервними копіями
                    string backupFolderPath = Path.Combine(Path.GetDirectoryName(selectedDatabasePath), "Copy");

                    // Створення папки для резервних копій, якщо вона не існує
                    if (!Directory.Exists(backupFolderPath))
                    {
                        Directory.CreateDirectory(backupFolderPath);
                    }

                    // Формування нового шляху для резервної копії
                    string backupFileName = Path.GetFileNameWithoutExtension(selectedDatabasePath) + "_backup.db";
                    string backupFilePath = Path.Combine(backupFolderPath, backupFileName);

                    // Копіювання файлу бази даних у папку з резервними копіями
                    File.Copy(selectedDatabasePath, backupFilePath, true);

                    MessageBox.Show("Резервна копія створена успішно.", "Створення резервної копії", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    резервнаКопіяToolStripMenuItem.Checked = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталася помилка при створенні резервної копії: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    резервнаКопіяToolStripMenuItem.Checked = false;
                }
            }
            else
            {
                MessageBox.Show("Спочатку оберіть файл бази даних.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Відкриття в провіднику папки з резервною копією
        private void переглянутиРезервнуКопіюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірка, чи обрано файл бази даних
            if (!string.IsNullOrEmpty(selectedDatabasePath))
            {
                // Визначення шляху до папки з резервними копіями
                string backupFolderPath = Path.Combine(Path.GetDirectoryName(selectedDatabasePath), "Copy");

                // Перевірка, чи існує папка "Copy" у папці бази даних
                if (!Directory.Exists(backupFolderPath))
                {
                    // Якщо папка "Copy" не існує, відкрити папку бази даних
                    backupFolderPath = Path.GetDirectoryName(selectedDatabasePath);
                }

                // Відкриття папки з резервними копіями в провіднику
                System.Diagnostics.Process.Start("explorer.exe", backupFolderPath);
            }
            else
            {
                MessageBox.Show("Спочатку оберіть файл бази даних.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Очищення вмісту бази даних
        private void очиститиФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try {
                // Перевірка, чи обрано файл бази даних
                if (!string.IsNullOrEmpty(selectedDatabasePath))
                {
                    // Виведення попередження про очищення бази даних та підтвердження користувача
                    DialogResult result = MessageBox.Show("Ви справді хочете очистити базу даних?", "Попередження", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        SQLiteConnection connection = new SQLiteConnection($"Data Source={selectedDatabasePath}");
                        connection.Open();

                        // Очищення вмісту кожної таблиці
                        string[] tableNames = GetAllTableNames(connection);
                        foreach (string tableName in tableNames)
                        {
                            string query = $"DELETE FROM {tableName}";
                            SQLiteCommand command = new SQLiteCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Вміст бази даних очищено успішно.", "Очищення бази даних", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Спочатку оберіть файл бази даних.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка під час очищення бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Відкриття вікна налаштувань
        private void налаштуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.FormClosed += SettingsForm_FormClosed;
            settings.ShowDialog();
        }
        //Закриття вікна налаштувань
        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Після закриття форми налаштувань оновлюємо налаштування
            LoadSettings();
        }
        //Відкриття вікна "Про програму"
        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про програму
            AboutInfo aboutProgram = new AboutInfo();
            aboutProgram.ShowDialog();
        }
        // Відкриття вікна "Гайд"
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

        // Метод експорту у Еxcel
        private void excelxlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedDatabasePath=="")
            {
                MessageBox.Show("Оберіть базу даних та день для експорту.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            List<string> groupNames = GetAllGroupNames();
            LoadDataFromDatabase(dateTimePicker1, dataGridView1);
            // Перевірка наявності даних у DataGridView
            if (hasData(dataGridView1)==false)
            {
                MessageBox.Show("Немає даних для експорту.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else 
            {
                string currentDate = dateTimePicker1.Value.ToString("dd-MM-yyyy");

                // Додаємо назви груп у перший рядок
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (dataGridView1.Columns[j].HeaderText != null)
                    {
                        xlWorkSheet.Cells[1, j + 2] = dataGridView1.Columns[j].HeaderText;
                    }
                }

                // Додаємо назви пар за розкладом у перший стовпець
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].HeaderCell.Value != null)
                    {
                        xlWorkSheet.Cells[i + 2, 1] = dataGridView1.Rows[i].HeaderCell.Value.ToString();
                    }
                }

                // Копіюємо дані з dataGridView1
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value != null)
                        {
                            xlWorkSheet.Cells[i + 2, j + 2] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                }

                string savePath = Path.Combine(Path.GetDirectoryName(selectedDatabasePath), $"ScheduleExport_{currentDate}.xlsx");
                xlWorkBook.SaveAs(savePath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                MessageBox.Show("Експорт завершено!");
            }
            }

            private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
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

        private void переглядЗаписівToolStripMenuItem_Click(object sender, EventArgs e)
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

            tabControl1.SelectTab(tabPageCheckSchedule);
        }

        private void додаванняЗаписівToolStripMenuItem_Click(object sender, EventArgs e)
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

            tabControl1.SelectTab(tabPageAdd);
        }

        private void вийтиЗПрограмиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}

