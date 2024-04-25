﻿using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace ScheduleUser
{
    public partial class Schedule
    {
        DateTime currentDate;

        // Метод для встановлення часу та перевірки поточного часу
        private void CheckTime(DateTime currentDate)
        {
            DateTime nextClassStartTime = currentDate.AddDays(1);
            DateTime currentClassStartTime = currentDate;

            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                string timeBeginHeader = GetTimeBeginHeader(row.Index);
                DateTime classStartTime = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);

                // Знаходимо час початку наступної пари
                if (classStartTime > DateTime.Now && classStartTime < nextClassStartTime)
                {
                    nextClassStartTime = classStartTime;
                }
                // Знаходимо час початку поточної пари
                if (classStartTime < DateTime.Now && DateTime.Now < nextClassStartTime)
                {
                    currentClassStartTime = classStartTime;
                }
            }
            TimeSpan timeUntilNextClass = nextClassStartTime - DateTime.Now;
            timer1.Interval = (int)timeUntilNextClass.TotalMilliseconds; //Оновлення інтервалу таймера
            CheckAndSendNotification();
        }

        // Метод для перевірки та відправлення сповіщення
        private void CheckAndSendNotification()
        {
            // Отримання поточної дати
            DateTime currentDate = DateTime.Today;

            // Перевірка часу початку пари
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                // Отримання часу початку пари з першого стовпця
                string timeBeginHeader = GetTimeBeginHeader(i); // Отримання часу початку пари
                DateTime timeBegin = DateTime.ParseExact(timeBeginHeader, "HH:mm", CultureInfo.InvariantCulture);
                // Якщо поточний час співпадає з часом початку пари, відправити сповіщення
                if (DateTime.Now.AddMinutes(5) >= timeBegin && DateTime.Now <= timeBegin.AddMinutes(5))
                {
                    UserProfile userProfile = GetUserProfile();
                    string notificationTitle = $"{GetTimeHeader(i)}";// Отримання пари за порядком для сповіщення
                    LoadPersonalSchedule(currentDate.ToString("dd.MM.yyyy"),true);
                    if (!String.IsNullOrEmpty(dataGridView3.Rows[i].Cells[1].Value as String))
                    {
                        // Отримання інформації про предмет, викладача та аудиторію з відповідних стовпців dataGridView3
                        string schedule = dataGridView3.Rows[i].Cells[1].Value.ToString();
                        string notificationBody = $"{userProfile.Name}, початок {i + 1} пари.\n{schedule}";
                        SendNotification(notificationTitle, notificationBody);
                        break;
                    }
                }
            }

        }

        // Метод для відправлення сповіщення
        public void SendNotification(string title, string body)
        {
            // Створення вмісту сповіщення
            new ToastContentBuilder()
                .AddText(title)
                .AddText(body)
                .Show();
        }
    }
}