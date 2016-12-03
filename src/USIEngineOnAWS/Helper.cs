using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace USIEngineOnAWS
{
    static class Helper
    {
        static public string get_remote_ipaddress()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://whatismyip.akamai.com/");
            req.Method = "GET";
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
        }

        static public string get_default_str(InifileUtils ini, string key)
        {
            return ini.getValueString("default", key);
        }

        static public void set_default_str(InifileUtils ini, string key, string val)
        {
            ini.setValue("default", key, val);
        }

        static public string get_aws_cmd_arg(InifileUtils ini, string key)
        {
            return ini.getValueString("aws_cli", key);
        }

        static public string build_name(InifileUtils ini, string tag)
        {
            return tag + "-" + ini.getValueString("common", "name_tag");
        }

        static public string get_region(FileStream fs, string region_name)
        {
            string rtn = "";

            fs.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                string[] arr = line.Split(',');
                if (arr[1].Equals(region_name))
                {
                    rtn = arr[0];
                    break;
                }
            }
            return rtn;
        }

        static public bool get_region_search_flag(FileStream fs, string region_name)
        {
            bool rtn = false;

            fs.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                string[] arr = line.Split(',');
                if (arr[1].Equals(region_name))
                {
                    rtn = (arr[2].ToLower().Equals("true"));
                    break;
                }
            }
            return rtn;
        }

        static public string get_install_script(FileStream fs, string name, int pos)
        {
            string rtn = "";

            fs.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                string[] arr = line.Split(',');
                if (arr[0].Trim().Equals(name.Trim()))
                {
                    rtn = arr[pos].Trim();
                    break;
                }
            }
            return rtn;
        }

        static public bool check_az(FileStream fs, string region_key, string tag, int num = 1)
        {
            bool rtn = false;

            fs.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                string[] arr = line.Split(',');
                if (arr[num].Equals(region_key))
                {
                    for (int ii = 3; ii < arr.Length; ii++)
                        if (arr[ii].Equals(tag.ToLower()))
                        {
                            rtn = true;
                            break;
                        }
                    if (rtn) break;
                }
                if (rtn) break;
            }
            return rtn;
        }

        static public void sleep(long nn)
        {
            long cnt = nn / 64;
            for (long ll = 0; ll < cnt; ll++)
            {
                Thread.Sleep(64);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        // todo どうすっか。
        static private string password_ = "__change_it__";
        static public string encrypt(string src)
        {
            if (src == null || src.Length == 0) return "";

            byte[] bytes = Encoding.UTF8.GetBytes(src);
            RijndaelManaged rijndael = new RijndaelManaged();
            byte[] key, iv;
            make_password(password_, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            using (ICryptoTransform encryptor = rijndael.CreateEncryptor())
            {
                byte[] enc_bytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Convert.ToBase64String(enc_bytes);
            }
        }

        static public string decrypt(string src)
        {
            if (src == null || src.Length == 0) return "";

            byte[] bytes = Convert.FromBase64String(src);
            RijndaelManaged rijndael = new RijndaelManaged();
            byte[] key, iv;
            make_password(password_, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;
            using (ICryptoTransform decryptor = rijndael.CreateDecryptor())
            {
                byte[] dec_bytes = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(dec_bytes);
            }
        }

        static private void make_password(string password, int key_size, out byte[] key, int block_size, out byte[] iv)
        {
            byte[] salt = Encoding.UTF8.GetBytes("_usiengine_on_aws_");
            using (PasswordDeriveBytes deriveBytes = new PasswordDeriveBytes(password, salt))
            {
                deriveBytes.IterationCount = 1024;
                key = deriveBytes.GetBytes(key_size / 8);
                iv = deriveBytes.GetBytes(block_size / 8);
            }
        }
    }
}
