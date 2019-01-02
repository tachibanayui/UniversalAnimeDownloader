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

namespace UniversalAnimeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for SneakyWatcherPasswordBox.xaml
    /// </summary>
    public partial class SneakyWatcherPasswordBox : Window
    {
        private string PasswordText = string.Empty;
        private Random rand = new Random();
        private bool isRandomizeTextBox;

        public SneakyWatcherPasswordBox()
        {
            InitializeComponent();
        }

        private void RandomizeCharacter(object sender, TextChangedEventArgs e)
        {
            if(isRandomizeTextBox)
            {
                if (randomCharBox.Text.Length == 0)
                {
                    PasswordText = string.Empty;
                    return;
                }

                if (randomCharBox.Text.Length > PasswordText.Length)
                {
                    string currentStr = randomCharBox.Text;
                    string stringToAdd = currentStr.Substring(PasswordText.Length);
                    PasswordText += stringToAdd;
                    currentStr = currentStr.Substring(0, currentStr.Length - stringToAdd.Length);
                    for (int i = 0; i < stringToAdd.Length; i++)
                        currentStr += (char)rand.Next(97, 122);

                    randomCharBox.Text = currentStr;
                    randomCharBox.CaretIndex = randomCharBox.Text.Length;
                }
                else
                    PasswordText = PasswordText.Substring(0, randomCharBox.Text.Length);
            }
            else
            {
                PasswordText = randomCharBox.Text;
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            randomCharBox.Text = string.Empty;
            PasswordText = string.Empty;
        }

        public bool ValidatePassword(string correctPassword, bool randomizePasswordTextbox = true)
        {
            isRandomizeTextBox = randomizePasswordTextbox;
            ShowDialog();
            return PasswordText == correctPassword;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
