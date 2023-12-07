﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class CreateLevelForm : Form
    {
        public int WidthValue { get; private set; }
        public int HeightValue { get; private set; }
        public string TitleValue { get; private set; }

        public CreateLevelForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtWidth.Text, out int width) && int.TryParse(txtHeight.Text, out int height))
            {
                WidthValue = width;
                HeightValue = height;
                TitleValue = txtTitle.Text;

                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please enter valid numeric values for width and height.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void CreateLevelForm_Load(object sender, EventArgs e)
        {

        }
    }
}
