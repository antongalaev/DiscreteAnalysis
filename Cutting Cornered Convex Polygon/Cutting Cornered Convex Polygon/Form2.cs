using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cutting_Cornered_Convex_Polygon
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private void ParseIt()
        {
            try
            {
                int n;
                if (!int.TryParse(comboBox1.Text, out n) || n < 4 || n > 100) throw new Exception();
                Form1.NC = n;
                Form1.Arr = new Min[n, n];
                Close();
            }
            catch { MessageBox.Show("Введите целое число от 4 до 100.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); comboBox1.Focus(); return; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ParseIt();
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) ParseIt();
        }

    }
}
