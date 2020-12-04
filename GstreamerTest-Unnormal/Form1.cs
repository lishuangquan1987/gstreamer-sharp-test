using Gst;
using GstreamerTest.Unnormal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        //if (sample == null) continue;
                        //GstreamerTest.Unnormal.Image image = new GstreamerTest.Unnormal.Image(sample);
                        //this.Invoke(new Action(() =>
                        //{
                        //    this.pictureBox1.Image?.Dispose();
                        //    this.pictureBox1.Image = image.FromImage(true);
                        //}));
                        //image.Dispose();
                        //sample.Dispose();

                        this.Invoke(new Action(() =>
                        {
                            this.pictureBox1.Image?.Dispose();
                            this.pictureBox1.Image = ConvertSampleToImage(sample);
                        }));
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
            
            videosink.Caps = Caps.FromString($"video/x-raw, format=BGRA");
            //videosink.Caps = Caps.FromString($"video/x-raw, format=ARGB");
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

        private  Bitmap ConvertSampleToImage(Sample sample, Gst.Video.VideoFormat videoFormat= Gst.Video.VideoFormat.Gbra)
        {
            using (var caps = sample.Caps)
            using (var structure = caps.GetStructure(0))
            {
                int width, height;
                structure.GetInt("width", out width);
                structure.GetInt("height", out height);
                var formatStr = structure.GetString("format");
                var format = ConvertVideoFormatStrToPixelFormat(formatStr);

                var img = new System.Drawing.Bitmap(width,height,format);
                var imageData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);

                var pointer = imageData.Scan0;
                var length = imageData.Stride * imageData.Height;

                sample.Buffer.Map(out MapInfo mapInfo, MapFlags.Read);
                var data = mapInfo.Data;

                System.Runtime.InteropServices.Marshal.Copy(data,0,pointer,length);
                img.UnlockBits(imageData);
                
                data = null;
                sample.Buffer.Unmap(mapInfo);//这一句一定要加，否则内存飙升
                
                return img;
            }
            
        }

        private System.Drawing.Imaging.PixelFormat ConvertVideoFormatStrToPixelFormat(string format)
        {
            Gst.Video.VideoFormat videoFormat;
            if (!Enum.TryParse(format, true, out videoFormat))
                return System.Drawing.Imaging.PixelFormat.Undefined;

            if (videoFormat == Gst.Video.VideoFormat.Encoded ||
                videoFormat == Gst.Video.VideoFormat.I420 ||
                videoFormat == Gst.Video.VideoFormat.Yv12 ||
                videoFormat == Gst.Video.VideoFormat.Yuy2 ||
                videoFormat == Gst.Video.VideoFormat.Uyvy ||
                videoFormat == Gst.Video.VideoFormat.Ayuv ||
                videoFormat == Gst.Video.VideoFormat.Rgbx ||
                videoFormat == Gst.Video.VideoFormat.Bgrx)
            {
                return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            }
            else if (videoFormat == Gst.Video.VideoFormat.Xrgb ||
                videoFormat == Gst.Video.VideoFormat.Xbgr ||
                videoFormat == Gst.Video.VideoFormat.Rgba ||
                videoFormat == Gst.Video.VideoFormat.Bgra)
            {
                return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else if (videoFormat == Gst.Video.VideoFormat.Argb ||
                videoFormat == Gst.Video.VideoFormat.Abgr ||
                videoFormat == Gst.Video.VideoFormat.Rgb)
            {
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }
            else
            {
                return System.Drawing.Imaging.PixelFormat.Undefined;
            }
        }
    }
}
