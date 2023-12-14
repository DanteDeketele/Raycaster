namespace LevelEditor
{
    partial class Form1
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
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStrip1 = new ToolStrip();
            AppInfoDropdown = new ToolStripSplitButton();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripSplitButton2 = new ToolStripSplitButton();
            createNewLevelToolStripMenuItem = new ToolStripMenuItem();
            selectLevelToolStripMenuItem = new ToolStripMenuItem();
            selectFolderToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripLabel1 = new ToolStripLabel();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabel2 = new ToolStripLabel();
            BrushSelectorButton = new ToolStripDropDownButton();
            grassToolStripMenuItem = new ToolStripMenuItem();
            wallsToolStripMenuItem = new ToolStripMenuItem();
            defaultToolStripMenuItem = new ToolStripMenuItem();
            specialToolStripMenuItem1 = new ToolStripMenuItem();
            interactiveToolStripMenuItem = new ToolStripMenuItem();
            doorToolStripMenuItem = new ToolStripMenuItem();
            specialToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            pictureBox1 = new PictureBox();
            statusStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 796);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 10, 0);
            statusStrip1.Size = new Size(1511, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.AccessibleRole = AccessibleRole.IpAddress;
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(70, 16);
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(51, 17);
            toolStripStatusLabel1.Text = "Saving...";
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = Color.Gainsboro;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { AppInfoDropdown, toolStripSeparator1, toolStripSplitButton2, toolStripLabel1, toolStripSeparator2, toolStripLabel2, BrushSelectorButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(1511, 28);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // AppInfoDropdown
            // 
            AppInfoDropdown.AccessibleRole = AccessibleRole.DropList;
            AppInfoDropdown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AppInfoDropdown.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem, aboutToolStripMenuItem });
            AppInfoDropdown.Font = new Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point);
            AppInfoDropdown.ImageTransparentColor = Color.Magenta;
            AppInfoDropdown.Name = "AppInfoDropdown";
            AppInfoDropdown.Size = new Size(111, 25);
            AppInfoDropdown.Text = "Level Editor";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Image = Properties.Resources.gear;
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(135, 24);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(135, 24);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 28);
            // 
            // toolStripSplitButton2
            // 
            toolStripSplitButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripSplitButton2.DropDownItems.AddRange(new ToolStripItem[] { createNewLevelToolStripMenuItem, selectLevelToolStripMenuItem, selectFolderToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem });
            toolStripSplitButton2.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point);
            toolStripSplitButton2.ImageTransparentColor = Color.White;
            toolStripSplitButton2.Name = "toolStripSplitButton2";
            toolStripSplitButton2.Size = new Size(50, 25);
            toolStripSplitButton2.Text = "File";
            // 
            // createNewLevelToolStripMenuItem
            // 
            createNewLevelToolStripMenuItem.Name = "createNewLevelToolStripMenuItem";
            createNewLevelToolStripMenuItem.Size = new Size(180, 26);
            createNewLevelToolStripMenuItem.Text = "Create Level";
            createNewLevelToolStripMenuItem.Click += createNewLevelToolStripMenuItem_Click;
            // 
            // selectLevelToolStripMenuItem
            // 
            selectLevelToolStripMenuItem.Name = "selectLevelToolStripMenuItem";
            selectLevelToolStripMenuItem.Size = new Size(180, 26);
            selectLevelToolStripMenuItem.Text = "Load Level";
            // 
            // selectFolderToolStripMenuItem
            // 
            selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
            selectFolderToolStripMenuItem.Size = new Size(180, 26);
            selectFolderToolStripMenuItem.Text = "Select Folder";
            selectFolderToolStripMenuItem.Click += selectFolderToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(180, 26);
            saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(180, 26);
            saveAsToolStripMenuItem.Text = "Save As";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.AutoSize = false;
            toolStripLabel1.Font = new Font("Century Gothic", 12F, FontStyle.Italic, GraphicsUnit.Point);
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(200, 22);
            toolStripLabel1.Text = "No Level Selected";
            toolStripLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 28);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point);
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(54, 25);
            toolStripLabel2.Text = "Brush:";
            // 
            // BrushSelectorButton
            // 
            BrushSelectorButton.DropDownItems.AddRange(new ToolStripItem[] { grassToolStripMenuItem, wallsToolStripMenuItem, interactiveToolStripMenuItem, specialToolStripMenuItem });
            BrushSelectorButton.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BrushSelectorButton.Name = "BrushSelectorButton";
            BrushSelectorButton.Size = new Size(73, 25);
            BrushSelectorButton.Text = "Empty";
            // 
            // grassToolStripMenuItem
            // 
            grassToolStripMenuItem.Name = "grassToolStripMenuItem";
            grassToolStripMenuItem.Size = new Size(168, 26);
            grassToolStripMenuItem.Text = "Air";
            // 
            // wallsToolStripMenuItem
            // 
            wallsToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            wallsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { defaultToolStripMenuItem, specialToolStripMenuItem1 });
            wallsToolStripMenuItem.Name = "wallsToolStripMenuItem";
            wallsToolStripMenuItem.Size = new Size(168, 26);
            wallsToolStripMenuItem.Text = "Walls";
            // 
            // defaultToolStripMenuItem
            // 
            defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            defaultToolStripMenuItem.Size = new Size(174, 26);
            defaultToolStripMenuItem.Text = "Default Wall";
            // 
            // specialToolStripMenuItem1
            // 
            specialToolStripMenuItem1.Name = "specialToolStripMenuItem1";
            specialToolStripMenuItem1.Size = new Size(174, 26);
            specialToolStripMenuItem1.Text = "Special Wall";
            // 
            // interactiveToolStripMenuItem
            // 
            interactiveToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            interactiveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { doorToolStripMenuItem });
            interactiveToolStripMenuItem.Name = "interactiveToolStripMenuItem";
            interactiveToolStripMenuItem.Size = new Size(168, 26);
            interactiveToolStripMenuItem.Text = "Interactive";
            // 
            // doorToolStripMenuItem
            // 
            doorToolStripMenuItem.Name = "doorToolStripMenuItem";
            doorToolStripMenuItem.Size = new Size(117, 26);
            doorToolStripMenuItem.Text = "Door";
            // 
            // specialToolStripMenuItem
            // 
            specialToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            specialToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            specialToolStripMenuItem.Name = "specialToolStripMenuItem";
            specialToolStripMenuItem.Size = new Size(168, 26);
            specialToolStripMenuItem.Text = "Special";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(107, 26);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 28);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1511, 768);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.SizeChanged += pictureBox1_Resize;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1511, 818);
            Controls.Add(pictureBox1);
            Controls.Add(toolStrip1);
            Controls.Add(statusStrip1);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Level Editor";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStrip toolStrip1;
        private ToolStripSplitButton AppInfoDropdown;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSplitButton toolStripSplitButton2;
        private ToolStripMenuItem createNewLevelToolStripMenuItem;
        private ToolStripMenuItem selectFolderToolStripMenuItem;
        private ToolStripLabel toolStripLabel1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripDropDownButton BrushSelectorButton;
        private ToolStripMenuItem grassToolStripMenuItem;
        private ToolStripMenuItem wallsToolStripMenuItem;
        private ToolStripMenuItem defaultToolStripMenuItem;
        private ToolStripMenuItem specialToolStripMenuItem1;
        private ToolStripMenuItem interactiveToolStripMenuItem;
        private ToolStripMenuItem doorToolStripMenuItem;
        private ToolStripMenuItem specialToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem selectLevelToolStripMenuItem;
        private ToolStripLabel toolStripLabel2;
        private PictureBox pictureBox1;
    }
}