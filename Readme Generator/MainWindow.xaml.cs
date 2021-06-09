using Readme_Generator.Models;
using Readme_Generator.Windows;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YamlDotNet.Serialization;

namespace Readme_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string APPLICATION_FOLDER = "Readme Generator";
        private static readonly string SECTION_TEMPLATES_FILE = "section templates.yml";
        private readonly string sectionTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, SECTION_TEMPLATES_FILE);

        private ObservableCollection<SectionTemplate> sectionsList = new();

        public MainWindow()
        {
            InitializeComponent();

            LoadSectionTemplates();
        }

        private void LoadSectionTemplates()
        {
            if (File.Exists(sectionTemplatesPath))
            {
                var input = new StringReader(File.ReadAllText(sectionTemplatesPath));
                var deserializer = new DeserializerBuilder().Build();

                ObservableCollection<SectionTemplate> sections = deserializer.Deserialize<ObservableCollection<SectionTemplate>>(input);
                sectionsList = new ObservableCollection<SectionTemplate>(sections);
            }

            sectionsListView.ItemsSource = sectionsList;
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
                string testString = readmeTxt.Text;

                MatchCollection matches = rgx.Matches(testString);

                if(matches.Count > 0)
                {
                    FocusTextBox(readmeTxt, matches[0].Index, matches[0].Value.Length);
                }
                else
                {
                    FocusTextBox(readmeTxt, readmeTxt.Text.Length);
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

        private void WrapSelectionWithCharacters(string startChars, string endChars = "")
        {
            if (readmeTxt.SelectionLength > 0)
            {
                string oldText = readmeTxt.Text;
                int selectionStartIndex = readmeTxt.SelectionStart;
                int selectionLength = readmeTxt.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, selectionStartIndex))
                    .Append(startChars)
                    .Append(oldText.Substring(selectionStartIndex, selectionLength))
                    .Append(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                readmeTxt.Text = newTextBuilder.ToString();
                FocusTextBox(readmeTxt, selectionStartIndex + startCharsLength, selectionLength);
            }
            else
            {
                string oldText = readmeTxt.Text;
                int caretIndex = readmeTxt.CaretIndex;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, caretIndex))
                    .Append(startChars)
                    .Append(endChars)
                    .Append(oldText.Substring(caretIndex));
                readmeTxt.Text = newTextBuilder.ToString();
                FocusTextBox(readmeTxt, caretIndex + startCharsLength);
            }
        }

        private void WrapSelectionWithLines(string startChars, string endChars)
        {
            if (readmeTxt.SelectionLength > 0)
            {
                string oldText = readmeTxt.Text;
                int selectionStartIndex = readmeTxt.SelectionStart;
                int selectionLength = readmeTxt.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .AppendLine(oldText.Substring(0, selectionStartIndex))
                    .AppendLine(startChars)
                    .AppendLine(oldText.Substring(selectionStartIndex, selectionLength))
                    .AppendLine(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                readmeTxt.Text = newTextBuilder.ToString();
                // the newlines shift the text by 4 characters
                FocusTextBox(readmeTxt, selectionStartIndex + startCharsLength + 4, selectionLength);
            }
            else
            {
                string oldText = readmeTxt.Text;
                int caretIndex = readmeTxt.CaretIndex;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .AppendLine(oldText.Substring(0, caretIndex))
                    .AppendLine(startChars)
                    .AppendLine()
                    .AppendLine(endChars)
                    .Append(oldText.Substring(caretIndex));
                readmeTxt.Text = newTextBuilder.ToString();
                // the newlines shift the text by 4 characters
                FocusTextBox(readmeTxt, caretIndex + startCharsLength + 4);
            }
        }

        private void WrapSelectionWithLink()
        {
            // [GitHub](http://github.com)
            if (readmeTxt.SelectionLength > 0)
            {
                string oldText = readmeTxt.Text;
                int selectionStartIndex = readmeTxt.SelectionStart;
                int selectionLength = readmeTxt.SelectionLength;
                string textBefore = oldText.Substring(0, selectionStartIndex);
                string selectionText = oldText.Substring(selectionStartIndex, selectionLength);
                string textAfter = oldText.Substring(selectionStartIndex + selectionLength);
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(textBefore)
                    .Append($"[{selectionText}]({selectionText})")
                    .Append(textAfter);
                readmeTxt.Text = newTextBuilder.ToString();
                FocusTextBox(readmeTxt, selectionStartIndex + 1, selectionLength);
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

        private void MakeTask(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters("- [ ] ");
        }

        private void MakeKey(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters("<kbd>", "</kbd>");
        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            SectionWindow sectionWindow = new();
            sectionWindow.Owner = this;
            if (sectionWindow.ShowDialog() == true)
            {
                sectionsList.Add(sectionWindow.GetSection());
            }
        }

        private void SectionSelected(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is SectionTemplate selectedSection)
            {
                StringBuilder newText = new();
                newText
                    .AppendLine(readmeTxt.Text)
                    .AppendLine(selectedSection.Body);
                readmeTxt.Text = newText.ToString();
            }
        }

        private void SaveData(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string appDirectory = Path.GetDirectoryName(sectionTemplatesPath);
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(sectionsList);
            File.WriteAllText(sectionTemplatesPath, yaml);
        }
    }
}
