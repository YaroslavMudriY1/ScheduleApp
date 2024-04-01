using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Xml.Serialization; //Для зберігання списку недавніх
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Project2
{
    public partial class Schedule : Form
    {
        // Шлях до файлу конфігурації
        private string configFilePath = "recentFilesConfig.xml";
        private List<string> recentlyOpenedFiles;
        // Завантаження списку нещодавно відкритих файлів
        private List<string> LoadRecentFiles()
        {
            if (File.Exists(configFilePath))
            {
                using (StreamReader reader = new StreamReader(configFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RecentFilesConfig));
                    RecentFilesConfig config = (RecentFilesConfig)serializer.Deserialize(reader);
                    return config.RecentlyOpenedFiles;
                }
            }
            return new List<string>();
        }

        // Збереження списку нещодавно відкритих файлів
        private void SaveRecentFiles(List<string> recentlyOpenedFiles)
        {
            RecentFilesConfig config = new RecentFilesConfig();
            config.RecentlyOpenedFiles = recentlyOpenedFiles;
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RecentFilesConfig));
                serializer.Serialize(writer, config);
            }
        }

        // private List<string> recentlyOpenedFiles = new List<string>(); // Список нещодавно відкритих файлів
        // string connectionString = "Data Source=db2.db;Version=3;";
        private static string selectedDatabasePath = "";
        string connectionString = $"Data Source={selectedDatabasePath};Version=3;";

        private void UpdateConnectionString()
        {
            connectionString = $"Data Source={selectedDatabasePath};Version=3;";
        }
        public Schedule()
        {
            InitializeComponent();
            // Завантаження списку нещодавно відкритих файлів при запуску програми
            LoadRecentFilesOnStartup();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Метод для отримання всіх доступних назв груп з бази даних
        private List<string> GetAllGroupNames()
        {
            List<string> groupNames = new List<string>();
            string query = "SELECT group_name FROM Groups";

            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                string groupName = reader["group_name"].ToString();
                groupNames.Add(groupName);
            }

            return groupNames;
        }

        //Фунція читання бази даних

        private void LoadDataFromDatabase(DateTimePicker dateTimePicker, DataGridView dataGridView)
        {

            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedDate = dateTimePicker.Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі день.місяць.рік
                                                                               // Отримання всіх доступних назв груп з бази даних
            List<string> groupNames = GetAllGroupNames();

            var connection = new SQLiteConnection(connectionString);
            // Відкриття підключення до бази даних
            connection.Open();
            // Очистка DataGridView перед відображенням нових даних
            dataGridView.Rows.Clear();

            // Перевірка наявності стовпців з назвами груп та їх додавання
            foreach (string groupName in groupNames)
            {
                if (!dataGridView.Columns.Cast<DataGridViewColumn>().Any(column => column.HeaderText == groupName))
                {
                    dataGridView.Columns.Add(groupName, groupName);
                }
            }
            // Перевірка наявності стовпців у DataGridView перед додаванням рядків
            if (dataGridView.Columns.Count == 0)
            {
                MessageBox.Show("База даних порожня.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Перевірка наявності шостої пари
                bool hasSixthPair = CheckSixthPairAvailability(selectedDate);

                // Додавання рядків у випадку наявності шостої пари
                int rowsCount = hasSixthPair ? 6 : 5;
                for (int i = 0; i < rowsCount; i++)
                {
                    dataGridView.Rows.Add();
                    dataGridView.Rows[i].HeaderCell.Value = $"{i + 1} пара";
                }


                // Запит до бази даних для отримання розкладу на обрану дату
                string query = "SELECT * FROM Schedule WHERE date = @selectedDate ORDER BY time_start";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@selectedDate", selectedDate);
                SQLiteDataReader reader = command.ExecuteReader();

                // Логіка для заповнення DataGridView з результатами запиту
                if (reader.HasRows)
                {
                    // Пройтись по всіх рядках результатів запиту
                    while (reader.Read())
                    {
                        // Отримати ідентифікатор групи
                        int groupId = Convert.ToInt32(reader["group_id"]);

                        // Отримати ім'я групи за допомогою методу GetGroupName
                        string groupName = GetGroupName(groupId);

                        // Отримати дані про предмет, викладача, аудиторію
                        int subjectId = Convert.ToInt32(reader["subject_id"]);
                        int teacherId = Convert.ToInt32(reader["teacher_id"]);
                        string classroom = reader["classroom"].ToString();
                        string timeStart = reader["time_start"].ToString();

                        // Отримати індекс стовпця, в який буде додана інформація
                        int columnIndex = GetColumnIndex(dataGridView, groupName);


                        // Отримати інформацію про предмет та викладача з відповідних таблиць
                        if (columnIndex >= 0)
                        {
                            string subject = GetSubjectName(subjectId);
                            string teacher = GetTeacherName(teacherId);

                            int rowIndex = GetRowIndex(timeStart);
                            if (rowIndex >= 0)
                            {
                                if (dataGridView.Rows.Count <= rowIndex)
                                {
                                    dataGridView.Rows.Add();
                                    dataGridView.Rows[rowIndex].HeaderCell.Value = GetTimeHeader(rowIndex);
                                }

                                if (dataGridView.Columns.Count <= columnIndex)
                                {
                                    dataGridView.Columns.Add(groupName, groupName);
                                }

                                dataGridView.Rows[rowIndex].Cells[columnIndex].Value = subject + "\n" + teacher + "\n" + classroom;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("За вказаною датою відсутні записи", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // Закриття підключення до бази даних
            connection.Close();

        }
        private bool CheckSixthPairAvailability(string selectedDate)
        {
            bool hasSixthPair = false;

            // SQL-запит для перевірки наявності шостої пари
            string query = "SELECT COUNT(*) FROM Schedule WHERE date = @selectedDate AND time_start = '14:30'";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Якщо є записи з часом початку "13:30", то шоста пара доступна
                    hasSixthPair = count > 0;
                }
            }

            return hasSixthPair;
        }

        private void buttonCheckSchedule1_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker1, dataGridView1); //Використання функції з наданням вихідних об'єктів в аргументів
        }

        // Метод, який встановлює значення DateTimePicker на інших вкладках
        private void SyncDateTimePickers(DateTime dateTime)
        {
            dateTimePicker1.Value = dateTime;
            dateTimePicker2.Value = dateTime;
            dateTimePicker3.Value = dateTime;
            dateTimePicker4.Value = dateTime;
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

        // Метод для отримання назви групи за ідентифікатором групи
        private string GetGroupName(int groupId)
        {
            string query = "SELECT group_name FROM Groups WHERE group_id = @groupId";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupId", groupId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }
        // Метод для отримання індексу стовпця за назвою групи
        private int GetColumnIndex(DataGridView dataGridView, string groupName)
        {
            // Перевірка, чи наданий dataGridView не є нульовим
            if (dataGridView == null)
            {
                return -1; // Повертаємо -1 у випадку, якщо dataGridView нульовий
            }

            // Припустимо, що у dataGridView стовпці названі як групи
            return dataGridView.Columns.Cast<DataGridViewColumn>().ToList().FindIndex(column => column.HeaderText == groupName);
        }

        // Метод для отримання назви предмету за його ідентифікатором
        private string GetSubjectName(int subjectId)
        {
            string query = "SELECT subject_name FROM Subjects WHERE subject_id = @subjectId";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@subjectId", subjectId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        // Метод для отримання прізвища викладача за його ідентифікатором
        private string GetTeacherName(int teacherId)
        {
            string query = "SELECT teacher_name FROM Teachers WHERE teacher_id = @teacherId";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teacherId", teacherId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        // Метод для отримання індексу рядка за часом початку пари
        private int GetRowIndex(string timeStart)
        {
            // Реалізація з урахуванням розкладу
            switch (timeStart)
            {
                case "08:00":
                    return 0;
                case "09:15":
                    return 1;
                case "10:30":
                    return 2;
                case "12:00":
                    return 3;
                case "13:15":
                    return 4;
                case "14:30":
                    return 5;
                default:
                    throw new ArgumentException("Invalid time start: " + timeStart);
            }
        }

        // Метод для визначення значення рядкового заголовку для заданого індексу часу
        private string GetTimeHeader(int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return "1 пара";
                case 1:
                    return "2 пара";
                case 2:
                    return "3 пара";
                case 3:
                    return "4 пара";
                case 4:
                    return "5 пара";
                case 5:
                    return "6 пара"; // Додано рядок для шостої пари
                default:
                    throw new ArgumentException("Invalid time row index: " + rowIndex);
            }
        }

        // Метод для визначення значення часу за обраним варіантом
        private string GetTimeStartFromPair(string text)
        {
            switch (text)
            {
                case "Перша пара":
                    return "08:00";
                case "Друга пара":
                    return "09:15";
                case "Третя пара":
                    return "10:30";
                case "Четверта пара":
                    return "12:00";
                case "П'ята пара":
                    return "13:15";
                case "Шоста пара":
                    return "14:30";
                default:
                    throw new ArgumentException("Invalid time: " + text);
            }
        }

        // Метод для визначення значення часу закінчення за обраним варіантом
        private string GetTimeEndFromPair(string text)
        {
            switch (text)
            {
                case "Перша пара":
                    return "09:00";
                case "Друга пара":
                    return "10:15";
                case "Третя пара":
                    return "11:30";
                case "Четверта пара":
                    return "13:00";
                case "П'ята пара":
                    return "14:15";
                case "Шоста пара":
                    return "15:30";
                default:
                    throw new ArgumentException("Invalid time: " + text);
            }
        }


        //Додавання запису
        private void buttonAddData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var connection = new SQLiteConnection(connectionString);
            // Отримання значень з текстових полів
            string groupName = textBox1.Text;
            string subjectName = textBox2.Text;
            string teacherName = textBox3.Text;
            string classroom = textBox4.Text;
            string selectedDate = dateTimePickerAdd.Value.ToString("dd.MM.yyyy");
            string timeStart = GetTimeStartFromPair(comboBox1.Text);
            string timeEnd = GetTimeEndFromPair(comboBox1.Text);

            // Перевірка і додавання нової групи, якщо вона не існує
            CheckAndAddGroup(groupName);

            // Перевірка і додавання нового предмету, якщо він не існує
            CheckAndAddSubject(subjectName);

            // Перевірка і додавання нового викладача, якщо він не існує
            CheckAndAddTeacher(teacherName);

            // Додавання нового рядка до таблиці Schedule
            string query = "INSERT INTO Schedule (group_id, subject_id, teacher_id, classroom, date, time_start, time_end) " +
                               "VALUES (@group_id, @subject_id, @teacher_id, @classroom, @date, @time_start, @time_end)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {                // Отримання group_id для введеної назви групи
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

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Дані успішно додано до бази даних.");
        }

        //Методи

        // Метод для перевірки наявності та додавання нової групи до таблиці Groups
        private void CheckAndAddGroup(string groupName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT COUNT(*) FROM Groups WHERE group_name = @group_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@group_name", groupName);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (count == 0)
                {
                    // Додавання нової групи до таблиці Groups
                    query = "INSERT INTO Groups (group_name) VALUES (@group_name)";
                    using (SQLiteCommand addGroupCommand = new SQLiteCommand(query, connection))
                    {
                        addGroupCommand.Parameters.AddWithValue("@group_name", groupName);
                        connection.Open();
                        addGroupCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }

        // Метод для перевірки наявності та додавання нового предмету до таблиці Subjects
        private void CheckAndAddSubject(string subjectName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT COUNT(*) FROM Subjects WHERE subject_name = @subject_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@subject_name", subjectName);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (count == 0)
                {
                    // Додавання нового предмету до таблиці Subjects
                    query = "INSERT INTO Subjects (subject_name) VALUES (@subject_name)";
                    using (SQLiteCommand addSubjectCommand = new SQLiteCommand(query, connection))
                    {
                        addSubjectCommand.Parameters.AddWithValue("@subject_name", subjectName);
                        connection.Open();
                        addSubjectCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }

        // Метод для перевірки наявності та додавання нового викладача до таблиці Teachers
        private void CheckAndAddTeacher(string teacherName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT COUNT(*) FROM Teachers WHERE teacher_name = @teacher_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@teacher_name", teacherName);
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (count == 0)
                {
                    // Додавання нового викладача до таблиці Teachers
                    query = "INSERT INTO Teachers (teacher_name) VALUES (@teacher_name)";
                    using (SQLiteCommand addTeacherCommand = new SQLiteCommand(query, connection))
                    {
                        addTeacherCommand.Parameters.AddWithValue("@teacher_name", teacherName);
                        connection.Open();
                        addTeacherCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }

        // Метод для отримання group_id за назвою групи
        private int GetGroupId(string groupName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT group_id FROM Groups WHERE group_name = @group_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@group_name", groupName);
                connection.Open();
                int groupId = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return groupId;
            }
        }

        // Метод для отримання subject_id за назвою предмету
        private int GetSubjectId(string subjectName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT subject_id FROM Subjects WHERE subject_name = @subject_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@subject_name", subjectName);
                connection.Open();
                int subjectId = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return subjectId;
            }
        }

        // Метод для отримання teacher_id за ім'ям викладача
        private int GetTeacherId(string teacherName)
        {
            var connection = new SQLiteConnection(connectionString);
            string query = "SELECT teacher_id FROM Teachers WHERE teacher_name = @teacher_name";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@teacher_name", teacherName);
                connection.Open();
                int teacherId = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return teacherId;
            }
        }

        //Вкладка видалення записів
        private void buttonDeleteEntry_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Отримання значень з полів вибору
            string groupName = textBox5.Text;
            string selectedDate = dateTimePicker2.Value.ToString("dd.MM.yyyy");
            string timeStart = GetTimeStartFromPair(comboBox2.Text);

            // Складання SQL-запиту для видалення записів
            string query = "DELETE FROM Schedule WHERE group_id = (SELECT group_id FROM Groups WHERE group_name = @groupName) " +
                           "AND date = @selectedDate AND time_start = @timeStart";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupName", groupName);
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    command.Parameters.AddWithValue("@timeStart", timeStart);

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

        // Метод для завантаження даних з бази даних з урахуванням фільтрації
        private void LoadFilteredDataFromDatabase(string groupNameFilter, string teacherFilter, string classroomFilter, string subjectFilter, DataGridView dataGridView)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedDate = dateTimePicker4.Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі день.місяць.рік

            var connection = new SQLiteConnection(connectionString);
            // Відкриття підключення до бази даних
            connection.Open();
            // Очистка DataGridView перед відображенням нових даних
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            // Отримання всіх доступних назв груп з бази даних
            List<string> groupNames = GetAllGroupNames();
            // Зберігаємо групи, які вже мають розклад з потрібним вчителем або предметом
            HashSet<string> groupsWithFilteredData = new HashSet<string>();

            // Перевірка груп на наявність розкладу з потрібним вчителем або предметом
            foreach (string groupName in groupNames)
            {
                bool groupContainsFilteredData = CheckGroupForFilteredData(groupName, selectedDate, teacherFilter, subjectFilter, connection);
                if (groupContainsFilteredData)
                {
                    groupsWithFilteredData.Add(groupName);
                }
            }

            // Додавання стовпців до DataGridView лише для груп, що містять потрібні дані
            foreach (string groupName in groupsWithFilteredData)
            {
                if (groupName.Contains(groupNameFilter))
                {
                    dataGridView.Columns.Add(groupName, groupName);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i].HeaderCell.Value = $"{i + 1} пара";
            }

            // Запит до бази даних для отримання розкладу з урахуванням фільтрації
            string query = "SELECT * FROM Schedule WHERE date = @selectedDate";

            // Додавання умов фільтрації за параметрами, якщо вони вказані
            if (!string.IsNullOrWhiteSpace(textBoxGroupSearch.Text))
            {
                query += " AND group_id IN (SELECT group_id FROM Groups WHERE group_name LIKE @groupName)";
            }

            if (!string.IsNullOrEmpty(teacherFilter))
            {
                query += " AND teacher_id IN (SELECT teacher_id FROM Teachers WHERE teacher_name LIKE @teacherFilter)";
            }

            if (!string.IsNullOrEmpty(classroomFilter))
            {
                query += " AND classroom LIKE @classroomFilter";
            }

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                query += " AND subject_id IN (SELECT subject_id FROM Subjects WHERE subject_name LIKE @subjectFilter)";
            }

            query += " ORDER BY time_start";

            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@selectedDate", selectedDate);

            // Додавання параметрів фільтрації за групою, якщо вона вказана
            if (!string.IsNullOrWhiteSpace(textBoxGroupSearch.Text))
            {
                command.Parameters.AddWithValue("@groupName", $"%{textBoxGroupSearch.Text}%");
            }

            if (!string.IsNullOrEmpty(teacherFilter))
            {
                command.Parameters.AddWithValue("@teacherFilter", "%" + teacherFilter + "%");
            }

            if (!string.IsNullOrEmpty(classroomFilter))
            {
                command.Parameters.AddWithValue("@classroomFilter", classroomFilter);
            }

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                command.Parameters.AddWithValue("@subjectFilter", "%" + subjectFilter + "%");
            }

            SQLiteDataReader reader = command.ExecuteReader();

            // Логіка для заповнення DataGridView з результатами запиту
            while (reader.Read())
            {
                // Отримати ідентифікатор групи
                int groupId = Convert.ToInt32(reader["group_id"]);

                // Отримати ім'я групи за допомогою методу GetGroupName
                string groupName = GetGroupName(groupId);

                // Перевірити, чи група містить потрібні дані
                if (groupsWithFilteredData.Contains(groupName))
                {

                    // Отримати дані про предмет, викладача, аудиторію
                    int subjectId = Convert.ToInt32(reader["subject_id"]);
                    int teacherId = Convert.ToInt32(reader["teacher_id"]);
                    string classroom = reader["classroom"].ToString();
                    string timeStart = reader["time_start"].ToString();

                    string subject = GetSubjectName(subjectId);
                    string teacher = GetTeacherName(teacherId);

                    // Отримати індекс стовпця, в який буде додана інформація
                    int columnIndex = dataGridView.Columns.Contains(groupName) ? dataGridView.Columns[groupName].Index : -1;

                    // Якщо стовпець не знайдено, додати новий
                    if (columnIndex == -1)
                    {
                        columnIndex = dataGridView.Columns.Add(groupName, groupName);
                    }

                    // Отримати індекс рядка або додати новий, якщо не існує
                    int rowIndex = GetRowIndex(timeStart);
                    if (rowIndex >= 0)
                    {
                        if (dataGridView.Rows.Count <= rowIndex)
                        {
                            dataGridView.Rows.Add();
                            dataGridView.Rows[rowIndex].HeaderCell.Value = GetTimeHeader(rowIndex);
                        }

                        dataGridView.Rows[rowIndex].Cells[columnIndex].Value = GetSubjectName(subjectId) + "\n" + GetTeacherName(teacherId) + "\n" + classroom;
                    }


                    else
                    {
                        MessageBox.Show("За вказаною датою відсутні записи", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("За вказаними фільтрами відсутні записи", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // Закриття підключення до бази даних
            connection.Close();
        }
        // Метод для перевірки наявності потрібних даних у розкладі групи за вибраний день
        private bool CheckGroupForFilteredData(string groupName, string selectedDate, string teacherFilter, string subjectFilter, SQLiteConnection connection)
        {
            string query = "SELECT COUNT(*) FROM Schedule WHERE group_id IN (SELECT group_id FROM Groups WHERE group_name = @groupName) AND date = @selectedDate";

            if (!string.IsNullOrEmpty(teacherFilter))
            {
                query += " AND teacher_id IN (SELECT teacher_id FROM Teachers WHERE teacher_name LIKE @teacherFilter)";
            }

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                query += " AND subject_id IN (SELECT subject_id FROM Subjects WHERE subject_name LIKE @subjectFilter)";
            }

            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@groupName", groupName);
            command.Parameters.AddWithValue("@selectedDate", selectedDate);

            if (!string.IsNullOrEmpty(teacherFilter))
            {
                command.Parameters.AddWithValue("@teacherFilter", "%" + teacherFilter + "%");
            }

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                command.Parameters.AddWithValue("@subjectFilter", "%" + subjectFilter + "%");
            }

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }


        private void buttonCheckSchedule4_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker4, dataGridView4);
        }

        // Відкриття файлу бази даних
        private void OpenDatabaseFile(string filePath)
        {
            // Оновлення вибраного файлу бази даних
            selectedDatabasePath = filePath;
            UpdateConnectionString();
            // Оновити меню нещодавно відкритих файлів
            AddRecentlyOpenedFile(selectedDatabasePath);
        }

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

        // Додати файл до списку нещодавно відкритих файлів
        private void AddRecentlyOpenedFile(string filePath)
        {
            recentlyOpenedFiles.Insert(0, filePath);
            if (recentlyOpenedFiles.Count > 3)
            {
                recentlyOpenedFiles.RemoveAt(recentlyOpenedFiles.Count - 1);
            }
            UpdateRecentlyOpenedFilesMenu(нещодавноВідкритіToolStripMenuItem, recentlyOpenedFiles);
            // Зберегти список нещодавно відкритих файлів
            SaveRecentFiles(recentlyOpenedFiles);
        }

        // Оновлення меню нещодавно відкритих файлів
        private void UpdateRecentlyOpenedFilesMenu(ToolStripMenuItem нещодавноВідкритіToolStripMenuItem, List<string> recentlyOpenedFiles)
        {
            нещодавноВідкритіToolStripMenuItem.DropDownItems.Clear();
            foreach (string filePath in recentlyOpenedFiles)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(Path.GetFileName(filePath));
                menuItem.Tag = filePath;
                menuItem.Click += RecentlyOpenedFile_Click;
                нещодавноВідкритіToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        // Обробник подій для пунктів меню нещодавно відкритих файлів
        private void RecentlyOpenedFile_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string filePath = menuItem.Tag.ToString();
            OpenDatabaseFile(filePath);
        }

        // Метод для завантаження списку нещодавно відкритих файлів при запуску програми
        private void LoadRecentFilesOnStartup()
        {
            recentlyOpenedFiles = LoadRecentFiles();
            UpdateRecentlyOpenedFilesMenu(нещодавноВідкритіToolStripMenuItem, recentlyOpenedFiles);
        }



        //Вікно "Про розробника"
        private void проРозробникаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про розробника
/*            AboutDeveloperForm aboutDeveloperForm = new AboutDeveloperForm();
            aboutDeveloperForm.ShowDialog();*/
        }

        //Вікно "Про навчальний заклад"
        private void проНавчальнийЗакладToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про навчальний заклад
/*            AboutSchoolForm aboutSchoolForm = new AboutSchoolForm();
            aboutSchoolForm.ShowDialog();*/
        }

        //Вікно про програму
        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми для відомостей про програму
            AboutProgram aboutProgram = new AboutProgram();
            aboutProgram.ShowDialog();
        }

        //Очищення вмісту бази даних
        private void очиститиФайлToolStripMenuItem1_Click(object sender, EventArgs e)
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

        // Метод для отримання імен усіх таблиць в базі даних
        private string[] GetAllTableNames(SQLiteConnection connection)
        {
            List<string> tableNames = new List<string>();
            SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", connection);
            SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
            return tableNames.ToArray();
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталася помилка при створенні резервної копії: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
    // Клас для збереження списку нещодавно відкритих файлів
    public class RecentFilesConfig
    {
        public List<string> RecentlyOpenedFiles { get; set; } = new List<string>();
    }
}

