using System.Drawing;
using System.Windows.Forms;

namespace ScheduleUser
{
    partial class Schedule
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Schedule));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.обратиФайлБазиДанихToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.нещодавноВідкритіToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.налаштуванняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.профільКористувачаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сповіщенняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.темаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.стандартнаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.чорнаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.змінитиНалаштуванняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.довідкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.проПрограмуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.гайдКористуванняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonGetSchedule = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxClassroomSearch = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxTeacherSearch = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxSubjectSearch = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxGroupSearch = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.налаштуванняToolStripMenuItem,
            this.довідкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 64);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(794, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.обратиФайлБазиДанихToolStripMenuItem,
            this.нещодавноВідкритіToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            this.файлToolStripMenuItem.DropDownClosed += new System.EventHandler(this.dropDownClosedItemClick1);
            this.файлToolStripMenuItem.DropDownOpening += new System.EventHandler(this.dropDownOpenedItemClick1);
            // 
            // обратиФайлБазиДанихToolStripMenuItem
            // 
            this.обратиФайлБазиДанихToolStripMenuItem.Name = "обратиФайлБазиДанихToolStripMenuItem";
            this.обратиФайлБазиДанихToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.обратиФайлБазиДанихToolStripMenuItem.Text = "Обрати файл бази даних";
            this.обратиФайлБазиДанихToolStripMenuItem.Click += new System.EventHandler(this.обратиФайлБазиДанихToolStripMenuItem_Click);
            // 
            // нещодавноВідкритіToolStripMenuItem
            // 
            this.нещодавноВідкритіToolStripMenuItem.Name = "нещодавноВідкритіToolStripMenuItem";
            this.нещодавноВідкритіToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.нещодавноВідкритіToolStripMenuItem.Text = "Нещодавно відкриті";
            // 
            // налаштуванняToolStripMenuItem
            // 
            this.налаштуванняToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.профільКористувачаToolStripMenuItem,
            this.сповіщенняToolStripMenuItem,
            this.темаToolStripMenuItem,
            this.змінитиНалаштуванняToolStripMenuItem});
            this.налаштуванняToolStripMenuItem.Name = "налаштуванняToolStripMenuItem";
            this.налаштуванняToolStripMenuItem.Size = new System.Drawing.Size(110, 20);
            this.налаштуванняToolStripMenuItem.Text = "Налаштування";
            this.налаштуванняToolStripMenuItem.DropDownClosed += new System.EventHandler(this.dropDownClosedItemClick2);
            this.налаштуванняToolStripMenuItem.DropDownOpening += new System.EventHandler(this.dropDownOpenedItemClick2);
            // 
            // профільКористувачаToolStripMenuItem
            // 
            this.профільКористувачаToolStripMenuItem.Name = "профільКористувачаToolStripMenuItem";
            this.профільКористувачаToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.профільКористувачаToolStripMenuItem.Text = "Профіль користувача";
            this.профільКористувачаToolStripMenuItem.Click += new System.EventHandler(this.профільКористувачаToolStripMenuItem_Click);
            // 
            // сповіщенняToolStripMenuItem
            // 
            this.сповіщенняToolStripMenuItem.CheckOnClick = true;
            this.сповіщенняToolStripMenuItem.Name = "сповіщенняToolStripMenuItem";
            this.сповіщенняToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.сповіщенняToolStripMenuItem.Text = "Сповіщення";
            this.сповіщенняToolStripMenuItem.Click += new System.EventHandler(this.сповіщенняToolStripMenuItem_Click);
            // 
            // темаToolStripMenuItem
            // 
            this.темаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.стандартнаToolStripMenuItem,
            this.чорнаToolStripMenuItem});
            this.темаToolStripMenuItem.Name = "темаToolStripMenuItem";
            this.темаToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.темаToolStripMenuItem.Text = "Тема";
            // 
            // стандартнаToolStripMenuItem
            // 
            this.стандартнаToolStripMenuItem.Checked = true;
            this.стандартнаToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.стандартнаToolStripMenuItem.Name = "стандартнаToolStripMenuItem";
            this.стандартнаToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.стандартнаToolStripMenuItem.Text = "Стандартна";
            this.стандартнаToolStripMenuItem.Click += new System.EventHandler(this.стандартнаToolStripMenuItem_Click);
            // 
            // чорнаToolStripMenuItem
            // 
            this.чорнаToolStripMenuItem.Name = "чорнаToolStripMenuItem";
            this.чорнаToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.чорнаToolStripMenuItem.Text = "Чорна";
            this.чорнаToolStripMenuItem.Click += new System.EventHandler(this.чорнаToolStripMenuItem_Click);
            // 
            // змінитиНалаштуванняToolStripMenuItem
            // 
            this.змінитиНалаштуванняToolStripMenuItem.Name = "змінитиНалаштуванняToolStripMenuItem";
            this.змінитиНалаштуванняToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.змінитиНалаштуванняToolStripMenuItem.Text = "Змінити налаштування";
            this.змінитиНалаштуванняToolStripMenuItem.Click += new System.EventHandler(this.змінитиНалаштуванняToolStripMenuItem_Click);
            // 
            // довідкаToolStripMenuItem
            // 
            this.довідкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.проПрограмуToolStripMenuItem,
            this.гайдКористуванняToolStripMenuItem});
            this.довідкаToolStripMenuItem.Name = "довідкаToolStripMenuItem";
            this.довідкаToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.довідкаToolStripMenuItem.Text = "Довідка";
            this.довідкаToolStripMenuItem.DropDownClosed += new System.EventHandler(this.dropDownClosedItemClick3);
            this.довідкаToolStripMenuItem.DropDownOpening += new System.EventHandler(this.dropDownOpenedItemClick3);
            // 
            // проПрограмуToolStripMenuItem
            // 
            this.проПрограмуToolStripMenuItem.Name = "проПрограмуToolStripMenuItem";
            this.проПрограмуToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.проПрограмуToolStripMenuItem.Text = "Про програму";
            this.проПрограмуToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.проПрограмуToolStripMenuItem.Click += new System.EventHandler(this.проПрограмуToolStripMenuItem_Click);
            // 
            // гайдКористуванняToolStripMenuItem
            // 
            this.гайдКористуванняToolStripMenuItem.Name = "гайдКористуванняToolStripMenuItem";
            this.гайдКористуванняToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.гайдКористуванняToolStripMenuItem.Text = "Гайд користування";
            this.гайдКористуванняToolStripMenuItem.Click += new System.EventHandler(this.гайдКористуванняToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(3, 88);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(794, 469);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(786, 441);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Загальний розклад";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(250, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 25);
            this.label1.TabIndex = 23;
            this.label1.Text = "Розклад занять за ...\r\n";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(3, 31);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersWidth = 70;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Size = new System.Drawing.Size(780, 407);
            this.dataGridView1.TabIndex = 21;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.buttonGetSchedule);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.textBoxClassroomSearch);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.textBoxTeacherSearch);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.textBoxSubjectSearch);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.textBoxGroupSearch);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(786, 441);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Фільтр";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Roboto", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(266, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 25);
            this.label2.TabIndex = 44;
            this.label2.Text = "Розклад занять за ...\r\n";
            // 
            // buttonGetSchedule
            // 
            this.buttonGetSchedule.BackColor = System.Drawing.Color.LemonChiffon;
            this.buttonGetSchedule.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonGetSchedule.Location = new System.Drawing.Point(20, 170);
            this.buttonGetSchedule.Name = "buttonGetSchedule";
            this.buttonGetSchedule.Size = new System.Drawing.Size(241, 34);
            this.buttonGetSchedule.TabIndex = 42;
            this.buttonGetSchedule.Text = "Знайти записи";
            this.buttonGetSchedule.UseVisualStyleBackColor = false;
            this.buttonGetSchedule.Click += new System.EventHandler(this.buttonGetSchedule_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(19, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 14);
            this.label8.TabIndex = 41;
            this.label8.Text = "Аудиторія";
            // 
            // textBoxClassroomSearch
            // 
            this.textBoxClassroomSearch.Location = new System.Drawing.Point(22, 131);
            this.textBoxClassroomSearch.Name = "textBoxClassroomSearch";
            this.textBoxClassroomSearch.Size = new System.Drawing.Size(100, 23);
            this.textBoxClassroomSearch.TabIndex = 40;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(130, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 14);
            this.label10.TabIndex = 39;
            this.label10.Text = "Викладач";
            // 
            // textBoxTeacherSearch
            // 
            this.textBoxTeacherSearch.AutoCompleteCustomSource.AddRange(new string[] {
            "Сухойваненко Ю.М.",
            "Суровицький М.М.",
            "Суровицька О.І."});
            this.textBoxTeacherSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxTeacherSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxTeacherSearch.Location = new System.Drawing.Point(133, 131);
            this.textBoxTeacherSearch.Name = "textBoxTeacherSearch";
            this.textBoxTeacherSearch.Size = new System.Drawing.Size(130, 23);
            this.textBoxTeacherSearch.TabIndex = 38;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(130, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 14);
            this.label11.TabIndex = 37;
            this.label11.Text = "Назва предмету";
            // 
            // textBoxSubjectSearch
            // 
            this.textBoxSubjectSearch.Location = new System.Drawing.Point(133, 75);
            this.textBoxSubjectSearch.Name = "textBoxSubjectSearch";
            this.textBoxSubjectSearch.Size = new System.Drawing.Size(130, 23);
            this.textBoxSubjectSearch.TabIndex = 36;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(19, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 14);
            this.label12.TabIndex = 35;
            this.label12.Text = "Группа";
            // 
            // textBoxGroupSearch
            // 
            this.textBoxGroupSearch.AutoCompleteCustomSource.AddRange(new string[] {
            "11КІ",
            "21КІ",
            "31КІ",
            "41КІ"});
            this.textBoxGroupSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxGroupSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBoxGroupSearch.Location = new System.Drawing.Point(22, 75);
            this.textBoxGroupSearch.Name = "textBoxGroupSearch";
            this.textBoxGroupSearch.Size = new System.Drawing.Size(100, 23);
            this.textBoxGroupSearch.TabIndex = 34;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new System.Drawing.Point(17, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(238, 15);
            this.label15.TabIndex = 33;
            this.label15.Text = "Знайти запис за цей день в базі даних";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView2.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.EnableHeadersVisualStyles = false;
            this.dataGridView2.Location = new System.Drawing.Point(271, 33);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 70;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView2.Size = new System.Drawing.Size(507, 405);
            this.dataGridView2.TabIndex = 31;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(786, 441);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Мій розклад";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Roboto", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(190, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(260, 25);
            this.label3.TabIndex = 33;
            this.label3.Text = "Мій розклад занять за ...\r\n";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView3.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView3.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView3.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.EnableHeadersVisualStyles = false;
            this.dataGridView3.Location = new System.Drawing.Point(160, 36);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView3.RowHeadersWidth = 70;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView3.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView3.Size = new System.Drawing.Size(470, 400);
            this.dataGridView3.TabIndex = 32;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Розклад";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(180, 92);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem1.Text = "Загальний розклад";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem2.Text = "Мій розклад";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.CheckOnClick = true;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem4.Text = "Сповіщення";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem3.Text = "Вийти з програми";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePicker1.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePicker1.Location = new System.Drawing.Point(628, 31);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(169, 23);
            this.dateTimePicker1.TabIndex = 22;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(477, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 16);
            this.label4.TabIndex = 23;
            this.label4.Text = "Встановлена дата:";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Schedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Schedule";
            this.Sizable = false;
            this.Text = "Розклад: Користувач";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Schedule_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem обратиФайлБазиДанихToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem налаштуванняToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem профільКористувачаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сповіщенняToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem темаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem стандартнаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem чорнаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem довідкаToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonGetSchedule;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxClassroomSearch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxTeacherSearch;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxSubjectSearch;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxGroupSearch;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem проПрограмуToolStripMenuItem;
        private ToolStripMenuItem гайдКористуванняToolStripMenuItem;
        private ToolStripMenuItem нещодавноВідкритіToolStripMenuItem;
        private ToolStripMenuItem змінитиНалаштуванняToolStripMenuItem;
    }
}

