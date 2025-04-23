namespace SyncroTeam
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            statusStrip1 = new StatusStrip();
            panelNavigation = new Panel();
            btnEcrans = new Button();
            btnActivites = new Button();
            btnHoraires = new Button();
            comboBoxWeeks = new ComboBox();
            panelComboBox = new Panel();
            panelContent = new Panel();
            fichierToolStripMenuItem = new ToolStripMenuItem();
            outilsToolStripMenuItem = new ToolStripMenuItem();
            debugToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            panelNavigation.SuspendLayout();
            panelComboBox.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fichierToolStripMenuItem, outilsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1898, 33);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Location = new Point(0, 946);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1898, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // panelNavigation
            // 
            panelNavigation.Controls.Add(btnEcrans);
            panelNavigation.Controls.Add(btnActivites);
            panelNavigation.Controls.Add(btnHoraires);
            panelNavigation.Dock = DockStyle.Top;
            panelNavigation.Location = new Point(0, 33);
            panelNavigation.Name = "panelNavigation";
            panelNavigation.Size = new Size(1898, 50);
            panelNavigation.TabIndex = 2;
            // 
            // btnEcrans
            // 
            btnEcrans.BackColor = Color.Transparent;
            btnEcrans.Dock = DockStyle.Left;
            btnEcrans.FlatStyle = FlatStyle.Flat;
            btnEcrans.Font = new Font("Segoe UI", 12F);
            btnEcrans.Location = new Point(239, 0);
            btnEcrans.Name = "btnEcrans";
            btnEcrans.Size = new Size(112, 50);
            btnEcrans.TabIndex = 2;
            btnEcrans.Text = "Écrans";
            btnEcrans.UseVisualStyleBackColor = false;
            btnEcrans.Click += btnEcrans_Click;
            // 
            // btnActivites
            // 
            btnActivites.BackColor = Color.Transparent;
            btnActivites.Dock = DockStyle.Left;
            btnActivites.FlatStyle = FlatStyle.Flat;
            btnActivites.Font = new Font("Segoe UI", 12F);
            btnActivites.Location = new Point(112, 0);
            btnActivites.Name = "btnActivites";
            btnActivites.Size = new Size(127, 50);
            btnActivites.TabIndex = 1;
            btnActivites.Text = "Activités";
            btnActivites.UseVisualStyleBackColor = false;
            btnActivites.Click += btnActivites_Click;
            // 
            // btnHoraires
            // 
            btnHoraires.BackColor = Color.Transparent;
            btnHoraires.Dock = DockStyle.Left;
            btnHoraires.FlatStyle = FlatStyle.Flat;
            btnHoraires.Font = new Font("Segoe UI", 12F);
            btnHoraires.Location = new Point(0, 0);
            btnHoraires.Name = "btnHoraires";
            btnHoraires.Size = new Size(112, 50);
            btnHoraires.TabIndex = 0;
            btnHoraires.Text = "Horaires";
            btnHoraires.UseVisualStyleBackColor = false;
            btnHoraires.Click += btnHoraires_Click;
            // 
            // comboBoxWeeks
            // 
            comboBoxWeeks.Dock = DockStyle.Fill;
            comboBoxWeeks.Font = new Font("Segoe UI", 12F);
            comboBoxWeeks.FormattingEnabled = true;
            comboBoxWeeks.Location = new Point(0, 0);
            comboBoxWeeks.Margin = new Padding(10, 5, 10, 5);
            comboBoxWeeks.Name = "comboBoxWeeks";
            comboBoxWeeks.Size = new Size(1898, 40);
            comboBoxWeeks.TabIndex = 4;
            comboBoxWeeks.SelectedIndexChanged += comboBoxWeeks_SelectedIndexChanged;
            // 
            // panelComboBox
            // 
            panelComboBox.BackColor = Color.Transparent;
            panelComboBox.Controls.Add(comboBoxWeeks);
            panelComboBox.Dock = DockStyle.Top;
            panelComboBox.Location = new Point(0, 83);
            panelComboBox.Name = "panelComboBox";
            panelComboBox.Size = new Size(1898, 40);
            panelComboBox.TabIndex = 5;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.White;
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 123);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(1898, 823);
            panelContent.TabIndex = 6;
            // 
            // fichierToolStripMenuItem
            // 
            fichierToolStripMenuItem.Name = "fichierToolStripMenuItem";
            fichierToolStripMenuItem.Size = new Size(78, 29);
            fichierToolStripMenuItem.Text = "&Fichier";
            // 
            // outilsToolStripMenuItem
            // 
            outilsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { debugToolStripMenuItem });
            outilsToolStripMenuItem.Name = "outilsToolStripMenuItem";
            outilsToolStripMenuItem.Size = new Size(74, 29);
            outilsToolStripMenuItem.Text = "&Outils";
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new Size(270, 34);
            debugToolStripMenuItem.Text = "&Debug";
            debugToolStripMenuItem.Click += debugToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1898, 968);
            Controls.Add(panelContent);
            Controls.Add(panelComboBox);
            Controls.Add(panelNavigation);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximumSize = new Size(1920, 1024);
            MinimumSize = new Size(1920, 1024);
            Name = "MainForm";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panelNavigation.ResumeLayout(false);
            panelComboBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private StatusStrip statusStrip1;
        private Panel panelNavigation;
        private Button btnEcrans;
        private Button btnActivites;
        private Button btnHoraires;
        private ComboBox comboBoxWeeks;
        private Panel panelComboBox;
        private Panel panelContent;
        private ToolStripMenuItem fichierToolStripMenuItem;
        private ToolStripMenuItem outilsToolStripMenuItem;
        private ToolStripMenuItem debugToolStripMenuItem;
    }
}
