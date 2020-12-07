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
using System.Diagnostics;

namespace GStrreamerEmbeddedPlayer
{
    public partial class PlayerControl : UserControl
    {
        const string processName = "Direct3D11 renderer";
        private Element playbin;
        //private Gst.App.AppSink videosink;
        private GLib.MainLoop mainLoop;
        IntPtr playerHandle;
        public PlayerControl()
        {
            InitializeComponent();

            this.Load += PlayerControl_Load;
            this.Resize += PlayerControl_Resize;
            Init();
        }

        private void PlayerControl_Resize(object sender, EventArgs e)
        {
            if (playerHandle == IntPtr.Zero) return;
           
            // 设置目标应用程序的窗体样式. 
            //IntPtr innerWnd = playerHandle;
            //WinAPI.SetWindowPos(innerWnd, IntPtr.Zero, -10, -30,
            //   this.ClientSize.Width+20, this.ClientSize.Height+40,
            //   WinAPI.SWP_NOZORDER);
        }

        private void PlayerControl_Load(object sender, EventArgs e)
        {
            Element overlay = ((Gst.Bin)playbin).GetByInterface(Gst.Video.VideoOverlayAdapter.GType);
            Gst.Video.VideoOverlayAdapter adapter = new Gst.Video.VideoOverlayAdapter(overlay.Handle);
            adapter.WindowHandle = this.Handle;
            adapter.HandleEvents(true);
        }
        private void Init()
        {
            Gst.Application.Init();
            playbin = ElementFactory.Make("playbin", "playbin");
            if (!string.IsNullOrEmpty(Url))
            {
                playbin["uri"] = Url;
            }

            //videosink = new Gst.App.AppSink("videosink");
            //videosink.Sync = true;
            //videosink.Qos = false;
            //videosink.Drop = false;

            //videosink.Caps = Caps.FromString($"video/x-raw, format={VideoFormat.ToString().ToUpperInvariant()}");
            //videosink.MaxBuffers = 1;
            //videosink.EmitSignals = true;
            //playbin["video-sink"] = videosink;

            mainLoop = new GLib.MainLoop();
            System.Threading.ThreadPool.QueueUserWorkItem(x => mainLoop.Run());//must be run to collect memory
        }

        private string url;
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
        public Gst.Video.VideoFormat VideoFormat
        {
            get { return videoFormat; }
            set
            {
                videoFormat = value;
                //if (videosink != null)
                //{
                //    videosink.Caps = Caps.FromString($"video/x-raw, format={videoFormat.ToString().ToUpperInvariant()}");
                //}
            }
        }

        public void Play(string url)
        {
            Url = url;
            Play();
        }
        public void Play()
        {
            this.playbin?.SetState(State.Playing);
            EmbededProcess();
        }
        private IntPtr GetProcess()
        {
            IntPtr handle = IntPtr.Zero;
            for (int i = 0; i < 10; i++)
            {
                handle = WinAPI.FindWindow(null, processName);
                if (handle == IntPtr.Zero)
                {

                    System.Threading.Thread.Sleep(500);
                    continue;
                }
                return handle;
            }
            return handle;
        }

        private void EmbededProcess()
        {
            var process = GetProcess();
            if (process == null) return;

            playerHandle = process;//保存起来


            WinAPI.SetParent(process, this.Handle);

            Int32 wndStyle = WinAPI.GetWindowLong(process, WinAPI.GWL_STYLE);

            WinAPI.SetWindowLong(process, WinAPI.GWL_STYLE, wndStyle);

            WinAPI.SetWindowPos(process, IntPtr.Zero, 0, 0, this.ClientSize.Width, this.ClientSize.Height, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER | WinAPI.SWP_FRAMECHANGED);

        }
    }
}
