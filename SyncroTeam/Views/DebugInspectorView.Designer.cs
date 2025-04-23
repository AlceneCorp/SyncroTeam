namespace SyncroTeam.UI.Debug
{
    partial class DebugInspectorView
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxSearch = new TextBox();
            panelSearch = new Panel();
            btnClear = new Button();
            propertyGrid = new PropertyGrid();
            panelSearch.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSearch
            // 
            textBoxSearch.Dock = DockStyle.Fill;
            textBoxSearch.Font = new Font("Segoe UI", 12F);
            textBoxSearch.Location = new Point(2, 2);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "Rechercher une propriété...";
            textBoxSearch.Size = new Size(396, 39);
            textBoxSearch.TabIndex = 0;
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(btnClear);
            panelSearch.Controls.Add(textBoxSearch);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(0, 0);
            panelSearch.Name = "panelSearch";
            panelSearch.Padding = new Padding(2);
            panelSearch.Size = new Size(400, 40);
            panelSearch.TabIndex = 2;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.White;
            btnClear.Cursor = Cursors.Hand;
            btnClear.Dock = DockStyle.Right;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Location = new Point(358, 2);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(40, 36);
            btnClear.TabIndex = 3;
            btnClear.Text = "✖";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // propertyGrid
            // 
            propertyGrid.BackColor = SystemColors.Control;
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new Point(0, 40);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(400, 460);
            propertyGrid.TabIndex = 3;
            // 
            // DebugInspectorView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(propertyGrid);
            Controls.Add(panelSearch);
            Name = "DebugInspectorView";
            Size = new Size(400, 500);
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBoxSearch;
        private Panel panelSearch;
        private Button btnClear;
        private PropertyGrid propertyGrid;
    }
}
