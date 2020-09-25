namespace SQLDescriptionToPapper
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_ConnectionString = new System.Windows.Forms.ComboBox();
            this.lb_ConnectionName = new System.Windows.Forms.Label();
            this.lb_TableName = new System.Windows.Forms.Label();
            this.txt_TableName = new System.Windows.Forms.TextBox();
            this.grp_Search = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.cb_Format = new System.Windows.Forms.ComboBox();
            this.lb_Format = new System.Windows.Forms.Label();
            this.bt_Send = new System.Windows.Forms.Button();
            this.txt_File = new System.Windows.Forms.TextBox();
            this.bt_Select = new System.Windows.Forms.Button();
            this.lb_Message = new System.Windows.Forms.Label();
            this.grp_Search.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_ConnectionString
            // 
            this.cb_ConnectionString.FormattingEnabled = true;
            this.cb_ConnectionString.Location = new System.Drawing.Point(111, 33);
            this.cb_ConnectionString.Name = "cb_ConnectionString";
            this.cb_ConnectionString.Size = new System.Drawing.Size(121, 20);
            this.cb_ConnectionString.TabIndex = 0;
            // 
            // lb_ConnectionName
            // 
            this.lb_ConnectionName.AutoSize = true;
            this.lb_ConnectionName.Location = new System.Drawing.Point(28, 41);
            this.lb_ConnectionName.Name = "lb_ConnectionName";
            this.lb_ConnectionName.Size = new System.Drawing.Size(77, 12);
            this.lb_ConnectionName.TabIndex = 1;
            this.lb_ConnectionName.Text = "選擇連線字串";
            // 
            // lb_TableName
            // 
            this.lb_TableName.AutoSize = true;
            this.lb_TableName.Location = new System.Drawing.Point(30, 67);
            this.lb_TableName.Name = "lb_TableName";
            this.lb_TableName.Size = new System.Drawing.Size(65, 12);
            this.lb_TableName.TabIndex = 2;
            this.lb_TableName.Text = "資料表名稱";
            // 
            // txt_TableName
            // 
            this.txt_TableName.Location = new System.Drawing.Point(111, 59);
            this.txt_TableName.Name = "txt_TableName";
            this.txt_TableName.Size = new System.Drawing.Size(198, 22);
            this.txt_TableName.TabIndex = 3;
            // 
            // grp_Search
            // 
            this.grp_Search.Controls.Add(this.radioButton3);
            this.grp_Search.Controls.Add(this.radioButton2);
            this.grp_Search.Controls.Add(this.radioButton1);
            this.grp_Search.Location = new System.Drawing.Point(32, 101);
            this.grp_Search.Name = "grp_Search";
            this.grp_Search.Size = new System.Drawing.Size(279, 103);
            this.grp_Search.TabIndex = 4;
            this.grp_Search.TabStop = false;
            this.grp_Search.Text = "搜尋條件";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(7, 67);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(83, 16);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Tag = "All";
            this.radioButton3.Text = "全部資料表";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(7, 44);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(83, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Tag = "Like";
            this.radioButton2.Text = "相似資料表";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(83, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "Single";
            this.radioButton1.Text = "單一資料表";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // cb_Format
            // 
            this.cb_Format.FormattingEnabled = true;
            this.cb_Format.Location = new System.Drawing.Point(111, 210);
            this.cb_Format.Name = "cb_Format";
            this.cb_Format.Size = new System.Drawing.Size(121, 20);
            this.cb_Format.TabIndex = 5;
            // 
            // lb_Format
            // 
            this.lb_Format.AutoSize = true;
            this.lb_Format.Location = new System.Drawing.Point(38, 217);
            this.lb_Format.Name = "lb_Format";
            this.lb_Format.Size = new System.Drawing.Size(53, 12);
            this.lb_Format.TabIndex = 6;
            this.lb_Format.Text = "下載格式";
            // 
            // bt_Send
            // 
            this.bt_Send.Location = new System.Drawing.Point(317, 293);
            this.bt_Send.Name = "bt_Send";
            this.bt_Send.Size = new System.Drawing.Size(75, 23);
            this.bt_Send.TabIndex = 7;
            this.bt_Send.Text = "產生字典";
            this.bt_Send.UseVisualStyleBackColor = true;
            this.bt_Send.Click += new System.EventHandler(this.bt_Send_Click);
            // 
            // txt_File
            // 
            this.txt_File.Location = new System.Drawing.Point(38, 255);
            this.txt_File.Name = "txt_File";
            this.txt_File.Size = new System.Drawing.Size(273, 22);
            this.txt_File.TabIndex = 8;
            // 
            // bt_Select
            // 
            this.bt_Select.Location = new System.Drawing.Point(317, 254);
            this.bt_Select.Name = "bt_Select";
            this.bt_Select.Size = new System.Drawing.Size(75, 23);
            this.bt_Select.TabIndex = 9;
            this.bt_Select.Text = "選擇路徑";
            this.bt_Select.UseVisualStyleBackColor = true;
            this.bt_Select.Click += new System.EventHandler(this.bt_Select_Click);
            // 
            // lb_Message
            // 
            this.lb_Message.AutoSize = true;
            this.lb_Message.Location = new System.Drawing.Point(38, 338);
            this.lb_Message.Name = "lb_Message";
            this.lb_Message.Size = new System.Drawing.Size(0, 12);
            this.lb_Message.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 359);
            this.Controls.Add(this.lb_Message);
            this.Controls.Add(this.bt_Select);
            this.Controls.Add(this.txt_File);
            this.Controls.Add(this.bt_Send);
            this.Controls.Add(this.lb_Format);
            this.Controls.Add(this.cb_Format);
            this.Controls.Add(this.grp_Search);
            this.Controls.Add(this.txt_TableName);
            this.Controls.Add(this.lb_TableName);
            this.Controls.Add(this.lb_ConnectionName);
            this.Controls.Add(this.cb_ConnectionString);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grp_Search.ResumeLayout(false);
            this.grp_Search.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_ConnectionString;
        private System.Windows.Forms.Label lb_ConnectionName;
        private System.Windows.Forms.Label lb_TableName;
        private System.Windows.Forms.TextBox txt_TableName;
        private System.Windows.Forms.GroupBox grp_Search;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.ComboBox cb_Format;
        private System.Windows.Forms.Label lb_Format;
        private System.Windows.Forms.Button bt_Send;
        private System.Windows.Forms.TextBox txt_File;
        private System.Windows.Forms.Button bt_Select;
        private System.Windows.Forms.Label lb_Message;

    }
}

