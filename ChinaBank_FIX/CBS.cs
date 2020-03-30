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
    public partial class CBS : Form
    {
        public CBS()
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
            dbcon.GetBranches(branchList);
            BindingSource checkBind = new BindingSource();

            checkBind.DataSource = branchList;

            dataGridView1.DataSource = checkBind;
            MessageBox.Show("Done!!!");
        }
        private void LoadBranches()
        {
            //   OleDbConnection conn = new OleDbConnection (@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source="+Application.StartupPath+"; Extended Properties=dBASE III;");

            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\PTRUST\Regular_Checks; Extended Properties=DBASE IV;");


            conn.Open();

            DataSet dataset = new DataSet();
            string script = "Select BRSTN, ADDRESS1 from Branches";
            OleDbDataAdapter da = new OleDbDataAdapter(script, conn);

            da.Fill(dataset);

            DataTable dt = dataset.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                BranchModel branch = new BranchModel();

                branch.BRSTN = dr[0].ToString();
                branch.BranchName = dr[1].ToString();

                branchList.Add(branch);
            }

            conn.Close();
        }
        private void GetRegular()
        {
            //OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath+"; Extended Properties=dBASE III;");

            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\PTRUST\Regular_Checks; Extended Properties=DBASE IV;");


            conn.Open();

            DataSet dataset = new DataSet();

            OleDbDataAdapter comm = new OleDbDataAdapter("SELECT RTNO, LASTNO FROM REF", conn);

            comm.Fill(dataset);

            DataTable dt = dataset.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                string BRSTN = dr[0].ToString();
                //string CheckType = dr[1].ToString();
                Int64 LastNO = Int64.Parse(dr[1].ToString().Replace("'", ""));

                var branch = branchList.FirstOrDefault(r => r.BRSTN == BRSTN);

                if (branch != null)
                    branch.LastNo_PA = LastNO;

            }

            conn.Close();
        }

        private void CBS_Load(object sender, EventArgs e)
        {
           
        }
    }
}
