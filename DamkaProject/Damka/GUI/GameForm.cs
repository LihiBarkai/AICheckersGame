using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damka
{
    public partial class GameForm : Form
    {
        Board board;
        int mode = 0;
        public GameForm(int mode)
        {
            InitializeComponent();
            labelTurn.ForeColor = Color.Black;
            labelTurn.Text = "Black turn";
            board = new Board(this, mode);
            pictureBox1.Size = new Size(450, 450);
            this.mode = mode;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            board.Paint(e.Graphics);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            board.clickOnScreen(e.Location);
            pictureBox1.Invalidate();
            this.Refresh();
 
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            board.checkWinner(true);
            this.Refresh();
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            this.board.Undo();
            this.Refresh();
        }
    }
}
