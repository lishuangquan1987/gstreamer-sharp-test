using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GStrreamerEmbeddedPlayer.Test
{
    public partial class Form1 : Form
    {
        GStrreamerEmbeddedPlayer.PlayerControl playerControl;
        public Form1()
        {
            InitializeComponent();
            playerControl = new PlayerControl();
            playerControl.Dock = DockStyle.Fill;
            this.Controls.Add(playerControl);
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playerControl.Play("https://stream7.iqilu.com/10339/upload_transcode/202002/18/20200218093206z8V1JuPlpe.mp4");
        }
    }
}
