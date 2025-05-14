
namespace Cheng.Windows.Forms
{
    partial class InputValueDialog
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
            this.col_textBox_input = new System.Windows.Forms.TextBox();
            this.col_button_ok = new System.Windows.Forms.Button();
            this.col_button_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // col_textBox_input
            // 
            this.col_textBox_input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.col_textBox_input.Location = new System.Drawing.Point(12, 12);
            this.col_textBox_input.MaxLength = 2048;
            this.col_textBox_input.Name = "col_textBox_input";
            this.col_textBox_input.Size = new System.Drawing.Size(558, 25);
            this.col_textBox_input.TabIndex = 0;
            // 
            // col_button_ok
            // 
            this.col_button_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.col_button_ok.Location = new System.Drawing.Point(142, 47);
            this.col_button_ok.Name = "col_button_ok";
            this.col_button_ok.Size = new System.Drawing.Size(100, 30);
            this.col_button_ok.TabIndex = 1;
            this.col_button_ok.Text = "OK";
            // 
            // col_button_cancel
            // 
            this.col_button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.col_button_cancel.Location = new System.Drawing.Point(325, 47);
            this.col_button_cancel.Name = "col_button_cancel";
            this.col_button_cancel.Size = new System.Drawing.Size(100, 30);
            this.col_button_cancel.TabIndex = 2;
            this.col_button_cancel.Text = "Cancel";
            // 
            // InputValueDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 88);
            this.Controls.Add(this.col_button_cancel);
            this.Controls.Add(this.col_button_ok);
            this.Controls.Add(this.col_textBox_input);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputValueDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputValueDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox col_textBox_input;
        private System.Windows.Forms.Button col_button_ok;
        private System.Windows.Forms.Button col_button_cancel;
    }
}