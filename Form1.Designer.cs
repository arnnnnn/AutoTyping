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
            this.KeyStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MainTextBOx
            // 
            this.MainTextBOx.Location = new System.Drawing.Point(25, 54);
            this.MainTextBOx.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MainTextBOx.Multiline = true;
            this.MainTextBOx.Name = "MainTextBOx";
            this.MainTextBOx.Size = new System.Drawing.Size(553, 596);
            this.MainTextBOx.TabIndex = 0;
            // 
            // KeyStatus
            // 
            this.KeyStatus.AutoSize = true;
            this.KeyStatus.Location = new System.Drawing.Point(623, 54);
            this.KeyStatus.Name = "KeyStatus";
            this.KeyStatus.Size = new System.Drawing.Size(117, 15);
            this.KeyStatus.TabIndex = 2;
            this.KeyStatus.Text = "입력 모드 : 영어";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1282, 666);
            this.Controls.Add(this.KeyStatus);
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
        private System.Windows.Forms.Label KeyStatus;
    }
}

