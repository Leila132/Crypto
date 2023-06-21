using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VigenereCipher
{
    public partial class Form1 : Form
    {
        private string skiplist = " .,+-*/!@#$%^&()_{}[]`~\"' ";
        private string[] alphabets =
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ0123456789",

        };
        private string[] key_alphabets =
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
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
            cmbLanguage.BackColor = Color.White;
        }

        private int vigenereEncrypt(string alphabet, string key, string message, ref string result)
        {
            string keynum = "";
            for (int j = 0; j < message.Length; j++)
            {
                keynum += key[j % key.Length];
            }
            for (int i = 0; i < message.Length; i++)
            {
                if (char.IsWhiteSpace(textBoxOrig.Text[i])) result += " ";
                else
                {
                    int charIdx = alphabet.IndexOf(message[i]);
                    if (charIdx >= 0)
                    {
                        result += alphabet[(charIdx + alphabet.IndexOf(keynum[i]) + 1) % alphabet.Length];

                    }
                    else
                    {
                        if (skiplist.IndexOf(message[i]) < 0)
                            return -(i);                       // такого символа нет в алфавите и в скиплисте
                        result += message[i];
                    }
                }
            }
            return 1;
        }

        private int vigenereDecrypt(string alphabet, string key, string message, ref string result)
        {
            string keynum = "";
            for (int j = 0; j < message.Length; j++)
            {
                keynum += key[j % key.Length];
            }
            for (int i = 0; i < message.Length; i++)
            {
                if (char.IsWhiteSpace(textBoxOrig.Text[i])) result += " ";
                else
                {
                    int charIdx = alphabet.IndexOf(message[i]);
                    if (charIdx >= 0)
                    {
                        result += alphabet[(alphabet.Length + charIdx - alphabet.IndexOf(keynum[i]) - 1) % alphabet.Length];
                    }
                    else
                    {
                        if (skiplist.IndexOf(message[i]) < 0)
                            return -(i);                       // такого символа нет в алфавите и в скиплисте
                        result += message[i];
                    }
                }
            }
            return 1;
        }

        private void btnEncrypt_Click_1(object sender, EventArgs e)
        {
            if (tbKey.Text == "")
            {
                MessageBox.Show("Введите ключ шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxOrig.Text == "")
            {
                MessageBox.Show("Введите текст для шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxEncrypt.Clear();

            string key = tbKey.Text.ToUpper();
            for (int j = 0; j < key.Length; j++)
            {
                int key_index = key_alphabets[cmbLanguage.SelectedIndex].IndexOf(key[j]);
                if (key_index < 0)
                {
                    textBoxEncrypt.Text = "";
                    MessageBox.Show("Введен недопустимый ключ!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                    return;
                }
            }

            for (int i = 0; i < textBoxOrig.Lines.Length; i++)
            {
                string result = "";
                string message = textBoxOrig.Lines[i].ToUpper();

                int rc = vigenereEncrypt(alphabets[cmbLanguage.SelectedIndex], key, message, ref result);
                if (rc > 0)
                {
                    textBoxEncrypt.Text += result + "\r\n";
                }
                else
                {
                    MessageBox.Show("В исходном тексте присуствуют недопустимые символы! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void btnDecrypt_Click_1(object sender, EventArgs e)
        {
            if (tbKey.Text == "")
            {
                MessageBox.Show("Введите ключ шифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxEncrypt.Text == "")
            {
                MessageBox.Show("Введите текст для дешифрования!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxDecrypt.Clear();

            string key = tbKey.Text.ToUpper();
            for (int j = 0; j < key.Length; j++)
            {
                int key_index = key_alphabets[cmbLanguage.SelectedIndex].IndexOf(key[j]);
                if (key_index < 0)
                {
                    textBoxDecrypt.Text = "";
                    MessageBox.Show("Введен недопустимый ключ!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            for (int i = 0; i < textBoxEncrypt.Lines.Length; i++)
            {
                string result = "";
                string message = textBoxEncrypt.Lines[i].ToUpper();

                int rc = vigenereDecrypt(alphabets[cmbLanguage.SelectedIndex], key, message, ref result);
                if (rc > 0)
                {
                    textBoxDecrypt.Text += result + "\r\n";
                }
                else
                {
                    MessageBox.Show("В исходном тексте присуствуют недопустимые символы!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tbKey_TextChanged(object sender, EventArgs e)
        {

        }
    }
}








