using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Netwolk
{
    public partial class CreatRoom : Form
    {
        public CreatRoom()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
      
            Room room = RoomManager.AddRoom();
            room.maxPlayer =int.Parse (maxplayer.Text);
            room.describe = describe.Text;
        }

        private void maxplayer_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
