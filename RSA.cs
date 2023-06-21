using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace RSA
{

    public partial class Form1 : Form
    {
        private readonly string[] alphabets =
        {
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,+-*/!@#$%^&()_{}[]~\'|;№>` ",    
            "0123456789АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ@#$%^&()_{}[]~\'|;№>` ",
        };

        public Form1()
        {
            InitializeComponent();
            Make_New_Style();
        }
        private void Make_New_Style()
        {
            cmbLanguage.Items.Add("ENG");
            cmbLanguage.Items.Add("RU");
            cmbLanguage.SelectedIndex = 0;          
        }
        public static string BigIntegerToHexString(BigInteger num, uint lenbytes)
        {
            string result = "";
            if (lenbytes <= 0)
                return result;
            result = num.ToString("X");
            while (lenbytes * 2 > result.Length)
                result = "0" + result;
            return result;
        }

        public static uint nbits2nbytes(uint nbits)
        {
            if(nbits == 0)
                return 0;
            return (uint)((nbits / 8) + ((nbits % 8) == 0 ? 0 : 1));
        }

        static bool miller_rabin_test(BigInteger num)
        {
            if (num == 2 || num == 3)
                return true;
            if (num % 2 == 0 || num < 2)
                return false;

            BigInteger t = num - 1;
            int s = 0;

            while (t % 2 == 0)
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
                } while (a < 2 || a > num - 2);

                BigInteger b = BigInteger.ModPow(a, t, num);
                if (b == 1 || b == num - 1)
                    continue;

                for (uint j = 1; j < s; j++)
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
                nbits = 8;
            if (nbits > 4096)
                nbits = 4096;

            RNGCryptoServiceProvider rngCSP = new RNGCryptoServiceProvider();
                        
            Byte[] dummy = new Byte[nbits2nbytes(nbits)];          // генерируем случайное число заданной длины
            rngCSP.GetBytes(dummy);                      // генерируем простое случайное число
            result = new BigInteger(dummy);              // если число четное добавляем единицу - т.к. простые числа нечетные
            if (result < 0)
                result = -result;                        // случайное число должно быть положительным
            if (result % 2 == 0)
                result += 1;
            while (miller_rabin_test(result) == false)  // проверям число тестом miller_rabin_test и добавлеям к нему +2 пока оно не пройдет тест.
            {
                result += 2;
            }
            return result;
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
        
        public BigInteger calc_e(uint l, BigInteger phiN)
        {
            uint k = l / 3;
            if(k < 8)
                k = 8;  // не может быть меньше 8 бит

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            uint nbytes = nbits2nbytes(k);
            byte[] _a = new byte[nbytes];
            BigInteger a;

            rng.GetBytes(_a);
            a = new BigInteger(_a);
            if (a < 0)
            {
                a = -a;
            }
            BigInteger x = 0, y = 0;
            while (calc_gcd(a, phiN, ref x, ref y) != 1)
            {
                a++;
            }
            return a;
        }

        public BigInteger calc_d(BigInteger phiN, BigInteger E)
        {
            BigInteger x = 0, y = 0;
            calc_gcd(phiN, E, ref x, ref y);

            return y < 0 ? y + phiN : y;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            uint l;            
            if (uint.TryParse(tbNumbLength.Text, out l))
            {
                if(l < 16)
                {
                    MessageBox.Show("Введите ключ меньшей длины!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
               
                uint nbytes = nbits2nbytes(l);
                BigInteger p = generate_prime_number(l/2);
                tb_p.Text = BigIntegerToHexString(p, nbytes / 2);
                BigInteger q = generate_prime_number(l/2);
                tb_q.Text = BigIntegerToHexString(q, nbytes / 2);

                BigInteger n = p * q;
                tb_n.Text = BigIntegerToHexString(n, nbytes);

                BigInteger phiN = (1 - p) * (1 - q);
                tb_f.Text = BigIntegerToHexString(phiN, nbytes);

                BigInteger E = calc_e(l, phiN);
                tb_e.Text = BigIntegerToHexString(E, nbytes / 2);

                BigInteger d = calc_d(phiN, E);
                tb_d.Text = BigIntegerToHexString(d, nbytes);

            } else
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private int rsaCrypt(string alphabet, string message, ref string result, string E_, string n_)
        {
            BigInteger E = BigInteger.Parse(E_, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger n = BigInteger.Parse(n_, System.Globalization.NumberStyles.AllowHexSpecifier);
            int kLen = n_.Length /2;
            int blockLen = kLen/2;

            result = "";
            for (int i = 0; i < message.Length; i+= blockLen)
            {
                Byte[] dummy = new Byte[kLen];
                for (int j = 0; j < blockLen && (i+j) < message.Length; j++)
                {
                    int symbolIdx = alphabet.IndexOf(message[i + j]);
                    if (symbolIdx >= 0)
                    {
                        dummy[j] = (byte)symbolIdx;
                    } else
                    {
                        return -(i + 1);                       // такого символа нет в алфавите
                    }
                }

                BigInteger bi = new BigInteger(dummy);
                
                BigInteger ci = BigInteger.ModPow(bi, E, n);
                Byte[] dummy2 = ci.ToByteArray();

                result += BigIntegerToHexString(ci, (uint)kLen);
            }
            return result.Length;
        }


        private int rsaDecrypt(string alphabet, string message, ref string result, string d_, string n_)
        {
            BigInteger d = BigInteger.Parse(d_, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger n = BigInteger.Parse(n_, System.Globalization.NumberStyles.AllowHexSpecifier);
            int kLen = n_.Length / 2;
            int blockLen = kLen / 2;

            result = "";
            for (int i = 0; i < (message.Length) && i + kLen * 2 <= message.Length; i += kLen*2)
            {
                string buff = message.Substring(i, kLen * 2);
                BigInteger ci = BigInteger.Parse(buff, System.Globalization.NumberStyles.AllowHexSpecifier);
                BigInteger bi = BigInteger.ModPow(ci, d, n);

                Byte[] dummy = bi.ToByteArray();
                for (int j = 0; j < blockLen && j < dummy.Length; j++)
                {
                    if(dummy[j] < alphabet.Length)
                    {
                        result += alphabet[dummy[j]];
                    } else
                    {
                        result += "?";     
                    }                    
                }
            }
            return result.Length;
        }


        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string buffer = "";
            string result = "";

            tbEncrypt.Clear();
            for (int i = 0; i < tbOriginal.Lines.Length; i++)
            {
                buffer = tbOriginal.Lines[i].ToUpper();

                if (rsaCrypt(alphabets[cmbLanguage.SelectedIndex], buffer, ref result, tb_e.Text, tb_n.Text) < 0)
                {
                    MessageBox.Show("Присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tbEncrypt.Text += result + "\r\n";
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string buffer = "";
            string result = "";

            tbDecrypt.Clear();
            for (int i = 0; i < tbEncrypt.Lines.Length; i++)
            {
                buffer = tbEncrypt.Lines[i];
                if (rsaDecrypt(alphabets[cmbLanguage.SelectedIndex], buffer, ref result, tb_d.Text, tb_n.Text) < 0)
                {
                    MessageBox.Show("Присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tbDecrypt.Text += result + "\r\n";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
