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


namespace ScheduleAdmin
{
    public partial class Settings : MaterialForm
    {
        // Шлях до файлу конфігурації
        private readonly string configFilePath = "settings.xml";
        
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

        public Settings()
        {

            InitializeComponent();
            //Налаштування теми форми
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

            //Read and load Settings.xml
            LoadSettings();
        }

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

                checkBoxLastConnect.Checked = settings.AutoLastConnected;
                comboBoxRecentlyOpenedList.Text = Convert.ToString(settings.RecentlyOpenedList);
                checkBoxTrayIcon.Checked = settings.TrayIcon;

                textBoxTime1_start.Text = settings.Time1Start;
                textBoxTime2_start.Text = settings.Time2Start;
                textBoxTime3_start.Text = settings.Time3Start;
                textBoxTime4_start.Text = settings.Time4Start;
                textBoxTime5_start.Text = settings.Time5Start;
                textBoxTime6_start.Text = settings.Time6Start;
                textBoxTime1_end.Text = settings.Time1End;
                textBoxTime2_end.Text = settings.Time2End;
                textBoxTime3_end.Text = settings.Time3End;
                textBoxTime4_end.Text = settings.Time4End;
                textBoxTime5_end.Text = settings.Time5End;
                textBoxTime6_end.Text = settings.Time6End;
            }
            else
            {
                SetDefaultSettings();
                MessageBox.Show("Файл налаштувань не знайдений. Встановлено налаштування за замовчуванням.");
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
            settings.TrayIcon = checkBoxTrayIcon.Checked;

            settings.Time1Start = textBoxTime1_start.Text;
            settings.Time2Start = textBoxTime2_start.Text;
            settings.Time3Start = textBoxTime3_start.Text;
            settings.Time4Start = textBoxTime4_start.Text;
            settings.Time5Start = textBoxTime5_start.Text;
            settings.Time6Start = textBoxTime6_start.Text;
            settings.Time1End = textBoxTime1_end.Text;
            settings.Time2End = textBoxTime2_end.Text;
            settings.Time3End = textBoxTime3_end.Text;
            settings.Time4End = textBoxTime4_end.Text;
            settings.Time5End = textBoxTime5_end.Text;
            settings.Time6End = textBoxTime6_end.Text;

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
            checkBoxTrayIcon.Checked = false;

            //Час занять
            textBoxTime1_start.Text = "08:00";
            textBoxTime2_start.Text = "09:15";
            textBoxTime3_start.Text = "10:30";
            textBoxTime4_start.Text = "12:00";
            textBoxTime5_start.Text = "13:15";
            textBoxTime6_start.Text = "14:30";
            textBoxTime1_end.Text = "09:00";
            textBoxTime2_end.Text = "10:15";
            textBoxTime3_end.Text = "11:30";
            textBoxTime4_end.Text = "13:00";
            textBoxTime5_end.Text = "14:15";
            textBoxTime6_end.Text = "15:30";

            buttonSave_Click(null, null); // Зберегти налаштування за замовчуванням
        }

    }
    [Serializable]
    public class UserSettings
    {
        public bool AutoLastConnected { get; set; }
        public int RecentlyOpenedList { get; set; }
        public bool TrayIcon { get; set; }

        public string Time1Start { get; set; }
        public string Time2Start { get; set; }
        public string Time3Start { get; set; }
        public string Time4Start { get; set; }
        public string Time5Start { get; set; }
        public string Time6Start { get; set; }
        public string Time1End { get; set; }
        public string Time2End { get; set; }
        public string Time3End { get; set; }
        public string Time4End { get; set; }
        public string Time5End { get; set; }
        public string Time6End { get; set; }
        public List<string> RecentlyOpenedFiles { get; set; } = new List<string>();
    }
}
