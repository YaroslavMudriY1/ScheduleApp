using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Project2U
{
    public partial class AboutInfo : MaterialForm
    {
        public AboutInfo()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);
            // Встановлюємо закруглені краї
           // this.FormBorderStyle = FormBorderStyles.SizableRound;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
