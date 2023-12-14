using System;
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
    public partial class About : Form
    {
        public About(string path)
        {
            InitializeComponent();
            label2.Text = path;
        }
    }
}
