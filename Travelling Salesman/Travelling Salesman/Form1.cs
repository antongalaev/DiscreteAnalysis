using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cutting_Cornered_Convex_Polygon;


namespace Travelling_Salesman
{
    public partial class Form1 : Form
    {
        //лист ожидания
        List<Matrix> Wait = new List<Matrix>();  
        Bitmap image;
        //точки
        Point[] points;
        public static int NC { get; set; }
        public static Matrix Source { get; set; }
        public static Matrix Answer { get; set; }
        public static bool fill { get; set; }
        public Form1()
        {
            InitializeComponent();
            InitialSettings();
            image = new Bitmap(1600, 1400);
            pictureBox1.Image = image;
        }
        private void InitialSettings()
        {
            CostMenuItem.Enabled = false;
            CostButton.Enabled = false;
            CalculateMenuItem.Enabled = false;
            CalculateButton.Enabled = false;
            LogMenuItem.Enabled = false;
            SaveButton.Enabled = false;
            SaveMenuItem.Enabled = false;
            SaveTxtMenuItem.Enabled = false;
            SaveImageMenuItem.Enabled = false;
            StatusLabel.Text = "Укажите количество городов";
            StatusLabel2.Text = "";
            StatusLabel2.Alignment = ToolStripItemAlignment.Right;
        }
        //----Рисование
        private void DrawPolygon(int n)
        {
            Graphics g = Graphics.FromImage(image);
            Pen p = new Pen(Color.Black, 4);
            g.Clear(Color.White);
            points = new Point[n];
            Point center = new Point(image.Width / 2, image.Height / 2);
            int rad = (int)(image.Height / 2.5);
            double alpha = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                points[i] = new Point((int)(center.X + rad * Math.Sin(alpha * i)),
                    (int)(center.Y + rad * Math.Cos(alpha * i)));
                g.FillEllipse(Brushes.Red, points[i].X - 5, points[i].Y - 5, 10, 10);
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
        private void FullGraph()
        {
            Graphics g = Graphics.FromImage(image);
            Pen p = new Pen(Color.Black, 4);
            for (int i = 0; i < NC; i++)
                for (int j = 0; j < NC; j++)
                    if (i != j)
                        g.DrawLine(p, points[i], points[j]);
            pictureBox1.Refresh();
            g.Dispose();
            p.Dispose();
        }
        //----Количество вершин
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
            LogMenuItem.Enabled = false;
            SaveButton.Enabled = false;
            SaveTxtMenuItem.Enabled = false;
            SaveMenuItem.Enabled = false;
            fill = false;
            StatusLabel.Text = "Укажите расстояния между городами";
            DrawPolygon(NC);
            FullGraph();
        }
        //----Стоимость перехода
        private void Cost()
        {
            if (Source.Mtr.Count == 0) fill = false;
            else fill = true;
            Form3 f = new Form3(NC);
            f.ShowDialog();
            f.Dispose();
            if (!fill) return;
            CalculateMenuItem.Enabled = true;
            CalculateButton.Enabled = true;
            LogMenuItem.Enabled = false;
            SaveButton.Enabled = true;
            SaveTxtMenuItem.Enabled = false;
            SaveMenuItem.Enabled = true;
            StatusLabel.Text = "Нажмите кнопку 'Вычислить' ";
        }
        //----Ветви и границы
        private Matrix Process(Matrix M)
        {
            while (true)
            {
                //***Выход из цикла
                if (M.Size == 1)
                {
                    if (M.Mtr[0].Val == -1) M.Cost = int.MaxValue;
                    else
                    {
                        M.Way.Add(string.Format("{0}-{1}", M.Mtr[0].From, M.Mtr[0].To));
                        return M;
                    }
                }
                //***Минимум из листа ожидания
                Matrix Min = M;
                for (int i = 0; i < Wait.Count; i++)
                    if (Wait[i].Cost < Min.Cost) Min = Wait[i];
                if (Min.Cost < M.Cost)
                {
                    Wait.Add(M);
                    M = Min;
                }

                int s = M.Size;
                //***Вычитание минимумов
                //По строкам:
                for (int i = 0; i < s; i++)
                {
                    int min = int.MaxValue;
                    for (int j = 0; j < s; j++)
                    {
                        if (M.Mtr[i * s + j].Val == -1) continue;
                        if (M.Mtr[i * s + j].Val < min) min = M.Mtr[i * s + j].Val;
                        if (min == 0) break;
                    }
                    if (min > 0 && min != int.MaxValue)
                    {
                        for (int j = 0; j < s; j++)
                        {
                            if (M.Mtr[i * s + j].Val == -1) continue;
                            Cell v = M.Mtr[i * s + j];
                            v.Val -= min;
                            M.Mtr[i * s + j] = v;
                        }
                        M.Cost += min;
                    }
                }
                //По столбцам
                for (int i = 0; i < s; i++)
                {
                    int min = int.MaxValue;
                    for (int j = 0; j < s; j++)
                    {
                        if (M.Mtr[i + j * s].Val == -1) continue;
                        if (M.Mtr[i + j * s].Val < min) min = M.Mtr[i + j * s].Val;
                        if (min == 0) break;
                    }
                    if (min > 0 && min != int.MaxValue)
                    {
                        for (int j = 0; j < s; j++)
                        {
                            if (M.Mtr[i + j * s].Val == -1) continue;
                            Cell v = M.Mtr[i + j * s];
                            v.Val -= min;
                            M.Mtr[i + j * s] = v;
                        }
                        M.Cost += min;
                    }
                }
                //Ноль
                int f = 0, t = 0;
                foreach (int i in M.Wayint)
                {
                    try
                    {
                        if (M[M.LastIndex, i].Val == -1) continue;

                        if (M[M.LastIndex, i].Val == 0)
                        {
                            f = M.LastIndex;
                            t = i;
                            break;
                        }
                    }
                    catch { continue; }
                }
                if (f == 0 && t == 0) M.Cost = int.MaxValue;
                M.LastIndex = t;
                Matrix alt = new Matrix(M, f, t);
                Wait.Add(alt);
                M.Way.Add(string.Format("{0}-{1}", f, t));
                M.Wayint.Remove(t);
                try
                {
                    Cell v = M[t, f];
                    v.Val = -1;
                    M[t, f] = v;
                }
                catch { }
                M.Size -= 1;

                if (M.Size > 1)
                {
                    try
                    {
                        Cell v = M[t, 0];
                        v.Val = -1;
                        M[t, 0] = v;
                    }
                    catch { }
                }
                for (int i = 0; i < M.Mtr.Count; i++)
                    if (M.Mtr[i].From == f | M.Mtr[i].To == t)
                    { M.Mtr.RemoveAt(i); i--; }
            }
        }
        //----Вывод на экран
        private void ShowWay(Matrix M)
        {
            DrawPolygon(NC);
            Graphics g = Graphics.FromImage(image);
            Pen p = new Pen(Color.Red, 5);
            p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            for (int i = 0; i < M.Way.Count; i++)
            {
                if (Regex.IsMatch(M.Way[i], @"-"))
                {
                    string[] t = M.Way[i].Split('-');
                    int p1 = int.Parse(t[0]), p2 = int.Parse(t[1]);
                    g.DrawLine(p, points[p1], points[p2]);                                      
                }
            }
            StatusLabel.Text = string.Format("Длина пути: {0}", M.Cost);
            StatusLabel2.Text = M.View();
            pictureBox1.Refresh();
            g.Dispose();
            p.Dispose();
            MessageBox.Show(string.Format("Длина пути: {0}", M.Cost) + "\r\n" + M.View(), "Ответ", MessageBoxButtons.OK);
        }
        //----Лог
        private void LogMenuItem_Click(object sender, EventArgs e)
        {
            string log = "Шаги алгоритма:\r\n " + String.Join("\r\n ", Answer.Way.ToArray());
            MessageBox.Show(log, "Лог", MessageBoxButtons.OK);
        }
        private void Go()
        {
            Wait = new List<Matrix>();
            Matrix Beg = Source.Copy();
            Answer = Process(Beg);
            ShowWay(Answer);
            SaveButton.Enabled = true;
            SaveMenuItem.Enabled = true;
            SaveTxtMenuItem.Enabled = true;
            LogMenuItem.Enabled = true;
        }
        private void SaveFile(int i)
        {
            if (i == 1)
            {
                saveFileDialog1.Filter = "Проект (*.tsp) |*.tsp";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, Source);
                    fs.Close();
                }
            }
            if (i == 2)
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
                    wr.WriteLine("Матрица с расстояниями между городами:");
                    wr.Write("M");
                    for (int g = 0; g < NC; g++)
                        wr.Write("\t" + (g + 1).ToString());
                    for (int a = 0; a < NC; a++)
                    {
                        wr.Write("\r\n");
                        wr.Write(a + 1);
                        for (int b = 0; b < NC; b++)
                        {
                            if (a != b)
                                wr.Write("\t" + Source.Mtr[a * NC + b].Val);
                            else
                                wr.Write("\t" + char.ConvertFromUtf32(8734));
                        }
                    }
                    wr.Write("\r\n");
                    wr.Write(string.Format("Длина пути: {0}", Answer.Cost) + "\r\n" + Answer.View());
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
                Source = (Matrix)bf.Deserialize(fs);
                NC = Source.Size;
                DrawPolygon(NC);
                FullGraph();
                fs.Close();
                CostButton.Enabled = true;
                CostMenuItem.Enabled = true;
                SaveImageMenuItem.Enabled = true;
                CalculateButton.Enabled = true;
                CalculateMenuItem.Enabled = true;
                SaveMenuItem.Enabled = true;
                SaveButton.Enabled = true;
                SaveTxtMenuItem.Enabled = false;
                LogMenuItem.Enabled = false;
            }
        }

        private void CornersButton_Click(object sender, EventArgs e)
        {
            Corners();
        }

        private void CornersMenuItem_Click(object sender, EventArgs e)
        {
            Corners();
        }

        private void CostButton_Click(object sender, EventArgs e)
        {
            Cost();
        }

        private void CostMenuItem_Click(object sender, EventArgs e)
        {
            Cost();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            Go();
        }

        private void CalculateMenuItem_Click(object sender, EventArgs e)
        {
            Go();
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
        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void HelpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Для работы с программой:\n 1. Задать количество городов.\n 2. Задать расстояния между городами.\n 3. Вычислить оптимальный путь обхода всех городов, нажав на кнопку.\nДругие функции:\n 1. Сохранение графа с городами и расстояниями в файл.\n 2. Прочтение графов из файлов программы.\n 3. Сохранение оптимального пути как изображения.\n 4. Сохранение матрицы расстояний между городами в текстовый файл", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Коммивояжёр \n\nПрограмма для решения задачи коммивояжёра\n(Travelling Salesman Problem)\n\nАвтор:\nГалаев Антон Олегович,\nстудент 1 курса НИУ ВШЭ\n\nМосква, 2012г.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
              
    }

    [Serializable]
    public struct Cell
    {
        int from, to, val;
        
        public int From
        {
            get { return from; }
            set { from = value; }
        }
        public int To
        {
            get { return to; }
            set { to = value; }
        }                
        public int Val
        {
            get { return val; }
            set { val = value; }
        }
        public Cell Copy()
        {
            return new Cell(this.from, this.to, this.val);
        }
        public Cell(int f, int t, int v)
        {
            from = f;
            to = t;
            val = v;
        }
    }
    [Serializable]
    public class Matrix
    {
        int cost, size;
        List<Cell> mtr;
        List<string> way;
        List<int> wayint;
        public int LastIndex
        { get; set; }
        //----свойства
        public List<int> Wayint
        {
            get { return wayint; }
            set { wayint = value; }
        }
        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        public List<Cell> Mtr
        {
            get { return mtr; }
            set { mtr = value; }
        }
        public List<string> Way
        {
            get { return way; }
            set { way = value; }
        }
        //----индексатор
        public Cell this[int i, int j]
        {
            get 
            {
                Cell t = new Cell();
                for (int a = 0; a < mtr.Count; a++)
                    if (mtr[a].From == i && mtr[a].To == j)
                    {
                        t = mtr[a];
                        break;
                    }
                return t;
            }
            set 
            {
                for (int a = 0; a < mtr.Count; a++)
                    if (mtr[a].From == i && mtr[a].To == j)
                    {
                        mtr[a] = value;
                        break;
                    }
            }
        }
        public Matrix(int sz)
        {
            cost = 0;
            size = sz;
            LastIndex = 0;
            mtr = new List<Cell>();
            way = new List<string>();
            wayint = new List<int>();
            for (int i = 1; i < sz; i++)
                wayint.Add(i);
           
        }
        public Matrix(Matrix pre, int f, int t)
        {
            cost = pre.cost;
            size = pre.size;
            LastIndex = f;
            wayint = pre.wayint.ToList<int>();
            way = pre.way.ToList<string>();
            way.Add(string.Format("{0}!{1}", f, t));
            mtr = pre.mtr.ToList();
            this[f,t] = new Cell(f, t, -1);
        }

        public Matrix Copy()
        {
            Matrix n = new Matrix(this.size);
            n.cost = this.cost;
            n.size = this.size;
            n.LastIndex = this.LastIndex;
            n.wayint = this.wayint.ToList<int>();
            n.way = this.way.ToList<string>();
            n.mtr = this.mtr.ToList();
            return n;
        }
        public string View()
        {
            string w = "";
            for (int i = 0; i < way.Count; i++)
            {
                if (Regex.IsMatch(way[i], @"-"))
                {
                    string[] t = way[i].Split('-');
                    w += (int.Parse(t[0]) + 1) + " - ";
                }
            }
            w += "1";
            return "Оптимальный путь обхода: " + w;
        }
    }
}
