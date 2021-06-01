namespace Damka
{
    partial class GameForm
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
            System.Windows.Forms.Button button1;
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTurn = new System.Windows.Forms.Label();
            this.lableEatBlack = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Damka.Properties.Resources.Damka;
            this.pictureBox1.Location = new System.Drawing.Point(9, 18);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(337, 365);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // labelTurn
            // 
            this.labelTurn.AutoSize = true;
            this.labelTurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.labelTurn.Location = new System.Drawing.Point(525, 45);
            this.labelTurn.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTurn.Name = "labelTurn";
            this.labelTurn.Size = new System.Drawing.Size(0, 26);
            this.labelTurn.TabIndex = 1;
            // 
            // lableEatBlack
            // 
            this.lableEatBlack.AutoSize = true;
            this.lableEatBlack.Location = new System.Drawing.Point(516, 159);
            this.lableEatBlack.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lableEatBlack.Name = "lableEatBlack";
            this.lableEatBlack.Size = new System.Drawing.Size(0, 13);
            this.lableEatBlack.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(530, 204);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(78, 42);
            button1.TabIndex = 3;
            button1.Text = "End Game";
            button1.UseVisualStyleBackColor = true;
            button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(846, 467);
            this.Controls.Add(button1);
            this.Controls.Add(this.lableEatBlack);
            this.Controls.Add(this.labelTurn);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.Name = "GameForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label labelTurn;
        private System.Windows.Forms.Label lableEatBlack;
    }
}

