namespace LevelEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void createNewLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (CreateLevelForm fileCreationForm = new CreateLevelForm())
            {
                if (fileCreationForm.ShowDialog() == DialogResult.OK)
                {
                    // Access the values entered by the user
                    int width = fileCreationForm.WidthValue;
                    int height = fileCreationForm.HeightValue;
                    string title = fileCreationForm.TitleValue;

                    // Use the values to create a file (or perform any other action)
                    // For demonstration purposes, we'll just display the values in a message box
                    MessageBox.Show($"File created with Width: {width}, Height: {height}, Title: {title}", "File Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void grassToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();
        }

        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}