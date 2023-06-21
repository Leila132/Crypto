using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;


//N 344572667627327574872986520507
//e 303498613397661458186613409505
//SW 42393687358080034300726554172
//16 61 66 56 49 56 30 66 56 58
// А н  т  и  б  и  О  т  И  К

namespace HackingRSA
{
    public partial class Form1 : Form
    {
        private static Random rnd = new Random(Environment.TickCount);
        private UInt64 rho_pollard_iterations;
        private long rho_pollard_startTime;
        private bool rho_pollard_terminate;

        public Form1()
        {
            InitializeComponent();
        }

        private BigInteger calc_gcd(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            BigInteger x1 = 0, y1 = 0;

            BigInteger d = calc_gcd(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        public BigInteger calc_d(BigInteger phiN, BigInteger E)
        {
            BigInteger x = 0, y = 0;
            calc_gcd(phiN, E, ref x, ref y);

            return y < 0 ? y + phiN : y;
        }

        BigInteger rho_pollard(BigInteger n)
        {
            rho_pollard_iterations = 0;
            rho_pollard_terminate = false;
            rho_pollard_startTime = DateTime.Now.Ticks;

            BigInteger x = rnd.Next(2, int.MaxValue);
            BigInteger y = 1;
            BigInteger i = 0;
            BigInteger stage = 2;
            BigInteger gcd;

            while ((gcd = BigInteger.GreatestCommonDivisor(n, BigInteger.Abs(x - y))) == 1)
            {
                if (i == stage)
                {
                    y = x;
                    stage = stage * 2;
                }
                x = BigInteger.ModPow(x, 2, n) + 1;
                i = i + 1;

                if (++rho_pollard_iterations % 1024 == 0)
                {
                    numIterations.Text = rho_pollard_iterations.ToString();
                    tbTime.Text = ((DateTime.Now.Ticks - rho_pollard_startTime)/TimeSpan.TicksPerSecond).ToString();
                    Application.DoEvents();
                }
                
                if (rho_pollard_terminate == true)
                    return 1;
            }
            return gcd;
        }

        private string rsaDecrypt(BigInteger msg, BigInteger d_, BigInteger n_)
        {
            BigInteger decr = BigInteger.ModPow(msg, d_, n_);

            string result = "";

            string buff = decr.ToString();
            if (buff.Length % 2 != 0)
                buff = "0" + buff;  // если количество символов в строке будет не четным - добавим ведущий 0
            while(buff.Length != 0)
            {
                int k = 0;
                if (Int32.TryParse(buff.Substring(0, 2), out k))
                {                    
                    result += Convert.ToChar(k + 1024);
                }
                buff = buff.Remove(0, 2);
            }
            return result;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (tb_n.Text == "" || tb_e.Text == "" || tbOriginal.Text == "")
            {
                MessageBox.Show("Введите число!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tbDecrypted.Clear();
            tb_p.Text = "calculating...";
            tb_q.Text = "calculating...";

            BigInteger N = BigInteger.Parse(tb_n.Text);
            
            btnDecrypt.Enabled = false;     // отключаем кнопку, чтобы нельзя было нажать ее второй раз
            btnStop.Enabled = true;         // включаем кнопку остановки 
            BigInteger p = rho_pollard(N);
            btnStop.Enabled = false;
            btnDecrypt.Enabled = true;            
            if (!rho_pollard_terminate)
            {                
                tb_p.Text = p.ToString();
                BigInteger q = N / p;
                tb_q.Text = q.ToString();

                BigInteger phiN = (1 - p) * (1 - q);
                BigInteger E = BigInteger.Parse(tb_e.Text);
                BigInteger d = calc_d(phiN, E);

                BigInteger msg = BigInteger.Parse(tbOriginal.Text);
                tbDecrypted.Text = rsaDecrypt(msg, d, N);
            } else
            {
                tb_q.Text = tb_p.Text = "terminated";
            }
        }            

        private void btnStop_Click(object sender, EventArgs e)
        {
            rho_pollard_terminate = true;
        }
    }
}
