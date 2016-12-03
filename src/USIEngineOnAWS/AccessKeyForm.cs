using System;
using System.Windows.Forms;

namespace USIEngineOnAWS
{
    public partial class AccessKeyForm : Form
    {
        private string resouce_dir = Globals.resouce_dir;
        private InifileUtils setting_;

        public AccessKeyForm()
        {
            InitializeComponent();
            setting_ = new InifileUtils(resouce_dir + "setting.ini");
            this.textBox1.Text = Globals.aws_access_key_id;
            this.textBox2.Text = Globals.aws_secret_access_key;
            this.checkBox1.Checked = (this.textBox1.Text.Length != 0 && this.textBox2.Text.Length != 0);
        }
        private void AccessKeyForm_Load(object sender, EventArgs e)
        {
            this.Icon = new System.Drawing.Icon(Globals.resouce_dir + "\\icon\\onaws.ico");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Globals.aws_access_key_id = this.textBox1.Text;
            Globals.aws_secret_access_key = this.textBox2.Text;
            if (this.checkBox1.Checked)
            {
                setting_.setValue("common", "aws_access_key_id", Helper.encrypt(Globals.aws_access_key_id));
                setting_.setValue("common", "aws_secret_access_key", Helper.encrypt(Globals.aws_secret_access_key));
            }
            else
            {
                setting_.setValue("common", "aws_access_key_id", "");
                setting_.setValue("common", "aws_secret_access_key", "");
            }
            this.Close();
        }

    }
}
