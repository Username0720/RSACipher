using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Numerics;
using System.Linq;

namespace RSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        char[] characters = new char[]
        { 
            '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
            'Э', 'Ю', 'Я', 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и',
            'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с',
            'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ы', 'ъ',
            'э', 'ю', 'я', ' ', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', '0' 
        };
        //рекурсивный алгоритм Евклида для нахождения НОД 2 больших чисел
        BigInteger gcd(BigInteger p, BigInteger q)
        {
            BigInteger a = p;
            BigInteger b = q;

            if (a == b || a == 1)
            {
                return a;
            }
            if (a > b)
            {
                BigInteger tmp = a;
                a = b;
                b = tmp;
            }
            return gcd(a, b - a);
        }

        //с использованием алгоритма Евклида проверяем 2 числа на простоту
        private bool IsTheNumberSimple(BigInteger p, BigInteger q)
        {
            BigInteger nn = gcd(p, q);
            if (nn == 1 || nn == p || nn == q)
                return true;
            else return false;
        }

        //вычисление параметра d. d должно быть взаимно простым с m
        //здесь также используем алгоритм Евклида
        private BigInteger Calculate_d(BigInteger m)
        {
            BigInteger d = m - 1;
            if (IsTheNumberSimple(d, m))//функция для проверки 2 чисел, которая использует алгоритм Евклида
                return d;
            else
                return Calculate_d(d - 1);
            //если числа не взаимно простые вызываем рекурсию с изменением параметра на -1
        }

        //вычисление параметра e
        private ulong Calculate_e(BigInteger d, BigInteger m)
        {
            ulong e = 3;

            while (true)
            {
                if ((e * d) % m == 1)
                    break;
                else
                    e++;
            }

            return e;
        }

        //метод для Cipher
        private List<string> RSA_Endoce(string sr, BigInteger e, BigInteger n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < sr.Length; i++)
            {
                long index = Array.IndexOf(characters, sr[i]);

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)e);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                result.Add(bi.ToString() + Environment.NewLine);
            }

            return result;
        }

        //метод для Decipher
        private string RSA_Dedoce(List<string> input, BigInteger d, BigInteger n)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToDouble(item));
                bi = BigInteger.Pow(bi, (int)d);

                BigInteger n_ = n;
                bi = bi % n_;
                while (bi > characters.Length)
                    bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += characters[index].ToString();
            }

            return result;
        }

        //Cipher
        private void button1_Click(object sender, EventArgs e)
        {
            ulong p = Convert.ToUInt64(textBox1.Text);
            ulong q = Convert.ToUInt64(textBox2.Text);

            if ((textBox1.Text.Length > 0) && (textBox2.Text.Length > 0) && (p != 0) && (q != 0))
            {

                if (IsTheNumberSimple(p, q))
                {
                    string items = "";
                    string sr = textBox5.Text;

                    BigInteger n = p * q;
                    BigInteger m = (p - 1) * (q - 1);
                    BigInteger d = Calculate_d(m);
                    BigInteger e_ = Calculate_e(d, m);

                    List<string> result = RSA_Endoce(sr, e_, n);

                    foreach (string item in result)
                        items += item;
                    textBox6.Text = items;
                    textBox3.Text = d.ToString();
                    textBox4.Text = n.ToString();
                    label8.Text = "Cipher";
                }
                else
                    MessageBox.Show("p или q - не простые числа!");
            }
            else
                MessageBox.Show("Введите p и q!");
        }

        //Decipher
        private void button2_Click(object sender, EventArgs e)
        {
            if ((textBox3.Text.Length > 0) && (textBox4.Text.Length > 0))
            {
                BigInteger d = Convert.ToUInt64(textBox3.Text);
                BigInteger n = Convert.ToUInt64(textBox4.Text);
                string text = textBox6.Text;
                string[] stringSeparators =  { "\n" };
                List<string> input = text.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

                string result = RSA_Dedoce(input, d, n);
                label8.Text = "Decipher";
                textBox6.Text = result;
            }
            else
                MessageBox.Show("Введите секретный ключ!");
        }
    }
}
