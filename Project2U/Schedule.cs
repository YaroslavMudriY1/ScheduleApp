using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Project2U.Authorization;

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
        private readonly string configFilePath = "recentDatabaseConfig.xml";

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
                dataGridView1.Columns.Clear();

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


        //Зміна написів відповідно до дати
        private void UpdateDateLabels(DateTime selectedDate)
        {
            // Отримайте текст з обраної дати та встановіть його для label1, label2 та label3
            string dateString = selectedDate.ToString("dd MMMM yyyy р.");
            label1.Text = $"Розклад занять за {dateString}";
            label3.Text = $"Розклад занять за {dateString}";
        }

        // Метод для завантаження даних з бази даних з урахуванням фільтрації
        private void LoadFilteredDataFromDatabase(string groupNameFilter, string teacherFilter, string classroomFilter, string subjectFilter, string selectedDate, DataGridView dataGridView)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Перевірка groupNameFilter на null
            if (string.IsNullOrEmpty(groupNameFilter))
            {
                groupNameFilter = "";
            }
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();            // Відкриття підключення до бази даних
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
                        if (groupName.Contains(groupNameFilter ?? ""))
                        {
                            dataGridView.Columns.Add(groupName, groupName);
                        }
                    }
                    // Перевірка наявності стовпців у DataGridView перед додаванням рядків
                    if (dataGridView.Columns.Count <= 0)
                    {
                        MessageBox.Show("Де стовпці?.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    query += " ORDER BY time_start";

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
                            string timeStart = reader["time_start"].ToString();

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

                }


        }

            private void LoadPesonalSchedule(string selectedDate)
        {
            // Перевірка, чи користувач перейшов на вкладку "Мій розклад"
            if (tabControl1.SelectedTab != tabPage3)
                return;

            // Перевірка наявності бази даних та користувача
            if (string.IsNullOrEmpty(selectedDatabasePath) || GetUserProfile() == null)
            {
                MessageBox.Show("Будь ласка, оберіть базу даних та налаштуйте користувача.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                // Очищення dataGridView3 перед відображенням нових даних
                dataGridView3.Rows.Clear();
                dataGridView3.Columns.Clear();

                // Отримання типу користувача (вчителя або студента)
                string userType = GetUserType();
                string userGroupName = "";
                string userTeacherName = "";
                string classroomFilter = ""; // Пусте значення для можливості виклику функції
                string subjectFilter = ""; // Пусте значення для можливості виклику функції

                // Додавання параметрів фільтрації для вчителя або студента
                if (userType == "Вчитель")
                {
                    // Отримання імені вчителя з XML файлу
                    UserProfile userProfile = GetUserProfile();
                    if (userProfile != null)
                    {
                        userTeacherName = userProfile.Name;
                    }
                }
                else if (userType == "Студент")
                {
                    // Отримання назви групи користувача з XML файлу
                    userGroupName = GetUserGroupName();
                }

                // Виклик функції LoadFilteredDataFromDatabase з відповідними параметрами
                LoadFilteredDataFromDatabase(userGroupName, userTeacherName, classroomFilter, subjectFilter, selectedDate, dataGridView3);


                /*                SQLiteCommand command = new SQLiteCommand(query, connection);
                                command.Parameters.AddWithValue("@selectedDate", selectedDate);

                                // Додавання параметрів фільтрації для вчителя або студента
                                if (userType == "Вчитель")
                                {
                                    command.Parameters.AddWithValue("@teacherName", teacherName);
                                }
                                else if (userType == "Студент")
                                {
                                    command.Parameters.AddWithValue("@groupName", groupName);
                                }

                                SQLiteDataReader reader = command.ExecuteReader();

                                // Логіка для заповнення dataGridView3 з результатами запиту або відображення повідомлення
                                if (reader.HasRows)
                                {
                                    // Додавання стовпців для знайдених груп/вчителів
                                    if (userType == "Вчитель")
                                    {
                                        dataGridView3.Columns.Add(teacherName, teacherName);
                                    }
                                    else if (userType == "Студент")
                                    {
                                        // Пошук груп на основі введеного тексту
                                        string searchQuery = "SELECT group_name FROM Groups WHERE group_name LIKE @groupName";
                                        SQLiteCommand searchCommand = new SQLiteCommand(searchQuery, connection);
                                        searchCommand.Parameters.AddWithValue("@groupName", $"%{groupName}%");
                                        SQLiteDataReader searchReader = searchCommand.ExecuteReader();

                                        // Додавання стовпців для знайдених груп
                                        while (searchReader.Read())
                                        {
                                            string group = searchReader["group_name"].ToString();
                                            dataGridView3.Columns.Add(group, group);
                                        }
                                        searchReader.Close(); // Закриття рідера для пошуку груп
                                    }

                                    // Логіка для заповнення dataGridView3 з результатами запиту
                                    while (reader.Read())
                                    {
                                        // Отримання необхідних даних з рядка розкладу
                                        string timeStart = reader["time_start"].ToString();
                                        string subjectName = reader["subject_name"].ToString();
                                        string classroom = reader["classroom"].ToString();

                                        // Додавання рядка до dataGridView3 з урахуванням отриманих даних
                                        int rowIndex = GetRowIndex(timeStart);
                                        if (rowIndex >= 0)
                                        {
                                            if (dataGridView3.Rows.Count <= rowIndex)
                                            {
                                                dataGridView3.Rows.Add();
                                                dataGridView3.Rows[rowIndex].HeaderCell.Value = GetTimeHeader(rowIndex);
                                            }

                                            // Додавання даних у відповідний стовпець
                                            if (userType == "Вчитель")
                                            {
                                                dataGridView3.Rows[rowIndex].Cells[teacherName].Value = $"{subjectName}\n{classroom}";
                                            }
                                            else if (userType == "Студент")
                                            {
                                                string groupNameFromData = reader["group_name"].ToString();
                                                dataGridView3.Rows[rowIndex].Cells[groupNameFromData].Value = $"{subjectName}\n{classroom}";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Відображення повідомлення про відсутність результатів
                                    dataGridView3.Columns.Add("Повідомлення", "");
                                    dataGridView3.Rows.Add("Розклад на цю дату відсутній.", "");
                                }

                                }
                */
            }
        }
            // Метод для отримання типу користувача та інших даних з XML файлу
            private UserProfile GetUserProfile()
        {
            string filePath = "userProfile.xml";
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserProfile));
                    return (UserProfile)serializer.Deserialize(reader);
                }
            }
            else
            {
                MessageBox.Show("Файл користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Метод для отримання типу користувача залежно від обраних на формі входу
        private string GetUserType()
        {
            UserProfile userProfile = GetUserProfile();
            if (userProfile != null)
            {
                if (userProfile.IsStudent)
                    return "Студент";
                else if (userProfile.IsTeacher)
                    return "Вчитель";
                else
                    return null;
            }
            else
            {
                // Обробка ситуації, коли дані користувача не знайдені
                MessageBox.Show("Невідомий тип користувача", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Метод для отримання назви групи користувача (лише для студентів)
        private string GetUserGroupName()
        {
            UserProfile userProfile = GetUserProfile();
            if (userProfile != null && userProfile.IsStudent)
            {
                return userProfile.GroupName;
            }
            else
            {
                MessageBox.Show("Групу користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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
            //LoadPesonalSchedule(dateTimePicker1.Value);
            // Оновити меню нещодавно відкритих файлів
            //AddRecentlyOpenedFile(selectedDatabasePath);
        }
        [Serializable]
        public class UserProfile
        {
            public bool IsStudent { get; set; } // Тип користувача (Студент)
            public bool IsTeacher { get; set; } // Тип користувача (Вчитель )
            public string Name { get; set; } // Прізвище та Ім'я користувача (якщо вчитель то використовується для виведення розкладу).
            public string GroupName { get; set; } // Назва групи користувача (лише для студентів)
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
                }

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
                }*/

    }
}
