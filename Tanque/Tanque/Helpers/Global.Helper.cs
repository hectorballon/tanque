using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tanque
{
    internal static class Global
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public IntPtr stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public const int EM_SETCUEBANNER = 0x1501;
        public const int EM_GETCUEBANNER = 0x1502;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        private static COMBOBOXINFO GetComboBoxInfo(Control control)
        {
            COMBOBOXINFO info = new COMBOBOXINFO();
            //a combobox is made up of three controls, a button, a list and textbox;
            //we want the textbox
            info.cbSize = Marshal.SizeOf(info);
            GetComboBoxInfo(control.Handle, ref info);
            return info;
        }

        public static void SetCueText(Control control, string text)
        {
            if (control is ComboBox)
            {
                COMBOBOXINFO info = GetComboBoxInfo(control);
                SendMessage(info.hwndItem, EM_SETCUEBANNER, 0, text);
            }
            else
            {
                SendMessage(control.Handle, EM_SETCUEBANNER, 0, text);
            }
        }
    }
}
