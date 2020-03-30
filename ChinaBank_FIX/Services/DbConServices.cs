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
            //Main f1 = new Main();
            //BPI bpi = new BPI();
            
            string DBConnection = "";
           //if (Main.activeB == "CBC")
           // {
                DBConnection = "datasource=localhost;port=3306;username=root;password=corpcaptive; convert zero datetime=True;";
           //   MessageBox.Show(Main.activeB);
           //  //   DBConnection = "islabank";
           // }
           // else
           // {

               //ableName;
               //BConnection = "";
              //DBConnection = "datasource=localhost;port=3306;username=root;password=secret; convert zero datetime=True;";
                //DBConnection = "datasource=192.168.0.254;port=3306;username=root;password=CorpCaptive; convert zero datetime=True;";
                MessageBox.Show(Main.activeB);
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

                branch.LastNo_PA = !reader.IsDBNull(2) ? reader.GetInt64(2) : 0;

                _branches.Add(branch);
            }


            DBClosed();

            return _branches;
        }
        public List<HistoryModel> GetHistory(List<HistoryModel> _history)
        {
            DBCon();
            string query = "SELECT BRSTN, ChequeName, MAX(CAST(EndingSerial as DECIMAL(18,0)))EndingSerial FROM captive_database.master_database_philtrust WHERE ChequeName <>'' GROUP BY BRSTN, ChequeName";

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

            DBClosed();
            return _history;
            // RemoveDuplicatePreEncoded();
        }
        public ErrorModel InsertData(ErrorModel _data, string _date, string _ChequeName)
        {
            MySqlCommand myCmd = new MySqlCommand();
            myCmd = new MySqlCommand("INSERT INTO captive_database.philtrust_fix (BRSTN, BranchName, CheckType, OldSerial, CorrectSerial,Date) VALUES" +
                            "('" + _data.BRSTN + "','" + _data.BranchName.Replace("'", " ") + "','" + _ChequeName + "','" + _data.CurrentSerial + "','" + _data.HistorySerial + "','" + _date + "');", myConnect);

            myCmd.ExecuteNonQuery();
            return _data;

        }
    }
}
