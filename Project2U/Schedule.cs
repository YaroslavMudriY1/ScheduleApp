using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization; //Для зберігання даних користувача в файлах формату .XML
using Windows.UI.Xaml.Documents;
using static ScheduleUser.Authorization;

namespace ScheduleUser
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
        private readonly string configFilePath = "settings.xml";
        private List<string> recentlyOpenedFiles = new List<string>();

        public bool autoLastConnected;
        public int recentlyOpenedList;
        public bool deleteUserProfile;
        public bool trayIcon;
        private void LoadSettings()
        {
            if (File.Exists(configFilePath))
            {
                UserSettings settings = XmlHelper.DeserializeFromFile<UserSettings>(configFilePath);

                autoLastConnected = settings.AutoLastConnected;
                recentlyOpenedList = settings.RecentlyOpenedList;
                deleteUserProfile = settings.DeleteUserProfile;
                trayIcon = settings.TrayIcon;
                if (trayIcon)
                {
                    notifyIcon1.Visible = true;
                }
                else { notifyIcon1.Visible = false; }
                recentlyOpenedFiles = settings.RecentlyOpenedFiles ?? new List<string>();
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
            deleteUserProfile = false;
            trayIcon = false;
            notifyIcon1.Visible=false;
            recentlyOpenedFiles = new List<string>();
            selectedDatabasePath = string.Empty;
        }

        private void SaveSettings()
        {
            UserSettings settings = new UserSettings
            {
                AutoLastConnected = autoLastConnected,
                RecentlyOpenedList = recentlyOpenedList,
                DeleteUserProfile = deleteUserProfile,
                TrayIcon = trayIcon,
                RecentlyOpenedFiles = recentlyOpenedFiles,
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
            LoadScheduleForDate(dateTimePicker1.Value);
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

        // Метод читання бази даних

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
        //Перевірка на наявність шостої пари
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

                    // Якщо є записи з часом початку "14:30", то шоста пара доступна
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
            label3.Text = $"Мій розклад занять за {dateString}";
        }

        // Метод для завантаження даних з бази даних з урахуванням фільтрації
        private void LoadFilteredDataFromDatabase(string groupNameFilter, string teacherFilter, string classroomFilter, string subjectFilter, string selectedDate, DataGridView dataGridView)
        {
            if (string.IsNullOrEmpty(selectedDatabasePath))
            {
                MessageBox.Show("Будь ласка, оберіть базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();            // Відкриття підключення до бази даних

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

                    // Перевірка наявності стовпців у DataGridView перед додаванням рядків
                    if (dataGridView.Columns.Count <= 0)
                    {
                        MessageBox.Show("За вказаною датою відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                MessageBox.Show("За вказаною датою відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("За вказаними фільтрами відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    // Закриття підключення до бази даних
                    connection.Close();
                }
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
                query += " AND classroom = @classroomFilter";
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
                command.Parameters.AddWithValue("@classroomFilter", classroomFilter);
            }

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
        //Метод для завантаження персонального розкладу
        private void LoadPersonalSchedule(string selectedDate, bool IsNotify)
        {

                // Перевірка, чи користувач перейшов на вкладку "Мій розклад"
                if (tabControl1.SelectedTab != tabPage3)
                    return;
                // Перевірка наявності бази даних
                if (string.IsNullOrEmpty(selectedDatabasePath))
                {
                    MessageBox.Show("Будь ласка, перевірте обрану базу даних.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            if (GetUserProfile()!= null)
            {
                // Отримання типу користувача (вчителя або студента)
                string userType = GetUserType();

                // Завантаження розкладу в залежності від типу користувача
                if (userType == "Вчитель")
                {
                    LoadTeacherSchedule(selectedDate, IsNotify);
                }
                else if (userType == "Студент")
                {
                    LoadStudentSchedule(selectedDate, IsNotify);
                }
            }
        }
        // Метод для завантаження персонального розкладу студента
        private void LoadStudentSchedule(string selectedDate, bool IsNotify)
        {
            // Отримання назви групи користувача з XML файлу
            string userGroupName = GetUserGroupName();

            // Очищення DataGridView перед завантаженням нових даних
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();

            dataGridView3.Columns.Add("Час початку", "Час початку");
            dataGridView3.Columns[0].Width = 55;
            // Зробити заголовок жирним лише для стовпця "Час початку"

            // Додавання стовпця до DataGridView з ім'ям групи студента
            dataGridView3.Columns.Add(userGroupName, userGroupName);

            // Перевірка наявності шостої пари
            bool hasSixthPair = CheckSixthPairAvailability(selectedDate);

            // Додавання рядків у випадку наявності шостої пари
            int rowsCount = hasSixthPair ? 6 : 5;
            for (int i = 0; i < rowsCount; i++)
            {
                dataGridView3.Rows.Add();
                dataGridView3.Rows[i].HeaderCell.Value = $"{i + 1} пара";
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                // Запит до бази даних для отримання розкладу з урахуванням фільтрації для студента
                string query = "SELECT * FROM Schedule WHERE date = @selectedDate AND group_id = (SELECT group_id FROM Groups WHERE group_name = @groupName)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@selectedDate", selectedDate);
                command.Parameters.AddWithValue("@groupName", userGroupName);
                SQLiteDataReader reader = command.ExecuteReader();

                // Логіка для заповнення DataGridView з результатами запиту
                while (reader.Read())
                {
                    int subjectId = Convert.ToInt32(reader["subject_id"]);
                    int teacherId = Convert.ToInt32(reader["teacher_id"]);
                    string classroom = reader["classroom"].ToString();
                    string timeStart = reader["time_start"].ToString();

                        int rowIndex = GetRowIndex(timeStart);

                        if (rowIndex >= 0)
                        {
                            if (dataGridView3.Rows.Count <= rowIndex)
                            {
                                dataGridView3.Rows.Add();
                            }

                        // Перевірка, чи є інформація про пару
                        if (!string.IsNullOrEmpty(classroom))
                        {
                            if (IsNotify)
                            {
                                dataGridView3.Rows[rowIndex].Cells[userGroupName].Value = "Предмет: " + GetSubjectName(subjectId) + ", Викладач: " + GetTeacherName(teacherId) + ", Аудиторія: " + classroom;
                            }
                            dataGridView3.Rows[rowIndex].Cells[userGroupName].Value = "Предмет: "+GetSubjectName(subjectId) + "\nВикладач: " + GetTeacherName(teacherId) + "\nАудиторія: " + classroom;

                            dataGridView3.Rows[rowIndex].Cells["Час початку"].Value = GetTimeBeginHeader(rowIndex);
                        }
                        else
                        {
                            MessageBox.Show("За вказаною датою відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        }
                        else
                        {
                            MessageBox.Show("За вказаними фільтрами відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                }

                // Закриття підключення до бази даних
                connection.Close();
            }

            // Перевірка, чи вдалося завантажити розклад користувача
            if (dataGridView3.Rows.Count == 0)
            {
                SendNotification("Помилка", "Неможливо завантажити персональний розклад. Система сповіщень працює некоректно.");
            }
        }

        // Метод для завантаження персонального розкладу вчителя
        private void LoadTeacherSchedule(string selectedDate, bool IsNotify)
        {     
            // Отримання імені вчителя з XML файлу
            UserProfile userProfile = GetUserProfile();
            string userTeacherName = userProfile != null ? userProfile.Name : "";
            // Отримання всіх доступних назв груп з бази даних
            List<string> groupNames = GetAllGroupNames();
            // Очищення DataGridView перед завантаженням нових даних
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.Columns.Add("Час початку", "Час початку");
            dataGridView3.Columns[0].Width = 55;

            // Додавання стовпця до DataGridView з ім'ям вчителя
            dataGridView3.Columns.Add(userTeacherName, userTeacherName);

            // Перевірка наявності шостої пари
            bool hasSixthPair = CheckSixthPairAvailability(selectedDate);

            // Додавання рядків у випадку наявності шостої пари
            int rowsCount = hasSixthPair ? 6 : 5;
            for (int i = 0; i < rowsCount; i++)
            {
                dataGridView3.Rows.Add();
                dataGridView3.Rows[i].HeaderCell.Value = $"{i + 1} пара";
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                // Запит до бази даних для отримання розкладу з урахуванням фільтрації для вчителя
                string query = "SELECT * FROM Schedule WHERE date = @selectedDate AND teacher_id = (SELECT teacher_id FROM Teachers WHERE teacher_name = @teacherName)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@selectedDate", selectedDate);
                command.Parameters.AddWithValue("@teacherName", userTeacherName);
                SQLiteDataReader reader = command.ExecuteReader();

                // Логіка для заповнення DataGridView з результатами запиту
                while (reader.Read())
                {
                    int subjectId = Convert.ToInt32(reader["subject_id"]);
                    string groupName = GetGroupName(Convert.ToInt32(reader["group_id"]));
                    string classroom = reader["classroom"].ToString();
                    string timeStart = reader["time_start"].ToString();
                    // Отримання індексу рядка
                    int rowIndex = GetRowIndex(timeStart);
                    if (rowIndex >= 0)
                    {
                        // Перевірка, чи індекс рядка не виходить за межі доступних рядків
                        while (rowIndex >= dataGridView3.Rows.Count)
                        {
                            // Додавання нового рядка
                            dataGridView3.Rows.Add();
                        }

                        // Перевірка, чи є інформація про пару
                        if (!string.IsNullOrEmpty(classroom))
                        {
                            if (IsNotify)
                            {
                                dataGridView3.Rows[rowIndex].Cells[userTeacherName].Value = "Предмет: " + GetSubjectName(subjectId) + ", Група: " + groupName + ", Аудиторія: " + classroom;
                            }
                            // Заповнення комірки з інформацією про пару
                            dataGridView3.Rows[rowIndex].Cells[userTeacherName].Value ="Предмет: "+ GetSubjectName(subjectId) +"\nГрупа: " + groupName + "\nАудиторія: " + classroom;

                            dataGridView3.Rows[rowIndex].Cells["Час початку"].Value = GetTimeBeginHeader(rowIndex);

                        }
                        else
                        {
                            MessageBox.Show("За вказаною датою відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("За вказаними фільтрами відсутні записи", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                // Закриття підключення до бази даних
                connection.Close();
            }

            // Перевірка, чи вдалося завантажити розклад користувача
            if (dataGridView3.Rows.Count == 0)
            {
                SendNotification("Помилка", "Неможливо завантажити персональний розклад. Система сповіщень працює некоректно.");
            }
        }

        // Метод для визначення значення рядкового заголовку для заданого індексу часу
        private string GetTimeBeginHeader(int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return "08:00";
                case 1:
                    return "09:15";
                case 2:
                    return "10:30";
                case 3:
                    return "12:00";
                case 4:
                    return "13:15";
                case 5:
                    return "14:30"; 
                default:
                    throw new ArgumentException("Invalid time row index: " + rowIndex);
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
                MessageBox.Show("Файл користувача не знайдено. Будь ласка, заповніть форму.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                return userProfile.Group;
            }
            else
            {
                MessageBox.Show("Групу користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        // Перевірка, чи є розклад на сьогоднішній день
        private bool hasScheduleForToday(DateTime currentDate)
        {
            bool hasScheduleForToday = false;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open(); // Відкриття підключення до бази даних

                // Запит до бази даних для перевірки наявності розкладу на сьогоднішній день
                string query = "SELECT COUNT(*) FROM Schedule WHERE date = @currentDate";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentDate", currentDate.ToString("dd.MM.yyyy"));
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    hasScheduleForToday = true;
                }

                connection.Close();
            }
            return hasScheduleForToday;
        }


        [Serializable]
        public class UserProfile //Клас для отримання та зберігання даних користувача 
        {
            public bool IsStudent { get; set; } // Тип користувача (Студент)
            public bool IsTeacher { get; set; } // Тип користувача (Вчитель)
            public string Name { get; set; } // Прізвище та Ім'я користувача (якщо вчитель то використовується для виведення розкладу).
            public string Group { get; set; } // Назва групи користувача (лише для студентів)
        }

    }
}
