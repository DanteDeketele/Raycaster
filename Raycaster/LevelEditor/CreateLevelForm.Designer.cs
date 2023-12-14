namespace LevelEditor
{
    partial class CreateLevelForm
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
            txtWidth = new TextBox();
            txtHeight = new TextBox();
            txtTitle = new TextBox();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // txtWidth
            // 
            txtWidth.Location = new Point(192, 12);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(292, 23);
            txtWidth.TabIndex = 0;
            // 
            // txtHeight
            // 
            txtHeight.Location = new Point(192, 41);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(292, 23);
            txtHeight.TabIndex = 1;
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(192, 70);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(292, 23);
            txtTitle.TabIndex = 2;
            // 
            // button1
            // 
            button1.Font = new Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(12, 232);
            button1.Name = "button1";
            button1.Size = new Size(144, 61);
            button1.TabIndex = 3;
            button1.Text = "Cancel";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(162, 232);
            button2.Name = "button2";
            button2.Size = new Size(322, 61);
            button2.TabIndex = 4;
            button2.Text = "Create Level";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // CreateLevelForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(496, 305);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtTitle);
            Controls.Add(txtHeight);
            Controls.Add(txtWidth);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "CreateLevelForm";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Create a new Level";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtWidth;
        private TextBox txtHeight;
        private TextBox txtTitle;
        private Button button1;
        private Button button2;
    }
}