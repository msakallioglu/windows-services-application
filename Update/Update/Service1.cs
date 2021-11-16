using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Update
{
    public partial class Service1 : ServiceBase
    {
        InnerOperation obj = new InnerOperation();
        protected override void OnStart(string[] args)
        {
            obj.Start();
        }
        protected override void OnStop()
        {
            obj.Stop();
        }
    }

    public class InnerOperation
    {
        Timer timer = new Timer();
        public void Start()
        {
            WriteToFile("Service Started " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 120000;
            timer.Enabled = true;

        }
        public void Stop()
        {
            WriteToFile("Service Stopped " + DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            string file_path = @"C:\Users\user\Desktop\db.txt";
            FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string mysqlcon = sw.ReadLine();

            while (mysqlcon != null)
            {
                MySqlConnection con = new MySqlConnection(mysqlcon);
                con.Open();
                try
                {
                    if (con.State != ConnectionState.Closed)
                        WriteToFile("Mysql Connection Successful " + DateTime.Now);
                    else
                        WriteToFile("Mysql Connection Fail " + DateTime.Now);

                }
                catch (Exception ex)
                {
                    WriteToFile(ex.Message + DateTime.Now);
                }
                try
                {
                    string query = sw.ReadLine();
                    MySqlCommand com = new MySqlCommand(query, con);
                    int check = com.ExecuteNonQuery();
                    if (check >= 0)
                    {
                        WriteToFile("Update Successful " + DateTime.Now);
                    }
                    else
                    {
                        WriteToFile("Update Fail " + DateTime.Now);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);

                }
                con.Close();
                mysqlcon = sw.ReadLine();
            }
        }
    }
}
