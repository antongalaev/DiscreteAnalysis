using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cutting_Cornered_Convex_Polygon
{
    public partial class Form1 : Form
    {
        public static int NC {get;set;}        
        public static Min[,] Arr {get;set;}
        public static bool fill { get; set; }
        Bitmap image;
        Point[] points;

        public Form1()
        {
            InitializeComponent();
            InitialSettings();
            image = new Bitmap(1600,1400);
            pictureBox1.Image = image;
            fill = false;
        }        
        private void InitialSettings()
        {
            CostMenuItem.Enabled = false;
            CostButton.Enabled = false;
            CalculateMenuItem.Enabled = false;            
            CalculateButton.Enabled = false;
            SaveButton.Enabled = false;
            SaveMenuItem.Enabled = false;
            SaveTxtMenuItem.Enabled = false;
            SaveImageMenuItem.Enabled = false;
            StatusLabel.Text = "Укажите количество вершин многоугольника";            
        }
        private void DrawPolygon(int n)
        {              
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White); 
            Pen p = new Pen(Color.Black, 3);            
            points = new Point[n];
            Point center = new Point(image.Width / 2, image.Height / 2);
            int rad = (int)(image.Height / 2.5);
            double alpha = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                points[i] = new Point((int)(center.X + rad * Math.Sin(alpha * i)),
                    (int)(center.Y + rad * Math.Cos(alpha * i)));
                g.FillEllipse(Brushes.Red, points[i].X - 10, points[i].Y - 10, 20, 20);
                if (i < n / 4)
                    g.DrawString((i + 1).ToString(), new Font(Font.FontFamily, 45), Brushes.Blue, points[i].X + 20, points[i].Y + 20);
                else if (i < n / 2)
                    g.DrawString((i + 1).ToString(), new Font(Font.FontFamily, 45), Brushes.Blue, points[i].X + 40, points[i].Y - 40);
                else if (i < 4 * n / 5)
                    g.DrawString((i + 1).ToString(), new Font(Font.FontFamily, 45), Brushes.Blue, points[i].X - 70, points[i].Y - 70);
                else
                    g.DrawString((i + 1).ToString(), new Font(Font.FontFamily, 45), Brushes.Blue, points[i].X - 40, points[i].Y + 40);

            }
            g.DrawPolygon(p, points);
            pictureBox1.Refresh();
            g.Dispose();
            p.Dispose();
        }
        private void Corners()
        {
            Form2 f = new Form2();
            f.ShowDialog();
            f.Dispose();
            if (NC == 0) return;
            CostMenuItem.Enabled = true;
            CostButton.Enabled = true;
            SaveImageMenuItem.Enabled = true;
            CalculateMenuItem.Enabled = false;
            CalculateButton.Enabled = false;
            SaveButton.Enabled = false;
            SaveTxtMenuItem.Enabled = false;
            SaveMenuItem.Enabled = false;
            fill = false;
            StatusLabel.Text = "Укажите стоимости разрезов многоугольника";   
            DrawPolygon(NC);
        }
        private void Cost()
        {
            Form3 f = new Form3(NC);
            f.ShowDialog();
            f.Dispose();
            if (!fill) return;            
            CalculateMenuItem.Enabled = true;
            CalculateButton.Enabled = true;
            SaveButton.Enabled = false;
            SaveTxtMenuItem.Enabled = false;
            SaveMenuItem.Enabled = false;
            StatusLabel.Text = "Нажмите кнопку 'Вычислить' ";
        }
        private void Calculate()
        {
            for (int dif = 3; dif < NC; dif++)
                for (int i = 0; i < NC - dif; i++)
                {
                    int min = Arr[i, i + 1].ND + Arr[i + 1, i + dif].ND;
                    int way = 1;
                    for (int k = i + 2; k < i + dif; k++)
                    {
                        int temp = Arr[i, k].ND + Arr[k, i + dif].ND;
                        if (temp < min)
                        {
                            min = temp;
                            way = k - i;
                        }
                    }
                    Arr[i, i + dif].N = min;
                    Arr[i, i + dif].Way1 = Arr[i, i + way];
                    Arr[i, i + dif].Way2 = Arr[i + way, i + dif];
                }
            StatusLabel.Text = "Оптимальная стоимость раскроя: " + Arr[0, NC - 1].N;
            SaveButton.Enabled = true;
            SaveMenuItem.Enabled = true;
            SaveTxtMenuItem.Enabled = true;
            DrawPolygon(NC);
            ShowWay(Arr[0, NC - 1]);
        }
        private void ShowWay(Min c)
        {
            if (c.Way1 == null) return;
            Graphics g = Graphics.FromImage(image);            
            Pen p = new Pen(Color.Black, 3);
            if (c.Way1.From + 1 == c.Way1.To)
                g.DrawLine(p, points[c.Way2.From], points[c.Way2.To]);
            else if (c.Way2.From + 1 == c.Way2.To)
                g.DrawLine(p, points[c.Way1.From], points[c.Way1.To]);
            else
            {
                g.DrawLine(p, points[c.Way1.From], points[c.Way1.To]);
                g.DrawLine(p, points[c.Way2.From], points[c.Way2.To]);
            }
            pictureBox1.Refresh();
            g.Dispose();
            p.Dispose();
            ShowWay(c.Way1);
            ShowWay(c.Way2);
        }
        static void ShowWayToTxt(Min c, StreamWriter w)
        {
            if (c.Way1 == null) return;
            w.WriteLine(c.ToString());
            ShowWayToTxt(c.Way1,w);
            ShowWayToTxt(c.Way2,w);
        }
        private void OpenFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                Arr = (Min[,])bf.Deserialize(fs);
                NC = Arr.GetLength(0);
                DrawPolygon(NC);
                StatusLabel.Text = "Оптимальная стоимость раскроя: " + Arr[0, NC - 1].N;
                ShowWay(Arr[0, NC - 1]);
                fs.Close();
                CostButton.Enabled = true;
                CostMenuItem.Enabled = true;
                SaveImageMenuItem.Enabled = true;
                CalculateButton.Enabled = true;
                CalculateMenuItem.Enabled = true;
                SaveMenuItem.Enabled = true;
                SaveButton.Enabled = true;
                SaveTxtMenuItem.Enabled = true;
            }
        }
        private void SaveFile(int i)
        {
            if (i == 1)
            {
                saveFileDialog1.Filter = "Многоугольник (*.cpt) |*.cpt";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Arr);
                    fs.Close();
                }
            }
            if (i==2)
            {
                saveFileDialog1.Filter = "Изображение (*.jpg) |*.jpg";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
            if (i == 3)
            {
                saveFileDialog1.Filter = "Текстовый файл (*.txt) |*.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter wr = new StreamWriter(saveFileDialog1.FileName);
                    wr.WriteLine("Матрица D со стоимостями разрезов от одной точки до другой");
                    wr.Write("D");
                    for (int g = 0; g < NC; g++)
                        wr.Write("\t" + (g + 1).ToString());
                    for (int a = 0; a < NC; a++)
                    {
                        wr.Write("\r\n");
                        wr.Write(a + 1);
                        for (int b = 0; b < NC; b++)
                        {
                            wr.Write("\t" + Arr[a,b].D);
                        }                        
                    }
                    wr.Write("\r\n");
                    wr.WriteLine("Матрица N со стоимостями разрезов многоугольников заключенных между точками");
                    wr.Write("N");
                    for (int g = 0; g < NC; g++)
                        wr.Write("\t" + (g + 1).ToString());
                    for (int a = 0; a < NC; a++)
                    {
                        wr.Write("\r\n");
                        wr.Write(a + 1);
                        for (int b = 0; b < NC; b++)
                        {
                            if (b < a) Arr[a, b] = Arr[b, a];
                            wr.Write("\t" + Arr[a,b].N);
                        }
                    }
                    wr.Write("\r\n");
                    wr.WriteLine("Оптимальная стоимость разреза:" + Arr[0, NC - 1].N);
                    ShowWayToTxt(Arr[0, NC - 1],wr);
                    wr.Flush();
                    wr.Close();
                }
            }
        }        
        private void CornersMenuItem_Click(object sender, EventArgs e)
        {
            Corners();
        }
        private void CornersButton_Click(object sender, EventArgs e)
        {
            Corners();
        }
        private void CostMenuItem_Click(object sender, EventArgs e)
        {
            Cost();
        }
        private void CostButton_Click(object sender, EventArgs e)
        {
            Cost();
        }
        private void CalculateMenuItem_Click(object sender, EventArgs e)
        {
            Calculate();
        }      
        private void CalculateButton_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFile(1);
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(1);
        }

        private void SaveImageMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(2);
        }

        private void SaveTxtMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(3);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа триангуляции выпуклого многоугольника\n\nАвтор:\nГалаев Антон Олегович,\nстудент 1 курса НИУ ВШЭ\n\nМосква, 2012г.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для работы с программой:\n 1. Задать количество вершин многоугольника.\n 2. Задать стоимости разрезов многоугольника.\n 3. Вычислить оптимальный раскрой, нажав на кнопку.\nДругие функции:\n 1. Сохранение многоугольников в файлы.\n 2. Прочтение многоугольников из файлов программы.\n 3. Сохранение многоугольника как изображения.\n 4. Сохранение матриц стоимостей разреза многоугольника в текстовый файл", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
                
    }
    [Serializable]
    public class Min
    {
        int n, d, from, to;
        public int To
        {
            get { return to; }
            set { to = value; }
        }
        public int From
        {
            get { return from; }
            set { from = value; }
        }
        public int D
        {
            get { return d; }
            set { d = value; }
        }
        public int N
        {
            get { return n; }
            set { n = value; }
        }
        public int ND
        {
            get { return n + d; }
        }
        Min way1, way2;
        public Min Way1
        {
            get { return way1; }
            set { way1 = value; }
        }
        public Min Way2
        {
            get { return way2; }
            set { way2 = value; }
        }
        public Min(int pfrom, int pto, int pd)
        {
            from = pfrom;
            to = pto;
            d = pd;
            n = 0;
            way1 = null;
            way2 = null;
        }
        public override string ToString()
        {
            if (way1.from + 1 == way1.to)
                return string.Format("Разрезать по {0}-{1}", way2.from + 1, way2.to + 1);
            else if (way2.from + 1 == way2.to)
                return string.Format("Разрезать по {0}-{1}", way1.from + 1, way1.to + 1);
            else
                return string.Format("Разрезать по {0}-{1} и {2}-{3}", way1.from + 1, way1.to + 1, way2.from + 1, way2.to + 1);
        }
    }
}
