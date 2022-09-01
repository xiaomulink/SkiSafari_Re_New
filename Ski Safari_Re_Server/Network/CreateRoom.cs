using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network
{
    public partial class CreateRoom : Form
    {
        public CreateRoom()
        {
            InitializeComponent();
        }

        private void CreateRoom_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Room room = RoomManager.AddRoom();
            room.maxPlayer = int.Parse(maxplayer.Text);
            room.describe = describe.Text;
            MessageBox.Show("创建成功！", "创建房间");
            Close();
        }
    }
}
