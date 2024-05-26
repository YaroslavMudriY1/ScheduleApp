using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ScheduleUser
{
    public partial class AboutInfo : MaterialForm
    {
        public AboutInfo(bool isDarkTheme)
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);

            if (isDarkTheme)
            {
                buttonOK.BackColor = System.Drawing.Color.FromArgb(255, 48, 48, 48);
                buttonOK.ForeColor = SystemColors.ControlLightLight; // Встановлення білого кольору тексту для темної теми
            }

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void labelGuide_Click(object sender, EventArgs e)
        {
            // Створення та відображення нової форми дляперегляду гайду
            Guide guide = new Guide();
            guide.Show();
        }
    }
}
