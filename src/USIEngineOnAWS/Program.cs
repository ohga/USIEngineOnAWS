using System;
using System.Windows.Forms;

namespace USIEngineOnAWS
{
    public static class Globals
    {
        static public string aws_access_key_id = "";
        static public string aws_secret_access_key = "";
        static public string resouce_dir = @"resource\";
    }

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var myId = Process.GetCurrentProcess().Id;
            //var query = string.Format("SELECT * FROM Win32_Process WHERE ProcessId = {0}", myId);
            //var search = new ManagementObjectSearcher("root\\CIMV2", query);
            //var results = search.Get().GetEnumerator();
            //results.MoveNext();
            //var queryObj = results.Current;
            //var parentId = (uint)queryObj["ParentProcessId"];
            //var parent = Process.GetProcessById((int)parentId);
            //Console.WriteLine("I was started by {0}", parent.ProcessName);
            //MessageBox.Show(parent.ProcessName);
            ////            Console.ReadLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new USIEngineOnAWS());

        }
    }
}
