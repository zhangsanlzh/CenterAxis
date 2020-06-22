using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultAxis
{
    [Docking(DockingBehavior.AutoDock)]
    public partial class Axis: UserControl
    {
        private Pen penBlack;
        private Pen penGray;
        private Pen penRed;
        private Graphics g;
        private Font fontArial = new Font("Arial", 6);

        private Panel pnlDraw => splitContainer1.Panel1;
        private Panel pnlAxis => splitContainer1.Panel2;

        /// <summary>
        /// 坐标轴原点横坐标
        /// </summary>
        private float OX
        {
            get { return pnlDraw.Width / 2; }
        }

        /// <summary>
        /// 坐标轴原点纵坐标
        /// </summary>
        private float OY
        {
            get { return pnlDraw.Height / 2; }
        }

        /// <summary>
        /// Y轴大刻度的宽度
        /// </summary>
        private float Big_Interval_Width_Y { get; set; }

        /// <summary>
        /// Y轴小刻度的宽度
        /// </summary>
        private float Small_Interval_Width_Y { get; set; }

        /// <summary>
        /// X轴大刻度的宽度
        /// </summary>
        private float Big_Interval_Width_X { get; set; }

        /// <summary>
        /// X轴小刻度的宽度
        /// </summary>
        private float Small_Interval_Width_X { get; set; }


        public Axis()
        {
            InitializeComponent();

            penBlack = new Pen(Color.Black, 1);
            penGray = new Pen(Color.LightGray, 1);
            penRed = new Pen(Color.FromArgb(234, 185, 175), 1);

            pnlDraw.BackColor = Color.White;
            pnlAxis.BackColor = Color.FromArgb(255, 255, 203);

            pnlDraw.Paint += PnlDraw_Paint;
            pnlAxis.Paint += PnlAxis_Paint;
        }

        private void PnlDraw_Paint(object sender, PaintEventArgs e)
        {
            g = pnlDraw.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            DrawXY();
        }

        private void PnlAxis_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        #region 画坐标系

        /* 
         * 注意：
         *      此坐标系默认水平方向为 x 轴，竖直方向为 y 轴。所有内部定位逻辑均按此展开。
         *      界面上观察出的坐标若是 (y, x) => (15, 14), 传入坐标系的坐标为 (x, y) => (15, 14)
         */

        private void DrawXY(int xcount = 10, int ycount = 10)
        {
            DrawX(xcount);
            DrawY(ycount);
        }

        /// <summary>
        /// 画出竖直方向的轴及其刻度，文字。界面上是 X 轴，按一般坐标系的规定是 Y 轴。
        /// </summary>
        /// <param name="count"></param>
        private void DrawY(int count)
        {
            int big_Tick_Width = 5;
            int small_Tick_Width = 2;
            int big_Interval_Count = count;
            int small_Interval_Count = 5;

            g.DrawLine(penBlack, new PointF(OX, 0), new PointF(OX, pnlDraw.Height));// 画 Y 主轴

            // 大刻度的宽
            float big_Interval_Width = OY / big_Interval_Count;
            Big_Interval_Width_Y = big_Interval_Width;

            // 小刻度的宽
            float small_Interval_Width = big_Interval_Width / small_Interval_Count;
            Small_Interval_Width_Y = small_Interval_Width;

            int top_Big_Count = big_Interval_Count;
            int bottom_Big_Count = big_Interval_Count;

            float top_Small_Count = OY % 100 / 20;
            float bottom_Small_Count = (pnlDraw.Height - OY) % 100 / 20;

            int i;
            for (i = 0; i <= top_Big_Count; i++)
            {
                var p0 = new PointF(OX + big_Tick_Width, OY - i * big_Interval_Width);
                var p1 = new PointF(OX, OY - i * big_Interval_Width);
                g.DrawLine(penBlack, p0, p1);

                // 标出刻度值
                if (i > 0 && i < top_Big_Count)
                {
                    string text = "-" + Convert.ToString(i * 10);
                    SizeF size = g.MeasureString(text, fontArial);
                    g.DrawString(text, fontArial, Brushes.Black, new PointF(p1.X - size.Width, p1.Y - 3));// 减 3 是为了让文字与大刻度线对齐
                }

                // 画出大格中间的小格
                for (int j = 1; j < small_Interval_Count; j++)
                {
                    var p3 = new PointF(OX + small_Tick_Width, OY - i * big_Interval_Width - j * small_Interval_Width);
                    var p4 = new PointF(OX, OY - i * big_Interval_Width - j * small_Interval_Width);
                    g.DrawLine(penBlack, p3, p4);
                }
            }

            if (top_Small_Count > 0)
            {
                // 画出剩余的小格
                for (int j = 0; j < top_Small_Count; j++)
                {
                    var p0 = new PointF(OX + small_Tick_Width, OY - i * big_Interval_Width - j * small_Interval_Width);
                    var p1 = new PointF(OX, OY - i * big_Interval_Width - j * small_Interval_Width);
                    g.DrawLine(penBlack, p0, p1);
                }
            }

            int m;
            for (m = 0; m <= bottom_Big_Count; m++)
            {
                var p0 = new PointF(OX + big_Tick_Width, OY + m * big_Interval_Width);
                var p1 = new PointF(OX, OY + m * big_Interval_Width);
                g.DrawLine(penBlack, p0, p1);

                // 标出刻度值
                if (m > 0 && m < bottom_Big_Count)
                {
                    string text = Convert.ToString(m * 10);
                    SizeF size = g.MeasureString(text, fontArial);
                    g.DrawString(text, fontArial, Brushes.Black, new PointF(p1.X - size.Width, p1.Y - 3));// 减 3 是为了让文字与大刻度线对齐
                }

                // 画出大格中间的小格
                for (int k = 1; k < small_Interval_Count; k++)
                {
                    var p3 = new PointF(OX + small_Tick_Width, OY + m * big_Interval_Width + k * small_Interval_Width);
                    var p4 = new PointF(OX, OY + m * big_Interval_Width + k * small_Interval_Width);
                    g.DrawLine(penBlack, p3, p4);
                }
            }

            if (bottom_Small_Count > 0)
            {
                // 画出剩余的小格
                for (int j = 0; j < bottom_Small_Count; j++)
                {
                    var p0 = new PointF(OX + small_Tick_Width, OY + m * big_Interval_Width + j * small_Interval_Width);
                    var p1 = new PointF(OX, OY + m * big_Interval_Width + j * small_Interval_Width);
                    g.DrawLine(penBlack, p0, p1);
                }
            }
        }

        /// <summary>
        /// 画出水平方向的轴及其刻度，文字。界面上是 Y 轴，按一般坐标系的规定是 X 轴。
        /// </summary>
        /// <param name="count"></param>
        private void DrawX(int count)
        {
            g.DrawLine(penBlack, new PointF(0, OY), new PointF(pnlDraw.Width, OY));// 画 X 主轴

            int big_Tick_Height = 5;// 表示一半轴上大刻度的长度            
            int small_Tick_Height = 2;// 表示一半轴上小刻度的长度            
            int big_Interval_Count = count;// 表示一半轴上大刻度的数量            
            int small_Interval_Count = 5;// 表示一半轴上小刻度的数量

            // 大间隔的宽
            float big_Interval_Width = OX / big_Interval_Count;
            Big_Interval_Width_X = big_Interval_Width;

            // 小间隔的宽
            float small_Interval_Width = big_Interval_Width / small_Interval_Count;
            Small_Interval_Width_X = small_Interval_Width;

            int left_Big_Count = big_Interval_Count; // 左侧画的大格数
            int right_Big_Count = big_Interval_Count; // 右侧画的大格数

            float left_Small_Count = OX % 100 / 20; // 左侧画完大格后还需要画的小格数
            float right_Small_Count = (pnlDraw.Width - OX) % 100 / 20; //右侧画完大格后还需要画的小格数

            int i;
            for (i = 0; i <= left_Big_Count; i++)
            {
                var p0 = new PointF(OX - i * big_Interval_Width, OY - big_Tick_Height);
                var p1 = new PointF(OX - i * big_Interval_Width, OY);
                g.DrawLine(penBlack, p0, p1);

                // 标出刻度值
                if (i > 0 && i < left_Big_Count)
                {
                    string text = "-" + Convert.ToString(i * 10);
                    SizeF size = g.MeasureString(text, fontArial);
                    g.DrawString(text, fontArial, Brushes.Black, new PointF(p1.X - size.Width / 2, p1.Y + 1));// 加 1 是为了不至于文字与坐标轴挨得太近
                }

                // 画出大格中间的小格
                for (int j = 1; j < small_Interval_Count; j++)
                {
                    var p3 = new PointF(OX - i * big_Interval_Width - j * small_Interval_Width, OY - small_Tick_Height);
                    var p4 = new PointF(OX - i * big_Interval_Width - j * small_Interval_Width, OY);
                    g.DrawLine(penBlack, p3, p4);
                }
            }

            if (left_Small_Count > 0)
            {
                // 画出剩余的小格
                for (int j = 0; j < left_Small_Count; j++)
                {
                    var p0 = new PointF(OX - i * big_Interval_Width - j * small_Interval_Width, OY - small_Tick_Height);
                    var p1 = new PointF(OX - i * big_Interval_Width - j * small_Interval_Width, OY);
                    g.DrawLine(penBlack, p0, p1);
                }
            }

            int m;
            for (m = 0; m <= right_Big_Count; m++)
            {
                var p0 = new PointF(OX + m * big_Interval_Width, OY - big_Tick_Height);
                var p1 = new PointF(OX + m * big_Interval_Width, OY);
                g.DrawLine(penBlack, p0, p1);

                // 标出刻度值
                if (m > 0 && m < left_Big_Count)
                {
                    string text = Convert.ToString(m * 10);
                    SizeF size = g.MeasureString(text, fontArial);
                    g.DrawString(text, fontArial, Brushes.Black, new PointF(p1.X - size.Width / 2, p1.Y + 1));// 加 1 是为了不至于文字与坐标轴挨得太近
                }

                for (int k = 1; k < small_Interval_Count; k++)
                {
                    var p3 = new PointF(OX + m * big_Interval_Width + k * small_Interval_Width, OY - small_Tick_Height);
                    var p4 = new PointF(OX + m * big_Interval_Width + k * small_Interval_Width, OY);
                    g.DrawLine(penBlack, p3, p4);
                }
            }

            if (right_Small_Count > 0)
            {
                // 画出剩余的小格
                for (int j = 0; j < right_Small_Count; j++)
                {
                    var p0 = new PointF(OX + m * big_Interval_Width + j * small_Interval_Width, OY - small_Tick_Height);
                    var p1 = new PointF(OX + m * big_Interval_Width + j * small_Interval_Width, OY);
                    g.DrawLine(penBlack, p0, p1);
                }
            }
        }

        #endregion

        /// <summary>
        /// 把输入点转为当前坐标系下的点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private PointF ConvertPoint(PointF point)
        {
            float bigCountX = point.X / 10;// 大格数
            float bigLeftX = point.X % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountX = bigLeftX / 2;// 小格数
            float smallLeftX = smallCountX % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nX = smallLeftX / 2; // 剩下的值与 1 小格的占比

            float x = bigCountX * Big_Interval_Width_X + smallCountX * Small_Interval_Width_X + nX * Small_Interval_Width_X;

            float bigCountY = point.Y / 10;// 大格数
            float bigLeftY = point.Y % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountY = bigLeftY / 2;// 小格数
            float smallLeftY = smallCountY % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nY = smallLeftY / 2; // 剩下的值与 1 小格的占比

            float y = bigCountY * Big_Interval_Width_Y + smallCountY * Small_Interval_Width_Y + nY * Small_Interval_Width_Y;

            return new PointF(OX + x, OY + y);
        }

        private float ConvertX(float x)
        {
            float bigCountX = x / 10;// 大格数
            float bigLeftX = x % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountX = bigLeftX / 2;// 小格数
            float smallLeftX = smallCountX % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nX = smallLeftX / 2; // 剩下的值与 1 小格的占比

            return OX + bigCountX * Big_Interval_Width_X + smallCountX * Small_Interval_Width_X + nX * Small_Interval_Width_X;
        }

        private float ConvertY(float y)
        {
            float bigCountY = y / 10;// 大格数
            float bigLeftY = y % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountY = bigLeftY / 2;// 小格数
            float smallLeftY = smallCountY % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nY = smallLeftY / 2; // 剩下的值与 1 小格的占比

            return OY + bigCountY * Big_Interval_Width_Y + smallCountY * Small_Interval_Width_Y + nY * Small_Interval_Width_Y;
        }

        private float ConvertWidth(float width)
        {
            float bigCountX = width / 10;// 大格数
            float bigLeftX = width % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountX = bigLeftX / 2;// 小格数
            float smallLeftX = smallCountX % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nX = smallLeftX / 2; // 剩下的值与 1 小格的占比

            return bigCountX * Big_Interval_Width_X + smallCountX * Small_Interval_Width_X + nX * Small_Interval_Width_X;
        }

        private float ConvertHeight(float height)
        {
            float bigCountY = height / 10;// 大格数
            float bigLeftY = height % 10;// 去除 bigCount 个大格后，剩下的值
            float smallCountY = bigLeftY / 2;// 小格数
            float smallLeftY = smallCountY % 2;// 去除 smallCount 个小格后，剩下的不满 1 小格的值
            float nY = smallLeftY / 2; // 剩下的值与 1 小格的占比

            return bigCountY * Big_Interval_Width_Y + smallCountY * Small_Interval_Width_Y + nY * Small_Interval_Width_Y;
        }

    }
}
