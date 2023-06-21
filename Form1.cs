using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CeaserCipher
{
    public partial class Form1 : Form
    {
        private string skiplist = " .,+-*/!@#$%^&()_{}[]`~\"' ";
        private string[] alphabets =                               
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ0123456789"
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

        private int ceaserEncrypt(string alphabet, int key, string message, ref string result) 
        {
            key = key % alphabet.Length;
            if (key < 0)
                key = alphabet.Length + key;
            for(int i = 0; i < message.Length; i++)
            {
                int charIdx = alphabet.IndexOf(message[i]);
                if (charIdx >= 0)
                {
                    result += alphabet[(charIdx + key) % alphabet.Length];                    
                } else
                {
                    if(skiplist.IndexOf(message[i]) < 0)
                        return -(i);                       // такого символа нет в алфавите и в скиплисте
                    result += message[i];
                }   
            }            
            return 1;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                int key = Convert.ToInt32(tbKey.Text);

                textBoxEncrypt.Clear();
                for (int i = 0; i < textBoxOrig.Lines.Length; i++)
                {                    
                    string result = "";
                    string message = textBoxOrig.Lines[i].ToUpper();
                    int rc = ceaserEncrypt(alphabets[cmbLanguage.SelectedIndex], key, message, ref result);
                    if (rc > 0)
                    {
                        textBoxEncrypt.Text += result + "\r\n";
                    } else
                    {
                        MessageBox.Show("В исходном тексте присуствуют недопустимые символы ","Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(String.Format("Значение '{0}' не может быть преобразовано в число", tbKey.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show(String.Format("Введено слишком большое значение ключа '{0}' ", tbKey.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                int key = -Convert.ToInt32(tbKey.Text);

                textBoxDecrypt.Clear();
                for (int i = 0; i < textBoxEncrypt.Lines.Length; i++)
                {
                    string result = "";
                    string message = textBoxEncrypt.Lines[i].ToUpper();
                    int rc = ceaserEncrypt(alphabets[cmbLanguage.SelectedIndex], key, message, ref result);
                    if (rc > 0)
                    {
                        textBoxDecrypt.Text += result + "\r\n";
                    } else
                    {
                        MessageBox.Show("В исходном тексте присуствуют недопустимые символы " , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(String.Format("Значение '{0}' не может быть преобразовано в число", tbKey.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (OverflowException)
            {
                MessageBox.Show(String.Format("Введено слишком большое значение ключа '{0}' ", tbKey.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
