using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Travelling_Salesman;

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
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j)
                        {
                            dataGridView1[j+1, i+1].Value = char.ConvertFromUtf32(8734);
                            continue;
                        }
                        dataGridView1[j+1, i+1].Value = Form1.Source.Mtr[i*n+j].Val;
                    }
            for (int i = 0; i < n; i++)
            {
                dataGridView1[i+1, i+1].Style.Font = new Font(dataGridView1.Font.FontFamily, 20);
                
                dataGridView1[i + 1, i + 1].Value = char.ConvertFromUtf32(8734);
                dataGridView1[i + 1, i + 1].ReadOnly = true;
                dataGridView1[i + 1, i + 1].Style.BackColor = Color.Khaki;                
            }
            dataGridView1[0, 0].Style.BackColor = Color.Red;
            dataGridView1[0, 0].Style.Font = new Font(dataGridView1.Font.FontFamily, 11, FontStyle.Bold);
            dataGridView1[0, 0].ReadOnly = true;
            dataGridView1[0, 0].Value = "M";
            dataGridView1.CurrentCell = dataGridView1[2, 1];
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            List<Cell> temp = new List<Cell>();
            try
            {
                int d;
                for (int i = 0; i < Form1.NC; i++)
                    for (int j = 0; j < Form1.NC; j++)
                    {
                        if (i == j)
                        {;
                            temp.Add(new Cell(i, j, -1));
                            continue;
                        }
                        if (!int.TryParse(dataGridView1[j+1, i+1].Value.ToString(), out d)) throw new Exception(string.Format("Введите целое положительное число в ячейку.\r\n Строка {0}. Столбец {1}.", i + 1, j + 1));
                        temp.Add(new Cell(i, j, d));
                    }
            }
            catch (NullReferenceException) { MessageBox.Show("Расстояния не заданы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            Form1.fill = true;
            Form1.Source.Mtr = temp.ToList();
            Close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int i;
                if (!int.TryParse(dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out i)&& e.ColumnIndex!=e.RowIndex) dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
            }
            catch { return; }
        }

        private void RandomButton2_Click(object sender, EventArgs e)
        {
            Random gen = new Random();
            for (int i = 0; i < Form1.NC; i++)
                for (int j = 0; j < Form1.NC; j++)
                {
                    if (i==j) continue;
                    else dataGridView1[j + 1, i + 1].Value = gen.Next(20);
                }
        }

    }
}
