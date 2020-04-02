using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using ChinaBank_FIX.Models;
using ChinaBank_FIX.Services;
using System.Windows.Forms;


namespace ChinaBank_FIX.Services
{
    class DbConServices
    {
        public MySqlConnection myConnect;
        List<HistoryModel> historyList = new List<HistoryModel>();
        List<BranchModel> branchList = new List<BranchModel>();
        List<ErrorModel> errorList = new List<ErrorModel>();

        public void DBCon()
        {
            
            string DBConnection = "";
           //if (Main.activeB == "CBC")
           // {
              //  DBConnection = "datasource=localhost;port=3306;username=root;password=corpcaptive; convert zero datetime=True;";
           //   MessageBox.Show(Main.activeB);
           //  //   DBConnection = "islabank";
           // }
           // else
           // {

               //ableName;
               //BConnection = "";
              //DBConnection = "datasource=localhost;port=3306;username=root;password=secret; convert zero datetime=True;";
                DBConnection = "datasource=192.168.0.254;port=3306;username=root;password=CorpCaptive; convert zero datetime=True;";
               // MessageBox.Show(Main.activeB);
            //   DBConnection = "captive_database";
              //}



            myConnect = new MySqlConnection(DBConnection);
            myConnect.Open();
        }
        public void DBClosed()
        {
            myConnect.Close();
        }

        public List<BranchModel> GetBranches(List<BranchModel> _branches)
        {
            DBCon();
            string sql = "Select BRSTN,Address1,Reg_LastNo from captive_database.master_database_cbc_branches";
            MySqlCommand cmd = new MySqlCommand(sql, myConnect);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                BranchModel branch = new BranchModel();

                branch.BRSTN = !reader.IsDBNull(0) ? reader.GetString(0) : "";

                branch.BranchName = !reader.IsDBNull(1) ? reader.GetString(1) : "";

                branch.LastNo_Regular = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0;

                _branches.Add(branch);
            }


            DBClosed();

            return _branches;
        }
        public List<BranchModel> GetBranchesCBS(List<BranchModel> _branches)
        {
            DBCon();
            string sql = "Select BRSTN,Address1 from captive_database.master_database_cbs_branches";
            MySqlCommand cmd = new MySqlCommand(sql, myConnect);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                BranchModel branch = new BranchModel();

                branch.BRSTN = !reader.IsDBNull(0) ? reader.GetString(0) : "";

                branch.BranchName = !reader.IsDBNull(1) ? reader.GetString(1) : "";

               // branch.LastNo_Regular = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0;

                _branches.Add(branch);
            }


            DBClosed();

            return _branches;
        }

        public List<HistoryModel> GetHistory(List<HistoryModel> _history)
        {
            DBCon();
            string query = "SELECT BRSTN, ChequeName, MAX(CAST(EndingSerial as DECIMAL(18,0)))EndingSerial FROM captive_database.master_database_"+Main.activeB+" WHERE ChequeName <>'' GROUP BY BRSTN, ChequeName";

            //  MySqlConnection conn = new MySqlConnection(myConnect);

            //conn.Open();

            MySqlCommand cmd = new MySqlCommand(query, myConnect);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                HistoryModel history = new HistoryModel();

                history.BRSTN = !reader.IsDBNull(0) ? reader.GetString(0) : "";

                history.ChequeName = !reader.IsDBNull(1) ? reader.GetString(1) : "";

                history.MaxEnding = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0;

                _history.Add(history);
            }
           // RemoveDuplicatePreEncoded();
            DBClosed();
            return _history;
           
        }
        //private void RemoveDuplicatePreEncoded()
        //{
        //    var personalPre = historyList.Where(r => r.ChequeName == "Personal Pre-Encoded").ToList();

        //    var commercialPre = historyList.Where(r => r.ChequeName == "Commercial Pre-Encoded").ToList();

        //    personalPre.ForEach(x =>
        //    {
        //        var temp = historyList.FirstOrDefault(r => r.BRSTN == x.BRSTN && r.ChequeName == "Regular Personal");

        //        if (temp != null)
        //        {
        //            if (x.MaxEnding >= temp.MaxEnding)
        //                historyList.Remove(temp);
        //            else
        //                historyList.Remove(x);
        //        }
        //    });

        //    commercialPre.ForEach(y =>
        //    {
        //        var temp = historyList.FirstOrDefault(r => r.BRSTN == y.BRSTN && r.ChequeName == "Regular Commercial");

        //        if (temp != null)
        //        {
        //            if (y.MaxEnding >= temp.MaxEnding)
        //                historyList.Remove(temp);
        //            else
        //                historyList.Remove(y);
        //        }
        //    });
        //}
        public ErrorModel InsertData(ErrorModel _data, string _date, string _ChequeName)
        {
            MySqlCommand myCmd = new MySqlCommand();
            myCmd = new MySqlCommand("INSERT INTO captive_database."+Main.activeB+"_fix (BRSTN, BranchName, CheckType, OldSerial, CorrectSerial,Date) VALUES" +
                            "('" + _data.BRSTN + "','" + _data.BranchName.Replace("'", " ") + "','" + _ChequeName + "','" + _data.CurrentSerial + "','" + _data.HistorySerial + "','" + _date + "');", myConnect);

            myCmd.ExecuteNonQuery();
            return _data;

        }
        public List<ErrorModel> FixError(List<ErrorModel> _errorList)
        {
            var Regular = _errorList.Where(r => r.CheckType == "Regular Checks").ToList();


            if (Regular != null)
            {
                DBCon();
                MySqlCommand command;
                Regular.ForEach(p =>
                {
                    //UPDATE REF
                    command = new MySqlCommand("UPDATE captive_database.master_database_cbc_branches SET Reg_LastNo = '" + p.HistorySerial + "' WHERE BRSTN = '" + p.BRSTN + "'", myConnect);

                    command.ExecuteNonQuery();

                    ////SAVE TO HISTORY
                    InsertData(p, DateTime.Now.ToString("yyyy-MM-dd"), "Regular Checks");
                });

            }

            DBClosed();
            return _errorList;
        }
    }
}
