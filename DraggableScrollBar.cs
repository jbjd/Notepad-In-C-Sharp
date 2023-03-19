using System;
using System.Drawing;
using System.Windows.Forms;

namespace Notepad
{
    internal class DraggableScrollBar : Panel
    {
        private int dragStart = 0;
        private bool dragging = false;
        public StyledTextBox associatedTextBox;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            dragStart = e.Y;
            dragging = associatedTextBox.ReadOnly = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            dragging = associatedTextBox.ReadOnly = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!dragging) return;

            int newY = this.Location.Y - (int)((dragStart - e.Y) / 6.0);  // Magic number I decided on by testing scrolling speeds. Larger number = slower scroll
            if (newY < 0)
            {
                newY = 0;
            }
            else if (newY > this.Parent.Height - this.Height)
            {
                newY = this.Parent.Height - this.Height;
            }
            if(!Visible || newY == this.Location.Y) return;
            this.Location = new Point(0, newY);
            associatedTextBox.SetInternalScrollInfo();

            base.OnMouseMove(e);
        }
    }
}
