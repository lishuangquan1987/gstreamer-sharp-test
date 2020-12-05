using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GStreamerPlayer.Test
{

    public partial class Form1 : Form
    {
        GStreamerPlayer.GStreamerPlayerControl player;
        private TimeSpan currentTime;
        private TimeSpan totalTime;
        private Gst.State state;
        public Form1()
        {
            InitializeComponent();
            player = new GStreamerPlayerControl();
            player.Dock = DockStyle.Fill;
            this.Controls.Add(player);
            player.Play("https://stream7.iqilu.com/10339/upload_transcode/202002/18/20200218114723HDu3hhxqIT.mp4");
            player.StatusChanged += (s) =>
            {
                this.state = s;
                UpdateText();
            };
            player.PositionChanged += (c, t) =>
            {
                this.totalTime = t;
                this.currentTime = c;
                UpdateText();
            };
            this.SizeChanged += Form1_SizeChanged;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            player.Height = this.Height;
            player.Width = this.Width;
            player.Dock = DockStyle.Fill;
            
        }

        private void UpdateText()
        {
            if (!this.IsHandleCreated) return;
            this.Invoke(new Action(() =>
            {
                this.Text = $"播放状态:{state},进度:{currentTime.TotalSeconds}s/{totalTime.TotalSeconds}s";
            }));
        }
    }
}
