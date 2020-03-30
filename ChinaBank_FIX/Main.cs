using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChinaBank_FIX
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
       public static string activeB = "";
        private void btnCBC_Click(object sender, EventArgs e)
        {
           
            CBC cbc = new CBC();
            cbc.Show();
            this.Hide();
            activeB = "CBC";

        }

        private void btmCBS_Click(object sender, EventArgs e)
        {
            activeB = "CBS";
            CBS cbs = new CBS();
            cbs.Show();
            this.Hide();
            
        }
    }
}
