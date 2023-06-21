using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XORCipher
{
    public partial class MainForm : Form
    {
        private string skiplist = " ";
        private string[] alphabets =
        {
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,+-*/!@#$%^&()_{}[]`~\'|;№>:", 
            "0123456789АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ.,+-*/!@#$%^&()_{}[]`",
        };
        private int bitsPerSymbol = 6;

        public MainForm()
        {
            InitializeComponent();
            Make_New_Style();
        }
        private void Make_New_Style()
        {
            cmbLanguage.Items.Add("ENG");
            cmbLanguage.Items.Add("RU");
            cmbLanguage.SelectedIndex = 0;
            cmbLanguage.BackColor = Color.White;
        }

        private int convert2binary(string alphabet, string message, ref string result)
        {
            for (int i = 0; i < message.Length; i++)
            {
                int charIdx = alphabet.IndexOf(message[i]);
                if (charIdx >= 0)
                {
                    for(int j = bitsPerSymbol-1; j >= 0; --j)
                    {
                        if((charIdx & (1<<j)) == 0)
                            result += '0';
                        else
                            result += '1';
                    }
                    result += ' ';
                } else
                {
                    if (skiplist.IndexOf(message[i]) < 0)
                        return -(i + 1);                       // такого символа нет в алфавите и в скиплисте
                    result += message[i];
                }
            }
            return result.Length;
        }

        private int xorcrypt(string alphabet, string key, string message, ref string result, bool decrypt = false)
        {
            for (int i = 0; i < message.Length; i++)
            {
                int symbolIdx = alphabet.IndexOf(message[i]);
                if (symbolIdx >= 0)
                {
                    int keyIdx = alphabet.IndexOf(key[i % key.Length]);
                    if (keyIdx < 0)
                        return -(i + 1);
                    
                    if (decrypt == false)
                    {
                        //result += alphabet[(symbolIdx + keyIdx) % alphabet.Length];   
                        result += alphabet[symbolIdx ^ keyIdx];
                    } else
                    {
                        //result += alphabet[(symbolIdx + (alphabet.Length - keyIdx)) % alphabet.Length];
                        result += alphabet[symbolIdx ^ keyIdx];
                    }
                    
                } else
                {
                    if (skiplist.IndexOf(message[i]) < 0)
                        return -(i + 1);                       // такого символа нет в алфавите и в скиплисте
                    result += message[i];
                }
            }
            return result.Length;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {

            if (tbKey.Text == "" )
            {
                MessageBox.Show("Введите ключ шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxOrig.Text == "")
            {
                MessageBox.Show("Введите текст для шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxKeyLength.Text == "")
            {
                MessageBox.Show("Введите длину ключа шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string buffer = "";
            string result = "";


            string key = tbKey.Text.ToUpper();
            if (convert2binary(alphabets[cmbLanguage.SelectedIndex], key, ref result) < 0)
            {
                MessageBox.Show("В исходном тексте присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxKeyBinary.Clear();
            textBoxKeyBinary.Text += result;

            textBoxOrigBinary.Clear();
            textBoxEncrypt.Clear();
            textBoxEncryptBinary.Clear();
            for (int i = 0; i < textBoxOrig.Lines.Length; i++)
            {
                result = "";
                buffer = textBoxOrig.Lines[i].ToUpper();
                if (convert2binary(alphabets[cmbLanguage.SelectedIndex], buffer, ref result) < 0)
                {
                    MessageBox.Show("В исходном тексте присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                textBoxOrigBinary.Text += result + "\r\n";

                result = "";
                if (xorcrypt(alphabets[cmbLanguage.SelectedIndex], key, buffer, ref result) < 0)
                {
                    MessageBox.Show("В исходном тексте присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                textBoxEncrypt.Text += result + "\r\n";

                buffer = result;
                result = "";
                convert2binary(alphabets[cmbLanguage.SelectedIndex], buffer, ref result);
                textBoxEncryptBinary.Text += result;                
            }
        }


        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            try
            {               
                int keyLength = Convert.ToInt32(textBoxKeyLength.Text);
                if (keyLength <= 0)
                    keyLength = 1;
                else if (keyLength > 10000)
                    keyLength = 10000;
                textBoxKeyLength.Text = keyLength.ToString();
                tbKey.Clear();
                tbKey.Text = generateKey(alphabets[cmbLanguage.SelectedIndex], keyLength);
            }
            catch (FormatException)
            {
                MessageBox.Show("Присуствуют недопустимые символы в записи длины ключа", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Присуствуют недопустимые символы в записи длины ключа", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private int calc1Bits(int value, int bitsCount)
        {
            if(bitsCount <= 0)
                return -1;

            int result = 0;
            for(int i = bitsCount; i >= 0; --i)
            {
                if((value & (1 << i)) != 0)
                    result += 1;
            }
            return result;
        }

        private string generateKey(string alphabet, int keyLen)
        {
            Random rnd = new Random();
            string result = "";

            int bitsCount = -1;
            for (int i = 0; i < keyLen; i++)
            {
                int symbolIdx;
                char symbol;
                int k = 0;
                do
                {
                    do
                    {
                        symbolIdx = rnd.Next(0, alphabet.Length);
                        symbol = alphabet[symbolIdx];
                    } while (!Char.IsLetter(symbol));
                } while ((bitsCount != -1 && (bitsCount != bitsPerSymbol - calc1Bits(symbolIdx, bitsPerSymbol)) && ++k < alphabet.Length) ||
                        bitsCount == -1 && i == keyLen-1 && (calc1Bits(symbolIdx, bitsPerSymbol) != bitsPerSymbol/2) && ++k < alphabet.Length);
                if (bitsCount == -1)
                    bitsCount = calc1Bits(symbolIdx, bitsPerSymbol);
                else
                    bitsCount = -1;
                result += symbol;
            }

            return result;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            textBoxDecrypt.Clear();
            textBoxDecryptBinary.Clear();
            for (int i = 0; i < textBoxEncrypt.Lines.Length; i++)
            {
                string result = "";
                string buffer = textBoxEncrypt.Lines[i].ToUpper();

                if (xorcrypt(alphabets[cmbLanguage.SelectedIndex], tbKey.Text.ToUpper(), buffer, ref result, true) < 0)
                {
                    MessageBox.Show("Присуствуют недопустимые символы ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                textBoxDecrypt.Text += result + "\r\n";

                buffer = result;
                result = "";
                convert2binary(alphabets[cmbLanguage.SelectedIndex], buffer, ref result);
                textBoxDecryptBinary.Text += result;
            }
        }

    }
}