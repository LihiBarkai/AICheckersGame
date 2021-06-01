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
        public GameForm()
        {
            InitializeComponent();
            labelTurn.ForeColor = Color.Black;
            labelTurn.Text = "Black turn (Player 2)";
            board = new Board(this);
            pictureBox1.Size = new Size(450, 450);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            board.Paint(e.Graphics);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            board.click(e.Location);
            pictureBox1.Invalidate();
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.board.endGame();
            this.Refresh();
        }
    }
}
