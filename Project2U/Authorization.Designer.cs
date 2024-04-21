namespace ScheduleUser
{
    partial class Authorization
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
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxTeacher = new System.Windows.Forms.CheckBox();
            this.checkBoxStudent = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.comboBoxGroup = new System.Windows.Forms.ComboBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(22, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Зберігання даних користувача";
            // 
            // checkBoxTeacher
            // 
            this.checkBoxTeacher.AutoSize = true;
            this.checkBoxTeacher.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxTeacher.Location = new System.Drawing.Point(141, 74);
            this.checkBoxTeacher.Name = "checkBoxTeacher";
            this.checkBoxTeacher.Size = new System.Drawing.Size(76, 19);
            this.checkBoxTeacher.TabIndex = 1;
            this.checkBoxTeacher.Text = "Вчитель";
            this.checkBoxTeacher.UseVisualStyleBackColor = true;
            this.checkBoxTeacher.CheckedChanged += new System.EventHandler(this.checkBoxTeacher_CheckedChanged);
            // 
            // checkBoxStudent
            // 
            this.checkBoxStudent.AutoSize = true;
            this.checkBoxStudent.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxStudent.Location = new System.Drawing.Point(59, 74);
            this.checkBoxStudent.Name = "checkBoxStudent";
            this.checkBoxStudent.Size = new System.Drawing.Size(75, 19);
            this.checkBoxStudent.TabIndex = 2;
            this.checkBoxStudent.Text = "Студент";
            this.checkBoxStudent.UseVisualStyleBackColor = true;
            this.checkBoxStudent.CheckedChanged += new System.EventHandler(this.checkBoxStudent_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(83, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Тип користувача";
            // 
            // buttonSave
            // 
            this.buttonSave.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSave.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonSave.Location = new System.Drawing.Point(3, 166);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(125, 30);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Зберегти дані";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonClear.ForeColor = System.Drawing.Color.Red;
            this.buttonClear.Location = new System.Drawing.Point(128, 166);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(125, 30);
            this.buttonClear.TabIndex = 5;
            this.buttonClear.Text = "Вийти з профілю";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxName.Location = new System.Drawing.Point(58, 105);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(158, 23);
            this.textBoxName.TabIndex = 6;
            this.textBoxName.Text = "Прізвище та Ім\'я";
            // 
            // comboBoxGroup
            // 
            this.comboBoxGroup.AutoCompleteCustomSource.AddRange(new string[] {
            "11KI",
            "21KI",
            "31KI",
            "41KI"});
            this.comboBoxGroup.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxGroup.FormattingEnabled = true;
            this.comboBoxGroup.Items.AddRange(new object[] {
            "11KI",
            "21KI",
            "31KI",
            "41KI"});
            this.comboBoxGroup.Location = new System.Drawing.Point(58, 134);
            this.comboBoxGroup.Name = "comboBoxGroup";
            this.comboBoxGroup.Size = new System.Drawing.Size(158, 23);
            this.comboBoxGroup.TabIndex = 8;
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonClose.ForeColor = System.Drawing.Color.Red;
            this.buttonClose.Location = new System.Drawing.Point(232, 2);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(25, 25);
            this.buttonClose.TabIndex = 9;
            this.buttonClose.Text = "X";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // Authorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 209);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.comboBoxGroup);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxStudent);
            this.Controls.Add(this.checkBoxTeacher);
            this.Controls.Add(this.label1);
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Authorization";
            this.Padding = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Форма входу";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxTeacher;
        private System.Windows.Forms.CheckBox checkBoxStudent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.ComboBox comboBoxGroup;
        private System.Windows.Forms.Button buttonClose;
    }
}