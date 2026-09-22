using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstDayPractice
{
    /// <summary>
    /// Interaction logic for CalculatorPage.xaml
    /// </summary>
    public partial class CalculatorPage : Page
    {
        private double num1 = 0;
        private string calculation = "";
        private bool newNum = true;
        

        public CalculatorPage()
        {
            InitializeComponent();
            Loaded += (s, e) => Output.Focus(); // Focus textbox on load
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                HandleNumberInput(button.Content.ToString());
            }
        }

        private void HandleNumberInput(string input)
        {
            if (newNum)
            {
                Output.Text = input == "." ? "0." : input;
                newNum = false;
            }
            else if (input == "." && Output.Text.Contains("."))
            {
                // Prevent multiple decimals
                return;
            }
            else
            {
                Output.Text += input;
            }
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                HandleOperator(button.Content.ToString());
            }
        }

        private void HandleOperator(string op)
        {
            if (double.TryParse(Output.Text, out double currentNum))
            {
                if (!string.IsNullOrEmpty(calculation))
                {
                    Calculate(currentNum);
                }
                else
                {
                    num1 = currentNum;
                }

                calculation = op;
                newNum = true;
                Output.Text = num1.ToString();
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Output.Text, out double num2))
            {
                Calculate(num2);
                Output.Text = num1.ToString();
                calculation = "";
                newNum = true;
            }
        }

        private void Calculate(double num2)
        {
            switch (calculation)
            {
                case "+":
                    num1 += num2;
                    break;
                case "-":
                    num1 -= num2;
                    break;
                case "x":
                    num1 *= num2;
                    break;
                case "/":
                    if (num2 != 0)
                        num1 /= num2;
                    else
                    {
                        Output.Text = "Cannot divide by zero!";
                        calculation = "";
                    }
                    break;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";
            num1 = 0;
            calculation = "";
            newNum = true;
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                HandleNumberInput((e.Key - Key.D0).ToString());
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                HandleNumberInput((e.Key - Key.NumPad0).ToString());
            else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                HandleNumberInput(".");
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
                HandleOperator("+");
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                HandleOperator("-");
            else if (e.Key == Key.Multiply)
                HandleOperator("x");
            else if (e.Key == Key.Divide || e.Key == Key.Oem2)
                HandleOperator("/");
            else if (e.Key == Key.Enter || e.Key == Key.Return)
                Equals_Click(null, null);
            else if (e.Key == Key.Back)
                HandleBackspace();

            e.Handled = true;
        }

        private void HandleBackspace()
        {
            if (!newNum && !string.IsNullOrEmpty(Output.Text))
            {
                Output.Text = Output.Text.Substring(0, Output.Text.Length - 1);
                if (string.IsNullOrEmpty(Output.Text))
                {
                    Output.Text = "0";
                    newNum = true;
                }
            }
        }

    }
}
