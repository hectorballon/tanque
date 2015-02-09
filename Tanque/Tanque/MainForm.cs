using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Tanque.Properties;

namespace Tanque
{
    partial class MainForm : Form
    {
        public MainForm()
        {
            SplashScreen splashScreen = new SplashScreen(Resources.Splash);
            Thread.CurrentThread.Name = "Tanque Main UI Thread";
            m_DownloadCompletedEvent = new ManualResetEvent(false);

            InitializeComponent();

            Thread.Sleep(1000);

            splashScreen.Close();
        }

        void OnClosed(object sender, FormClosedEventArgs e)
        {
            m_DownloadCompletedEvent.Close();
        }

        private void OnResize(object sender, EventArgs e)
        {
            stbAction.Width = scHeader.Panel2.Width - (pSessionInfo.Width + 100);
        }
    }
}
