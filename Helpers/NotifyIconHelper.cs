using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RetroGameHandler.Helpers
{
    public class NotifyIco : IDisposable
    {
        private NotifyIcon nIcon = new NotifyIcon();

        public NotifyIco()
        {
            nIcon.Visible = false;
            //nicon = new NotifyIcon();
        }

        public void ChangeIcon(string iconPath, string text)
        {
            if (string.IsNullOrWhiteSpace(iconPath)) return;
            using (Stream iconStream = System.Windows.Application.GetResourceStream(new Uri($"pack://application:,,,/RetroGameHandler;component/images/{iconPath}")).Stream)
            {
                nIcon.Icon = new System.Drawing.Icon(iconStream); ;
                nIcon.Visible = true;
                nIcon.Text = text;
            }
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                handle.Dispose();
                try
                {
                    nIcon.Visible = false;
                    nIcon.Dispose();
                    nIcon.Disposed += (s, e) => nIcon = null;
                }
                catch (Exception) { }
            }
            disposed = true;
        }

        public bool IsVisible { get => nIcon.Visible; set => nIcon.Visible = value; }
        public ContextMenuStrip ContextMenu { get => nIcon.ContextMenuStrip; set => nIcon.ContextMenuStrip = value; }
    }

    public static class ConsoleIconHelper
    {
        public static event EventHandler Close;

        public static event EventHandler OpenConsole;

        public static NotifyIco Instance { get; private set; }

        public static void Init()
        {
            if (Instance == null) Instance = new NotifyIco();
            Instance.ChangeIcon("TimeOnlineLogo.ico", "TimeOnline \n Retro-Game Console Handler 2020");
            Instance.ContextMenu = new System.Windows.Forms.ContextMenuStrip();
            Instance.ContextMenu.Items.Add("Close", null, (s, e) => ThrowEvent(Close));
            Instance.ContextMenu.Items.Add("Open Console", null, (s, e) => ThrowEvent(OpenConsole));
        }

        public static void Dispose()
        {
            Instance.Dispose();
        }

        private static void ThrowEvent(EventHandler Event)
        {
            Event?.Invoke(null, new EventArgs());
        }
    }

    public static class BatteryIconHelper
    {
        public static NotifyIco Instance { get; private set; }

        public static void Init()
        {
            if (Instance == null) Instance = new NotifyIco();
            var bat = "";
            var text = $"RetroHandheld 2020";
            Instance.ChangeIcon(bat, text);
        }

        private static string[] _batteryIconPath = { "Battery-0.ico", "Battery-1.ico", "Battery-2.ico", "Battery-3.ico", "Battery-4.ico", "Battery-charging.ico" };

        public static void ChangeIcon(int iconIndex, string powerText)
        {
            if (Instance == null) Init();
            if (iconIndex < 0 || iconIndex > _batteryIconPath.Length)
            {
                Instance.IsVisible = false;
                return;
            }
            var bat = _batteryIconPath[iconIndex];
            var text = $"Console Battery {powerText}";
            Instance.ChangeIcon(bat, text);
        }

        public static void Dispose()
        {
            Instance.Dispose();
        }
    }
}