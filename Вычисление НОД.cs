using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;


namespace gcd
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private BigInteger calc_gcd(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if(a == 0)
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Clear();

            BigInteger a; 
            BigInteger b;
            BigInteger x = 0; 
            BigInteger y = 0;
            
            bool isnum1 = BigInteger.TryParse(textBox1.Text, out a);
            bool isnum2 = BigInteger.TryParse(textBox2.Text, out b);
            if(!isnum1 || !isnum2)
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BigInteger gcd= calc_gcd(a, b, ref x, ref y);
            textBox3.AppendText("НОД: " + gcd.ToString() + Environment.NewLine);
            textBox3.AppendText("x: " + x.ToString() + Environment.NewLine);
            textBox3.AppendText("y: " + y.ToString() + Environment.NewLine);

            string expression = "a*x + b*y = НОД (a,b): " + Environment.NewLine + "(" + a.ToString() + ") * (" + x.ToString() + ") + " +
                                "(" + b.ToString() + ") * (" + y.ToString() + ") = " + gcd.ToString();
            textBox3.AppendText(expression + Environment.NewLine);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
