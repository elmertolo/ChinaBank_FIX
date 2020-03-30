using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChinaBank_FIX.Models;
using ChinaBank_FIX.Services;
using System.Data.OleDb;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ChinaBank_FIX
{
    public partial class CBC : Form
    {
        public CBC()
        {
            InitializeComponent();
        }
        DbConServices dbcon = new DbConServices();
        List<BranchModel> branchList = new List<BranchModel>();

        List<HistoryModel> historyList = new List<HistoryModel>();
        // string activeB = "";
        List<ErrorModel> errorList = new List<ErrorModel>();
        private void checkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbcon.DBCon();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
