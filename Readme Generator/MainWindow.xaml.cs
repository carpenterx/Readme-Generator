using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Readme_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private int currentMatchIndex;
        //private MatchCollection matches;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestTab(object sender, KeyEventArgs e)
        {
            bool snippetMode = true;
            if (e.Key == Key.Tab)
            {
                
                if (snippetMode)
                {
                    FindNextMatch();
                    e.Handled = true;
                }
            }
        }

        private void FindNextMatch()
        {
            try
            {
                Regex rgx = new Regex(@"\$\S+\$");
                string testString = testTxt.Text;

                MatchCollection matches = rgx.Matches(testString);

                if(matches.Count > 0)
                {
                    FocusTextBox(testTxt, matches[0].Index, matches[0].Value.Length);
                }
                else
                {
                    FocusTextBox(testTxt, testTxt.Text.Length);
                }
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FocusTextBox(TextBox textBox, int selectionStart, int selectionLength = 0)
        {
            textBox.Focus();
            textBox.SelectionStart = selectionStart;
            textBox.SelectionLength = selectionLength;
        }
    }
}
