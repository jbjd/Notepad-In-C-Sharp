using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Notepad
{
    internal class StyledTextBox : TextBox
    {
        public int HEADER_SIZE;

        [StructLayout(LayoutKind.Sequential)]
        protected struct SCROLLINFO
        {
            public int cbSize;
            public ScrollInfoMask fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;

        }
        protected enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }

        [DllImport("user32.dll")]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar,
            ref SCROLLINFO lpsi);
        public Panel
            scrollPanel,
            scrollWrapper;

        public void GetInternalScrollInfo()
        {
            SCROLLINFO info = new SCROLLINFO()
            {
                cbSize = Marshal.SizeOf<SCROLLINFO>(),
                fMask = ScrollInfoMask.SIF_ALL
            };
            GetScrollInfo(this.Handle, 1, ref info);
            int pages = (int)(info.nMax - info.nPage) + 1;
            if (pages > 0)
            {
                int newBarSize;
                if(pages < this.Height - HEADER_SIZE)
                {
                    newBarSize = this.Height - pages;
                }
                else
                {
                    newBarSize = HEADER_SIZE;
                }
                scrollPanel.Height = newBarSize;
                
                int loc = (int)(((this.Height - newBarSize) / (double)pages) * info.nPos);
               
                scrollPanel.Location = new Point(0, loc);
                
                scrollPanel.Visible = true;
            }
            else
            {
                scrollPanel.Visible = false;
            }
            
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 0x20A:
                    GetInternalScrollInfo();
                    break;
            }   
        }
    }
}
