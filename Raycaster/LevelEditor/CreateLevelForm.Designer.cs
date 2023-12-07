namespace LevelEditor
{
    partial class CreateLevelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblWidth;
        private Label lblHeight;
        private Label lblTitle;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private TextBox txtTitle;
        private Button btnOK;
        private Button btnCancel;
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
            lblWidth = new Label();
            lblHeight = new Label();
            lblTitle = new Label();
            txtWidth = new TextBox();
            txtHeight = new TextBox();
            txtTitle = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.Location = new Point(12, 15);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(44, 13);
            lblWidth.TabIndex = 0;
            lblWidth.Text = "Width:";
            // 
            // lblHeight
            // 
            lblHeight.AutoSize = true;
            lblHeight.Location = new Point(12, 41);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(47, 13);
            lblHeight.TabIndex = 2;
            lblHeight.Text = "Height:";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(12, 67);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(30, 13);
            lblTitle.TabIndex = 4;
            lblTitle.Text = "Title:";
            // 
            // txtWidth
            // 
            txtWidth.Location = new Point(85, 12);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(100, 31);
            txtWidth.TabIndex = 1;
            // 
            // txtHeight
            // 
            txtHeight.Location = new Point(85, 38);
            txtHeight.Name = "txtHeight";
            txtHeight.Size = new Size(100, 31);
            txtHeight.TabIndex = 3;
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(85, 64);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(100, 31);
            txtTitle.TabIndex = 5;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(15, 100);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 6;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(110, 100);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // CreateLevelForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 396);
            Name = "CreateLevelForm";
            Text = "CreateLevelForm";
            Load += CreateLevelForm_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}