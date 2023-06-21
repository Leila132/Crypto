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

namespace FastExponentiation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static BigInteger powmod(BigInteger a, BigInteger b, BigInteger n)
        {          
            if (b == 0)
                return 1;
            BigInteger c = powmod(a, b / 2, n);
            if (b % 2 == 0)
            {
                return (c * c) % n;
            }else
            {
                return (a * c * c) % n;
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {

            BigInteger a, b, n;
            bool isnum1 = BigInteger.TryParse(tbInputA.Text, out a);
            bool isnum2 = BigInteger.TryParse(tbInputB.Text, out b);
            bool isnum3 = BigInteger.TryParse(tbInputN.Text, out n);
            if(!isnum1 || !isnum2 || !isnum3 || b < 0 || n == 0)
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;               
            }

            tbResult.Text = powmod(a, b, n).ToString();
        }
    }
}
