namespace ScheduleUser
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.checkBoxLastConnect = new System.Windows.Forms.CheckBox();
            this.checkBoxAsyncNotif = new System.Windows.Forms.CheckBox();
            this.comboBoxRecentlyOpenedList = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxDelUser = new System.Windows.Forms.CheckBox();
            this.checkBoxTrayIcon = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(169, 325);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(151, 33);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Зберегти зміни";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonDefault
            // 
            this.buttonDefault.Location = new System.Drawing.Point(29, 325);
            this.buttonDefault.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(132, 33);
            this.buttonDefault.TabIndex = 5;
            this.buttonDefault.Text = "За замовчуванням";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // checkBoxLastConnect
            // 
            this.checkBoxLastConnect.AutoSize = true;
            this.checkBoxLastConnect.Checked = true;
            this.checkBoxLastConnect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLastConnect.Location = new System.Drawing.Point(29, 101);
            this.checkBoxLastConnect.Name = "checkBoxLastConnect";
            this.checkBoxLastConnect.Size = new System.Drawing.Size(216, 34);
            this.checkBoxLastConnect.TabIndex = 21;
            this.checkBoxLastConnect.Text = "Підключатися до останньої БД\r\n при відкритті програми";
            this.checkBoxLastConnect.UseVisualStyleBackColor = true;
            // 
            // checkBoxAsyncNotif
            // 
            this.checkBoxAsyncNotif.AutoSize = true;
            this.checkBoxAsyncNotif.Location = new System.Drawing.Point(29, 296);
            this.checkBoxAsyncNotif.Name = "checkBoxAsyncNotif";
            this.checkBoxAsyncNotif.Size = new System.Drawing.Size(164, 19);
            this.checkBoxAsyncNotif.TabIndex = 22;
            this.checkBoxAsyncNotif.Text = "Асинхронні сповіщеня";
            this.checkBoxAsyncNotif.UseVisualStyleBackColor = true;
            this.checkBoxAsyncNotif.Visible = false;
            // 
            // comboBoxRecentlyOpenedList
            // 
            this.comboBoxRecentlyOpenedList.Items.AddRange(new object[] {
            "3",
            "4",
            "5"});
            this.comboBoxRecentlyOpenedList.Location = new System.Drawing.Point(29, 153);
            this.comboBoxRecentlyOpenedList.Name = "comboBoxRecentlyOpenedList";
            this.comboBoxRecentlyOpenedList.Size = new System.Drawing.Size(36, 23);
            this.comboBoxRecentlyOpenedList.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(71, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 30);
            this.label8.TabIndex = 24;
            this.label8.Text = "Кількість файлів у списку \r\nнещодавно відкритих";
            // 
            // checkBoxDelUser
            // 
            this.checkBoxDelUser.AutoSize = true;
            this.checkBoxDelUser.Location = new System.Drawing.Point(29, 202);
            this.checkBoxDelUser.Name = "checkBoxDelUser";
            this.checkBoxDelUser.Size = new System.Drawing.Size(217, 34);
            this.checkBoxDelUser.TabIndex = 25;
            this.checkBoxDelUser.Text = "Видаляти профіль користувача\r\nпри закритті програми";
            this.checkBoxDelUser.UseVisualStyleBackColor = true;
            // 
            // checkBoxTrayIcon
            // 
            this.checkBoxTrayIcon.AutoSize = true;
            this.checkBoxTrayIcon.Location = new System.Drawing.Point(29, 249);
            this.checkBoxTrayIcon.Name = "checkBoxTrayIcon";
            this.checkBoxTrayIcon.Size = new System.Drawing.Size(200, 34);
            this.checkBoxTrayIcon.TabIndex = 26;
            this.checkBoxTrayIcon.Text = "Показувати іконку програми\r\nв треї";
            this.checkBoxTrayIcon.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(37, 70);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 19);
            this.label9.TabIndex = 27;
            this.label9.Text = "Опції";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 370);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.checkBoxTrayIcon);
            this.Controls.Add(this.checkBoxDelUser);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxRecentlyOpenedList);
            this.Controls.Add(this.checkBoxAsyncNotif);
            this.Controls.Add(this.checkBoxLastConnect);
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.buttonSave);
            this.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Налаштування програми";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.CheckBox checkBoxLastConnect;
        private System.Windows.Forms.CheckBox checkBoxAsyncNotif;
        private System.Windows.Forms.ComboBox comboBoxRecentlyOpenedList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBoxDelUser;
        private System.Windows.Forms.CheckBox checkBoxTrayIcon;
        private System.Windows.Forms.Label label9;
    }
}