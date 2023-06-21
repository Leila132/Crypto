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
using System.Security.Cryptography;


// онлайн каолькулятор простых чисел для проверки
// https://ru.numberempire.com/primenumbers.php

namespace Miller_RabinTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static bool miller_rabin_test(BigInteger num)
        {           
            if (num == 2 || num == 3)
                return true;
            if (num % 2 == 0 || num < 2)
                return false;

            BigInteger t = num - 1;
            int s = 0;

            while(t % 2 == 0)
            {
                t /= 2;
                s++;
            }
            double k = BigInteger.Log(num, 2);

            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();  //криптографический генератор случайных чисел
            for (uint i = 0; i < k; i++)
            {                
                byte[] c = new byte[num.ToByteArray().LongLength];

                BigInteger a;

                do
                {
                    rngCSP.GetBytes(c);
                    a = new BigInteger(c);
                }while (a < 2 || a > num - 2);

                BigInteger b = BigInteger.ModPow(a, t, num);
                if (b == 1 || b == num - 1)
                    continue;

                for(uint j = 1; j < s; j++)
                {
                    b = BigInteger.ModPow(b, 2, num);

                    if (b == 1)
                        return false;
                    if (b == num - 1)
                        break;
                }
                if (b != num - 1)
                    return false;
            }
            return true;
        }


        static BigInteger generate_prime_number(uint nbits)
        {
            BigInteger result;

            if (nbits == 0)
                nbits = 4;
            if (nbits > 4096)
                nbits = 4096;

            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
            
            //потребуется в среднем ln(T) итераций
       
            Byte[] dummy = new Byte[nbits / 8];          // генерируем случайное число заданной длины
            rngCSP.GetBytes(dummy);                      // генерируем простое случайное число
            result = new BigInteger(dummy);              // если число четное добавляем единицу - т.к. простые числа нечетные
            if (result < 0)
                result = -result;                        // случайное число должно быть положительным
            if (result % 2 == 0)
                result += 1;
            while(miller_rabin_test(result) == false)      // проверям число тестом miller_rabin_test и добавлеям к нему +2 пока оно не пройдет тест.
            {
                result += 2;
            }
            return result;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (tbNumber.Text == "")
            {
                MessageBox.Show("Введите число!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }       

            BigInteger num;  

            tbResult.Clear();
            bool isnum = BigInteger.TryParse(tbNumber.Text, out num);
           
            if (!isnum )
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool tested = miller_rabin_test(num);
            if(tested)
            {
                tbResult.AppendText("Число вероятно простое");
            }else
            {
                tbResult.AppendText("Число составное");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbNumbLength.Text == "")
            {
                MessageBox.Show("Введите длину генерируемого числа!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            uint n = 4;
            if (uint.TryParse(tbNumbLength.Text, out n))
            {
                BigInteger primeNumber = generate_prime_number(n);
                textBox2.Text = primeNumber.ToString();
            }
            else
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
