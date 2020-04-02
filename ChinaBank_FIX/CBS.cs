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
            dbcon.GetBranchesCBS(branchList);
            GetRegular();
            GetSmallBiz();
            dbcon.GetHistory(historyList);
            CheckErrors();

            BindingSource checkBind = new BindingSource();

            checkBind.DataSource = errorList;

            dataGridView1.DataSource = checkBind;
            lblErrors.Text = errorList.Count().ToString();
            MessageBox.Show("Done!!!");
        }
      

        private void GetRegular()
        {
            //TEST DB
           // OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath+ @"\Regular_Checks; Extended Properties=dBASE III;");
            //LIVE
            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\CBC_Savings\Regular_Checks; Extended Properties=DBASE IV;");


            conn.Open();

            DataSet dataset = new DataSet();

            OleDbDataAdapter comm = new OleDbDataAdapter("SELECT BRSTN, ENDSN FROM Ref", conn);

            comm.Fill(dataset);

            DataTable dt = dataset.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                string BRSTN = dr[0].ToString();
                //string CheckType = dr[1].ToString();
                Int64 LastNO = Int64.Parse(dr[1].ToString().Replace("'", ""));

                var branch = branchList.FirstOrDefault(r => r.BRSTN == BRSTN);

                if (branch != null)
                    branch.LastNo_Regular = LastNO;

            }

            conn.Close();
        }
        private void GetSmallBiz()
        {
            //TEST DB
            //OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath + @"\SmallBiz; Extended Properties=dBASE III;");
            //LIVE
             OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\CBC_Savings\Smallbiz; Extended Properties=DBASE IV;");


            conn.Open();

            DataSet dataset = new DataSet();

            OleDbDataAdapter comm = new OleDbDataAdapter("SELECT BRSTN, CHKTYPE ,ENDSN FROM Ref", conn);

            comm.Fill(dataset);

            DataTable dt = dataset.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                string BRSTN = dr[0].ToString();
                string CheckType = dr[1].ToString();
                Int64 LastNO = Int64.Parse(dr[2].ToString().Replace("'", ""));

                var branch = branchList.FirstOrDefault(r => r.BRSTN == BRSTN);

                if (branch != null)
                    if (CheckType == "A")
                        branch.LastNo_SmallBiz_PA = LastNO;
                    else if (CheckType == "B")
                        branch.LastNo_SmallBiz_CA = LastNO;
            }

            conn.Close();
        }
        private void CheckErrors()
        {
            historyList.ForEach(hist =>
            {
                var branch = branchList.FirstOrDefault(r => r.BRSTN == hist.BRSTN);

                if (branch != null)
                {
                    if (hist.ChequeName == "Personal Checks" || hist.ChequeName == "Commercial Checks")
                    {
                        if (hist.MaxEnding > branch.LastNo_Regular)
                        {
                            ErrorModel error = new ErrorModel
                            {
                                BRSTN = branch.BRSTN,
                                BranchName = branch.BranchName,
                                CheckType = "Regular Checks",
                                HistorySerial = hist.MaxEnding,
                                CurrentSerial = branch.LastNo_Regular
                            };

                            errorList.Add(error);
                        }

                    }
                    else if (hist.ChequeName == "SmallBiz Personal Checks")
                    {
                        if (hist.MaxEnding > branch.LastNo_SmallBiz_PA)
                        {
                            ErrorModel error = new ErrorModel
                            {
                                BRSTN = branch.BRSTN,
                                BranchName = branch.BranchName,
                                CheckType = "SmallBiz Personal",
                                HistorySerial = hist.MaxEnding,
                                CurrentSerial = branch.LastNo_SmallBiz_PA
                            };

                            errorList.Add(error);
                        }
                    }
                    else if (hist.ChequeName == "SmallBiz Commercial Checks")
                    {
                        if (hist.MaxEnding > branch.LastNo_SmallBiz_CA)
                        {
                            ErrorModel error = new ErrorModel
                            {
                                BRSTN = branch.BRSTN,
                                BranchName = branch.BranchName,
                                CheckType = "SmallBiz Commercial",
                                HistorySerial = hist.MaxEnding,
                                CurrentSerial = branch.LastNo_SmallBiz_CA
                            };

                            errorList.Add(error);
                        }
                    }

                }
            });
        }

        private void CBS_Load(object sender, EventArgs e)
        {
           
        }

        private void fixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = "";

            DialogResult result = InputBox("Admin Credentials", "Input Admin Password", ref value);

            if (result == DialogResult.OK)
            {
                if (value == "secret")
                {
                    FixError();

                    MessageBox.Show("China Bank Savings Database has been Fixed");
                }
                else
                    MessageBox.Show("Invalid Password", "System Error");
            }
            dataGridView1.Refresh();
            dataGridView1.Rows.Clear();
            lblErrors.Text = "0";
        }
        private void FixError()
        {
            OleDbConnection conn;

            OleDbCommand command;
            dbcon.DBCon();


            var Regular = errorList.Where(r => r.CheckType == "Regular Checks").ToList();
            var SmallBizA = errorList.Where(r => r.CheckType == "SmallBiz Personal").ToList();
            var SmallBizB = errorList.Where(r => r.CheckType == "SmallBiz Commercial").ToList();
            if (Regular != null)
            {
                //TEST DB
                // conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath + "\\Regular_Checks; Extended Properties=DBASE IV;");
                //LIVE
                conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\CBC_Savings\Regular_Checks; Extended Properties=DBASE IV;");

                conn.Open();

                Regular.ForEach(p =>
                {
                    //UPDATE REF
                    command = new OleDbCommand("UPDATE Ref SET ENDSN = '" + p.HistorySerial + "' WHERE BRSTN = '" + p.BRSTN + "'", conn);

                    command.ExecuteNonQuery();

                    ////SAVE TO HISTORY
                    dbcon.InsertData(p, DateTime.Now.ToString("yyyy-MM-dd"), "Regular Checks");
                });
                conn.Close();
            }

            


            if (SmallBizA != null)
            {

                //TEST DB
                // conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath + "\\Smallbiz; Extended Properties=DBASE IV;");
                //LIVE
                conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\CBC_Savings\Smallbiz; Extended Properties=DBASE IV;");

                conn.Open();

                SmallBizA.ForEach(p =>
                {
                    //UPDATE REF
                    command = new OleDbCommand("UPDATE Ref SET ENDSN = '" + p.HistorySerial + "' WHERE BRSTN = '" + p.BRSTN + "' AND CHKTYPE ='A'", conn);

                    command.ExecuteNonQuery();

                    ////SAVE TO HISTORY
                    dbcon.InsertData(p, DateTime.Now.ToString("yyyy-MM-dd"), "SmallBiz Personal");
                });
                conn.Close();
            }
            


             if (SmallBizB != null)
            {
                //TEST DB
                 //conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Application.StartupPath + "\\Smallbiz; Extended Properties=DBASE IV;");
               //LIVE
                 conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\192.168.0.254\captive\Auto\CBC_Savings\Smallbiz; Extended Properties=DBASE IV;");

                conn.Open();

                SmallBizB.ForEach(p =>
                {
                    //UPDATE REF
                    command = new OleDbCommand("UPDATE Ref SET ENDSN = '" + p.HistorySerial + "' WHERE BRSTN = '" + p.BRSTN + "' AND CHKTYPE ='B'", conn);

                    command.ExecuteNonQuery();

                    ////SAVE TO HISTORY
                    dbcon.InsertData(p, DateTime.Now.ToString("yyyy-MM-dd"), "SmallBiz Personal");
                });
                conn.Close();
            }
            dbcon.DBClosed();
        }
        private static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            textBox.PasswordChar = '*';

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
