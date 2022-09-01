using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Netwolk_Battle
{
    public partial class BinaryToFile : Form
    {
        public BinaryToFile()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpNetmanager.BinaryToFile(AppDomain.CurrentDomain.BaseDirectory + "\\" + textBox2.Text, textBox1.Text);
        }
    }
}
