using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DebugForm
{
    public partial class ucLog : UserControl
    {
        public ucLog()
        {
            InitializeComponent();
        }

        private void ucLog_Load(object sender, EventArgs e)
        {

        }

        delegate void delegatePrint(string s);
        public void Print(string s)
        {
            if (InvokeRequired)
            {
                delegatePrint d = new delegatePrint(Print);
                //d.Invoke(s);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                richTextBoxLogger.Text += s + "\r\n";
                richTextBoxLogger.SelectionStart = richTextBoxLogger.TextLength;
                richTextBoxLogger.ScrollToCaret();
            }

        }
    }
}
