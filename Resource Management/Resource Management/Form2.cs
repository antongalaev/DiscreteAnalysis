using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Resource_Management;

namespace Cutting_Cornered_Convex_Polygon
{
    public partial class Form2 : Form
    {
        int I;
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(int i)
            : this()
        {
            I = i;
            if (i == 1)
            {
                this.Text = "Количество ресурса";
                groupBox1.Text = "Введите количество ресурса";
            }
            if (i == 2)
            {
                this.Text = "Количество проектов";
                groupBox1.Text = "Введите количество проектов";
            }
        }
        private void ParseIt()
        {
            try
            {
                int n;
                if (!int.TryParse(comboBox1.Text, out n) || n <= 0 || n > 50) throw new Exception();
                if (I == 1) Form1.C = n;
                if (I == 2) Form1.N = n;
                Close();
            }
            catch { MessageBox.Show("Введите целое число от 1 до 50.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); comboBox1.Focus(); return; }
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

