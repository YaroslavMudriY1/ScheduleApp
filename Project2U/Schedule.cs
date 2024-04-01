using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Project2U
{
    // Клас для збереження списку нещодавно відкритих файлів
    [Serializable]
    public class RecentDatabaseConfig
    {
        public string RecentlyOpenedDatabase { get; set; }
    }
    public partial class Schedule
    {
        // Шлях до файлу конфігурації
        private string configFilePath = "recentDatabaseConfig.xml";

        // Завантаження списку нещодавно відкритих файлів
        private string LoadRecentDatabase()
        {
            if (File.Exists(configFilePath))
            {
                using (StreamReader reader = new StreamReader(configFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(RecentDatabaseConfig));
                    RecentDatabaseConfig config = (RecentDatabaseConfig)serializer.Deserialize(reader);

                    // Повний шлях до бази даних
                    string databasePath = config.RecentlyOpenedDatabase;

                    // Перевірка наявності файлу бази даних
                    if (File.Exists(databasePath))
                    {
                        // Назначити значення selectedDatabasePath
                        selectedDatabasePath = databasePath;
                        UpdateConnectionString();
                        return databasePath;
                    }
                    else
                    {
                        MessageBox.Show("Файл бази даних не існує за зазначеним шляхом.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Файл конфігурації не існує.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return null;
        }  


    // Збереження списку нещодавно відкритих файлів
    private void SaveRecentDatabase(string recentlyOpenedDatabase)
        {
            RecentDatabaseConfig config = new RecentDatabaseConfig();
            config.RecentlyOpenedDatabase = recentlyOpenedDatabase;
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RecentDatabaseConfig));
                serializer.Serialize(writer, config);
            }
        }

        private static string selectedDatabasePath = "";
        string connectionString = $"Data Source={selectedDatabasePath};Version=3;";

        private void UpdateConnectionString()
        {
            connectionString = $"Data Source={selectedDatabasePath};Version=3;";
        }
        // Метод для отримання всіх доступних назв груп з бази даних
        private List<string> GetAllGroupNames()
        {
            List<string> groupNames = new List<string>();
            string query = "SELECT group_name FROM Groups";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    string groupName = reader["group_name"].ToString();
                    groupNames.Add(groupName);
                }
            }

                return groupNames;
        }

        //Фунція читання бази даних

        private void LoadScheduleForDate(DateTime Value)
        {

            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedDate = Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі день.місяць.рік
                                                                               // Отримання всіх доступних назв груп з бази даних
            List<string> groupNames = GetAllGroupNames();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                // Очистка DataGridView перед відображенням нових даних
                dataGridView1.Rows.Clear();

                // Перевірка наявності стовпців з назвами груп та їх додавання
                foreach (string groupName in groupNames)
                {
                    if (!dataGridView1.Columns.Cast<DataGridViewColumn>().Any(column => column.HeaderText == groupName))
                    {
                        dataGridView1.Columns.Add(groupName, groupName);
                    }
                }
                // Перевірка наявності стовпців у DataGridView перед додаванням рядків
                if (dataGridView1.Columns.Count == 0)
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
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[i].HeaderCell.Value = $"{i + 1} пара";
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
                            int columnIndex = GetColumnIndex(dataGridView1, groupName);


                            // Отримати інформацію про предмет та викладача з відповідних таблиць
                            if (columnIndex >= 0)
                            {
                                string subject = GetSubjectName(subjectId);
                                string teacher = GetTeacherName(teacherId);

                                int rowIndex = GetRowIndex(timeStart);
                                if (rowIndex >= 0)
                                {
                                    if (dataGridView1.Rows.Count <= rowIndex)
                                    {
                                        dataGridView1.Rows.Add();
                                        dataGridView1.Rows[rowIndex].HeaderCell.Value = GetTimeHeader(rowIndex);
                                    }

                                    if (dataGridView1.Columns.Count <= columnIndex)
                                    {
                                        dataGridView1.Columns.Add(groupName, groupName);
                                    }

                                    dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = subject + "\n" + teacher + "\n" + classroom;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("За вказаною датою відсутні записи", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

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

        //Зміна написів відповідно до дати
        private void UpdateDateLabels(DateTime selectedDate)
        {
            // Отримайте текст з обраної дати та встановіть його для label1, label2 та label3
            string dateString = selectedDate.ToString("dd MMMM yyyy р.");
            label1.Text = $"Розклад занять за {dateString}";
            label3.Text = $"Розклад занять за {dateString}";
        }

        // Метод для завантаження даних з бази даних з урахуванням фільтрації
        private void LoadFilteredDataFromDatabase(string groupNameFilter, string teacherFilter, string classroomFilter, string subjectFilter)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedDate = dateTimePicker2.Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі день.місяць.рік

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();            // Відкриття підключення до бази даних
                                              // Очистка DataGridView перед відображенням нових даних
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
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
                        dataGridView2.Columns.Add(groupName, groupName);
                    }
                }
                // Перевірка наявності стовпців у DataGridView перед додаванням рядків
                if (dataGridView2.Columns.Count == 0)
                {
                    MessageBox.Show("База даних порожня.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        dataGridView2.Rows.Add();
                        dataGridView2.Rows[i].HeaderCell.Value = $"{i + 1} пара";
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
                            int columnIndex = dataGridView2.Columns.Contains(groupName) ? dataGridView2.Columns[groupName].Index : -1;

                            // Якщо стовпець не знайдено, додати новий
                            if (columnIndex == -1)
                            {
                                columnIndex = dataGridView2.Columns.Add(groupName, groupName);
                            }

                            // Отримати індекс рядка або додати новий, якщо не існує
                            int rowIndex = GetRowIndex(timeStart);
                            if (rowIndex >= 0)
                            {
                                if (dataGridView2.Rows.Count <= rowIndex)
                                {
                                    dataGridView2.Rows.Add();
                                    dataGridView2.Rows[rowIndex].HeaderCell.Value = GetTimeHeader(rowIndex);
                                }

                                dataGridView2.Rows[rowIndex].Cells[columnIndex].Value = GetSubjectName(subjectId) + "\n" + GetTeacherName(teacherId) + "\n" + classroom;
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
                }
            }
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
        // Відкриття файлу бази даних
        private void OpenDatabaseFile(string filePath)
        {
            // Оновлення вибраного файлу бази даних
            selectedDatabasePath = filePath;
            UpdateConnectionString();
            LoadScheduleForDate(dateTimePicker1.Value);
            // Оновити меню нещодавно відкритих файлів
            //AddRecentlyOpenedFile(selectedDatabasePath);
        }
        // Додати файл до списку нещодавно відкритих файлів
/*        private void AddRecentlyOpenedFile(string filePath)
        {
            recentlyOpenedDatabase.Insert(0, filePath);
            if (recentlyOpenedDatabase.Count > 3)
            {
                recentlyOpenedDatabase.RemoveAt(recentlyOpenedDatabase.Count - 1);
            }
            UpdateRecentlyOpenedFilesMenu(нещодавноВідкритіToolStripMenuItem, recentlyOpenedDatabase);
            // Зберегти список нещодавно відкритих файлів
            SaveRecentDatabase(recentlyOpenedDatabase);
        }*/

        // Оновлення меню нещодавно відкритих файлів
        private void UpdateRecentlyOpenedFilesMenu(ToolStripMenuItem нещодавноВідкритіToolStripMenuItem, List<string> recentlyOpenedFiles)
        {
            нещодавноВідкритіToolStripMenuItem.DropDownItems.Clear();
            foreach (string filePath in recentlyOpenedFiles)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(Path.GetFileName(filePath));
                menuItem.Tag = filePath;
                menuItem.Click += нещодавноВідкритіToolStripMenuItem_Click;
                нещодавноВідкритіToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

    }
}
