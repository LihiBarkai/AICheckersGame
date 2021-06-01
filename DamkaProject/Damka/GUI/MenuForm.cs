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
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // player vs player
            Hide();
            (new GameForm(Board.PLAYER_VS_PLAYER)).ShowDialog();
            Show();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            // player vs computer

            Hide();
            (new GameForm(Board.PLAYER_VS_COMPUTER)).ShowDialog();
            Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

    }
}
