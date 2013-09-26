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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        public Form3(int n)
            : this()
        {
            dataGridView1.ColumnCount = dataGridView1.RowCount = n+1;
            for (int i = 1; i < n + 1; i++)
            {
                dataGridView1[0, i].Value = dataGridView1[i, 0].Value = i;
                dataGridView1[0, i].ReadOnly = dataGridView1[i, 0].ReadOnly = true;
                dataGridView1[0, i].Style.BackColor = dataGridView1[i, 0].Style.BackColor = Color.Yellow;
                dataGridView1[0, i].Style.Font = dataGridView1[i, 0].Style.Font = new Font(dataGridView1.Font.FontFamily, 12, FontStyle.Bold); ;
            }
            if (Form1.fill)
                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                    {
                        dataGridView1[j+1, i+1].Value = dataGridView1[i+1,j+1].Value = Form1.Arr[i, j].D;
                    }
            for (int i = 0; i < n; i++)
                for (int j = 0; j < i + 2; j++)
                {
                    if (j == n) break;
                    dataGridView1[j+1, i+1].ReadOnly = true;
                    dataGridView1[j+1, i+1].Style.BackColor = Color.Khaki;
                    if (Math.Abs(j-i)<2) dataGridView1[j+1, i+1].Value = 0;
                }            
            dataGridView1[0, 0].Style.BackColor = Color.Red;
            dataGridView1[0, 0].Style.Font = new Font(dataGridView1.Font.FontFamily, 11, FontStyle.Bold);
            dataGridView1[0, 0].ReadOnly = true;
            dataGridView1[0, 0].Value = "D";
            dataGridView1.CurrentCell = dataGridView1[3, 1];
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int d;
                for (int i = 0; i < Form1.NC; i++)
                    for (int j = i; j < Form1.NC; j++)
                    {
                        if (!int.TryParse(dataGridView1[j+1, i+1].Value.ToString(), out d)) throw new Exception(string.Format("Введите целое число в ячейку.\r\n Строка {0}. Столбец {1}.", i + 1, j + 1));
                        Form1.Arr[i, j] = new Min(i, j, d);
                        Form1.Arr[j, i] = new Min(j, i, d);
                    }
            }
            catch (NullReferenceException) { MessageBox.Show("Стоимость разрезов не задана", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Form1.fill = true;
            Close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int i;
                if (!int.TryParse(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out i)&& e.ColumnIndex!=0&& e.RowIndex!=0) dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
                dataGridView1[e.RowIndex, e.ColumnIndex].Value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            }
            catch { return; }
        }

        private void RandomButton2_Click(object sender, EventArgs e)
        {
            Random gen = new Random();
            for (int i = 0; i < Form1.NC; i++)
                for (int j = i; j < Form1.NC; j++)
                {
                    if (Math.Abs(j - i) < 2) dataGridView1[j + 1, i + 1].Value = 0;
                    else dataGridView1[j + 1, i + 1].Value = gen.Next(20);
                }
        }

    }
}
