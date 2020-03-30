namespace ChinaBank_FIX
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCBC = new System.Windows.Forms.Button();
            this.btmCBS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCBC
            // 
            this.btnCBC.BackColor = System.Drawing.Color.White;
            this.btnCBC.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCBC.Location = new System.Drawing.Point(27, 13);
            this.btnCBC.Name = "btnCBC";
            this.btnCBC.Size = new System.Drawing.Size(175, 55);
            this.btnCBC.TabIndex = 0;
            this.btnCBC.Text = "CBC";
            this.btnCBC.UseVisualStyleBackColor = false;
            // 
            // btmCBS
            // 
            this.btmCBS.BackColor = System.Drawing.Color.White;
            this.btmCBS.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btmCBS.Location = new System.Drawing.Point(28, 81);
            this.btmCBS.Name = "btmCBS";
            this.btmCBS.Size = new System.Drawing.Size(175, 55);
            this.btmCBS.TabIndex = 1;
            this.btmCBS.Text = "CBS";
            this.btmCBS.UseVisualStyleBackColor = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(231, 153);
            this.Controls.Add(this.btmCBS);
            this.Controls.Add(this.btnCBC);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCBC;
        private System.Windows.Forms.Button btmCBS;
    }
}

