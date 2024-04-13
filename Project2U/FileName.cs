using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project2U
{
    internal class FileName
    {
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

    }
}
