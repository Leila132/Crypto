using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SieveOfEratosthenes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static List<uint> sieveEratosthenes( uint length)
        {
            var numbers = new List<uint>();
            
            for (uint i = 2; i < length; i++)                  //заполняем список числами от 2 до length-1
            {
                numbers.Add(i);
            }

            for (var i = 0; i < numbers.Count; i++)
            {
                for (uint j = 2; j < length; j++)
                {                   
                    numbers.Remove(numbers[i] * j);             //удаляем кратные числа из списка
                }
            }
            return numbers;
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (tbIntervalLength.Text == "")
            {
                MessageBox.Show("Введите число!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tbPrintPrimeNumb.Clear();

            uint num;
               
            if (uint.TryParse(tbIntervalLength.Text, out num))
            {
                if(num == 0 || num > 1000000)
                {
                    MessageBox.Show("Введите число от 0 до 1000000", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var prime_numbers = sieveEratosthenes(num);

                for (int i = 0; i < prime_numbers.Count; i++)
                {
                    tbPrintPrimeNumb.Text += prime_numbers[i].ToString() + "    ";
                }
            }
            else
            {
                MessageBox.Show("Исходные данные введены некорректно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
