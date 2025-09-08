using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navmaxia
{
    internal class Alignment
    {
        public void AlignToRight(Form form, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                int newX = form.ClientSize.Width - control.Width - 10;
                control.Location = new System.Drawing.Point(newX, control.Location.Y);
            }
        }
    }
}
