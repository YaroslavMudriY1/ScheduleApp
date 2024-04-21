using System;
using System.IO;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ScheduleUser
{
    public partial class Guide : MaterialForm
    {
        public Guide()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green800, Primary.Green900, Primary.Green500, Accent.LightGreen200, TextShade.WHITE);
            string fileName = "GuideUser.html";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            webBrowser1.Url = new Uri(filePath);
        }
    }
}
