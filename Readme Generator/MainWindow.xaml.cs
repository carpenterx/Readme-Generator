using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Readme_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void MakeItalic(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters("*", "*");
        }

        private void WrapSelectionWithCharacters(string startChars, string endChars)
        {
            if (testTxt.SelectionLength > 0)
            {
                string oldText = testTxt.Text;
                int selectionStartIndex = testTxt.SelectionStart;
                int selectionLength = testTxt.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, selectionStartIndex))
                    .Append(startChars)
                    .Append(oldText.Substring(selectionStartIndex, selectionLength))
                    .Append(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                testTxt.Text = newTextBuilder.ToString();
                FocusTextBox(testTxt, selectionStartIndex + startCharsLength, selectionLength);
            }
        }

        private void WrapSelectionWithLines(string startChars, string endChars)
        {
            if (testTxt.SelectionLength > 0)
            {
                string oldText = testTxt.Text;
                int selectionStartIndex = testTxt.SelectionStart;
                int selectionLength = testTxt.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .AppendLine(oldText.Substring(0, selectionStartIndex))
                    .AppendLine(startChars)
                    .AppendLine(oldText.Substring(selectionStartIndex, selectionLength))
                    .AppendLine(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                testTxt.Text = newTextBuilder.ToString();
                // the newlines shift the text by 4 characters
                FocusTextBox(testTxt, selectionStartIndex + startCharsLength + 4, selectionLength);
            }
        }

        private void WrapSelectionWithLink()
        {
            // [GitHub](http://github.com)
            if (testTxt.SelectionLength > 0)
            {
                string oldText = testTxt.Text;
                int selectionStartIndex = testTxt.SelectionStart;
                int selectionLength = testTxt.SelectionLength;
                string textBefore = oldText.Substring(0, selectionStartIndex);
                string selectionText = oldText.Substring(selectionStartIndex, selectionLength);
                string textAfter = oldText.Substring(selectionStartIndex + selectionLength);
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(textBefore)
                    .Append($"[{selectionText}]({selectionText})")
                    .Append(textAfter);
                testTxt.Text = newTextBuilder.ToString();
                FocusTextBox(testTxt, selectionStartIndex + 1, selectionLength);
            }
        }

        private void MakeBold(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters("**", "**");
        }

        private void MakeCode(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithLines("```", "```");
        }

        private void MakeLink(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithLink();
        }
    }
}
