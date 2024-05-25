using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using MaterialSkin; // Підключення пакету для покращення інтерфейсу
using MaterialSkin.Controls;


namespace ScheduleUser
{
    public partial class Settings : MaterialForm
    {
        // Шлях до файлу конфігурації
        private readonly string configFilePath = "settings.xml";

        public static class XmlHelper
        {
            public static void SerializeToFile<T>(T data, string filePath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, data);
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

        public Settings()
        {

            InitializeComponent();
            // Налаштування теми форми
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

            // Завантажити налаштування
            LoadSettings();
        }

        public bool autoLastConnected;
        public int recentlyOpenedList;
        public bool deleteUserProfile;
        public bool trayIcon;
        public bool asyncNotification;
        private void LoadSettings()
        {
            if (File.Exists(configFilePath))
            {
                UserSettings settings = XmlHelper.DeserializeFromFile<UserSettings>(configFilePath);

                checkBoxLastConnect.Checked = settings.AutoLastConnected;
                comboBoxRecentlyOpenedList.Text = Convert.ToString(settings.RecentlyOpenedList);
                checkBoxDelUser.Checked = settings.DeleteUserProfile;
                checkBoxTrayIcon.Checked = settings.TrayIcon;
                checkBoxAsyncNotif.Checked = settings.AsyncNotification;

            }
            else
            {
                SetDefaultSettings();
            }
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            UserSettings settings;

            // Завантажити існуючі налаштування, якщо файл існує
            if (File.Exists(configFilePath))
            {
                settings = XmlHelper.DeserializeFromFile<UserSettings>(configFilePath);
            }
            else
            {
                // Якщо файл не існує, створити новий об'єкт налаштувань
                settings = new UserSettings();
            }

            // Оновити тільки ті поля, які змінюються на формі налаштувань
            settings.AutoLastConnected = checkBoxLastConnect.Checked;
            settings.RecentlyOpenedList = Convert.ToInt32(comboBoxRecentlyOpenedList.Text);
            settings.DeleteUserProfile = checkBoxDelUser.Checked;
            settings.TrayIcon = checkBoxTrayIcon.Checked;
            settings.AsyncNotification = checkBoxAsyncNotif.Checked;

            // Зберегти оновлені налаштування у файл
            XmlHelper.SerializeToFile(settings, configFilePath);
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            checkBoxLastConnect.Checked = true;
            comboBoxRecentlyOpenedList.Text = "4";
            checkBoxDelUser.Checked = false;
            checkBoxTrayIcon.Checked = false;
            checkBoxAsyncNotif.Checked = false;

            buttonSave_Click(null, null); // Зберегти налаштування за замовчуванням
        }

    }
    [Serializable]
    public class UserSettings
    {
        public bool AutoLastConnected { get; set; }
        public int RecentlyOpenedList { get; set; }
        public bool DeleteUserProfile { get; set; }
        public bool TrayIcon { get; set; }
        public bool AsyncNotification { get; set; }
        public List<string> RecentlyOpenedFiles { get; set; } = new List<string>();
    }
}
