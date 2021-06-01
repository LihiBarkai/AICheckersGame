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
            System.Windows.Forms.Button button_end;
            System.Windows.Forms.Button buttonUndo;
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTurn = new System.Windows.Forms.Label();
            this.lableEatBlack = new System.Windows.Forms.Label();
            button_end = new System.Windows.Forms.Button();
            buttonUndo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_end
            // 
            button_end.Location = new System.Drawing.Point(795, 210);
            button_end.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            button_end.Name = "button_end";
            button_end.Size = new System.Drawing.Size(154, 85);
            button_end.TabIndex = 3;
            button_end.Text = "End Game";
            button_end.UseVisualStyleBackColor = true;
            button_end.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // buttonUndo
            // 
            buttonUndo.Location = new System.Drawing.Point(795, 340);
            buttonUndo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonUndo.Name = "buttonUndo";
            buttonUndo.Size = new System.Drawing.Size(154, 87);
            buttonUndo.TabIndex = 4;
            buttonUndo.Text = "Undo";
            buttonUndo.UseVisualStyleBackColor = true;
            buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Damka.Properties.Resources.Damka;
            this.pictureBox1.Location = new System.Drawing.Point(14, 28);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(506, 562);
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
            this.labelTurn.Location = new System.Drawing.Point(788, 69);
            this.labelTurn.Name = "labelTurn";
            this.labelTurn.Size = new System.Drawing.Size(0, 37);
            this.labelTurn.TabIndex = 1;
            // 
            // lableEatBlack
            // 
            this.lableEatBlack.AutoSize = true;
            this.lableEatBlack.Location = new System.Drawing.Point(774, 245);
            this.lableEatBlack.Name = "lableEatBlack";
            this.lableEatBlack.Size = new System.Drawing.Size(0, 20);
            this.lableEatBlack.TabIndex = 2;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1191, 992);
            this.Controls.Add(buttonUndo);
            this.Controls.Add(button_end);
            this.Controls.Add(this.lableEatBlack);
            this.Controls.Add(this.labelTurn);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "GameForm";
            this.Text = "Checkers Game";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label labelTurn;
        private System.Windows.Forms.Label lableEatBlack;
    }
}

