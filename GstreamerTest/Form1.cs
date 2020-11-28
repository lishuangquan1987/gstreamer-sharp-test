using Gst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GstreamerTest
{
    public partial class Form1 : Form
    {
        Gst.Element playbin;
        GLib.MainLoop mainLoop;
        public Form1()
        {
            InitializeComponent();

            Gst.Application.Init();
            mainLoop = new GLib.MainLoop();
            System.Threading.ThreadPool.QueueUserWorkItem(x => mainLoop.Run());

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playbin = ElementFactory.Make("playbin", "playbin");
            playbin["uri"] = "http://stream4.iqilu.com/ksd/video/2020/02/17/97e3c56e283a10546f22204963086f65.mp4";
            

            var overlay= ((Gst.Bin)playbin).GetByInterface(Gst.Video.VideoOverlayAdapter.GType);

            Gst.Video.VideoOverlayAdapter adapter = new Gst.Video.VideoOverlayAdapter(overlay.Handle);
            adapter.WindowHandle = this.panel1.Handle;
            adapter.HandleEvents(true);

            playbin.SetState( State.Playing);
        }

       
        private void btnPlay_Click(object sender, EventArgs e)
        {
            playbin.SetState( State.Playing);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            playbin.SetState(State.Paused);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            playbin.SetState(State.Ready);
        }
    }
}
