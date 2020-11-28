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
using VL.Lib.Basics.Imaging;

namespace GstreamerTest_Unnormal
{
    public partial class Form1 : Form
    {
        Gst.Element playbin;
        GLib.MainLoop mainLoop;
        Gst.App.AppSink videosink;
        public Form1()
        {
            InitializeComponent();

            Gst.Application.Init();
            mainLoop = new GLib.MainLoop();
            System.Threading.ThreadPool.QueueUserWorkItem(x => mainLoop.Run());
            GLib.Timeout.Add(1000, () =>
             {
                

                 return true;
             });

            System.Threading.ThreadPool.QueueUserWorkItem(x =>
            {
                while (true)
                {
                    if (playbin == null) continue ;
                    if (videosink == null) continue;

                    var sample = videosink.TryPullSample(500);
                    if(sample!=null)
                    {
                        if (sample == null) continue ;
                        GstreamerTest.Unnormal.Image image = new GstreamerTest.Unnormal.Image(sample);
                        this.Invoke(new Action(() =>
                        {
                            this.pictureBox1.Image?.Dispose();
                            this.pictureBox1.Image = image.FromImage(true);
                        }));
                        image.Dispose();
                        sample.Dispose();
                    }
                    System.Threading.Thread.Sleep(10);
                }
            });

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playbin = ElementFactory.Make("playbin", "playbin");
            playbin["uri"] = "http://stream4.iqilu.com/ksd/video/2020/02/17/97e3c56e283a10546f22204963086f65.mp4";

            videosink = new Gst.App.AppSink("videosink");
            videosink.Sync = true;
            videosink.Qos = false;
            videosink.Drop = false;
            
            videosink.Caps = Caps.FromString($"video/x-raw, format=RGBA");
            videosink.MaxBuffers = 1;
            videosink.EmitSignals = true;

            playbin["video-sink"] = videosink;


            playbin.SetState(State.Playing);

            
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            playbin.SetState(State.Playing);
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
