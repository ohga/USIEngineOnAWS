using Renci.SshNet;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace USIEngineViaSSH
{
    class Program
    {
        static bool sync_flg_ = true;
        static string remote_dir_ = "/opt/usi_engine/share/eval_dir";

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            Environment.CurrentDirectory = assembly.Location.Substring(0, assembly.Location.LastIndexOf('\\'));

            string[] arr = File.ReadAllLines("info.txt")[0].Split(',');
            string ipaddress = arr[0].Trim();
            string private_key = arr[1].Trim();

            try
            {
                if (args.Length == 1)
                {
                    do_scp(ipaddress, private_key, args[0], remote_dir_);
                    return;
                }

                do_service(ipaddress, private_key);

            }
            catch (Exception e)
            {
                Console.WriteLine("異常を検知しました。");
                Console.WriteLine(e.ToString());
                Console.WriteLine("続行するには何かキーを押してください . . .");
                Console.ReadKey();
            }

        }

        static void do_scp(string ipaddress, string private_key, string from, string to)
        {
            PrivateKeyFile private_key_file = new PrivateKeyFile(private_key);
            ConnectionInfo connect_info = new ConnectionInfo(ipaddress, 22, "ubuntu",
            new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod("ubuntu",
                new PrivateKeyFile[] { private_key_file }) });
            connect_info.Timeout = TimeSpan.FromMinutes(30);

            FileInfo fi = new FileInfo(from);
            Console.WriteLine(from + " を " + to + "/" + fi.Name + " へアップロードします");
            using (var scp = new ScpClient(connect_info))
            {
                scp.KeepAliveInterval = TimeSpan.FromSeconds(30);
                scp.Connect();
                scp.Upload(fi, "/home/ubuntu/" + fi.Name);
                scp.Disconnect();
            }
            Thread.Sleep(3000);
            Console.WriteLine("後処理中です。");
            int read_timeout = (30);
            using (var client = new SshClient(connect_info))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(30);
                client.Connect();
                using (ShellStream stream = client.CreateShellStream("ubuntu", 1024, 24, 800, 600, 1024))
                {
                    var reader = new StreamReader(stream);
                    var writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    read_text_data(reader, stream, read_timeout);
                    Thread.Sleep(1000);
                    write_stream(
                        "sudo sh -c 'install -o usi_engine -g usi_engine -m 777 \"/home/ubuntu/" + fi.Name + "\" \"" + to + "/" + fi.Name + "\"'; echo '__EXIT__'; exit;",
                        writer, stream);
                    Thread.Sleep(1000);
                    Console.WriteLine("アップロード完了。");
                    Console.WriteLine("続行するには何かキーを押してください . . .");
                    Console.ReadKey();
                }
            }
        }

        static private bool read_text_data(StreamReader sr, ShellStream stream, int timeout)
        {
            bool rtn = true;
            while (stream.Length == 0)
            {
                Thread.Sleep(64);
            }
            long nn = 0;
            while (true)
            {
                string str = sr.ReadLine();
                if (str == null)
                {
                    Thread.Sleep(64);
                    nn = checked(nn + 64);
                    if (nn > checked(timeout * 1000))
                    {
                        rtn = false;
                        break;
                    }
                    continue;
                }
                if (str.Trim().IndexOf("__EXIT__") != -1) break;
                nn = 0;
            }
            return rtn;
        }


        static void do_service(string ipaddress, string private_key)
        {
            PrivateKeyFile private_key_file = new PrivateKeyFile(private_key);
            ConnectionInfo connect_info = new ConnectionInfo(ipaddress, 22, "usi_engine",
            new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod("usi_engine",
                new PrivateKeyFile[] { private_key_file }) });
            connect_info.Timeout = TimeSpan.FromMinutes(30);

            int read_timeout = checked(60 * 60 * 5);
            using (var client = new SshClient(connect_info))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(30);
                client.Connect();
                using (ShellStream stream = client.CreateShellStream("usi_engine", 80, 24, 800, 600, 1024))
                {
                    var reader = new StreamReader(stream);
                    var writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    read_stream_async(reader, read_timeout);
                    while (sync_flg_)
                    {
                        var str = Console.ReadLine();
                        write_stream(str.TrimEnd(), writer, stream);
                        if (str.Equals("quit")) break;
                    }
                }
            }
        }

        static private async void read_stream_async(StreamReader sr, int timeout, bool output = true)
        {
            long cur = checked(DateTime.Now.Ticks / TimeSpan.TicksPerSecond + timeout);
            while (true)
            {
                if (cur < (DateTime.Now.Ticks / TimeSpan.TicksPerSecond)) break;
                string str = await sr.ReadLineAsync();
                if (str == null)
                {
                    Thread.Sleep(64);
                    continue;
                }
                if (output) Console.WriteLine(str);
                cur = checked(DateTime.Now.Ticks / TimeSpan.TicksPerSecond + timeout);
            }
            sync_flg_ = false;
        }

        private static void write_stream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.Write(cmd.TrimEnd());
            writer.Write("\n");
            writer.Flush();
        }
    }
}
