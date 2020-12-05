using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gst;
using System.Threading;

namespace GStreamerPlayer
{
    public partial class GStreamerPlayerControl : UserControl
    {
        private Element playbin;
        private Gst.App.AppSink videosink;
        /// <summary>
        /// 播放总时长
        /// </summary>
        TimeSpan totalTime;
        /// <summary>
        /// 播放当前位置
        /// </summary>
        TimeSpan currentPosition;
        /// <summary>
        /// 播放位置变化触发的事件
        /// </summary>
        public event Action<TimeSpan, TimeSpan> PositionChanged;
        /// <summary>
        /// 播放状态变化触发的事件
        /// </summary>
        public event Action<State> StatusChanged;

        private State state;
        GLib.MainLoop mainLoop;

        public GStreamerPlayerControl()
        {
            InitializeComponent();

            Type type = this.pictureBox.GetType();
            System.Reflection.PropertyInfo pi = type.GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi.SetValue(this.pictureBox, true, null);

            this.Load += GStreamerPlayerControl_Load;
            Init();

            this.SizeChanged += (sender, e) =>
              {
                  this.pictureBox.Height = this.Height - 100;
              };
        }

        private void GStreamerPlayerControl_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (!IsDisposed)
                {
                    Thread.Sleep(10);
                    if (playbin == null)
                        continue;
                    if (videosink == null)
                        continue;


                    if (!this.pictureBox.IsHandleCreated) continue;

                    var sample = videosink.TryPullSample(500);
                    if (sample == null) continue;

                    System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

                    var image1 = ConvertSampleToImage(sample);
                    var time1 = stopwatch.ElapsedMilliseconds;

                    stopwatch.Restart();
                    var image = new Bitmap(image1,this.pictureBox.Width,this.pictureBox.Height);
                    var time2 = stopwatch.ElapsedMilliseconds;

                    stopwatch.Restart();

                    this.Invoke(new Action(() =>
                    {
                        this.pictureBox.Image?.Dispose();

                        this.pictureBox.Image = image;

                        this.lbTest.Text = $"{time1}ms-{time2}ms-{stopwatch.ElapsedMilliseconds}ms";

                    }));
                    
                    sample.Dispose();

                }
            });

            ThreadPool.QueueUserWorkItem(x =>
            {
                while (!IsDisposed)
                {
                    Thread.Sleep(300);
                    if (playbin == null)
                        continue;
                    if (videosink == null)
                        continue;

                    if (playbin.QueryDuration(Gst.Format.Time, out long duration) &&
                        playbin.QueryPosition(Gst.Format.Time, out long pos))
                    {
                        var tsPos = new TimeSpan(pos / 100).RoundSeconds(0);
                        var tsDur = new TimeSpan(duration / 100).RoundSeconds(0);

                        if (tsPos != currentPosition || tsDur != totalTime)
                        {
                            currentPosition = tsPos;
                            totalTime = tsDur;
                            PositionChanged?.Invoke(currentPosition, totalTime);
                        }
                    }

                    var ret = playbin.GetState(out var playStatus, out var pending, Gst.Constants.SECOND * 5L);
                    if (ret == StateChangeReturn.Success && playStatus != state)
                    {
                        state = playStatus;
                        StatusChanged?.Invoke(state);
                    }

                }
            });
        }

        private void Init()
        {
            Gst.Application.Init();
            playbin = ElementFactory.Make("playbin", "playbin");
            if (!string.IsNullOrEmpty(Url))
            {
                playbin["uri"] = Url;
            }

            videosink = new Gst.App.AppSink("videosink");
            videosink.Sync = true;
            videosink.Qos = false;
            videosink.Drop = false;

            videosink.Caps = Caps.FromString($"video/x-raw, format={videoFormat.ToString().ToUpperInvariant()}");
            videosink.MaxBuffers = 1;
            videosink.EmitSignals = true;
            playbin["video-sink"] = videosink;

            mainLoop = new GLib.MainLoop();
            ThreadPool.QueueUserWorkItem(x => mainLoop.Run());//must be run to collect memory

        }


        private string url;
        /// <summary>
        /// 视频播放地址
        /// </summary>
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                if (playbin != null)
                {
                    playbin["uri"] = value;
                }
            }
        }

        private Gst.Video.VideoFormat videoFormat = Gst.Video.VideoFormat.Bgra;
        /// <summary>
        /// 视频格式
        /// </summary>
        public Gst.Video.VideoFormat VideoFormat
        {
            get { return videoFormat; }
            set
            {
                videoFormat = value;
                videosink.Caps = Caps.FromString($"video/x-raw, format={videoFormat.ToString().ToUpperInvariant()}");
            }
        }

        public void Play()
        {
            playbin.SetState(State.Playing);
        }
        public void Play(string url)
        {
            Url = url;
            Play();
        }
        public void Pause()
        {
            playbin.SetState(State.Paused);
        }
        public void Stop()
        {
            playbin.SetState(State.Ready);
        }

        private Bitmap ConvertSampleToImage(Sample sample, Gst.Video.VideoFormat videoFormat = Gst.Video.VideoFormat.Gbra)
        {
            using (var caps = sample.Caps)
            using (var structure = caps.GetStructure(0))
            {
                int width, height;
                structure.GetInt("width", out width);
                structure.GetInt("height", out height);
                var formatStr = structure.GetString("format");
                var format = ConvertVideoFormatStrToPixelFormat(formatStr);

                var img = new System.Drawing.Bitmap(width, height, format);
                var imageData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);

                var pointer = imageData.Scan0;
                var length = imageData.Stride * imageData.Height;

                sample.Buffer.Map(out MapInfo mapInfo, MapFlags.Read);
                var data = mapInfo.Data;

                System.Runtime.InteropServices.Marshal.Copy(data, 0, pointer, length);
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
