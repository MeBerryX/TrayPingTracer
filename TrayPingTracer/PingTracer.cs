using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TrayPingTracer
{
    public partial class PingTracer : Form
    {
        private NotifyIcon notifyIcon;
        private Timer timer;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem openToolbarItem;
        private ToolStripMenuItem exitItem;
        private bool isToolbarOpen;

        public PingTracer()
        {
            // Create a NotifyIcon for the system tray
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "Ping Tracker";
            notifyIcon.Visible = true;

            // Create a Timer to update the ping value
            timer = new Timer();
            timer.Interval = 3000; // Update every 5 seconds
            timer.Tick += Timer_Tick;

            // Create a ContextMenuStrip for the system tray icon
            contextMenu = new ContextMenuStrip();
            openToolbarItem = new ToolStripMenuItem("Open Toolbar");
            exitItem = new ToolStripMenuItem("Exit");
            openToolbarItem.Click += OpenToolbarItem_Click;
            exitItem.Click += ExitItem_Click;
            contextMenu.Items.Add(openToolbarItem);
            contextMenu.Items.Add(exitItem);
            notifyIcon.ContextMenuStrip = contextMenu;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Hide the form on startup
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            // Start the timer
            timer.Start();

            base.OnLoad(e);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Ping a remote server to get the current ping value
            Ping ping = new Ping();
            PingReply reply = ping.Send("www.google.com");

            // Update the tooltip text of the system tray icon with the ping value
            Icon icon = CreatePingIcon((int)reply.RoundtripTime);
            notifyIcon.Icon = icon;
            notifyIcon.Text = $"Ping: {reply.RoundtripTime} ms";
        }

        private Icon CreatePingIcon(int pingValue)
        {
            // Create a dynamic bitmap with the ping value
            Bitmap bitmap = new Bitmap(16, 16);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                if (pingValue < 100)
                {
                    using (Font font = new Font(FontFamily.GenericSansSerif, 10))
                    {
                        graphics.DrawString(pingValue.ToString(), font, Brushes.Black, new PointF(0, 0));
                    }
                } else
                {
                    using (Font font = new Font(FontFamily.GenericSansSerif, 6))
                    {
                        graphics.DrawString(pingValue.ToString(), font, Brushes.Red, new PointF(0, 0));
                    }
                }
                
            }

            // Convert the bitmap to an icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon).Clone() as Icon;
            DestroyIcon(hIcon);
            bitmap.Dispose();

            return icon;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void ExitItem_Click(object sender, EventArgs e)
        {
            // Close the application
            this.Close();
        }

        private void OpenToolbarItem_Click(object sender, EventArgs e)
        {
            if (!isToolbarOpen)
            {
                // Open your toolbar or perform any desired actions
                // Here, we will simply display a message box
                MessageBox.Show("Toolbar opened!");
                isToolbarOpen = true;
            }
        }


        protected override void Dispose(bool disposing)
        {
            // Clean up resources
            if (disposing)
            {
                notifyIcon.Dispose();
                timer.Dispose();
            }

            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
    }
}