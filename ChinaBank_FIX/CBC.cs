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
            dbcon.GetBranches(branchList);
            dbcon.GetHistory(historyList);
            CheckErrors();
            BindingSource checkBind = new BindingSource();

            checkBind.DataSource = errorList;

            dataGridView1.DataSource = checkBind;
            lblErrors.Text = errorList.Count.ToString();
            MessageBox.Show("Done!!!");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void CheckErrors()
        {
            historyList.ForEach(hist =>
            {
                var branch = branchList.FirstOrDefault(r => r.BRSTN == hist.BRSTN);

                if (branch != null)
                {
                    if (hist.ChequeName == "Personal Checks" || hist.ChequeName == "Commercial Checks"
                                        || hist.ChequeName == "Personal Pre-Encoded" || hist.ChequeName == "Commercial Pre-Encoded")
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

                }
            });
        }

        private void fixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = "";

            DialogResult result = InputBox("Admin Credentials", "Input Admin Password", ref value);

            if (result == DialogResult.OK)
            {
                if (value == "secret")
                {
                    dbcon.FixError(errorList);

                    MessageBox.Show("China Bank Database has been Fixed");
                }
                else
                    MessageBox.Show("Invalid Password", "System Error");
            }
            dataGridView1.Refresh();
            dataGridView1.Rows.Clear();
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

    }
}
