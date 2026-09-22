using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FirstDayPractice
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : Window
    {

        public int num1;
        public string calculation;
        public bool newNum = true;


        public Calculator()
        {
            InitializeComponent();
        }



        private void Number_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string number = button.Content.ToString();
                if (newNum)
                {
                    Output.Text = number;
                    newNum = false;
                }
                else
                {
                    Output.Text += number;
                }
            }
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (int.TryParse(Output.Text, out num1))
                {
                    calculation = button.Content.ToString();
                    newNum = true;
                }
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {

            if (int.TryParse(Output.Text, out int num2))
            {
                int result = 0;
                switch (calculation)
                {
                    case "+":
                        result = num1 + num2;
                        break;
                    case "-":
                        result = num1 - num2;
                        break;
                    case "x":
                        result = num1 * num2;
                        break;
                    case "/":
                        if (num2 != 0)
                            result = num1 / num2;
                        else
                        {
                            Output.Text = "Cannot divide by zero!";
                            return;
                        }
                        break;
                }

                Output.Text = result.ToString();
                newNum = true;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = string.Empty;
            num1 = 0;
            calculation = "";
            newNum = true;


        }
    }
}
