using System.Drawing;
using System.Windows.Forms;

namespace Notepad
{
    internal class StyledRenderer : ToolStripRenderer
    {
        private readonly Color background = Color.FromArgb(255, 60, 60, 60);
        private readonly Color selectedBackground = Color.FromArgb(255, 75, 75, 80);

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(e.Item.Selected ? selectedBackground : background), new Rectangle(Point.Empty, e.Item.Size));
        }
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(Point.Empty, e.ToolStrip.Size);
            e.Graphics.FillRectangle(new SolidBrush(background), rect);
        }
    }
}
