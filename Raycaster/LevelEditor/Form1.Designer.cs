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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStrip1 = new ToolStrip();
            toolStripSplitButton1 = new ToolStripSplitButton();
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
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            grassToolStripMenuItem = new ToolStripMenuItem();
            wallsToolStripMenuItem = new ToolStripMenuItem();
            defaultToolStripMenuItem = new ToolStripMenuItem();
            specialToolStripMenuItem1 = new ToolStripMenuItem();
            interactiveToolStripMenuItem = new ToolStripMenuItem();
            doorToolStripMenuItem = new ToolStripMenuItem();
            specialToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 324);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 10, 0);
            statusStrip1.Size = new Size(814, 22);
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
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripSplitButton1, toolStripSeparator1, toolStripSplitButton2, toolStripLabel1, toolStripSeparator2, toolStripDropDownButton1 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(814, 31);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem, aboutToolStripMenuItem });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(40, 28);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(180, 22);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(180, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 31);
            // 
            // toolStripSplitButton2
            // 
            toolStripSplitButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton2.DropDownItems.AddRange(new ToolStripItem[] { createNewLevelToolStripMenuItem, selectLevelToolStripMenuItem, selectFolderToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem });
            toolStripSplitButton2.Image = (Image)resources.GetObject("toolStripSplitButton2.Image");
            toolStripSplitButton2.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton2.Name = "toolStripSplitButton2";
            toolStripSplitButton2.Size = new Size(40, 28);
            toolStripSplitButton2.Text = "toolStripSplitButton2";
            // 
            // createNewLevelToolStripMenuItem
            // 
            createNewLevelToolStripMenuItem.Name = "createNewLevelToolStripMenuItem";
            createNewLevelToolStripMenuItem.Size = new Size(141, 22);
            createNewLevelToolStripMenuItem.Text = "Create Level";
            createNewLevelToolStripMenuItem.Click += createNewLevelToolStripMenuItem_Click;
            // 
            // selectLevelToolStripMenuItem
            // 
            selectLevelToolStripMenuItem.Name = "selectLevelToolStripMenuItem";
            selectLevelToolStripMenuItem.Size = new Size(141, 22);
            selectLevelToolStripMenuItem.Text = "Select Level";
            // 
            // selectFolderToolStripMenuItem
            // 
            selectFolderToolStripMenuItem.Name = "selectFolderToolStripMenuItem";
            selectFolderToolStripMenuItem.Size = new Size(141, 22);
            selectFolderToolStripMenuItem.Text = "Select Folder";
            selectFolderToolStripMenuItem.Click += selectFolderToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(141, 22);
            saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(141, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(69, 28);
            toolStripLabel1.Text = "Level Name";
            toolStripLabel1.Click += toolStripLabel1_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 31);
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { grassToolStripMenuItem, wallsToolStripMenuItem, interactiveToolStripMenuItem, specialToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(37, 28);
            toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            toolStripDropDownButton1.Click += toolStripDropDownButton1_Click;
            // 
            // grassToolStripMenuItem
            // 
            grassToolStripMenuItem.Name = "grassToolStripMenuItem";
            grassToolStripMenuItem.Size = new Size(129, 22);
            grassToolStripMenuItem.Text = "Air";
            grassToolStripMenuItem.Click += grassToolStripMenuItem_Click;
            // 
            // wallsToolStripMenuItem
            // 
            wallsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { defaultToolStripMenuItem, specialToolStripMenuItem1 });
            wallsToolStripMenuItem.Name = "wallsToolStripMenuItem";
            wallsToolStripMenuItem.Size = new Size(129, 22);
            wallsToolStripMenuItem.Text = "Walls";
            // 
            // defaultToolStripMenuItem
            // 
            defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            defaultToolStripMenuItem.Size = new Size(112, 22);
            defaultToolStripMenuItem.Text = "Default";
            // 
            // specialToolStripMenuItem1
            // 
            specialToolStripMenuItem1.Name = "specialToolStripMenuItem1";
            specialToolStripMenuItem1.Size = new Size(112, 22);
            specialToolStripMenuItem1.Text = "Special";
            // 
            // interactiveToolStripMenuItem
            // 
            interactiveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { doorToolStripMenuItem });
            interactiveToolStripMenuItem.Name = "interactiveToolStripMenuItem";
            interactiveToolStripMenuItem.Size = new Size(129, 22);
            interactiveToolStripMenuItem.Text = "Interactive";
            // 
            // doorToolStripMenuItem
            // 
            doorToolStripMenuItem.Name = "doorToolStripMenuItem";
            doorToolStripMenuItem.Size = new Size(100, 22);
            doorToolStripMenuItem.Text = "Door";
            // 
            // specialToolStripMenuItem
            // 
            specialToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            specialToolStripMenuItem.Name = "specialToolStripMenuItem";
            specialToolStripMenuItem.Size = new Size(129, 22);
            specialToolStripMenuItem.Text = "Special";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(93, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 346);
            Controls.Add(toolStrip1);
            Controls.Add(statusStrip1);
            Margin = new Padding(2, 2, 2, 2);
            Name = "Form1";
            Text = "Level Editor";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStrip toolStrip1;
        private ToolStripSplitButton toolStripSplitButton1;
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
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem grassToolStripMenuItem;
        private ToolStripMenuItem wallsToolStripMenuItem;
        private ToolStripMenuItem defaultToolStripMenuItem;
        private ToolStripMenuItem specialToolStripMenuItem1;
        private ToolStripMenuItem interactiveToolStripMenuItem;
        private ToolStripMenuItem doorToolStripMenuItem;
        private ToolStripMenuItem specialToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem selectLevelToolStripMenuItem;
    }
}