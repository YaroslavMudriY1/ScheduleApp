using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
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
        string connectionString = "Data Source=db2.db;Version=3;";
        public Schedule()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Додавання стовпців заголовків рядків
            dataGridView1.RowHeadersVisible = true;
/*
            // Отримання всіх доступних назв груп з бази даних
            List<string> groupNames = GetAllGroupNames();

            // Додавання стовпців для кожної групи
            foreach (string groupName in groupNames)
            {
                dataGridView1.Columns.Add(groupName, groupName);
            }

            // Додавання рядків у відповідності до пар
            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = $"{i + 1} пара";
            }*/
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

            while (reader.Read()) {

                string groupName = reader["group_name"].ToString();
                groupNames.Add(groupName);
            }
                        
            return groupNames;
        }

        //Фунція читання бази даних

        private void LoadDataFromDatabase(DateTimePicker dateTimePicker, DataGridView dataGridView)
        {
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

            for (int i = 0; i < 5; i++)
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
                    int columnIndex = GetColumnIndex(groupName);


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

            // Закриття підключення до бази даних
            connection.Close();

        }

        private void buttonCheckSchedule1_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase(dateTimePicker1, dataGridView1); //Використання функції з наданням вихідних об'єктів в аргументів

        }

        // Подія для зміни значення DateTimePicker
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Оновити DataGridView згідно з новим обраним днем тижня
            UpdateDataGridView();
        }

        // Метод для оновлення DataGridView згідно з обраним днем тижня
        private void UpdateDataGridView()
        {
            string selectedDate = dateTimePicker1.Value.ToString("dd.MM.yyyy"); // Отримуємо дату у форматі року-місяця-дня

            // Запит до бази даних для отримання розкладу з урахуванням обраної дати
            string query = "SELECT * FROM Schedule WHERE date = @selectedDate ORDER BY time_start";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selectedDate", selectedDate);
                    SQLiteDataReader reader = command.ExecuteReader();

                    // Очищення DataGridView перед оновленням
                    dataGridView1.Rows.Clear();

                }
            }
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
        private int GetColumnIndex(string groupName)
        {
            // Припустимо, що у  DataGridView стовпці названі як групи
            return dataGridView1.Columns.Cast<DataGridViewColumn>().ToList().FindIndex(column => column.HeaderText == groupName);
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
                default:
                    throw new ArgumentException("Invalid time: " + text);
            }
        }
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
                default:
                    throw new ArgumentException("Invalid time: " + text);
            }
        }

        //Додавання запису
        private void buttonAddData_Click(object sender, EventArgs e)
        {
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
    }
}

