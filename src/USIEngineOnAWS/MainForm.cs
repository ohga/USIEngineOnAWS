using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace USIEngineOnAWS
{
    public partial class USIEngineOnAWS : Form
    {
        private string resouce_dir_ = Globals.resouce_dir;
        private InifileUtils setting_;
        private string end_time_str_ = "";

        private delegate void set_text_callback_(string text);

        public USIEngineOnAWS()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetEntryAssembly();
            Environment.CurrentDirectory = assembly.Location.Substring(0, assembly.Location.LastIndexOf('\\'));

            setting_ = new InifileUtils(resouce_dir_ + "setting.ini");

            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 1000;
            toolTip1.AutoPopDelay = 30000;
            string[] names = new string[] {
                "label1", "label3", "label4", "label6", "label7", "label8",
                "availability_zone_combo_box", "create_instance_button",
                "install_script_combo_box", "instance_text_box",
                "price_numeric_up_down", "price_of_instance_button",
                "region_combo_box", "shutdown_combo_box"
            };
            foreach (string name in names)
            {
                var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    string val = "";
                    try
                    {
                        val = File.ReadAllText(resouce_dir_ + "tooltip\\" + name + ".txt");
                    }
                    catch
                    {
                        // none
                    }
                    if (val.Length == 0) continue;

                    toolTip1.SetToolTip((System.Windows.Forms.Control)field.GetValue(this), val);
                }
            }
        }

        private void USIEngineOnAWS_Load(object sender, EventArgs e)
        {
            string region = Helper.get_default_str(setting_, "region");
            string engine = Helper.get_default_str(setting_, "engine");
            string eval = Helper.get_default_str(setting_, "eval");
            instance_combo_box.Text = Helper.get_default_str(setting_, "instance_type");
            shutdown_combo_box.SelectedIndex = Convert.ToInt32(Helper.get_default_str(setting_, "shutdown_option"));

            this.Icon = new System.Drawing.Icon(Globals.resouce_dir + "\\icon\\onaws.ico");

            using (StreamReader sr = new StreamReader(resouce_dir_ + "region.txt"))
            {
                var list = new List<object>();
                string line = "";
                int pos = 0, nn = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(',');
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line[0] == '#') continue;
                    list.Add((object)arr[1]);
                    if (arr[0].Equals(region)) pos = nn;
                    nn++;
                }
                region_combo_box.Items.Clear();
                region_combo_box.Items.AddRange(list.ToArray());
                region_combo_box.SelectedIndex = pos;
            }

            using (StreamReader sr = new StreamReader(resouce_dir_ + "instance_type.txt"))
            {
                var list = new List<object>();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0) continue;
                    if (line[0] == '#') continue;
                    list.Add((object)line);
                }
                instance_combo_box.Items.Clear();
                instance_combo_box.Items.AddRange(list.ToArray());
            }
            instance_combo_box_SelectedIndexChanged(null, null);

            using (StreamReader sr = new StreamReader(resouce_dir_ + "install_script.txt"))
            {
                var list = new List<object>();
                string line = "";
                int pos = 0, nn = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arr = line.Split(',');
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line[0] == '#') continue;
                    list.Add((object)arr[0]);
                    if (arr[1].Trim().Equals(engine.Trim())
                        && arr[2].Trim().Equals(eval.Trim()))
                        pos = nn;
                    nn++;
                }
                install_script_combo_box.Items.Clear();
                install_script_combo_box.Items.AddRange(list.ToArray());
                install_script_combo_box.SelectedIndex = pos;
            }

            folder_browser_dialog.SelectedPath = Environment.CurrentDirectory;
            try
            {
                if (Helper.get_default_str(setting_, "save_folder").Length != 0)
                    folder_browser_dialog.SelectedPath =
                         Helper.get_default_str(setting_, "save_folder");
                if (!Directory.Exists(folder_browser_dialog.SelectedPath))
                    folder_browser_dialog.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            catch
            {
                folder_browser_dialog.SelectedPath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }
            setting_.setValue("default", "save_folder", folder_browser_dialog.SelectedPath);

            tool_strip_menu_item.DropDownOpened += new System.EventHandler(this.search_hover);
            this.LostFocus += new EventHandler(this.search_leave);
            this.Click += new EventHandler(this.search_leave);
            this.Resize += new EventHandler(this.search_leave);
            this.Move += new EventHandler(this.search_leave);

            List<ToolStripItem> arr2 = new List<ToolStripItem>();
            using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
            {
                foreach (string title in region_combo_box.Items)
                {
                    bool flg = Helper.get_region_search_flag(fs, title);
                    ToolStripMenuItem tmp = new System.Windows.Forms.ToolStripMenuItem();
                    tmp.Text = title;
                    tmp.Checked = flg;
                    tmp.Click += new EventHandler(this.search_click);
                    arr2.Add(tmp);
                }
            }
            this.tool_strip_menu_item.DropDownItems.Clear();
            this.tool_strip_menu_item.DropDownItems.AddRange(arr2.ToArray());

            try
            {
                Globals.aws_access_key_id = Helper.decrypt(setting_.getValueString("common", "aws_access_key_id"));
                Globals.aws_secret_access_key = Helper.decrypt(setting_.getValueString("common", "aws_secret_access_key"));
            }
            catch
            {
                // none
            }

            if (Globals.aws_access_key_id == null || Globals.aws_access_key_id.Length == 0
                || Globals.aws_secret_access_key == null || Globals.aws_secret_access_key.Length == 0)
            {
                accesskey_form_menu_item_Click(null, null);
            }

        }

        #region menu {{{
        private void create_vpc_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            DialogResult rtn = MessageBox.Show(
                this, region_combo_box.Text + "にVPCを作成します。\r\nよろしいですか？", "VPC作成", MessageBoxButtons.OKCancel);
            if (rtn == DialogResult.Cancel) return;
            set_enable(false);
            try
            {
                create_vpc();
            }
            finally
            {
                set_enable(true);
            }
        }
        private void delete_vpc_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            DialogResult rtn = MessageBox.Show(
                this, region_combo_box.Text + "のVPCを削除します。\r\nよろしいですか？", "VPC削除", MessageBoxButtons.OKCancel);
            if (rtn == DialogResult.Cancel) return;
            set_enable(false);
            try
            {
                delete_vpc();
            }
            finally
            {
                set_enable(true);
            }
        }
        private void delete_instance_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            DialogResult rtn = MessageBox.Show(
                 this, "AWSから " + region_combo_box.Text + " 内のすべてのインタンスの削除を試みます。\r\nよろしいですか？", "インスタンス削除", MessageBoxButtons.OKCancel);
            if (rtn == DialogResult.Cancel) return;
            delete_region_instance();
        }
        private void save_folder_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            if (folder_browser_dialog.ShowDialog(this) == DialogResult.OK)
            {
                setting_.setValue("default", "save_folder", folder_browser_dialog.SelectedPath);
            }
        }
        private void spot_instance_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            Process.Start("https://aws.amazon.com/jp/ec2/spot/pricing/#スポットインスタンス価格");
        }
        private void aws_instance_stat_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            string region = "";
            using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
            {
                region = Helper.get_region(fs, region_combo_box.Text);
            }
            string url = string.Format(
                "https://{0}.console.aws.amazon.com/ec2/v2/home?region={0}#Instances", region);
            Process.Start(url);
        }
        private void billing_menu_item_Click(object sender, EventArgs e)
        {
            menu_close();

            Process.Start("https://console.aws.amazon.com/billing/home");
        }
        private void accesskey_form_menu_item_Click(object sender, EventArgs e)
        {
            AccessKeyForm accessKeyForm = new AccessKeyForm();
            accessKeyForm.ShowDialog();
            accessKeyForm.Dispose();
            if (Globals.aws_access_key_id == null || Globals.aws_access_key_id.Length == 0
                || Globals.aws_secret_access_key == null || Globals.aws_secret_access_key.Length == 0)
            {
                this.Close();
            }
        }
        #endregion }}}

        #region button {{{
        private void price_of_instance_button_Click(object sender, EventArgs e)
        {
            menu_close();

            string instance_type = instance_combo_box.Text;
            if (instance_type == "")
            {
                error("インスタンスタイプが選ばれていません。");
                return;
            }

            set_enable(false);
            try
            {
                decimal min = 10000;

                int min_pos = 0;
                string min_az = "";
                string min_region = "";
                write_log(instance_type + " の最安値を調べています。");
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    int pos = 0;
                    foreach (string str in region_combo_box.Items)
                    {
                        if (!Helper.get_region_search_flag(fs, str))
                        {
                            pos++;
                            continue;
                        }

                        using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
                        {
                            awsUtils.region = Helper.get_region(fs, str);
                            region_combo_box.SelectedIndex = pos;
                            int a_pos = 0;
                            foreach (object obj in availability_zone_combo_box.Items)
                            {
                                availability_zone_combo_box.SelectedIndex = a_pos;
                                a_pos++;
                                decimal dd = awsUtils.load_spot_price(instance_type, ((string)obj).ToLower());
                                if (dd >= 10000)
                                {
                                    break;
                                }
                                price_numeric_up_down.Value = dd;
                                if (dd < min)
                                {
                                    min_az = ((string)obj).ToUpper();
                                    min = dd;
                                    min_pos = pos;
                                    min_region = awsUtils.region;
                                }
                            }
                        }
                        pos++;
                    }
                }
                region_combo_box.SelectedIndex = min_pos;
                availability_zone_combo_box.Text = min_az;
                price_numeric_up_down.Value = min;
                write_log("最安値は " + min_region + min_az.ToLower() + " の " + min + " です。");
                set_instance_name();
            }
            catch (Exception ex)
            {
                write_log("ERROR:\n" + ex.ToString());
            }
            finally
            {
                set_enable(true);
            }
        }
        private void create_instance_button_Click(object sender, EventArgs e)
        {
            menu_close();

            using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                bool has_err = false;
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    awsUtils.region = Helper.get_region(fs, region_combo_box.Text);
                }
                string availability_zone = availability_zone_combo_box.Text.ToLower();

                string instance_name = instance_text_box.Text;
                if (instance_name == "" || instance_name.ToLower().Equals("resource"))
                {
                    write_log("ERROR: インスタンス名が入力されていません。");
                    has_err = true;
                }
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                if (instance_name.IndexOfAny(invalidChars) >= 0)
                {
                    write_log("ERROR: インスタンス名に使用できない文字が使用されています。(フォルダ名として不適切です)");
                    has_err = true;
                }
                if (instance_name.Length != Encoding.GetEncoding("shift_jis").GetByteCount(instance_name))
                {
                    write_log("ERROR: インスタンス名に使用できない文字が使用されています。(全角文字が含まれています)");
                    has_err = true;
                }

                string instance_type = instance_combo_box.Text;
                if (instance_type == "")
                {
                    write_log("ERROR: インスタンスタイプが選ばれていません。");
                    has_err = true;
                }

                string vm_type_ = "hvm";
                if (AWSEC2Utils.paravirtual_instance_types.Contains(instance_type))
                {
                    vm_type_ = "paravirtual";
                }

                string spot_price = price_numeric_up_down.Text;
                if (spot_price == "")
                {
                    write_log("ERROR: 値段が入力されていません。");
                    has_err = true;
                }
                if (Convert.ToDouble(spot_price) < 0.001)
                {
                    MessageBox.Show("金額は $0.001 以上の値を入力してください。", "エラー");
                    return;
                }

                string engine = "", eval = "";
                using (FileStream fs = new FileStream(resouce_dir_ + "install_script.txt", FileMode.Open, FileAccess.Read))
                {
                    engine = Helper.get_install_script(fs, install_script_combo_box.Text, 1);
                    eval = Helper.get_install_script(fs, install_script_combo_box.Text, 2);
                }
                if (engine == "" || eval == "")
                {
                    write_log("ERROR: エンジンが入力されていません。");
                    has_err = true;
                }

                string save_folder =
                    folder_browser_dialog.SelectedPath + "\\" + instance_text_box.Text;
                if (Directory.Exists(save_folder) || File.Exists(save_folder))
                {
                    DialogResult rtn = MessageBox.Show(
                        this, "既にフォルダが存在します。上書きしますか？", "上書き確認", MessageBoxButtons.OKCancel);
                    if (rtn == DialogResult.Cancel) return;
                    Directory.Delete(save_folder + "\\", true);

                }

                save_folder = save_folder + "\\";
                try
                {
                    Directory.CreateDirectory(save_folder);
                }
                catch
                {
                    write_log("ERROR: 保存フォルダ(" + save_folder + ")の作成に失敗しました。");
                    has_err = true;
                }

                if (has_err)
                {
                    MessageBox.Show("失敗しました。", "エラー");
                    return;
                }

                Helper.set_default_str(setting_, "region", awsUtils.region);
                Helper.set_default_str(setting_, "availability_zone", availability_zone);

                set_enable(false);

                string private_key_tmp = null;
                FileInfo fi_tmp = null;
                try
                {
                    private_key_tmp = Path.GetTempFileName();
                    fi_tmp = new FileInfo(private_key_tmp);
                    Mutex mutex = new Mutex(false, Application.ProductName + awsUtils.region);
                    try
                    {
                        if (mutex.WaitOne(30 * 1000) == false)
                        {
                            MessageBox.Show("処理の開始に失敗しました", "エラー");
                            return;
                        }

                        if (!awsUtils.check_vpc_id())
                        {
                            write_log("VPC が存在しないので作成します。");
                            create_vpc();
                            awsUtils.check_vpc_id();
                        }

                        awsUtils.reload_key_pair(private_key_tmp, fi_tmp.Name);
                        if (!awsUtils.check_subnet_id(availability_zone))
                        {
                            return;
                        }

                        if (!awsUtils.check_security_group_id())
                        {
                            error("セキュリティグループでエラーが発生しました。");
                            return;
                        }

                        if (!awsUtils.load_image_id(vm_type_))
                        {
                            error("マシンイメージでエラーが発生しました。");
                            return;
                        }

                        if (!awsUtils.request_spot(instance_type, availability_zone, spot_price, fi_tmp.Name))
                        {
                            error("スポットリクエストでエラーが発生しました。");
                            return;
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }

                    int spot_request_timeout_seconds = setting_.getValueInt("common", "spot_request_timeout_seconds");
                    string instance_id = "";
                    long timeout = DateTime.Now.Ticks / TimeSpan.TicksPerSecond + spot_request_timeout_seconds;
                    for (int ii = 0; ii < 1024; ii++)
                    {
                        if (timeout < (DateTime.Now.Ticks / TimeSpan.TicksPerSecond)) break;
                        Helper.sleep(100);

                        if ((ii % 30) != 1) continue;

                        string request_spot_status = awsUtils.query_request_spot();
                        if (request_spot_status.IndexOf(",") >= 0)
                        {
                            instance_id = request_spot_status.Substring(request_spot_status.IndexOf(',') + 1);
                            break;
                        }
                        if (request_spot_status.IndexOf("pending-evaluation") < 0
                            && request_spot_status.IndexOf("pending-fulfillment") < 0)
                        {
                            write_log("スポットリクエストが落札できませんでした。");
                            instance_id = "";
                            awsUtils.cancel_spot();
                            break;
                        }
                    }
                    if (instance_id == "")
                    {
                        awsUtils.cancel_spot();
                        error("スポットインスタンスが落札できませんでした。");
                        return;
                    }

                    awsUtils.set_name_tag(instance_id, instance_name);
                    File.WriteAllText(save_folder + "instance_status-" + instance_id + ".url",
                        String.Format("[InternetShortcut]\r\nURL=https://{0}.console.aws.amazon.com/ec2/v2/home?region={0}#Instances:search={1};sort=instanceId",
                            awsUtils.region, instance_id));

                    bool rollback = true;
                    try
                    {
                        string ipaddress = awsUtils.get_instance_public_ipaddress(instance_id);
                        if (ipaddress == null || ipaddress == "")
                        {
                            error("インスタンスの ipaddress の取得に失敗しました。");
                            return;
                        }

                        bool flg = false;
                        for (int ii = 0; ii < 3; ii++)
                        {
                            try
                            {
                                if (!execute_script(ipaddress, private_key_tmp, instance_type, engine, eval))
                                {
                                    write_log("ERROR: セットアップスクリプトの実行でエラーが発生しました。(retry)");
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                write_log("ERROR: " + ex.ToString());
                                write_log("ERROR: セットアップスクリプトの実行でエラーが発生しました。(retry)");
                                continue;
                            }
                            flg = true;
                            break;
                        }
                        if (flg == false)
                        {
                            error("セットアップスクリプトの実行に失敗しました。");
                            return;
                        }

                        File.Copy(resouce_dir_ + "USIEngineViaSSH.ex_", save_folder + "USIEngineViaSSH.exe", true);
                        File.Copy(private_key_tmp, save_folder + "private.pem", true);
                        File.Copy("Renci.SshNet.dll", save_folder + "Renci.SshNet.dll", true);
                        try
                        {
                            File.Copy("Renci.SshNet.LICENSE", save_folder + "Renci.SshNet.LICENSE", true);
                        }
                        catch
                        {
                            // none.
                        }
                        File.WriteAllText(save_folder + "info.txt",
                            ipaddress + ",private.pem\r\n"
                            + "# " + spot_price + "," + instance_type + "," + awsUtils.region + availability_zone + "," + end_time_str_ + "\r\n"
                            + "# ssh -o StrictHostKeyChecking=no -i private.pem ubuntu@" + ipaddress + "\r\n\r\n"
                            + "# UsiClient.exe.config patch (https://sites.google.com/site/shogixyz/home/shogiutil)... \r\n"
                            + "<appSettings>\r\n"
                            + "  <add key=\"HostName\" value=\"" + ipaddress + "\"/>\r\n"
                            + "  <add key=\"Port\" value=\"53556\"/>\r\n"
                            + "  <add key=\"ExePath\" value=\"" + engine + "\"/>\r\n"
                            + "</appSettings>\r\n");
                        write_log(" " + save_folder + " にエンジンを作成しました");
                        Process.Start(save_folder);
                        rollback = false;
                    }
                    finally
                    {
                        if (rollback)
                        {
                            write_log("*** 失敗を検知しました、インスタンスを削除を試行します。");
                            delete_instance();
                        }
                    }
                }
                catch (Exception ex)
                {
                    write_log("ERROR:\n" + ex.ToString());
                }
                finally
                {
                    if (private_key_tmp != null)
                    {
                        awsUtils.delete_key_pair(fi_tmp.Name);
                        File.Delete(private_key_tmp);
                    }
                    set_enable(true);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            menu_close();

            if (view_log_button.Text.Equals("v"))
            {
                view_log_button.Text = "^";
                this.Size = new Size(this.Size.Width, this.MinimumSize.Height + 128);
            }
            else
            {
                view_log_button.Text = "v";
                this.Size = new Size(this.Size.Width, this.MinimumSize.Height);
            }
        }
        #endregion }}}

        #region combo_change {{{
        private void region_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                string availability_zone = availability_zone_combo_box.Text.ToLower();

                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    string region = Helper.get_region(fs, region_combo_box.Text);
                    if (region == "") return;
                    List<string> arr = new List<string>();
                    IList<string> list = new List<string>(awsUtils.subnet_ids.Keys);
                    foreach (string tag in list)
                    {
                        if (Helper.check_az(fs, region_combo_box.Text, tag))
                            arr.Add(tag.ToUpper());
                    }
                    availability_zone_combo_box.Items.Clear();
                    availability_zone_combo_box.Items.AddRange((object[])arr.ToArray());
                    bool hit = false;
                    for (int ii = 0; ii < arr.ToArray().Length; ii++)
                    {
                        if (availability_zone.Equals(arr[ii].ToLower()))
                        {
                            availability_zone_combo_box.SelectedIndex = ii;
                            hit = true;
                            break;
                        }
                    }
                    if (!hit) availability_zone_combo_box.SelectedIndex = 0;
                }
                set_instance_name();
            }
        }
        private void availability_zone_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.set_default_str(setting_, "availability_zone", availability_zone_combo_box.Text.ToLower());
            set_instance_name();
        }
        private void instance_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.set_default_str(setting_, "instance_type", instance_combo_box.Text);
            set_instance_name();
        }
        private void install_script_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(resouce_dir_ + "install_script.txt", FileMode.Open, FileAccess.Read))
            {
                Helper.set_default_str(setting_, "engine", Helper.get_install_script(fs, install_script_combo_box.Text, 1));
                Helper.set_default_str(setting_, "eval", Helper.get_install_script(fs, install_script_combo_box.Text, 2));

            }
            set_instance_name();
        }
        private void shutdown_combo_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.set_default_str(setting_, "shutdown_option", "" + shutdown_combo_box.SelectedIndex);
        }
        #endregion }}}

        #region etc_event {{{
        private void search_click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;

            string all = "";
            string region_name = item.Text;
            using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    string new_line = "";
                    string line = sr.ReadLine();
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line[0] == '#') continue;
                    string[] arr = line.Split(',');
                    if (arr[1].Equals(region_name))
                    {
                        if (item.Checked)
                            new_line = line.Replace("false", "true") + "\r\n";
                        else
                            new_line = line.Replace("true", "false") + "\r\n";
                    }
                    else
                    {
                        new_line = line + "\r\n";
                    }
                    all = all + new_line;
                }
            }
            File.WriteAllText(resouce_dir_ + "region.txt", all);
        }
        private void search_hover(object sender, EventArgs e)
        {
            file_menu_item.DropDown.AutoClose = false;
            tool_strip_menu_item.DropDown.AutoClose = false;
        }
        private void search_leave(object sender, EventArgs e)
        {
            file_menu_item.DropDown.AutoClose = true;
            tool_strip_menu_item.DropDown.AutoClose = true;
            menu_close();
        }
        private void USIEngineOnAWS_Deactivate(object sender, EventArgs e)
        {
            menu_close();
        }
        #endregion }}}

        #region helper {{{
        private void set_enable(bool flg)
        {
            region_combo_box.Enabled =
            availability_zone_combo_box.Enabled =
            instance_text_box.Enabled =
            instance_combo_box.Enabled =
            price_of_instance_button.Enabled =
            price_numeric_up_down.Enabled =
            create_instance_button.Enabled =
            shutdown_combo_box.Enabled =
            install_script_combo_box.Enabled =
            menu_strip.Enabled = flg;
            if (flg)
            {
                this.Activate();
                this.TopMost = true;
                this.TopMost = false;
            }
        }
        private void create_vpc()
        {
            using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    awsUtils.region = Helper.get_region(fs, region_combo_box.Text);
                }

                Helper.set_default_str(setting_, "region", awsUtils.region);
                try
                {
                    if (!awsUtils.load_vpc_id())
                    {
                        error("VPC の読み込みに失敗しました。");
                        return;
                    }

                    if (!awsUtils.load_internet_gateway_id())
                    {
                        error("インターネットゲートウェイの読み込みに失敗しました。");
                        return;
                    }

                    if (!awsUtils.load_routetable_id())
                    {
                        error("ルートテーブルの読み込みに失敗しました。");
                        return;
                    }

                    using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                    {
                        IList<string> list = new List<string>(awsUtils.subnet_ids.Keys);
                        foreach (string tag in list)
                        {
                            if (Helper.check_az(fs, region_combo_box.Text, tag) == false) continue;
                            if (!awsUtils.load_subnet_id(tag))
                            {
                                error(awsUtils.region + tag + " の読み込みに失敗しました。");
                                return;
                            }
                        }
                    }

                    if (!awsUtils.load_security_group_id())
                    {
                        error("セキュリティグループの読み込みに失敗しました。");
                        return;
                    }

                    write_log(awsUtils.region + " の VPC を作成しました。");
                }
                catch (Exception ex)
                {
                    write_log("ERROR:\n" + ex.ToString());
                }
            }
        }
        private void delete_vpc()
        {
            using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    awsUtils.region = Helper.get_region(fs, region_combo_box.Text);
                }

                Helper.set_default_str(setting_, "region", awsUtils.region);
                try
                {
                    if (awsUtils.delete_vpc_all(resouce_dir_ + "region.txt") == false)
                    {
                        MessageBox.Show("削除に失敗しました。", "エラー");
                    }

                }
                catch (Exception ex)
                {
                    write_log("ERROR:\n" + ex.ToString());
                }
            }
        }
        private void delete_instance()
        {
            using (AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                bool has_err = false;
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    awsUtils.region = Helper.get_region(fs, region_combo_box.Text);
                }

                string instance_name = instance_text_box.Text;
                if (instance_name == "" || instance_name.ToLower().Equals("resource"))
                {
                    write_log("ERROR: インスタンスネームが入力されていません。");
                    has_err = true;
                }
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                if (instance_name.IndexOfAny(invalidChars) >= 0)
                {
                    write_log("ERROR: インスタンス名に使用できない文字が使用されています。(フォルダ名として不適切です)");
                    has_err = true;
                }
                if (instance_name.Length != Encoding.GetEncoding("shift_jis").GetByteCount(instance_name))
                {
                    write_log("ERROR: インスタンス名に使用できない文字が使用されています。(全角文字が含まれています)");
                    has_err = true;
                }

                if (has_err)
                {
                    MessageBox.Show("削除に失敗しました。", "エラー");
                    return;
                }

                set_enable(false);
                try
                {
                    awsUtils.delete_instance(instance_name);
                    write_log(instance_name + " を削除しました。");
                }
                catch (Exception ex)
                {
                    write_log("ERROR:\n" + ex.ToString());
                }
                finally
                {
                    set_enable(true);
                }
            }
        }
        private void delete_region_instance()
        {
            using (
            AWSEC2Utils awsUtils = new AWSEC2Utils(setting_, write_log))
            {
                using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
                {
                    awsUtils.region = Helper.get_region(fs, region_combo_box.Text);
                }

                set_enable(false);
                try
                {
                    awsUtils.delete_region_instance();
                    write_log(awsUtils.region + " のインスタンスを削除しました。");
                }
                catch (Exception ex)
                {
                    write_log("ERROR:\n" + ex.ToString());
                }
                finally
                {
                    set_enable(true);
                }

            }
        }
        private bool execute_script(string ipaddress, string private_key, string instance_type, string engine, string eval)
        {
            bool success = false;
            string[] tags = new string[] { "nosse", "sse2", "sse41", "sse42", "avx2", "sse", "bmi2", "release", "all" };
            IPAddress addr = IPAddress.Parse(ipaddress);

            int ssh_connection_timeout_seconds = setting_.getValueInt("common", "ssh_connection_timeout_seconds");
            long timeout = DateTime.Now.Ticks / TimeSpan.TicksPerSecond + ssh_connection_timeout_seconds;
            for (int ii = 0; ii < 60 * 10; ii++)
            {
                if (timeout < (DateTime.Now.Ticks / TimeSpan.TicksPerSecond)) break;
                if ((ii % 20) == 1) write_log(ipaddress + "への接続を試行しています。");
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = socket.BeginConnect(addr, 22, null, null);
                success = result.AsyncWaitHandle.WaitOne(500, true);
                socket.Close();
                Helper.sleep(100);
                if (success) break;
            }
            if (!success)
            {
                write_log(" ERROR: connect timeout.");
                return false;
            }

            write_log(ipaddress + " でのスクリプト実行準備をしています。");
            Helper.sleep(8 * 1000);

            PrivateKeyFile privateKeyFile = new PrivateKeyFile(private_key);
            string user = setting_.getValueString("common", "ssh_user");
            ConnectionInfo connect_info = new ConnectionInfo(ipaddress, 22, user,
                 new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile[] { privateKeyFile }) });

            int nn = -1;
            switch (shutdown_combo_box.SelectedIndex)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    nn = checked((60 * shutdown_combo_box.SelectedIndex) - 5);
                    break;
                case 7:
                    nn = (60 * 12) - 5;
                    break;
                case 8:
                    nn = (60 * 24) - 5;
                    break;
            }
            end_time_str_ = "";
            if (nn != -1)
            {
                end_time_str_ = System.Xml.XmlConvert.ToString(DateTime.Now.AddMinutes(nn), System.Xml.XmlDateTimeSerializationMode.Local);
                Helper.sleep(3 * 1000);
                using (var client = new SshClient(connect_info))
                {
                    client.Connect();
                    using (ShellStream stream = client.CreateShellStream("usi_engine", 80, 24, 800, 600, 1024))
                    {
                        var reader = new StreamReader(stream);
                        var writer = new StreamWriter(stream);
                        writer.AutoFlush = true;
                        read_text_data(reader, stream, 3);
                        Helper.sleep(1000);
                        write_stream("nohup sh -c \"sleep " + nn + "m; sudo shutdown -h now\" > /dev/null 2>&1 &", writer, stream);
                        Helper.sleep(1000);
                    }
                }
            }

            Helper.sleep(3000);
            using (var scp = new ScpClient(connect_info))
            {
                string uploadfn = resouce_dir_ + "setup.tar.gz";
                scp.Connect();
                scp.Upload(new FileInfo(uploadfn), "/tmp");

                uploadfn = resouce_dir_ + engine + ".deb";
                if (File.Exists(uploadfn))
                {
                    scp.Upload(new FileInfo(uploadfn), "/tmp");
                }
                uploadfn = resouce_dir_ + engine + "-{0}.deb";
                foreach (string tag in tags)
                {
                    string tmp = string.Format(uploadfn, tag);
                    if (File.Exists(tmp))
                    {
                        scp.Upload(new FileInfo(tmp), "/tmp");
                    }
                }
                scp.Disconnect();
            }

            write_log(ipaddress + " にてスクリプトの実行を行います。");

            Helper.sleep(3000);
            using (var client = new SshClient(connect_info))
            {
                client.Connect();
                using (ShellStream stream = client.CreateShellStream("usi_engine", 80, 24, 800, 600, 1024))
                {
                    var reader = new StreamReader(stream);
                    var writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    read_text_data(reader, stream, 3);
                    Helper.sleep(1000);

                    write_stream("mkdir -p /tmp/.setup; cd /tmp/.setup; sudo tar zxvf /tmp/setup.tar.gz; " +
                        "sudo sh ./bootstrap.sh '" + instance_type + "' '" + engine + "' '" + eval + "';" +
                        "sudo rm -Rf /tmp/.setup /tmp/setup.tar.gz;" +
                        "echo '__EXIT__';", writer, stream);
                    if (read_text_data(reader, stream, 300) == false)
                    {
                        write_log(ipaddress + " からの応答異常を検知しました。");
                        return false;
                    }
                }
            }

            using (var scp = new ScpClient(connect_info))
            {
                string downloadfn = "";
                scp.Connect();
                downloadfn = resouce_dir_ + engine + ".deb";
                if (!File.Exists(downloadfn))
                {
                    try
                    {
                        scp.Download("/tmp/" + engine + ".deb", new FileInfo(downloadfn));
                        write_log(ipaddress + " から " + engine + ".deb" + " をダウンロードしました。");
                    }
                    catch
                    {
                        // none.
                    }
                }

                downloadfn = engine + "-{0}.deb";
                foreach (string tag in tags)
                {
                    string tmp = string.Format(downloadfn, tag);
                    if (!File.Exists(resouce_dir_ + tmp))
                    {
                        try
                        {
                            scp.Download("/tmp/" + tmp, new FileInfo(resouce_dir_ + tmp));
                            write_log(ipaddress + " から " + tmp + " をダウンロードしました。");
                        }
                        catch
                        {
                            // none.
                        }
                    }
                }
                scp.Disconnect();
            }

            return true;
        }
        private bool read_text_data(StreamReader sr, ShellStream stream, int timeout)
        {
            bool rtn = true;
            while (stream.Length == 0)
            {
                Thread.Sleep(64);
            }
            long nn = 0;
            while (true)
            {
                Application.DoEvents();
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
                if (str.Trim().IndexOf("ERROR:") != -1)
                {
                    write_log("***** " + str);
                    rtn = false;
                    break;
                }
                if (str.Trim().Equals("__EXIT__")) break;
                write_log(str);
                nn = 0;
            }
            return rtn;
        }
        private void write_stream(string cmd, StreamWriter writer, ShellStream stream)
        {
            writer.Write(cmd.TrimEnd());
            writer.Write("\n");
            writer.Flush();
            while (stream.Length == 0)
            {
                Thread.Sleep(64);
            }
        }
        private void set_instance_name()
        {
            string region = "";
            using (FileStream fs = new FileStream(resouce_dir_ + "region.txt", FileMode.Open, FileAccess.Read))
            {
                region = Helper.get_region(fs, region_combo_box.Text);
            }
            string availability_zone = availability_zone_combo_box.Text.ToLower();
            string instance_type = instance_combo_box.Text;
            string engine = Helper.get_default_str(setting_, "engine");
            using (FileStream fs = new FileStream(resouce_dir_ + "install_script.txt", FileMode.Open, FileAccess.Read))
            {
                engine = Helper.get_install_script(fs, install_script_combo_box.Text, 1);
            }
            instance_text_box.Text = string.Format("{0}-{1}{2}-{3}", engine, region, availability_zone, instance_type);
        }
        private void menu_close()
        {
            file_menu_item.DropDown.Close();
            tool_strip_menu_item.DropDown.Close();
        }
        private void write_log(String text)
        {
            if (text == null || text.Trim().Length == 0) return;
            System.Windows.Forms.Application.DoEvents();
            if (this.log_text_box.InvokeRequired)
            {
                set_text_callback_ d = new set_text_callback_(write_log);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                log_text_box.SelectionStart = log_text_box.Text.Length;
                log_text_box.SelectionLength = 0;
                log_text_box.SelectedText = "\r\n[" + System.DateTime.Now.ToString() + "] " + text.Trim();
                this.Text = text.Trim() + " - USIEngineOnAWS";
            }
        }
        private void error(string str)
        {
            write_log("ERROR: " + str);
            MessageBox.Show(str, "エラー");
        }
        #endregion }}}
    }
}

