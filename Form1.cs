using Notepad.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Notepad
{    
    public partial class Form1 : Form
    {
        // Constants
        private const int 
            HEADER_SIZE = 30,
            GRIP = 8,  // Grip size
            DGRIP = GRIP+GRIP;  // Double the grip size
        private readonly SaveFileDialog saveAsDialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };
        private readonly OpenFileDialog openDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 2
        };

        // Members
        private readonly ImageList exitImages = new ImageList();
        private readonly ImageList maxImages = new ImageList();
        private readonly ImageList minImages = new ImageList();
        private Panel 
            topbar,
            searchBar;
        private PictureBox 
            exitButton,
            maxButton,
            minButton;
        private TextBox
            searchBox,
            textBox;
        
        private int FontSize;
        private string curFile = "";
        private bool edited = false;
        private ToolStripMenuItem windowSaveMenu;

        public Form1(string fileToReadFrom)
        {
            InitializeComponent();

            // init assets for topbar
            LoadAssets();
            // add UI Elements
            AddUI(OpenFile(fileToReadFrom));
            this.Load += this.OnFormLoad;
        }

        // loads images for buttons
        private void LoadAssets()
        {   // colors we will use
            Color red = Color.FromArgb(255, 190, 40, 40),
            lineColor = Color.FromArgb(255, 170, 170, 170),
            iconColor = Color.FromArgb(255, 100, 104, 102),
            hoverColor = Color.FromArgb(255, 95, 92, 88);
            // exit button
            exitImages.ImageSize = new Size(HEADER_SIZE, HEADER_SIZE);
            exitImages.ColorDepth = ColorDepth.Depth32Bit;
            Bitmap exitNormal = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            Graphics drawHelper = Graphics.FromImage(exitNormal);
            SolidBrush mainBrush = new SolidBrush(red);
            drawHelper.FillRectangle(mainBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            Bitmap exitHovered = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            drawHelper = Graphics.FromImage(exitHovered);
            drawHelper.FillRectangle(mainBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            int cornerLocation = HEADER_SIZE - 7;
            Pen lineMaker = new Pen(lineColor, 2);
            drawHelper.DrawLine(lineMaker, 6, 6, cornerLocation, cornerLocation);
            drawHelper.DrawLine(lineMaker, 6, cornerLocation, cornerLocation, 6);
            exitImages.Images.Add("normal", exitNormal);
            exitImages.Images.Add("hover", exitHovered);
            // maximize button
            maxImages.ImageSize = new Size(HEADER_SIZE, HEADER_SIZE);
            maxImages.ColorDepth = ColorDepth.Depth32Bit;
            Bitmap maxNormal = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            mainBrush = new SolidBrush(iconColor);
            drawHelper = Graphics.FromImage(maxNormal);
            drawHelper.FillRectangle(mainBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            int rectLength = HEADER_SIZE - 15;
            drawHelper.DrawRectangle(lineMaker, 6, 9, rectLength, rectLength);
            int lineLength = 9 + rectLength;
            drawHelper.DrawLine(lineMaker, 6, 6, lineLength, 6);
            drawHelper.DrawLine(lineMaker, lineLength, 6, lineLength, lineLength);
            Bitmap maxHovered = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            SolidBrush hoveredBrush = new SolidBrush(hoverColor);
            drawHelper = Graphics.FromImage(maxHovered);
            drawHelper.FillRectangle(hoveredBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            drawHelper.DrawRectangle(lineMaker, 6, 9, rectLength, rectLength);
            drawHelper.DrawLine(lineMaker, 6, 6, lineLength, 6);
            drawHelper.DrawLine(lineMaker, lineLength, 6, lineLength, lineLength);
            maxImages.Images.Add("normal", maxNormal);
            maxImages.Images.Add("hover", maxHovered);
            // minimize button
            minImages.ImageSize = new Size(HEADER_SIZE, HEADER_SIZE);
            minImages.ColorDepth = ColorDepth.Depth32Bit;
            Bitmap minNormal = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            drawHelper = Graphics.FromImage(minNormal);
            drawHelper.FillRectangle(mainBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            drawHelper.DrawLine(lineMaker, 6, cornerLocation, cornerLocation, cornerLocation);
            Bitmap minHovered = new Bitmap(HEADER_SIZE, HEADER_SIZE);
            drawHelper = Graphics.FromImage(minHovered);
            drawHelper.FillRectangle(hoveredBrush, 0, 0, HEADER_SIZE, HEADER_SIZE);
            drawHelper.DrawLine(lineMaker, 6, cornerLocation, cornerLocation, cornerLocation);
            minImages.Images.Add("normal", minNormal);
            minImages.Images.Add("hover", minHovered);
        }

        private void AddUI(string textToAdd)
        {
            Color HeaderColor = Color.FromArgb(255, 60, 60, 60),
            TextColor = Color.FromArgb(255, 245, 245, 245);
            // determine font size in future?
            FontSize = 11;
            Font defaultFont = new Font(new FontFamily("Arial"), FontSize);
            

            // main text box
            textBox = new TextBox
            {
                BackColor = Color.FromArgb(255, 90, 90, 90),
                ForeColor = TextColor,
                AcceptsReturn = true,
                AcceptsTab = true,
                Multiline = true,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                Width = this.Width - DGRIP + SystemInformation.VerticalScrollBarWidth,
                Height = this.Height - GRIP - HEADER_SIZE - (HEADER_SIZE / 3),
                BorderStyle = BorderStyle.None,
                Font = defaultFont,
                Text = textToAdd,
                TabStop = false,
                TabIndex = 0,
                ScrollBars = ScrollBars.Vertical,
            };

            textBox.TextChanged += this.OnTextChanged;

            // make topbar and buttons within
            Panel textBoxWrapper = new Panel
            {
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                Width = this.Width - DGRIP,
                Height = this.Height - GRIP - HEADER_SIZE - (HEADER_SIZE / 3),
                Location = new Point(GRIP, HEADER_SIZE + (HEADER_SIZE / 3)),
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = false
            };
            textBoxWrapper.Controls.Add(textBox);

            // make topbar and buttons within
            topbar = new Panel
            {
                BackColor = HeaderColor,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Height = HEADER_SIZE,
                Width = this.Width,
                Location = new Point(0, 0),
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = false
            };
            topbar.MouseDown += this.TopbarMouseDown;

            // maximize button in topbar
            minButton = new PictureBox
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Dock = DockStyle.Right,
                ClientSize = new Size(HEADER_SIZE, HEADER_SIZE),
                Image = minImages.Images["normal"]
            };
            minButton.Click += this.MinClick;
            minButton.MouseEnter += this.MinHoverEnter;
            minButton.MouseLeave += this.MinHoverExit;
            topbar.Controls.Add(minButton);

            // maximize button in topbar
            maxButton = new PictureBox
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Dock = DockStyle.Right,
                ClientSize = new Size(HEADER_SIZE, HEADER_SIZE),
                Image = maxImages.Images["normal"]
            };
            maxButton.Click += this.MaxClick;
            maxButton.MouseEnter += this.MaxHoverEnter;
            maxButton.MouseLeave += this.MaxHoverExit;
            topbar.Controls.Add(maxButton);

            // exit button in topbar
            exitButton = new PictureBox
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Dock = DockStyle.Right,
                ClientSize = new Size(HEADER_SIZE, HEADER_SIZE),
                Image = exitImages.Images["normal"]
            };
            exitButton.Click += this.ExitClick;
            exitButton.MouseEnter += this.ExitHoverEnter;
            exitButton.MouseLeave += this.ExitHoverExit;
            topbar.Controls.Add(exitButton);

            MenuStrip Menu = new MenuStrip
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Dock = DockStyle.None,
                Height = HEADER_SIZE,
                BackColor = HeaderColor,
                ForeColor = TextColor,
                Renderer = new StyledRenderer() // custom renderer
            };

            ToolStripMenuItem windowMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem windowOpenMenu = new ToolStripMenuItem("Open", null, new EventHandler(OpenFileWrapper))
            {
                ForeColor = TextColor
            };
            windowSaveMenu = new ToolStripMenuItem("Save", null, new EventHandler(SaveToFileWrapper))
            {
                ForeColor = TextColor,
                Enabled = (curFile != "")
            };
            ToolStripMenuItem windowSaveAsMenu = new ToolStripMenuItem("Save As", null, new EventHandler(SaveAsToFileWrapper))
            {
                ForeColor = TextColor
            };
            windowMenu.DropDownItems.Add(windowOpenMenu);
            windowMenu.DropDownItems.Add(windowSaveMenu);
            windowMenu.DropDownItems.Add(windowSaveAsMenu);

            Menu.MdiWindowListItem = windowMenu;
            Menu.Items.Add(windowMenu);

            topbar.Controls.Add(Menu);

            searchBar = new Panel
            {
                BackColor = HeaderColor,
                Height = HEADER_SIZE,
                Width = 360,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(this.Width - 360, 30),
                Padding = new Padding(4),
                Margin = new Padding(0),
                AutoSize = false,
                Visible = false
            };

            // text box for searching
            searchBox = new TextBox
            {
                BackColor = Color.FromArgb(255, 120, 120, 120),
                ForeColor = TextColor,
                AcceptsTab = true,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = defaultFont,
                TabStop = false
            };

            searchBar.Controls.Add(searchBox);

            textBox.KeyDown += this.HandleHotkey;
            searchBox.KeyDown += this.HandleHotkey;
            this.KeyDown += this.HandleHotkey;

            this.Controls.Add(searchBar);
            this.Controls.Add(topbar);
            this.Controls.Add(textBoxWrapper);
        }


        private void OnFormLoad(object sender, EventArgs e)
        {
            textBox.Select();
            this.ActiveControl = textBox;
            textBox.Focus();
        }

        // Windows event message constants
        private const int
            WM_NCLBUTTONDOWN = 0xA1,
            HT_CAPTION = 0x2,
            WM_NCHITTEST = 0x84;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void TopbarMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
           
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void HandleHotkey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Shutdown();
            if (!e.Control) return;

            switch (e.KeyCode)
            {
                case Keys.S:
                    if (curFile == "")
                    {
                        SaveAsToFile();
                    }
                    else
                    {
                        SaveToFile();
                    }
                    break;
                case Keys.O:
                    OpenFileWithDialog();
                    break;
                case Keys.F:
                    ToggleTextSearch();
                    break;
                default:
                    // return so only these key bindings are considered handled
                    return;
            }
            // prevent windows error sound from playing when ctrl+f
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void ToggleTextSearch()
        {
            searchBar.Visible = !searchBar.Visible;
            if (searchBar.Visible)
            {
                searchBox.Focus();
            }
            else
            {
                searchBox.Clear();
                textBox.Focus();
            }
            
        }

        private void Shutdown()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void ExitClick(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void ExitHoverEnter(object sender, EventArgs e)
        {
            exitButton.Image = exitImages.Images["hover"];
        }

        private void ExitHoverExit(object sender, EventArgs e)
        {
            exitButton.Image = exitImages.Images["normal"];
        }

        private void MaxClick(object sender, EventArgs e)
        {
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void MaxHoverEnter(object sender, EventArgs e)
        {
            maxButton.Image = maxImages.Images["hover"];
        }

        private void MaxHoverExit(object sender, EventArgs e)
        {
            maxButton.Image = maxImages.Images["normal"];
        }

        private void MinClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void MinHoverEnter(object sender, EventArgs e)
        {
            minButton.Image = minImages.Images["hover"];
        }

        private void MinHoverExit(object sender, EventArgs e)
        {
            minButton.Image = minImages.Images["normal"];
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    Point pos = new Point(m.LParam.ToInt32());
                    pos = this.PointToClient(pos);
                    if (pos.Y < HEADER_SIZE)
                    {
                        m.Result = (IntPtr)2;  // HTCAPTION
                        return;
                    }
                    bool onBottom = (pos.Y >= this.ClientSize.Height - GRIP);

                    if (pos.X >= this.ClientSize.Width - GRIP)
                    {
                        if(onBottom)
                        {
                            m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                            return;
                        }
                        m.Result = (IntPtr)11; // HTRIGHT
                        return;
                    }
                    if (pos.X <= GRIP)
                    {
                        if (onBottom)
                        {
                            m.Result = (IntPtr)16; // HTBOTTOMLEFT
                            return;
                        }
                        m.Result = (IntPtr)10; // HTLEFT
                        return;
                    }
                    if (onBottom)
                    {
                        m.Result = (IntPtr)15; // HTBOTTOM
                    }
                    return;
            }
            base.WndProc(ref m);
        }

        private void OpenFileWrapper(object sender, EventArgs e)
        {
            OpenFileWithDialog();
        }

        private void OpenFileWithDialog()
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = OpenFile(openDialog.FileName);
            }
        }

        private string OpenFile(string fileToReadFrom)
        {
            string fileContents;
            if(fileToReadFrom == "")
            {
                return "";
            }
            try
            {
                fileContents = File.ReadAllText(fileToReadFrom);
                curFile = fileToReadFrom;
            }
            catch
            {
                fileContents = "";
            }
            return fileContents;
        }

        private void SaveToFileWrapper(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void OnSaveFail() 
        {
            // TODO: somehow tell user save to file failed
        }

        private void SaveToFile()
        {
            if (curFile == "" || !edited) return;

            try
            {
                File.WriteAllText(curFile, textBox.Text);
                edited = false;
            }
            catch
            {
                OnSaveFail();
            }
        }

        private void SaveAsToFileWrapper(object sender, EventArgs e)
        {
            SaveAsToFile();
        }

        private void SaveAsToFile()
        {
            if (saveAsDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveAsDialog.FileName, textBox.Text);
                    curFile = saveAsDialog.FileName;
                    edited = false;
                    windowSaveMenu.Enabled = true;
                }
                catch
                {
                    OnSaveFail();
                }
            }
        }

        /*[StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public int cbSize;
            public ScrollInfoMask fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
        public enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }

        [DllImport("user32.dll")]
        private static extern bool GetScrollInfo(IntPtr hwnd, SBOrientation fnBar,
            ref SCROLLINFO lpsi);
        public enum SBOrientation : int { SB_HORZ = 0x0, SB_VERT = 0x1 }*/

        private void OnTextChanged(object sender, EventArgs e)
        {
            /*var info = new SCROLLINFO()
            {
                cbSize = (Marshal.SizeOf<SCROLLINFO>()),
                fMask = ScrollInfoMask.SIF_ALL
            };
            Console.WriteLine(GetScrollInfo(textBox.Handle, SBOrientation.SB_VERT, ref info));
            Console.WriteLine(info.nMax);*/
            edited = true;
        }
    }
}
