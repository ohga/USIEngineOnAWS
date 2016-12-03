using System.Windows.Forms;

namespace USIEngineOnAWS
{
    partial class USIEngineOnAWS
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(USIEngineOnAWS));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folder_browser_dialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.instance_combo_box = new System.Windows.Forms.ComboBox();
            this.price_of_instance_button = new System.Windows.Forms.Button();
            this.price_numeric_up_down = new System.Windows.Forms.NumericUpDown();
            this.region_combo_box = new System.Windows.Forms.ComboBox();
            this.availability_zone_combo_box = new System.Windows.Forms.ComboBox();
            this.install_script_combo_box = new System.Windows.Forms.ComboBox();
            this.shutdown_combo_box = new System.Windows.Forms.ComboBox();
            this.instance_text_box = new System.Windows.Forms.TextBox();
            this.create_instance_button = new System.Windows.Forms.Button();
            this.view_log_button = new System.Windows.Forms.Button();
            this.log_text_box = new System.Windows.Forms.TextBox();
            this.menu_strip = new System.Windows.Forms.MenuStrip();
            this.file_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.accesskey_form_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tool_strip_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.save_folder_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.spot_instance_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.aws_instance_stat_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.billing_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.delete_instance_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.create_vpc_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            this.delete_vpc_menu_item = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.price_numeric_up_down)).BeginInit();
            this.menu_strip.SuspendLayout();
            this.SuspendLayout();
            // 
            // folder_browser_dialog
            // 
            this.folder_browser_dialog.Description = "エンジンを保存するフォルダを選択してください";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(18, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "リージョン/アベイラビリティゾーン";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(83, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "インスタンスタイプ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(12, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "スポットインスタンス価格(米ドル)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(27, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "インスタンス削除の予約設定";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(70, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "インストールエンジン";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(97, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "インスタンス名";
            // 
            // instance_combo_box
            // 
            this.instance_combo_box.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.instance_combo_box.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.instance_combo_box.FormattingEnabled = true;
            this.instance_combo_box.Location = new System.Drawing.Point(173, 34);
            this.instance_combo_box.Name = "instance_combo_box";
            this.instance_combo_box.Size = new System.Drawing.Size(107, 20);
            this.instance_combo_box.TabIndex = 6;
            this.instance_combo_box.SelectedIndexChanged += new System.EventHandler(this.instance_combo_box_SelectedIndexChanged);
            // 
            // price_of_instance_button
            // 
            this.price_of_instance_button.Location = new System.Drawing.Point(296, 32);
            this.price_of_instance_button.Name = "price_of_instance_button";
            this.price_of_instance_button.Size = new System.Drawing.Size(122, 23);
            this.price_of_instance_button.TabIndex = 7;
            this.price_of_instance_button.Text = "現在の最安値を検索";
            this.price_of_instance_button.UseVisualStyleBackColor = true;
            this.price_of_instance_button.Click += new System.EventHandler(this.price_of_instance_button_Click);
            // 
            // price_numeric_up_down
            // 
            this.price_numeric_up_down.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.price_numeric_up_down.DecimalPlaces = 4;
            this.price_numeric_up_down.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.price_numeric_up_down.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.price_numeric_up_down.Location = new System.Drawing.Point(173, 69);
            this.price_numeric_up_down.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.price_numeric_up_down.Name = "price_numeric_up_down";
            this.price_numeric_up_down.Size = new System.Drawing.Size(84, 25);
            this.price_numeric_up_down.TabIndex = 9;
            this.price_numeric_up_down.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.price_numeric_up_down.ThousandsSeparator = true;
            this.price_numeric_up_down.Value = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            // 
            // region_combo_box
            // 
            this.region_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.region_combo_box.FormattingEnabled = true;
            this.region_combo_box.Items.AddRange(new object[] {
            "us-east-1,米国東部（バージニア北部）",
            "us-east-2,米国西部（オハイオ）",
            "us-west-1,米国西部（北カリフォルニア）",
            "us-west-2,米国西部（オレゴン）",
            "eu-west-1,欧州（アイルランド）",
            "eu-central-1,欧州（フランクフルト）",
            "ap-northeast-1,アジアパシフィック（東京）",
            "ap-northeast-2,アジアパシフィック (ソウル)",
            "ap-southeast-1,アジアパシフィック（シンガポール）",
            "ap-southeast-2,アジアパシフィック（シドニー）",
            "ap-south-1,アジアパシフィック (ムンバイ)",
            "sa-east-1,南米 (サンパウロ)"});
            this.region_combo_box.Location = new System.Drawing.Point(173, 113);
            this.region_combo_box.Name = "region_combo_box";
            this.region_combo_box.Size = new System.Drawing.Size(206, 20);
            this.region_combo_box.TabIndex = 10;
            this.region_combo_box.SelectedIndexChanged += new System.EventHandler(this.region_combo_box_SelectedIndexChanged);
            // 
            // availability_zone_combo_box
            // 
            this.availability_zone_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.availability_zone_combo_box.FormattingEnabled = true;
            this.availability_zone_combo_box.Location = new System.Drawing.Point(394, 113);
            this.availability_zone_combo_box.Name = "availability_zone_combo_box";
            this.availability_zone_combo_box.Size = new System.Drawing.Size(48, 20);
            this.availability_zone_combo_box.TabIndex = 11;
            this.availability_zone_combo_box.SelectedIndexChanged += new System.EventHandler(this.availability_zone_combo_box_SelectedIndexChanged);
            // 
            // install_script_combo_box
            // 
            this.install_script_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.install_script_combo_box.FormattingEnabled = true;
            this.install_script_combo_box.Location = new System.Drawing.Point(173, 150);
            this.install_script_combo_box.Name = "install_script_combo_box";
            this.install_script_combo_box.Size = new System.Drawing.Size(269, 20);
            this.install_script_combo_box.TabIndex = 12;
            this.install_script_combo_box.SelectedIndexChanged += new System.EventHandler(this.install_script_combo_box_SelectedIndexChanged);
            // 
            // shutdown_combo_box
            // 
            this.shutdown_combo_box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shutdown_combo_box.FormattingEnabled = true;
            this.shutdown_combo_box.Items.AddRange(new object[] {
            "自動削除しない",
            "１時間後",
            "２時間後",
            "３時間後",
            "４時間後",
            "５時間後",
            "６時間後",
            "１２時間後",
            "２４時間後"});
            this.shutdown_combo_box.Location = new System.Drawing.Point(173, 188);
            this.shutdown_combo_box.Name = "shutdown_combo_box";
            this.shutdown_combo_box.Size = new System.Drawing.Size(107, 20);
            this.shutdown_combo_box.TabIndex = 13;
            this.shutdown_combo_box.SelectedIndexChanged += new System.EventHandler(this.shutdown_combo_box_SelectedIndexChanged);
            // 
            // instance_text_box
            // 
            this.instance_text_box.Location = new System.Drawing.Point(173, 225);
            this.instance_text_box.Name = "instance_text_box";
            this.instance_text_box.Size = new System.Drawing.Size(269, 19);
            this.instance_text_box.TabIndex = 14;
            // 
            // create_instance_button
            // 
            this.create_instance_button.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.create_instance_button.Location = new System.Drawing.Point(173, 261);
            this.create_instance_button.Name = "create_instance_button";
            this.create_instance_button.Size = new System.Drawing.Size(136, 25);
            this.create_instance_button.TabIndex = 15;
            this.create_instance_button.Text = "インスタンス作成";
            this.create_instance_button.UseVisualStyleBackColor = true;
            this.create_instance_button.Click += new System.EventHandler(this.create_instance_button_Click);
            // 
            // view_log_button
            // 
            this.view_log_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.view_log_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.view_log_button.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.view_log_button.Location = new System.Drawing.Point(433, 274);
            this.view_log_button.Name = "view_log_button";
            this.view_log_button.Size = new System.Drawing.Size(20, 19);
            this.view_log_button.TabIndex = 16;
            this.view_log_button.Text = "v";
            this.view_log_button.UseVisualStyleBackColor = true;
            this.view_log_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // log_text_box
            // 
            this.log_text_box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log_text_box.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.log_text_box.Location = new System.Drawing.Point(6, 301);
            this.log_text_box.Multiline = true;
            this.log_text_box.Name = "log_text_box";
            this.log_text_box.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.log_text_box.Size = new System.Drawing.Size(453, 0);
            this.log_text_box.TabIndex = 17;
            // 
            // menu_strip
            // 
            this.menu_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_menu_item});
            this.menu_strip.Location = new System.Drawing.Point(0, 0);
            this.menu_strip.Name = "menu_strip";
            this.menu_strip.Size = new System.Drawing.Size(464, 24);
            this.menu_strip.TabIndex = 20;
            this.menu_strip.Text = "menu_strip";
            // 
            // file_menu_item
            // 
            this.file_menu_item.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accesskey_form_menu_item,
            this.toolStripSeparator3,
            this.tool_strip_menu_item,
            this.save_folder_menu_item,
            this.toolStripSeparator2,
            this.spot_instance_menu_item,
            this.aws_instance_stat_menu_item,
            this.billing_menu_item,
            this.toolStripSeparator1,
            this.delete_instance_menu_item,
            this.create_vpc_menu_item,
            this.delete_vpc_menu_item});
            this.file_menu_item.Name = "file_menu_item";
            this.file_menu_item.Size = new System.Drawing.Size(66, 20);
            this.file_menu_item.Text = "ファイル(&F)";
            // 
            // accesskey_form_menu_item
            // 
            this.accesskey_form_menu_item.Name = "accesskey_form_menu_item";
            this.accesskey_form_menu_item.Size = new System.Drawing.Size(237, 22);
            this.accesskey_form_menu_item.Text = "アクセスキーの設定";
            this.accesskey_form_menu_item.Click += new System.EventHandler(this.accesskey_form_menu_item_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(234, 6);
            // 
            // tool_strip_menu_item
            // 
            this.tool_strip_menu_item.Name = "tool_strip_menu_item";
            this.tool_strip_menu_item.Size = new System.Drawing.Size(237, 22);
            this.tool_strip_menu_item.Text = "検索対象リージョン";
            // 
            // save_folder_menu_item
            // 
            this.save_folder_menu_item.Name = "save_folder_menu_item";
            this.save_folder_menu_item.Size = new System.Drawing.Size(237, 22);
            this.save_folder_menu_item.Text = "保存先の設定";
            this.save_folder_menu_item.Click += new System.EventHandler(this.save_folder_menu_item_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(234, 6);
            // 
            // spot_instance_menu_item
            // 
            this.spot_instance_menu_item.Name = "spot_instance_menu_item";
            this.spot_instance_menu_item.Size = new System.Drawing.Size(237, 22);
            this.spot_instance_menu_item.Text = "AWS スポットインスタンスの値段 ...";
            this.spot_instance_menu_item.Click += new System.EventHandler(this.spot_instance_menu_item_Click);
            // 
            // aws_instance_stat_menu_item
            // 
            this.aws_instance_stat_menu_item.Name = "aws_instance_stat_menu_item";
            this.aws_instance_stat_menu_item.Size = new System.Drawing.Size(237, 22);
            this.aws_instance_stat_menu_item.Text = "AWS インスタンスの状態 ...";
            this.aws_instance_stat_menu_item.Click += new System.EventHandler(this.aws_instance_stat_menu_item_Click);
            // 
            // billing_menu_item
            // 
            this.billing_menu_item.Name = "billing_menu_item";
            this.billing_menu_item.Size = new System.Drawing.Size(237, 22);
            this.billing_menu_item.Text = "AWS 請求情報 ...";
            this.billing_menu_item.Click += new System.EventHandler(this.billing_menu_item_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(234, 6);
            // 
            // delete_instance_menu_item
            // 
            this.delete_instance_menu_item.Name = "delete_instance_menu_item";
            this.delete_instance_menu_item.Size = new System.Drawing.Size(237, 22);
            this.delete_instance_menu_item.Text = "すべてのインスタンスを削除";
            this.delete_instance_menu_item.Click += new System.EventHandler(this.delete_instance_menu_item_Click);
            // 
            // create_vpc_menu_item
            // 
            this.create_vpc_menu_item.Name = "create_vpc_menu_item";
            this.create_vpc_menu_item.Size = new System.Drawing.Size(237, 22);
            this.create_vpc_menu_item.Text = "VPCの作成";
            this.create_vpc_menu_item.Click += new System.EventHandler(this.create_vpc_menu_item_Click);
            // 
            // delete_vpc_menu_item
            // 
            this.delete_vpc_menu_item.Name = "delete_vpc_menu_item";
            this.delete_vpc_menu_item.Size = new System.Drawing.Size(237, 22);
            this.delete_vpc_menu_item.Text = "VPCの削除";
            this.delete_vpc_menu_item.Click += new System.EventHandler(this.delete_vpc_menu_item_Click);
            // 
            // USIEngineOnAWS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 301);
            this.Controls.Add(this.view_log_button);
            this.Controls.Add(this.instance_text_box);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.price_of_instance_button);
            this.Controls.Add(this.instance_combo_box);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.availability_zone_combo_box);
            this.Controls.Add(this.price_numeric_up_down);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.region_combo_box);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.install_script_combo_box);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.shutdown_combo_box);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.create_instance_button);
            this.Controls.Add(this.log_text_box);
            this.Controls.Add(this.menu_strip);
            // this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu_strip;
            this.MinimumSize = new System.Drawing.Size(480, 340);
            this.Name = "USIEngineOnAWS";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "USIEngineOnAWS";
            this.Deactivate += new System.EventHandler(this.USIEngineOnAWS_Deactivate);
            this.Load += new System.EventHandler(this.USIEngineOnAWS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.price_numeric_up_down)).EndInit();
            this.menu_strip.ResumeLayout(false);
            this.menu_strip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ToolTip toolTip1;
        private FolderBrowserDialog folder_browser_dialog;
        private Label label1;
        private Label label4;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label3;
        private ComboBox instance_combo_box;
        private Button price_of_instance_button;
        private NumericUpDown price_numeric_up_down;
        private ComboBox region_combo_box;
        private ComboBox availability_zone_combo_box;
        private ComboBox install_script_combo_box;
        private ComboBox shutdown_combo_box;
        private TextBox instance_text_box;
        private Button create_instance_button;
        private Button view_log_button;
        private TextBox log_text_box;
        private MenuStrip menu_strip;
        private ToolStripMenuItem file_menu_item;
        private ToolStripMenuItem accesskey_form_menu_item;
        private ToolStripMenuItem tool_strip_menu_item;
        private ToolStripMenuItem save_folder_menu_item;
        private ToolStripMenuItem spot_instance_menu_item;
        private ToolStripMenuItem aws_instance_stat_menu_item;
        private ToolStripMenuItem billing_menu_item;
        private ToolStripMenuItem delete_instance_menu_item;
        private ToolStripMenuItem create_vpc_menu_item;
        private ToolStripMenuItem delete_vpc_menu_item;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
    }
}

