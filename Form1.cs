using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Euler393
{
    public partial class Form1 : Form
    {
        const int sz = 4;
        Stopwatch sw;
        Dictionary<string, long[]> dict = new Dictionary<string, long[]>();

        public Form1()
        {
            InitializeComponent();
            this.Show();
            TimeStart();

            bool[,] board = new bool[sz, sz];
            long[] ar = BuildFrom(new Point(0, 0), board, 0);

            long res = 0;
            for (int i = 0; i <= sz; i++)
            {
                res += (long)Math.Pow(2, ar[i]);
            }
            textBox1.Text = res.ToString();
            label1.Text = TimeStop();
        }

        long[] BuildFrom(Point st, bool[,] bd, int ct)
        {
            string k = "";
            if (ct > 0)
            {
                for (int y = 0; y + 1 < sz; y++)
                {
                    for (int x = 0; x + 1 < sz; x++)
                    {
                        k += (bd[x, y] ? "1" : "0");
                    }
                }
                if (dict.ContainsKey(k)) return dict[k];
            }

            bool[,] pc = new bool[sz, sz];
            pc[st.X, st.Y] = true;
            pc[st.X + 1, st.Y] = true;
            Point targ = new Point(st.X, st.Y + 1);
            long[] ans = MakeLoops(ct, bd, pc, new Point(st.X, st.Y + 1), new Point(st.X + 1, st.Y));
            if (ct > 0) dict.Add(k, ans);
            return ans;
        }

        bool StillOkay(bool[,] bd)
        {
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    if (!bd[x, y])
                    {
                        int c = 0;
                        if (x + 1 < sz && !bd[x + 1, y]) c++;
                        if (y + 1 < sz && !bd[x, y + 1]) c++;
                        if (x > 0 && !bd[x - 1, y]) c++;
                        if (y > 0 && !bd[x, y - 1]) c++;
                        if (c < 2) return false;
                    }
                }
            }
            return true;
        }
        bool StillOkayWIP(bool[,] bd, bool[,] bd2, Point curr, Point target)
        {
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    if (!bd[x, y] && !bd2[x, y])
                    {
                        int c = 0;
                        if (x + 1 < sz && !bd[x + 1, y] && !bd2[x + 1, y]) c++;
                        if (y + 1 < sz && !bd[x, y + 1] && !bd2[x, y + 1]) c++;
                        if (x > 0 && !bd[x - 1, y] && !bd2[x - 1, y]) c++;
                        if (y > 0 && !bd[x, y - 1] && !bd2[x, y - 1]) c++;
                        if (c < 2)
                        {
                            if (curr.X == x && curr.Y == y) ;
                            else if (target.X == x && target.Y == y && c == 1) ;
                            else if (curr.X == x && Math.Abs(curr.Y - y) == 1) ;
                            else if (curr.Y == y && Math.Abs(curr.X - x) == 1) ;
                            else return false;
                        }
                    }
                }
            }
            return true;
        }
        Point FindOpening(bool[,] bd)
        {
            bool[,] pc = new bool[sz, sz];
            for (int y = 0; y + 1 < sz; y++)
            {
                for (int x = 0; x + 1 < sz; x++)
                {
                    if (!bd[x, y] && !bd[x + 1, y] && !bd[x, y + 1]) return new Point(x, y);
                }
            }
            return new Point(-1, -1);
        }

        long[] MakeLoops(int ct, bool[,] bd, bool[,] pc, Point target, Point curr)
        {
            long[] ans = new long[sz + 1];
            for (int d = 1; d < 5; d++)
            {
                bool ok = true;

                Point w = new Point(curr.X, curr.Y);
                if (d == 1) // right
                {
                    if (w.X + 1 == sz) ok = false;
                    else w.X++;
                }
                else if (d == 2) // down
                {
                    if (w.Y + 1 == sz) ok = false;
                    else w.Y++;
                }
                else if (d == 3) // left
                {
                    if (w.X == 0) ok = false;
                    else w.X--;
                }
                else // up
                {
                    if (w.Y == 0) ok = false;
                    else w.Y--;
                }
                if (ok) // is this an empty cell?
                {
                    if (bd[w.X, w.Y] || pc[w.X, w.Y]) ok = false;
                }
                if (ok) // any unreachables created?
                {
                    if (!StillOkayWIP(bd, pc, w, target)) ok = false;
                }

                if (ok) // this direction is legitimate
                {
                    bool[,] npc = new bool[sz, sz];
                    for (int y = 0; y < sz; y++)
                    {
                        for (int x = 0; x < sz; x++)
                        {
                            npc[x, y] = pc[x, y];
                        }
                    }                        
                    npc[w.X, w.Y] = true;
                    if (w.X == target.X && w.Y == target.Y) // closed the loop
                    {
                        bool allFull = true;
                        int nct = ct + 1;
                        bool[,] nbd = new bool[sz, sz];
                        for (int y = 0; y < sz; y++)
                        {
                            for (int x = 0; x < sz; x++)
                            {
                                if (bd[x, y] || npc[x, y]) nbd[x, y] = true;
                                else allFull = false;
                            }
                        }
                        if (allFull) ans[1]++; // packed the whole board
                        else if (StillOkay(nbd)) // space left and all squares have at least 2 approaches
                        {
                            Point p = FindOpening(nbd);
                            long[] wk = BuildFrom(p, nbd, nct);
                            for (int i = 2; i <= sz; i++) ans[i] += wk[i - 1];
                        }
                    }
                    else
                    {
                        long[] wk = MakeLoops(ct, bd, npc, target, w);
                        for (int i = 2; i <= sz; i++) ans[i] += wk[i - 1];
                    }
                }
            }
            return ans;
        }

        void TimeStart()
        {
            sw = new Stopwatch();
            sw.Start();
        }
        string TimeStop()
        {
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ("Elapsed: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
        }
    }
}
