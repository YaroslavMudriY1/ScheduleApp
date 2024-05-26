using ScheduleAdmin;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ScheduleAdmin
{
    public class XmlHelper
    {
        public static void SerializeToFile<T>(T obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
    public partial class Schedule
    {
        // Шлях до файлу конфігурації
        private string configFilePath = "settings.xml";
        private List<string> recentlyOpenedFiles = new List<string>();

        public bool autoLastConnected;
        public int recentlyOpenedList;
        public bool trayIcon;
        public string time1_start;
        public string time2_start;
        public string time3_start;
        public string time4_start;
        public string time5_start;
        public string time6_start;
        public string time1_end;
        public string time2_end;
        public string time3_end;
        public string time4_end;
        public string time5_end;
        public string time6_end;

        private void LoadSettings()
        {
            if (File.Exists(configFilePath))
            {
                UserSettings settings = XmlHelper.DeserializeFromFile<UserSettings>(configFilePath);

                autoLastConnected = settings.AutoLastConnected;
                recentlyOpenedList = settings.RecentlyOpenedList;
                trayIcon = settings.TrayIcon;
                if (trayIcon)
                {
                    notifyIcon1.Visible = true;
                }
                else { notifyIcon1.Visible = false; }
                recentlyOpenedFiles = settings.RecentlyOpenedFiles ?? new List<string>();
                time1_start = settings.Time1Start;
                time2_start = settings.Time2Start;
                time3_start = settings.Time3Start;
                time4_start = settings.Time4Start;
                time5_start = settings.Time5Start;
                time6_start = settings.Time6Start;
                time1_end = settings.Time1End;
                time2_end = settings.Time2End;
                time3_end = settings.Time3End;
                time4_end = settings.Time4End;
                time5_end = settings.Time5End;
                time6_end = settings.Time6End;
            }
            else
            {
                UseDefaultSettings();
                SaveSettings();
                MessageBox.Show("Файл налаштувань не знайдений. Встановлено налаштування за замовчуванням.");
            }
        }

        private void UseDefaultSettings()
        {
            autoLastConnected = true;
            recentlyOpenedList = 4;
            trayIcon = false;
            notifyIcon1.Visible = false;
            recentlyOpenedFiles = new List<string>();
            time1_start = "08:00";
            time2_start = "09:15";
            time3_start = "10:30";
            time4_start = "12:00";
            time5_start = "13:15";
            time6_start = "14:30";
            time1_end = "09:00";
            time2_end = "10:15";
            time3_end = "11:30";
            time4_end = "13:00";
            time5_end = "14:15";
            time6_end = "15:30";
        }

        private void SaveSettings()
        {
            UserSettings settings = new UserSettings
            {
                AutoLastConnected = autoLastConnected,
                RecentlyOpenedList = recentlyOpenedList,
                TrayIcon = trayIcon,
                RecentlyOpenedFiles = recentlyOpenedFiles,
                Time1Start = time1_start,
                Time2Start = time2_start,  
                Time3Start = time3_start,
                Time4Start = time4_start,
                Time5Start = time5_start,
                Time6Start = time6_start,
                Time1End = time1_end,
                Time2End = time2_end,
                Time3End = time3_end,
                Time4End = time4_end,
                Time5End = time5_end,
                Time6End = time6_end,
            };

            XmlHelper.SerializeToFile(settings, configFilePath);
        }
        private void UpdateRecentlyOpenedFiles(string filePath)
        {
            // Перевірка, чи файл вже є у списку
            if (recentlyOpenedFiles.Contains(filePath))
            {
                recentlyOpenedFiles.Remove(filePath); // Видалити файл зі списку
            }
            recentlyOpenedFiles.Insert(0, filePath); // Додати файл на початок списку
            if (recentlyOpenedFiles.Count > recentlyOpenedList)
            {
                recentlyOpenedFiles.RemoveAt(recentlyOpenedFiles.Count - 1); // Видалити останній елемент, якщо списку більше ліміту
            }

            UpdateRecentlyOpenedFilesMenu(нещодавноВідкритіToolStripMenuItem, recentlyOpenedFiles);
            // Зберегти список нещодавно відкритих файлів та останню базу даних
            SaveSettings();
        }

        private void UpdateRecentlyOpenedFilesMenu(ToolStripMenuItem menuItem, List<string> recentlyOpenedFiles)
        {
            menuItem.DropDownItems.Clear();
            foreach (string filePath in recentlyOpenedFiles)
            {
                ToolStripMenuItem fileItem = new ToolStripMenuItem(Path.GetFileName(filePath));
                fileItem.Tag = filePath;
                fileItem.Click += RecentlyOpenedFile_Click;
                // Перевірка, чи поточний елемент є обраним файлом
                if (filePath == selectedDatabasePath)
                {
                    fileItem.Checked = true;
                }
                menuItem.DropDownItems.Add(fileItem);
            }
        }

        private void RecentlyOpenedFile_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string filePath = menuItem.Tag.ToString();
            OpenDatabaseFile(filePath);
        }

        private void LoadRecentFilesOnStartup()
        {
            LoadSettings();
            UpdateRecentlyOpenedFilesMenu(нещодавноВідкритіToolStripMenuItem, recentlyOpenedFiles);
        }

        private void OpenDatabaseFile(string filePath)
        {
            // Оновлення вибраного файлу бази даних
            selectedDatabasePath = filePath;
            UpdateConnectionString();
            UpdateRecentlyOpenedFiles(filePath);
        }

        private void UpdateConnectionString()
        {
            connectionString = $"Data Source={selectedDatabasePath};Version=3;";
        }

        private static string selectedDatabasePath = "";
        string connectionString = $"Data Source={selectedDatabasePath};Version=3;";

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

        // Метод читання бази даних

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
                return;
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
                string query = "SELECT * FROM Schedule WHERE date = @selectedDate ORDER BY pair_index";
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
                        int pairIndex = Convert.ToInt32(reader["pair_index"]);

                        // Отримати індекс стовпця, в який буде додана інформація
                        int columnIndex = GetColumnIndex(dataGridView, groupName);


                        // Отримати інформацію про предмет та викладача з відповідних таблиць
                        if (columnIndex >= 0)
                        {
                            string subject = GetSubjectName(subjectId);
                            string teacher = GetTeacherName(teacherId);

                            if (dataGridView.Rows.Count <= pairIndex)
                            {
                                dataGridView.Rows.Add();
                                dataGridView.Rows[pairIndex].HeaderCell.Value = GetTimeHeader(pairIndex);
                            }

                            if (dataGridView.Columns.Count <= columnIndex)
                            {
                                dataGridView.Columns.Add(groupName, groupName);
                            }

                            dataGridView.Rows[pairIndex].Cells[columnIndex].Value = subject + "\n" + teacher + "\n" + classroom;
                        
                    }
                    }
                }
                else
                {
                    MessageBox.Show("За вказаною датою відсутні записи", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            // Закриття підключення до бази даних
            connection.Close();

        }
        private bool CheckSixthPairAvailability(string selectedDate)
        {
            bool hasSixthPair = false;
            int sixthPairIndex = 5;

            // SQL-запит для перевірки наявності шостої пари
            string query = "SELECT COUNT(*) FROM Schedule WHERE date = @selectedDate AND pair_index = @sixthPairIndex";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    command.Parameters.AddWithValue("@sixthPairIndex", sixthPairIndex);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Якщо є записи що належать до шостої пари, то шоста пара доступна
                    hasSixthPair = count > 0;
                }
            }

            return hasSixthPair;
        }
        bool hasData(DataGridView dataGridView)
        {
            bool hasData = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (row.Cells[i].Value != null)
                        {
                            hasData = true;
                            break;
                        }
                    }
                }

                if (hasData)
                    break;
            }

            if (!hasData)
            {
                return false;
            }
            return hasData;
        }
        // Метод, який встановлює значення DateTimePicker на інших вкладках
        private void SyncDateTimePickers(DateTime dateTime)
        {
            dateTimePicker1.Value = dateTime;
            dateTimePicker2.Value = dateTime;
            dateTimePicker3.Value = dateTime;
            dateTimePicker4.Value = dateTime;
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

        // Метод для визначення значення рядкового заголовку для заданого індексу часу
        private string GetTimeHeader(int rowIndex)
        {
            switch (rowIndex)
            {
                case 0: return "1 пара";
                case 1: return "2 пара";
                case 2: return "3 пара";
                case 3: return "4 пара";
                case 4: return "5 пара";
                case 5: return "6 пара";
                default: throw new ArgumentException("Invalid time row index: " + rowIndex);
            }
        }
        // Метод для визначення заняття за розкладом
        private int GetPairIndex(string text)
        {
            switch (text)
            {
                case "Перша пара": return 0;
                case "Друга пара": return 1;
                case "Третя пара": return 2;
                case "Четверта пара": return 3;
                case "П'ята пара": return 4;
                case "Шоста пара": return 5;
                default: throw new ArgumentException("Invalid time: " + text);
            }
        }

        // Метод для визначення значення часу за обраним варіантом
        private string GetTimeStartFromPair(string text)
        {
            switch (text)
            {
                case "Перша пара": return time1_start;
                case "Друга пара": return time2_start;
                case "Третя пара": return time3_start;
                case "Четверта пара": return time4_start;
                case "П'ята пара": return time5_start;
                case "Шоста пара": return time6_start;
                default: throw new ArgumentException("Invalid time: " + text);
            }
        }

        // Метод для визначення значення часу закінчення за обраним варіантом
        private string GetTimeEndFromPair(string text)
        {
            switch (text)
            {
                case "Перша пара": return time1_end;
                case "Друга пара": return time2_end;
                case "Третя пара": return time3_end;
                case "Четверта пара": return time4_end;
                case "П'ята пара": return time5_end;
                case "Шоста пара": return time6_end;
                default: throw new ArgumentException("Invalid time: " + text);
            }
        }

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
        //Метод перевірки чи є вже запис за вказаними параметрами 
        private bool CheckIfRecordExists(string groupName, string selectedDate, int pairIndex)
        {
            bool recordExists = false;
            int groupId = GetGroupId(groupName);
            string query = "SELECT COUNT(*) FROM Schedule " +
                           "WHERE group_id = @groupId AND date = @selectedDate AND pair_index = @pairIndex";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@groupId", groupId);
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    command.Parameters.AddWithValue("@pairIndex", pairIndex);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    recordExists = count > 0;
                }
                connection.Close();
            }

            return recordExists;
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
                bool groupContainsFilteredData = CheckGroupForFilteredData(groupName, selectedDate, teacherFilter, subjectFilter, classroomFilter, connection);
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
            if (dataGridView.Columns.Count <= 0)
            {
                MessageBox.Show("Введіть коректні параметри для фільтрації.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    dataGridView.Rows.Add();
                    dataGridView.Rows[i].HeaderCell.Value = $"{i + 1} пара";
                }
            }
            // Запит до бази даних для отримання розкладу з урахуванням фільтрації
            string query = "SELECT * FROM Schedule WHERE date = @selectedDate";

            // Додавання умов фільтрації за параметрами, якщо вони вказані
            if (!string.IsNullOrWhiteSpace(groupNameFilter))
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

            query += " ORDER BY pair_index";

            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@selectedDate", selectedDate);

            // Додавання параметрів фільтрації за групою, якщо вона вказана
            if (!string.IsNullOrWhiteSpace(groupNameFilter))
            {
                command.Parameters.AddWithValue("@groupName", $"%{groupNameFilter}%");
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
                    int pairIndex = Convert.ToInt32(reader["pair_index"]);

                    // Отримати індекс стовпця, в який буде додана інформація
                    int columnIndex = dataGridView.Columns.Contains(groupName) ? dataGridView.Columns[groupName].Index : -1;

                    // Якщо стовпець не знайдено, додати новий
                    if (columnIndex == -1)
                    {
                        columnIndex = dataGridView.Columns.Add(groupName, groupName);
                    }

                    // Отримати індекс рядка або додати новий, якщо не існує
                    if (pairIndex >=0)
                    {
                        if (dataGridView.Rows.Count <= pairIndex)
                        {
                            dataGridView.Rows.Add();
                            dataGridView.Rows[pairIndex].HeaderCell.Value = GetTimeHeader(pairIndex);
                        }

                        dataGridView.Rows[pairIndex].Cells[columnIndex].Value = GetSubjectName(subjectId) + "\n" + GetTeacherName(teacherId) + "\n" + classroom;
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
        private bool CheckGroupForFilteredData(string groupName, string selectedDate, string teacherFilter, string subjectFilter, string classroomFilter, SQLiteConnection connection)
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
            if (!string.IsNullOrEmpty(classroomFilter))
            {
                query += " AND classroom IN (SELECT classroom FROM Subjects WHERE classroom LIKE @classroomFilter)";
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
            if (!string.IsNullOrEmpty(classroomFilter))
            {
                command.Parameters.AddWithValue("@classroomFilter", "%" + classroomFilter + "%");
            }

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        // Перевірка наявності резервних копій
        private void CheckBackupFolderExists()
        {
            // Визначення шляху до папки з резервними копіями
            string backupFolderPath = Path.Combine(Path.GetDirectoryName(selectedDatabasePath), "Copy");

            // Перевірка, чи існує папка "Copy" у папці бази даних та встановлення маркеру
            if (Directory.Exists(backupFolderPath))
            {
                резервнаКопіяToolStripMenuItem.Checked = true;
            }
            else
            {
                резервнаКопіяToolStripMenuItem.Checked = false;
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

    }
}
