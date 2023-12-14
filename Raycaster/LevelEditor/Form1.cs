namespace LevelEditor
{
    public partial class Form1 : Form
    {
        private string _selectedBrush = "Air";
        private int _selectedBrushId = 0;

        private Point GridSize = new Point(20,30);
        private float AspectRatio => GridSize.X / GridSize.Y;
        private int[,] levelData;

        private string _folderPath;

        private Bitmap canvasImage;
        private float zoom = 1.0f;
        private Point canvasOffset = Point.Empty;
        private Point previewCell = Point.Empty;

        public Form1()
        {
            InitializeComponent();

            // Initialize the level data array (e.g., set default values)
            levelData = new int[GridSize.X, GridSize.Y];

            // Create a blank canvas image with the same size as the PictureBox
            canvasImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About(_folderPath);
            aboutForm.ShowDialog();
        }

        private void ChangeBrush(string name, int id)
        {
            _selectedBrush = name;
            _selectedBrushId = id;
        }

        public void ChangeBrush(object sender, BrushEventArgs e)
        {
            ChangeBrush(e.Name, e.Id);
        }

        public class BrushEventArgs : EventArgs
        {
            public int Id;
            public string Name;
            public BrushEventArgs(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // Update the variable or perform any action with the selected folder path
                    _folderPath = folderBrowserDialog.SelectedPath;
                }
            }
        }


        private void DrawGrid(Graphics g)
        {
            // Set the grid color and pen
            Pen gridPen = new Pen(Color.Gray);

            // Calculate grid spacing based on zoom level
            int cellWidth = (int)(pictureBox1.Height / (float)GridSize.X * zoom);
            int cellHeight = (int)(pictureBox1.Height / (float)GridSize.Y * zoom);

            // Draw vertical lines
            for (int i = 0; i <= GridSize.X; i++)
            {
                int x = (int)((i * pictureBox1.Height / (float)GridSize.X - canvasOffset.X) * zoom);

                // Draw only if within the visible bounds of the PictureBox
                if (x >= 0 && x <= pictureBox1.Width)
                {
                    g.DrawLine(gridPen, x, 0, x, pictureBox1.Height);
                }
            }

            // Draw horizontal lines
            for (int i = 0; i <= GridSize.Y; i++)
            {
                int y = (int)((i * pictureBox1.Height / (float)GridSize.Y - canvasOffset.Y) * zoom);

                // Draw only if within the visible bounds of the PictureBox
                if (y >= 0 && y <= pictureBox1.Height)
                {
                    g.DrawLine(gridPen, 0, y, pictureBox1.Width, y);
                }
            }
        }

        private void DrawLevel(Graphics g)
        {
            // Calculate cell size based on zoom level
            int cellWidth = (int)(pictureBox1.Height / (float)GridSize.X * zoom);
            int cellHeight = (int)(pictureBox1.Height / (float)GridSize.Y * zoom);

            // Loop through the levelData array and draw cells based on values
            for (int i = 0; i < GridSize.X; i++)
            {
                for (int j = 0; j < GridSize.Y; j++)
                {
                    // Customize the drawing based on your levelData values
                    Brush cellBrush = levelData[i, j] == 1 ? Brushes.Black : Brushes.White;

                    // Calculate the position based on zoom level and canvas offset
                    float x = (i * pictureBox1.Height / (float)GridSize.X - canvasOffset.X) * zoom;
                    float y = (j * pictureBox1.Height / (float)GridSize.Y - canvasOffset.Y) * zoom;

                    // Draw only if within the visible bounds of the PictureBox
                    if (x >= 0 && x <= pictureBox1.Width && y >= 0 && y <= pictureBox1.Height)
                    {
                        // Draw the cell
                        g.FillRectangle(cellBrush, x, y, cellWidth, cellHeight);
                    }
                }
            }
            DrawPreviewCell(g);
        }

        private void DrawPreviewCell(Graphics g)
        {
            // Calculate cell size based on zoom level
            int cellWidth = (int)(pictureBox1.Height / (float)GridSize.X * zoom);
            int cellHeight = (int)(pictureBox1.Height / (float)GridSize.Y * zoom);

            // Calculate the position based on zoom level and canvas offset for the preview cell
            float x = (previewCell.X * pictureBox1.Height / (float)GridSize.X - canvasOffset.X) * zoom;
            float y = (previewCell.Y * pictureBox1.Height / (float)GridSize.Y - canvasOffset.Y) * zoom;

            // Draw only if within the visible bounds of the PictureBox
            if (x >= 0 && x <= pictureBox1.Width && y >= 0 && y <= pictureBox1.Height)
            {
                // Draw a preview rectangle representing the hovered cell
                using (Pen previewPen = new Pen(Color.Red))
                {
                    g.DrawRectangle(previewPen, x, y, cellWidth, cellHeight);
                }
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            // Draw the grid
            DrawGrid(pictureBox1.CreateGraphics());

            // Draw the level based on the levelData array
            DrawLevel(pictureBox1.CreateGraphics());
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Update the previewCell based on the mouse position
            int cellWidth = (int)(pictureBox1.Width / (float)GridSize.X * zoom);
            int cellHeight = (int)(pictureBox1.Height / (float)GridSize.Y * zoom);

            int columnIndex = (int)((e.X / (float)pictureBox1.Height  - canvasOffset.X / (pictureBox1.Height)) * GridSize.X / zoom);
            int rowIndex = (int)((e.Y / (float)pictureBox1.Height - canvasOffset.Y / pictureBox1.Height) * GridSize.Y / zoom);

            // Ensure the indices are within valid range
            columnIndex = Math.Max(0, Math.Min(GridSize.X - 1, columnIndex));
            rowIndex = Math.Max(0, Math.Min(GridSize.Y - 1, rowIndex));

            // Update the previewCell
            previewCell = new Point(columnIndex, rowIndex);

            // Refresh the PictureBox to update the display
            pictureBox1.Refresh();
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Update the levelData array based on the clicked cell
            int cellWidth = (int)(pictureBox1.Height / (float)GridSize.X * zoom);
            int cellHeight = (int)(pictureBox1.Height / (float)GridSize.Y * zoom);

            int columnIndex = (int)((e.X / (float)pictureBox1.Height - canvasOffset.X / (pictureBox1.Height)) * GridSize.X / zoom);
            int rowIndex = (int)((e.Y / (float)pictureBox1.Height - canvasOffset.Y / pictureBox1.Height) * GridSize.Y / zoom);

            // Ensure the indices are within valid range
            columnIndex = Math.Max(0, Math.Min(GridSize.X - 1, columnIndex));
            rowIndex = Math.Max(0, Math.Min(GridSize.Y - 1, rowIndex));

            // Toggle the cell value (e.g., 0 to 1 or 1 to 0)
            levelData[columnIndex, rowIndex] = 1 - levelData[columnIndex, rowIndex];

            // Refresh the PictureBox to update the display
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the grid
            DrawGrid(e.Graphics);

            // Draw the level based on the levelData array
            DrawLevel(e.Graphics);
        }
    }
}