﻿using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using MaterialSkin;
using MaterialSkin.Controls;
namespace ScheduleUser
{
    public partial class Authorization : MaterialForm
    {
        public delegate void AuthorizationClosedEventHandler();
        public event AuthorizationClosedEventHandler AuthorizationClosed;

        private readonly string configFilePath = "userProfile.xml";
        public Authorization()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

            // Заповнення форми даними з файлу, якщо вони є
            LoadUserProfile();
            // Приховати поле comboBoxGroup на початку
            if (!checkBoxStudent.Checked)
            { comboBoxGroup.Visible = false; }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Створення об'єкта користувача на основі введених даних
            UserProfile user = new UserProfile
            {
                IsStudent = checkBoxStudent.Checked,
                IsTeacher = checkBoxTeacher.Checked,
                Name = textBoxName.Text,
                Group = comboBoxGroup.Visible ? comboBoxGroup.Text.ToString() : ""
            };

            // Збереження об'єкту користувача в XML файл
            SaveUserProfile(user);
            AuthorizationClosed?.Invoke(); // Викликаємо подію при закритті форми
            this.Close();

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            // Перевірка, чи поля форми не заповнені
            if (File.Exists(configFilePath))
            {
                // Виклик функції для видалення даних про користувача з XML файлу
                DeleteUserProfile();
                // Очищення полів форми
                checkBoxStudent.Checked = false;
                checkBoxTeacher.Checked = false;
                textBoxName.Text = "";
                comboBoxGroup.SelectedIndex = -1;
                // Повідомлення про успішну дію
                MessageBox.Show("Дані користувача видалено успішно!", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Повідомлення про неможливу дію
                MessageBox.Show("Дані користувача неможливо видалити - відсутній файл даних.", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            // Закриття вікна
            this.Close();
        }

        private void checkBoxStudent_CheckedChanged(object sender, EventArgs e)
        {
            // Перевірка, чи вибраний тип студент
            if (checkBoxStudent.Checked)
            {
                // Заборона вибору типу вчителя
                checkBoxTeacher.Enabled = false;

                // Дозвіл на вибір групи для студента
                comboBoxGroup.Visible = true;
            }
            else
            {
                // Дозвіл на вибір типу вчителя, якщо вибір типу студента скасований
                checkBoxTeacher.Enabled = true;

                // Вимкнення вибору групи для студента
                comboBoxGroup.Visible = false;
            }
        }
        private void checkBoxTeacher_CheckedChanged(object sender, EventArgs e)
        {
            // Перевірка, чи вибраний тип вчитель
            if (checkBoxTeacher.Checked)
            {
                // Заборона вибору типу студент
                checkBoxStudent.Enabled = false;
            }
            else
            {
                // Дозвіл на вибір типу студент, якщо вибір вчителя скасований
                checkBoxStudent.Enabled = true;
            }
        }
        private void LoadUserProfile()
        {
            // Перевірка наявності файлу профілю користувача
            if (File.Exists(configFilePath))
            {
                using (StreamReader reader = new StreamReader(configFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserProfile));
                    UserProfile profile = (UserProfile)serializer.Deserialize(reader);

                    // Заповнення форми даними з файлу
                    textBoxName.Text = profile.Name;
                    checkBoxStudent.Checked = profile.IsStudent;
                    checkBoxTeacher.Checked = !profile.IsStudent; // У разі якщо користувач не студент, викладач

                    if (profile.IsStudent)
                    {
                        comboBoxGroup.SelectedItem = profile.Group;
                    }
                }
            }
        }
        private void SaveUserProfile(UserProfile user)
        {
            // Збереження об'єкту користувача в XML файл
            XmlSerializer serializer = new XmlSerializer(typeof(UserProfile));
            using (StreamWriter writer = new StreamWriter(configFilePath))
            {
                serializer.Serialize(writer, user);
            }

            MessageBox.Show("Дані збережено успішно!", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void DeleteUserProfile()
        {
            // Перевірка, чи файл існує
            if (File.Exists(configFilePath))
            {
                // Видалення файлу
                File.Delete(configFilePath);
            }
        }

        // Клас, що представляє користувацький профіль
        [Serializable]
        public class UserProfile
        {
            public bool IsStudent { get; set; }
            public bool IsTeacher { get; set; }
            public string Name { get; set; }
            public string Group { get; set; }
        }


    }
}
