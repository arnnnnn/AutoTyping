namespace AutoTyping
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTextBOx = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MainTextBOx
            // 
            this.MainTextBOx.Location = new System.Drawing.Point(22, 43);
            this.MainTextBOx.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MainTextBOx.Multiline = true;
            this.MainTextBOx.Name = "MainTextBOx";
            this.MainTextBOx.Size = new System.Drawing.Size(484, 478);
            this.MainTextBOx.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(545, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Page Up을 이용하여 Text Send 기능을 이용가능";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 533);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainTextBOx);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Auto Typing Software";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MainTextBOx;
        private System.Windows.Forms.Label label1;
    }
}

