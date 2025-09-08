using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navmaxia
{
    internal class HelpButton
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Μήνυμα", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}