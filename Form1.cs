using System;
using System.Collections;
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
        const int sz = 6;
        Stopwatch sw;
        //Dictionary<BitArray, long[]> dict = new Dictionary<BitArray, long[]>();

        public Form1()
        {
            InitializeComponent();
            this.Show();
            TimeStart();

            BitArray board = new BitArray(sz * sz);
            long[] ar = BuildFrom(0, board);

            long res = 0;
            for (int i = 0; i < ar.Length; i++)
            {
                res += (long)Math.Pow(2, i) * ar[i];
            }
            textBox1.Text = res.ToString();
            label1.Text = TimeStop();
        }

        long[] BuildFrom(int st, BitArray bd)
        {
            //if (dict.ContainsKey(bd)) return dict[bd];

            BitArray pc = new BitArray(sz * sz);
            pc[st] = true;
            pc[st + 1] = true;
            int targ = st + sz;
            long[] ans = MakeLoops(bd, pc, targ, st + 1);
            //dict.Add(bd, ans);
            return ans;
        }

        bool StillOkay(BitArray bd)
        {
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    int w = y * sz + x;
                    if (!bd[w])
                    {
                        int c = 0;
                        if (x + 1 < sz && !bd[w + 1]) c++;
                        if (y + 1 < sz && !bd[w + sz]) c++;
                        if (x > 0 && !bd[w - 1]) c++;
                        if (y > 0 && !bd[w - sz]) c++;
                        if (c < 2) return false;
                    }
                }
            }
            return true;
        }
        bool StillOkayWIP(BitArray bd, BitArray bd2, int curr, int target)
        {
            for (int y = 0; y < sz; y++)
            {
                for (int x = 0; x < sz; x++)
                {
                    int w = y * sz + x;
                    if (!bd[w] && !bd2[w])
                    {
                        int c = 0;
                        if (x + 1 < sz && !bd[w + 1] && !bd2[w + 1]) c++;
                        if (y + 1 < sz && !bd[w + sz] && !bd2[w + sz]) c++;
                        if (x > 0 && !bd[w - 1] && !bd2[w - 1]) c++;
                        if (y > 0 && !bd[w - sz] && !bd2[w - sz]) c++;
                        if (c < 2)
                        {
                            if (curr == w) ;
                            else if (target == w && c == 1) ;
                            else if (curr % sz == x && Math.Abs((curr / sz) - y) == 1) ;
                            else if (curr / sz == y && Math.Abs((curr % sz) - x) == 1) ;
                            else return false;
                        }
                    }
                }
            }
            return true;
        }
        int FindOpening(BitArray bd)
        {
            BitArray pc = new BitArray(sz * sz);
            for (int y = 0; y + 1 < sz; y++)
            {
                for (int x = 0; x + 1 < sz; x++)
                {
                    int i = (y * sz) + x;
                    if (!bd[i] && !bd[i + 1] && !bd[i + sz]) return i;
                }
            }
            return -1;
        }

        long[] MakeLoops(BitArray bd, BitArray pc, int target, int curr)
        {
            long[] ans = new long[(sz * sz / 4) + 1];
            for (int d = 1; d < 5; d++)
            {
                bool ok = true;

                int w = curr;
                if (d == 1) // right
                {
                    if ((w + 1) % sz == 0) ok = false;
                    else w++;
                }
                else if (d == 2) // down
                {
                    if ((w / sz) + 1 == sz) ok = false;
                    else w += sz;
                }
                else if (d == 3) // left
                {
                    if (w % sz == 0) ok = false;
                    else w--;
                }
                else // up
                {
                    if (w / sz == 0) ok = false;
                    else w -= sz;
                }
                if (ok) // is this an empty cell?
                {
                    if (bd[w] || pc[w]) ok = false;
                }
                if (ok) // any unreachables created?
                {
                    if (!StillOkayWIP(bd, pc, w, target)) ok = false;
                }

                if (ok) // this direction is legitimate
                {
                    BitArray npc = new BitArray(sz * sz);
                    for (int i = 0; i < npc.Length; i++) npc[i] = pc[i];
                    npc[w] = true;
                    if (w == target) // closed the loop
                    {
                        bool allFull = true;
                        BitArray nbd = new BitArray(sz * sz);
                        for (int i = 0; i < npc.Length; i++) 
                        {
                            if (bd[i] || npc[i]) nbd[i] = true;
                            else allFull = false;
                        }
                        if (allFull) ans[1]++; // packed the whole board
                        else if (StillOkay(nbd)) // space left and all squares have at least 2 approaches
                        {
                            int p = FindOpening(nbd);
                            long[] wk = BuildFrom(p, nbd);
                            for (int i = 2; i < wk.Length; i++) ans[i] += wk[i - 1];
                        }
                    }
                    else
                    {
                        long[] wk = MakeLoops(bd, npc, target, w);
                        for (int i = 1; i < ans.Length; i++) ans[i] += wk[i];
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
