using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using System.IO;
namespace mophsnake1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int iterNum;
        double lamda1;
        double lamda2;
        double v;
        Image tmpImage;
        Bitmap oriimage;//originale Image
        Bitmap oriimage0;
        Bitmap oriimage1;
        Size tmpsiz;
        double fgVal;
        double bkVal;
        double[,] Mphi;
        double[,] MphiAn;
        double[,] dx;
        double[,] dy;
        double[,] absGrMphi;
        double[,] Kontur;
        Point pt;
        Graphics graf = null;
        bool mousedown1 = false;
        bool mousedown2 = false;
        bool mousedown3 = false;
        int x;
        List<Point> ptlist=null;
        string Filename;
        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog opd = new OpenFileDialog();
            opd.DefaultExt = ".bmp";
            opd.Filter = "Image Files(*.bmp,*.jpg,*.png,*.TIF)|*.bmp;*.jpg;*.png;*.TIF||";
            if (DialogResult.OK != opd.ShowDialog(this))
            {
                return;
            }
            Filename = opd.FileName;
            textBox1.AppendText(opd.FileName);
            lodaimage();
        }
        private void button2_Click(object sender, EventArgs e)//outside = 0; indside = 1;
        {
            x = Convert.ToInt32(textBox2.Text);
            if (mousedown1 == false)
            {
                mousedown1 = true;
                button2.Enabled = false;
            }
            else
            {
                return;
            }
            textBox1.AppendText("Koordinate: " + Convert.ToDouble(pt.X) + "," + Convert.ToDouble(pt.Y) + "\n");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            lamda1 = 1;
            if (textBox3.Text == "")
            {
                v = 0;
            }
            else
            {
                v = Convert.ToDouble(textBox3.Text);
            }
            lamda2 = Convert.ToDouble(textBox4.Text);
            if (textBox5.Text == "")
            {

                Segauto(); 
            }
            else
            {
                iterNum = Convert.ToInt32(textBox5.Text);
                Segmentierung();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (mousedown2 == false)
            {
                mousedown2 = true;
                button4.Enabled = false;
            }
            else
            {
                return;
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {
            string savename;
            string Lnum0;
            string Lnum1;
            int annum;
            int anzahl;
            annum = Convert.ToInt32(textBox7.Text);
            savename = Convert.ToString(textBox6.Text);
            anzahl = Convert.ToInt16(textBox8.Text);
            oriimage0.Save(savename, System.Drawing.Imaging.ImageFormat.Png);
            for (int i = 0; i < anzahl; i++)
            {
                Lnum0 = "_0000".Remove(5 - Convert.ToString(annum).Length);
                Lnum0 = Lnum0 + Convert.ToString(annum);
                Lnum1 = "_0000".Remove(5 - Convert.ToString(annum + 1).Length);
                Lnum1 = Lnum1 + Convert.ToString(annum + 1);
                savename = savename.Replace(Lnum0, Lnum1);
                Filename = Filename.Replace(Lnum0, Lnum1);
                annum++;
                lodaimage();
                textBox1.AppendText(Filename);
                Initialisierung();
                Segauto();
                oriimage0.Save(savename, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (mousedown3 == false)
            {
                mousedown3 = true;
                button6.Enabled = false;
            }
            else
            {
                return;
            }
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (mousedown1 == true)
            {
                pt = new Point(e.Location.X, e.Location.Y);
                Initialisierung();
                mousedown1 = false;
            }
            if (mousedown2 == true)
            {
                button4.Enabled = true;
                double dist;
                if (oriimage1.Width > oriimage1.Height)
                {
                    dist = oriimage1.Width;
                }
                else
                {
                    dist = oriimage1.Height;
                }
                Point ptanf = new Point(e.Location.X, e.Location.Y);
                Point pt1=new Point();
                ptlist = new List<Point>();
                double[,] coKontur = Kontur;
                for (int i = 0; i < oriimage1.Width; i++)
                {
                    for(int j = 0; j < oriimage1.Height; j++)
                    {
                        if (coKontur[i, j] == 1)
                        {
                            if(Math.Sqrt((i - ptanf.X) * (i - ptanf.X) + (j - ptanf.Y) * (j - ptanf.Y)) < dist)
                            {
                                dist = Math.Sqrt((i - ptanf.X) * (i - ptanf.X) + (j - ptanf.Y) * (j - ptanf.Y));
                                pt1.X=i;
                                pt1.Y = j;
                            }
                        }
                    }
                }
                coKontur[pt1.X, pt1.Y] = 0;
                textBox1.AppendText("Point" + "(" + pt1.X + "," + pt1.Y + ")");
                ptlist.Add(pt1);//AnfangsPunkt
                int n = 1;
                do
                {
                    dist = 255;
                    for (int i = 0; i < oriimage1.Width; i++)
                    {
                        for (int j = 0; j < oriimage1.Height; j++)
                        {
                            if (coKontur[i, j] == 1)
                            {
                                if (Math.Sqrt((i - ptlist[n - 1].X) * (i - ptlist[n - 1].X) + (j - ptlist[n - 1].Y) * (j - ptlist[n - 1].Y)) < dist)
                                {
                                    dist = Math.Sqrt((i - ptlist[n - 1].X) * (i - ptlist[n - 1].X) + (j - ptlist[n - 1].Y) * (j - ptlist[n - 1].Y));
                                    pt1 = new Point(i, j);
                                    
                                }
                            }
                        }
                    }
                    coKontur[pt1.X, pt1.Y] = 0;
                    //textBox1.AppendText("Point" + "(" + pt1.X + "," + pt1.Y + ")");
                    ptlist.Add(pt1);
                    if (n == 2)
                    {
                        coKontur[ptlist[0].X, ptlist[0].Y] = 1;
                    }
                    n++;
                }
                while (ptlist[n-1].X != ptlist[0].X || ptlist[n-1].Y != ptlist[0].Y);
                for(int i = 0; i < n; i++)
                {
                    textBox1.AppendText("Point" + i + "(" + ptlist[i].X + "," + ptlist[i].Y + ")");
                }
                mousedown2 = false;
                button4.Enabled = true;
            }
            if (mousedown3 == true)
            {
                pt = new Point(e.Location.X, e.Location.Y);
                textBox1.AppendText(Convert.ToString(oriimage1.GetPixel(pt.X, pt.Y).R));
                button6.Enabled = true;
                mousedown3 = false;
            }
        }
        public void Segmentierung()
        {
            for (int n = 0; n < iterNum; n++)
            {

                BrGray();
                abs_gradient();
                for (int i = 0; i < oriimage1.Width; i++)
                {
                    for (int j = 0; j < oriimage1.Height; j++)
                    {
                        double aux;
                        int p = oriimage1.GetPixel(i, j).R;
                        aux = absGrMphi[i, j] * (lamda1 * (p - fgVal) * (p - fgVal) - lamda2 * (p - bkVal) * (p - bkVal) + v);
                        if (aux < 0)
                        {
                            Mphi[i, j] = 1;
                        }
                        if (aux > 0)
                        {
                            Mphi[i, j] = 0;
                        }
                    }
                }
                SI();
                IS();
                //IS();
                //SI();
                plot1();
                textBox1.AppendText(Convert.ToString(tmpsiz.Width) + "  " + Convert.ToString(tmpsiz.Height));
                //textBox1.AppendText("c1: " + Convert.ToString(fgVal) + "\n" + "c2： " + Convert.ToString(bkVal));
                //double GrenzeP;
                //double GrenzeN;               
                //GrenzeP = ((fgVal - lamda2 * bkVal) + Math.Sqrt((fgVal - lamda2 * bkVal)* (fgVal - lamda2 * bkVal) - (1 - lamda2) * (fgVal * fgVal - lamda2 * bkVal * bkVal + v))) / (1 - lamda2);
                //GrenzeN = ((fgVal - lamda2 * bkVal) - Math.Sqrt((fgVal - lamda2 * bkVal)* (fgVal - lamda2 * bkVal) - (1 - lamda2) * (fgVal * fgVal - lamda2 * bkVal * bkVal + v))) / (1 - lamda2);
               // textBox1.AppendText("Grenze1: " + Convert.ToString(GrenzeP) + "\n" + "Grenze2: " + Convert.ToString(GrenzeN));
            }
        }
        public void Segauto()
        {
            int auto = 0;
            int dif;
            MphiAn = Mphi; ;
            do
            {
                //I();
                dif = 0;
                BrGray();
                abs_gradient();
               
                for (int i = 0; i < oriimage1.Width; i++)
                {
                    for (int j = 0; j < oriimage1.Height; j++)
                    {
                        double aux;
                        int p = oriimage1.GetPixel(i, j).R;
                        aux = absGrMphi[i, j] * (lamda1 * (p - fgVal) * (p - fgVal) - lamda2 * (p - bkVal) * (p - bkVal) + v);
                        if (aux < 0)
                        {
                            Mphi[i, j] = 1;
                        }
                        if (aux > 0)
                        {
                            Mphi[i, j] = 0;
                        }
                    }
                }
                
                //S();
                //I();
                //I();
                //S();
                //I();
                //S();
                //S();
                //I();
                SI();
                IS();
                //IS();
                //SI();
                plot1();
                for (int i = 0; i < oriimage1.Width; i++)
                {
                    for (int j = 0; j < oriimage1.Height; j++)
                    {
                        if (MphiAn[i, j] != Kontur[i, j])
                        {
                            dif++;
                        }
                    }
                }
                if (dif == 0)
                {
                    auto = 1;
                }
                else
                {
                    MphiAn = Kontur;
                }
            }
            while (auto==0);
        }
        public void Initialisierung()
        {
            graf = Graphics.FromImage(oriimage0);
            Pen pen = new Pen(Color.Red);
            graf.DrawEllipse(pen, pt.X - x, pt.Y - x, 2 * x, 2 * x);
            graf.Save();
            panel1.Refresh();
            panel1.BackgroundImage = oriimage0;
            panel1.Refresh();
            button2.Enabled = true;
            Mphi = new double[oriimage1.Width, oriimage1.Height];

            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    if ((Math.Pow(i - pt.X, 2) + Math.Pow(j - pt.Y, 2)) >= x * x)
                    {
                        Mphi[i, j] = 0;
                    }
                    else
                    {
                        Mphi[i, j] = 1;
                    }
                }
            }
        }
        public void lodaimage()
        {
            tmpImage = Image.FromFile(Filename);
            Size pansiz = panel1.ClientSize;
            tmpsiz = tmpImage.Size;
            if (tmpsiz.Width > pansiz.Width || tmpsiz.Height > pansiz.Height)
            {
                double rImage = tmpsiz.Width * 1.0 / tmpsiz.Height;
                double rWnd = pansiz.Width * 1.0 / pansiz.Height;
                if (rImage < rWnd) // image more high
                {

                    tmpsiz.Height = pansiz.Height;
                    tmpsiz.Width = (int)(tmpsiz.Height * rImage);
                }
                else //image is more wide
                {

                    tmpsiz.Width = pansiz.Width;
                    tmpsiz.Height = (int)(pansiz.Width / rImage);
                }
            }
            panel1.Size = tmpsiz;
            oriimage = new Bitmap(tmpImage, tmpsiz);
            Color pixcolor = Color.FromArgb(0);
            double pixval = 0;
            for (int i = 0; i < oriimage.Height; i++)
            {
                for (int j = 0; j < oriimage.Width; j++)
                {
                    pixcolor = oriimage.GetPixel(j, i);
                    pixval = pixcolor.R * 0.3 + pixcolor.G * 0.59 + pixcolor.B * 0.11;
                    oriimage.SetPixel(j, i, Color.FromArgb(Convert.ToInt32(pixval), Convert.ToInt32(pixval), Convert.ToInt32(pixval)));
                }
            }
            oriimage1 = oriimage;
            //Grauss();//useless
            panel1.BackgroundImage = oriimage1;
            textBox1.AppendText("Image load" + "\n");
            panel1.Refresh();
            oriimage0 = new Bitmap(oriimage1, tmpsiz);
            //oriimage=>grau;oriimage1=>nach gaussian;oriimage0=>copy von oriimage1
        }
        public void Grauss()
        {
            double pixval;
            oriimage1 = new Bitmap(oriimage, tmpsiz);
            for (int i = 2; i < oriimage.Height - 2; i++)
            {
                for (int j = 2; j < oriimage.Width - 2; j++)
                {
                    pixval = (oriimage.GetPixel(j - 2, i - 2).R + oriimage.GetPixel(j - 1, i - 2).R + oriimage.GetPixel(j, i - 2).R * 2 + oriimage.GetPixel(j + 1, i - 2).R + oriimage.GetPixel(j + 2, i - 2).R
                        + oriimage.GetPixel(j - 2, i - 1).R + oriimage.GetPixel(j - 1, i - 1).R * 2 + oriimage.GetPixel(j, i - 1).R * 4 + oriimage.GetPixel(j + 1, i - 1).R * 2 + oriimage.GetPixel(j + 2, i - 1).R
                        + oriimage.GetPixel(j - 2, i).R * 2 + oriimage.GetPixel(j - 1, i).R * 4 + oriimage.GetPixel(j, i).R * 8 + oriimage.GetPixel(j + 1, i).R * 4 + oriimage.GetPixel(j + 2, i).R * 2
                        + oriimage.GetPixel(j - 2, i + 1).R + oriimage.GetPixel(j - 1, i + 1).R * 2 + oriimage.GetPixel(j, i + 1).R * 4 + oriimage.GetPixel(j + 1, i + 1).R * 2 + oriimage.GetPixel(j + 2, i + 1).R
                        + oriimage.GetPixel(j - 2, i + 2).R + oriimage.GetPixel(j - 1, i + 2).R + oriimage.GetPixel(j, i + 2).R * 2 + oriimage.GetPixel(j + 1, i + 2).R + oriimage.GetPixel(j + 2, i + 2).R) / 52;
                    oriimage1.SetPixel(j, i, Color.FromArgb(Convert.ToInt32(pixval), Convert.ToInt32(pixval), Convert.ToInt32(pixval)));
                }
            }
        }
        public void plot1()
        {
            oriimage0 = new Bitmap(oriimage1, tmpsiz);;
            
            double au;
            Kontur = new double[oriimage1.Width, oriimage1.Height];
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        dx[i, j] = 0;
                        dy[i, j] = 0;
                    }
                    else
                    {
                        dx[i, j] = (Mphi[i + 1, j] - Mphi[i, j]) / Math.Sqrt(Math.Pow(Mphi[i + 1, j] - Mphi[i, j], 2) + Math.Pow((Mphi[i, j + 1] - Mphi[i, j - 1]) / 2, 2) + 0.0000000001);
                        dy[i, j] = (Mphi[i, j + 1] - Mphi[i, j]) / Math.Sqrt(Math.Pow((Mphi[i + 1, j] - Mphi[i - 1, j]) / 2, 2) + Math.Pow(Mphi[i, j + 1] - Mphi[i, j], 2) + 0.0000000001);
                    }
                    au = Math.Sqrt(Math.Pow(dx[i, j], 2) + Math.Pow(dy[i, j], 2));
                    if (au != 0)
                    {
                        Kontur[i, j] = 1;
                    }
                    else
                    {
                        Kontur[i, j] = 0;
                    }
                }
            }
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    if (Kontur[i, j] == 1)
                    {
                        oriimage0.SetPixel(i, j, Color.Red);
                    }
                }
            }
            panel1.BackgroundImage = oriimage0;
            panel1.Refresh();
        }
        public void plot()//nutzlos
        {
            oriimage0 = oriimage1;
            for(int i = 0; i < oriimage1.Width; i++)
            {
                for(int j = 0;j < oriimage1.Height; j++)
                {
                    if (Mphi[i,j]>0)
                    {
                        oriimage0.SetPixel(i, j, Color.FromArgb(255, 100, 255));
                    }
                } 
            }
            panel1.BackgroundImage = oriimage0;
            panel1.Refresh();
        }
        public void abs_gradient()
        {
            absGrMphi = new double[oriimage1.Width, oriimage1.Height];
            dx = new double[oriimage1.Width, oriimage1.Height];
            dy = new double[oriimage1.Width, oriimage1.Height];
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        dx[i, j] = 0;
                        dy[i, j] = 0;
                    }
                    else
                    {
                        dx[i, j] = 0.5*(Mphi[i + 1, j] - Mphi[i, j]) / Math.Sqrt(Math.Pow(Mphi[i + 1, j] - Mphi[i, j], 2) + Math.Pow((Mphi[i, j + 1] - Mphi[i, j - 1]) / 2, 2) + 0.0000000001)+ 0.5 * (Mphi[i, j] - Mphi[i - 1, j]) / Math.Sqrt(Math.Pow(Mphi[i , j] - Mphi[i - 1, j], 2) + Math.Pow((Mphi[i, j + 1] - Mphi[i, j - 1]) / 2, 2) + 0.0000000001);
                        dy[i, j] = 0.5*(Mphi[i, j + 1] - Mphi[i, j]) / Math.Sqrt(Math.Pow((Mphi[i + 1, j] - Mphi[i - 1, j]) / 2, 2) + Math.Pow(Mphi[i, j + 1] - Mphi[i, j], 2) + 0.0000000001)+ 0.5 * (Mphi[i, j] - Mphi[i, j - 1]) / Math.Sqrt(Math.Pow((Mphi[i + 1, j] - Mphi[i - 1, j]) / 2, 2) + Math.Pow(Mphi[i, j ] - Mphi[i, j - 1], 2) + 0.0000000001);
                    }
                    absGrMphi[i, j] = Math.Sqrt(Math.Pow(dx[i, j], 2) + Math.Pow(dy[i, j], 2));
                }
            }
        }
        public void BrGray()
        {
            double sumfgval = 0;
            double sumbkval = 0;
            int fgn = 0;
            int bkn = 0;
            for (int i = 1; i < oriimage1.Width; i++)
            {
                for (int j = 1; j < oriimage1.Height; j++)
                {
                    if (Mphi[i, j] == 1)
                    {
                        sumfgval = sumfgval + oriimage1.GetPixel(i, j).R;
                        fgn++;
                        if (oriimage1.GetPixel(i, j).R ==0)
                        {
                            fgn--;
                        }

                    }
                    else
                    {
                        sumbkval = sumbkval + oriimage1.GetPixel(i, j).R;
                        bkn++;
                        if (oriimage1.GetPixel(i, j).R ==0)
                        {
                            bkn--;
                        }
                    }
                }
            }
            fgVal = sumfgval / fgn;
            if (bkn == 0)
            {
                bkVal = 0;
            }
            else
            {
                bkVal = sumbkval / bkn;
            }
        }
        public void S()//dilaton
        {
            double[,] M;
            M = new double[oriimage1.Width, oriimage1.Height];
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    int aus = 0;
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        Mphi[i, j] = 0;
                    }
                    else
                    {
                        if (Mphi[i - 1, j] == 1|| Mphi[i+1, j] == 1 || Mphi[i, j + 1] == 1 || Mphi[i ,j - 1] == 1)
                        {
                            aus = 1;
                        }
                        
                        if (aus == 1)
                        {
                            M[i, j] = 1;
                        }
                        else
                        {
                            M[i, j] = 0;
                        }

                    }
                }
            }
            Mphi = M;
        }
        public void I()//erosion
        {
            double[,] M;
            M = new double[oriimage1.Width, oriimage1.Height];
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    int aus = 0;
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        Mphi[i, j] = 0;
                    }
                    else
                    {
                        if (Mphi[i - 1, j] == 0 || Mphi[i + 1, j] == 0 || Mphi[i, j + 1] == 0 || Mphi[i, j - 1] == 0)
                        {
                            aus = 1;
                        }
                        if (aus == 1)
                        {
                            M[i, j] = 0;
                        }
                        else
                        {
                            M[i, j] = 1;
                        }
                    }
                }
            }
            Mphi = M;
        }
        public void IS()
        {
            double[,] M;
            M = new double[oriimage1.Width, oriimage1.Height];
            for(int i = 0; i < oriimage1.Width; i++)
            {
                for(int j = 0; j < oriimage1.Height; j++)
                {
                    int aus = 0;
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        Mphi[i, j] = 0;
                    }
                    else
                    {
                        if (Mphi[i - 1, j - 1] == 1 && Mphi[i, j] == 1 && Mphi[i + 1, j + 1] == 1)
                        {
                            aus = 1;
                        }
                        if (Mphi[i, j - 1] == 1 && Mphi[i, j] == 1 && Mphi[i, j + 1] == 1)
                        {
                            aus = 1;
                        }
                        if (Mphi[i - 1, j + 1] == 1 && Mphi[i, j] == 1 && Mphi[i + 1, j - 1] == 1)
                        {
                            aus = 1;
                        }
                        if (Mphi[i - 1, j] == 1 && Mphi[i, j] == 1 && Mphi[i + 1, j] == 1)
                        {
                            aus = 1;
                        }
                        if (aus == 1)
                        {
                            M[i, j] = 1;
                        }
                        else
                        {
                            M[i, j] = 0;
                        }
                    }
                }
            }
            Mphi = M;
        }
        private void SI()//腐蚀 膨胀  erode dilate
        {
            double[,] M;
            M = new double[oriimage1.Width, oriimage1.Height];
            for (int i = 0; i < oriimage1.Width; i++)
            {
                for (int j = 0; j < oriimage1.Height; j++)
                {
                    int aus = 1;
                    if (i == 0 || j == 0 || i == (oriimage1.Width - 1) || j == (oriimage1.Height - 1))
                    {
                        Mphi[i, j] = 0;
                    }
                    else
                    {
                        if (Mphi[i - 1, j - 1] == 0 && Mphi[i, j] == 0 && Mphi[i + 1, j + 1] == 0)
                        {
                            aus = 0;
                        }
                        if (Mphi[i, j - 1] == 0 && Mphi[i, j] == 0 && Mphi[i, j + 1] == 0)
                        {
                            aus = 0;
                        }
                        if (Mphi[i - 1, j + 1] == 0 && Mphi[i, j] == 0 && Mphi[i + 1, j - 1] == 0)
                        {
                            aus = 0;
                        }
                        if (Mphi[i - 1, j] == 0 && Mphi[i, j] == 0 && Mphi[i + 1, j] == 0)
                        {
                            aus = 0;
                        }
                        if (aus == 0)
                        {
                            M[i, j] = 0;
                        }
                        else
                        {
                            M[i, j] = 1;
                        }
                    }
                }
            }
            Mphi = M;
        }
        
        private void button7_Click(object sender, EventArgs e)
        {
            StreamWriter SW;
            SW = File.CreateText("E:\\You\\Fanuc.txt");
            String ss;
            String ss1;
            String ss2;
            String ss3;
            String ss4;
            string ss5;
            string ss6;
            
            for (int i = 1; i*10 <= ptlist.Count; i++)
            {
                ss = "   "+(i+2)+":J P[" + i + "] 50% FINE ;";
                SW.WriteLine(ss);
            }
            for (int i = 1; i*10 < ptlist.Count; i++)
            {
                ss1 = "P[" + i + "]{";
                ss2 = "   GP1:";
                ss3 = "    UF : 0, UT : 0,     CONFIG : 'N U T, 0, 0, 0',";
                ss4 = "    X =  " + (-1000.000 + 3*ptlist[i*10 - 1].X) + " mm,   Y =  " + (500.000 + 3*ptlist[i*10 - 1].Y) + "  mm,  Z =  1000.000  mm,";
                ss5 = "    W =   0.000 deg,   P =   180.000 deg,   R =  -90.000 deg";
                ss6 = "};";
                SW.WriteLine(ss1);
                SW.WriteLine(ss2);
                SW.WriteLine(ss3);
                SW.WriteLine(ss4);
                SW.WriteLine(ss5);
                SW.WriteLine(ss6);
            }

                SW.Close();
            MessageBox.Show("OK");
        }
    }
}
