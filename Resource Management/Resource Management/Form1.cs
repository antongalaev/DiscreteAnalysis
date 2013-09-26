using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Cutting_Cornered_Convex_Polygon;

namespace Resource_Management
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StatusLabel.Text = "Укажите количество ресурса и проектов";
            StatusLabel2.Text = "";
            StatusLabel2.Alignment = ToolStripItemAlignment.Right;
        }
        public static int C { get; set; }
        public static int N { get; set; }
        double[][] F, B;
        int[][] K;
        string answer;
        void SetNumbers(int i)
        {
            Form2 f = new Form2(i);
            f.ShowDialog();
            f.Dispose();
            if (C == 0 || N == 0)
            {
                CalculateButton.Enabled = false;
                CalculateMenuItem.Enabled = false;
                RandomButton.Enabled = false;
                CalculateCodeMenuItem.Enabled = false;
                return;
            }
            else
            {
                CalculateButton.Enabled = true;
                CalculateMenuItem.Enabled = true;
                RandomButton.Enabled = true;
                CalculateCodeMenuItem.Enabled = true;
                SaveButton.Enabled = false;
                SaveMenuItem.Enabled = false;
                SaveTxtMenuItem.Enabled = false;
                SaveTEXMenuItem.Enabled = false;
                SetMatrix(dataGridView1, "F \\ C");
                StatusLabel.Text = "Задайте функции проектов и нажмите кнопку \"Вычислить\"";
                StatusLabel2.Text = "";
                F = new double[N][];
                B = new double[N][];
                K = new int[N][];
            }
        }
        void SetMatrix(DataGridView d, string name)
        {
            d.ColumnCount = C + 2;
            d.RowCount = N + 1;
            for (int i = 1; i < N + 1; i++)
            {
                d[0, i].Value = i;
                d[0, i].ReadOnly = true;
                d[0, i].Style.BackColor = Color.Yellow;
                d[0, i].Style.Font = new Font(d.Font.FontFamily, 12, FontStyle.Bold);
            }
            for (int i = 1; i < C + 2; i++)
            {
                d[i, 0].Value = i - 1;
                d[i, 0].ReadOnly = true;
                d[i, 0].Style.BackColor = Color.Yellow;
                d[i, 0].Style.Font = new Font(d.Font.FontFamily, 12, FontStyle.Bold);
            }
            d[0, 0].Style.BackColor = Color.Red;
            d[0, 0].Style.Font = new Font(d.Font.FontFamily, 11, FontStyle.Bold);
            d[0, 0].ReadOnly = true;
            d[0, 0].Value = name;
            d.CurrentCell = d[1, 1];
        }
        void Calculate(int ch)
        {
            if (ch == 0)
            {
                SetMatrix(dataGridView1, "F \\ C");
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j <= C; j++)
                    {
                        dataGridView1[j + 1, i + 1].Value = F[i][j].ToString("F2");
                    }
                }
            }
            if (ch == 1)
            {
                try
                {
                    for (int i = 0; i < N; i++)
                    {
                        F[i] = new double[C + 1];
                        for (int j = 0; j <= C; j++)
                        {
                            F[i][j] = func(i, j);
                            dataGridView1[j + 1, i + 1].Value = F[i][j].ToString("F2");
                        }
                    }
                }
                catch { MessageBox.Show("Задание функций из кода невозможно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
            if (ch == 2)
            {
                try
                {
                    double v;
                    for (int i = 0; i < N; i++)
                    {
                        F[i] = new double[C + 1];
                        for (int j = 0; j <= C; j++)
                        {
                            if (!double.TryParse(dataGridView1[j + 1, i + 1].Value.ToString(), out v))
                            {
                                dataGridView1.CurrentCell = dataGridView1[j + 1, i + 1];
                                throw new Exception(string.Format("Введите число в ячейку.\r\n Строка {0}. Столбец {1}.", i + 1, j));
                            }
                            F[i][j] = v;
                        }
                    }
                }
                catch (NullReferenceException) { MessageBox.Show("Значения не заданы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
            for (int i = 0; i < N; i++)
            {
                B[i] = new double[C + 1];
                K[i] = new int[C + 1];
            }
            for (int i = 0; i <= C; i++)
            {
                B[0][i] = F[0][i];
                K[0][i] = i;
            }
            for (int i = 1; i < N; i++)
            {
                for (int j = 0; j <= C; j++)
                {
                    double max = 0;
                    int k = 0;
                    for (int z = 0; z <= j; z++)
                    {
                        double t = B[i - 1][j - z] + F[i][z];
                        if (t > max)
                        {
                            max = t;
                            k = z;
                        }
                    }
                    B[i][j] = max;
                    K[i][j] = k;
                }
            }
            answer = "Максимальная прибыль: " + B[N - 1][C] + "\r\n\r\n";
            StatusLabel2.Text = "Max = " + B[N - 1][C];
            string ans2 = "";
            int index = C, n = N;
            do
            {
                answer += n + " проект - " + K[n - 1][index] + " ед. ресурса\r\n";
                ans2 += n + " пр - " + K[n - 1][index] + " рес. , ";
                index -= K[n - 1][index];
            } while (--n > 0);
            StatusLabel.Text = ans2;
            SetMatrix(dataGridView2, "B");
            SetMatrix(dataGridView3, "R");
            for (int i = 0; i < N; i++)
                for (int j = 0; j <= C; j++)
                {
                    dataGridView2[j + 1, i + 1].Value = B[i][j];
                    dataGridView3[j + 1, i + 1].Value = K[i][j];
                }
            SaveButton.Enabled = true;
            SaveMenuItem.Enabled = true;
            SaveTxtMenuItem.Enabled = true;
            SaveTEXMenuItem.Enabled = true;
            MessageBox.Show(answer, "Ответ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void SaveFile(int i)
        {
            if (i == 1)
            {
                saveFileDialog1.Filter = "Проект (*.rsp) |*.rsp";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, F);
                    fs.Close();
                }
            }
            if (i == 2)
            {
                saveFileDialog1.Filter = "Текстовый файл (*.txt) |*.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter wr = new StreamWriter(saveFileDialog1.FileName);
                    wr.WriteLine("Функции проектов:");
                    wr.Write("F");
                    for (int g = 0; g <= C; g++)
                        wr.Write("\t" + g);
                    for (int a = 0; a < N; a++)
                    {
                        wr.Write("\r\n");
                        wr.Write(a + 1);
                        for (int b = 0; b <= C; b++)
                            wr.Write("\t" + F[a][b]);
                    }
                    wr.Write("\r\n");
                    wr.Write(answer);
                    wr.Flush();
                    wr.Close();
                }
            }
            if (i == 3)
            {
                saveFileDialog1.Filter = "Текстовый файл для TEX (*.txt) |*.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter wr = new StreamWriter(saveFileDialog1.FileName);

                    wr.WriteLine(@"Функции проектов: \\");
                    wr.WriteLine(@"\begin{gather*}");
                    wr.Write(@"\begin{tabular}{|");
                    for (int g = 0; g <= C+1; g++) wr.Write("c|");
                    wr.WriteLine(@"}\hline");
                    wr.Write(@"\notag F");
                    for (int g = 0; g <= C; g++)
                        wr.Write("&" + g);
                    wr.WriteLine(@"\\");
                    wr.WriteLine(@"\hline");
                    for (int a = 0; a < N; a++)
                    {
                        wr.Write(@"\notag " );
                        wr.Write(a + 1);
                        for (int b = 0; b <= C; b++)
                            wr.Write("&" + F[a][b].ToString("F2"));
                        wr.WriteLine(@"\\");
                        wr.WriteLine(@"\hline");
                    }
                    wr.WriteLine(@"\end{tabular}");
                    wr.WriteLine(@"\end{gather}");

                    wr.WriteLine(@"Максимальные значения прибыли при оптимальном использовании ресурсов: \\");
                    wr.WriteLine(@"\begin{gather*}");
                    wr.WriteLine(@"\begin{tabular}{|");
                    for (int g = 0; g <= C + 1; g++) wr.Write("c|");
                    wr.WriteLine(@"}\hline");
                    wr.Write(@"\notag B");
                    for (int g = 0; g <= C; g++)
                        wr.Write("&" + g);
                    wr.WriteLine(@"\\");
                    wr.WriteLine(@"\hline");
                    for (int a = 0; a < N; a++)
                    {
                        wr.Write(@"\notag ");
                        wr.Write(a + 1);
                        for (int b = 0; b <= C; b++)
                            wr.Write("&" + B[a][b].ToString("F2")+"("+K[a][b]+")");
                        wr.WriteLine(@"\\");
                        wr.WriteLine(@"\hline");
                    }
                    wr.WriteLine(@"\end{tabular}");
                    wr.WriteLine(@"\end{gather}");

                    wr.Write(answer);
                    wr.Flush();
                    wr.Close();
                }
            }
        }
        private void OpenFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                F = (double[][])bf.Deserialize(fs);
                fs.Close();
                N = F.Length;
                B = new double[N][];
                K = new int[N][];
                C = F[0].Length - 1;
                Calculate(0);
                CalculateButton.Enabled = true;
                CalculateMenuItem.Enabled = true;
                RandomButton.Enabled = true;
                CalculateCodeMenuItem.Enabled = true;

            }
        }
        double func(int n, double x)
        {
            double v;
            switch (n)
            {
                case 0:
                    v = 2 * x * x + 3; break;
                case 1:
                    v = x * x * x / 32; break;
                case 2:
                    v = x * x - 5; break;
                case 3:
                    v = x > 2 ? (int)(15 - Math.Sqrt(x - 3)) : 0; break;
                case 4:
                    v = x * x / 4; break;
                default:
                    v = 0; break;
            }
            return v;
        }
        private void RandomButton_Click(object sender, EventArgs e)
        {
            Random gen = new Random();
            for (int i = 0; i < N; i++)
            {
                int[] temp = new int[C + 1];
                for (int j = 0; j <= C; j++)
                {
                    temp[j] = gen.Next(20);
                }
                Array.Sort(temp);
                for (int j = 0; j <= C; j++)
                    dataGridView1[j + 1, i + 1].Value = temp[j];
            }
        }
        private void ResourceButton_Click(object sender, EventArgs e)
        {
            SetNumbers(1);
        }

        private void ProjectButton_Click(object sender, EventArgs e)
        {
            SetNumbers(2);
        }
        private void ResourceMenuItem_Click(object sender, EventArgs e)
        {
            SetNumbers(1);
        }

        private void ProjectMenuItem_Click(object sender, EventArgs e)
        {
            SetNumbers(2);
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            Calculate(2);
        }
        private void CalculateMenuItem_Click(object sender, EventArgs e)
        {
            Calculate(2);
        }

        private void CalculateCodeMenuItem_Click(object sender, EventArgs e)
        {
            Calculate(1);
        }

        private void HelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для работы с программой:\n 1. Задать количество ресурса и проектов.\n 2. Задать функции таблично или использовать заданные в коде программы.\n 3. Вычислить оптимальное распределение ресурса, нажав на кнопку.\nДругие функции:\n 1. Сохранение проекта программы с заданными функциями.\n 2. Прочтение таблиц функций из файлов проектов программы.\n 3. Сохранение таблиц и ответа в текстовый файл.\n 4. Сохранение таблиц и ответа в текстовый файл для последующей записи в TEX", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа распределения ресурса\n\nАвтор:\nГалаев Антон Олегович,\nстудент 1 курса НИУ ВШЭ\n\nМосква, 2012г.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(1);
        }

        private void SaveTxtMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(2);
        }

        private void SaveTEXMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(3);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFile(1);
        }


    }
}
