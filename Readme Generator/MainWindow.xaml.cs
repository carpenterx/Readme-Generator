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
using System.Linq;

namespace Readme_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string APPLICATION_FOLDER = "Readme Generator";
        private static readonly string SECTION_TEMPLATES_FILE = "section templates.yml";
        private static readonly string README_TEMPLATE_FILE = "readme template.yml";
        private readonly string sectionTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, SECTION_TEMPLATES_FILE);
        private readonly string readmeTemplatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, README_TEMPLATE_FILE);

        private ObservableCollection<SectionTemplate> allSectionsList = new();
        private ObservableCollection<SectionTemplate> selectedSectionsList = new();

        public MainWindow()
        {
            InitializeComponent();

            LoadSectionTemplates();
        }

        private void LoadSectionTemplates()
        {
            allSectionsList = LoadFileToList<SectionTemplate>(sectionTemplatesPath);
            selectedSectionsList = LoadFileToList<SectionTemplate>(readmeTemplatePath);

            allSectionsListView.ItemsSource = allSectionsList;
            selectedSectionsListView.ItemsSource = selectedSectionsList;
        }

        private ObservableCollection<T> LoadFileToList<T>(string filePath)
        {
            ObservableCollection<T> list = new();
            if (File.Exists(filePath))
            {
                var input = new StringReader(File.ReadAllText(filePath));
                var deserializer = new DeserializerBuilder().Build();

                ObservableCollection<T> items = deserializer.Deserialize<ObservableCollection<T>>(input);
                list = new ObservableCollection<T>(items);
            }

            return list;
        }

        private void TestTab(object sender, KeyEventArgs e)
        {
            bool snippetMode = true;
            if (e.Key == Key.Tab)
            {
                if (snippetMode)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        FindPrevPlaceholderOrSection(sectionTxt);
                    }
                    else
                    {
                        FindNextPlaceholderOrSection(sectionTxt);
                    }
                    e.Handled = true;
                }
            }
        }

        private void FindNextPlaceholder(TextBox textBox)
        {
            try
            {
                Match match = GetPlaceholderMatch(textBox.Text);

                if (match.Success)
                {
                    FocusTextBox(textBox, match.Index, match.Value.Length);
                }
                else
                {
                    FocusTextBox(textBox, textBox.Text.Length);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FindNextPlaceholderOrSection(TextBox textBox)
        {
            try
            {
                Match match = GetPlaceholderMatch(textBox.Text);

                if (match.Success)
                {
                    FocusTextBox(textBox, match.Index, match.Value.Length);
                }
                else
                {
                    if (selectedSectionsListView.SelectedIndex < selectedSectionsListView.Items.Count - 1)
                    {
                        selectedSectionsListView.SelectedIndex++;
                    }
                    else
                    {
                        selectedSectionsListView.SelectedIndex = 0;
                    }
                    selectedSectionsListView.ScrollIntoView(selectedSectionsListView.SelectedItem);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FindPrevPlaceholderOrSection(TextBox textBox)
        {
            if (selectedSectionsListView.SelectedIndex > 0)
            {
                selectedSectionsListView.SelectedIndex--;
            }
            else
            {
                selectedSectionsListView.SelectedIndex = selectedSectionsListView.Items.Count - 1;
            }
            selectedSectionsListView.ScrollIntoView(selectedSectionsListView.SelectedItem);
        }

        private Match GetPlaceholderMatch(string testString)
        {
            Regex rgx = new Regex(@"\$.+?\$");

            return rgx.Match(testString);
        }

        private void FocusTextBox(TextBox textBox, int selectionStart, int selectionLength = 0)
        {
            textBox.Focus();
            textBox.SelectionStart = selectionStart;
            textBox.SelectionLength = selectionLength;
        }

        private void MakeItalic(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters(sectionTxt, "*", "*");
        }

        private void WrapSelectionWithCharacters(TextBox textBox, string startChars, string endChars = "")
        {
            if (textBox.SelectionLength > 0)
            {
                string oldText = textBox.Text;
                int selectionStartIndex = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, selectionStartIndex))
                    .Append(startChars)
                    .Append(oldText.Substring(selectionStartIndex, selectionLength))
                    .Append(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                textBox.Text = newTextBuilder.ToString();
                FocusTextBox(textBox, selectionStartIndex + startCharsLength, selectionLength);
            }
            else
            {
                string oldText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, caretIndex))
                    .Append(startChars)
                    .Append(endChars)
                    .Append(oldText.Substring(caretIndex));
                textBox.Text = newTextBuilder.ToString();
                FocusTextBox(textBox, caretIndex + startCharsLength);
            }
        }

        private void WrapSelectionWithLines(TextBox textBox, string startChars, string endChars)
        {
            if (textBox.SelectionLength > 0)
            {
                string oldText = textBox.Text;
                int selectionStartIndex = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .AppendLine(oldText.Substring(0, selectionStartIndex))
                    .AppendLine(startChars)
                    .AppendLine(oldText.Substring(selectionStartIndex, selectionLength))
                    .AppendLine(endChars)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                textBox.Text = newTextBuilder.ToString();
                // the newlines shift the text by 4 characters
                FocusTextBox(textBox, selectionStartIndex + startCharsLength + 4, selectionLength);
            }
            else
            {
                string oldText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                int startCharsLength = startChars.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .AppendLine(oldText.Substring(0, caretIndex))
                    .AppendLine(startChars)
                    .AppendLine()
                    .AppendLine(endChars)
                    .Append(oldText.Substring(caretIndex));
                textBox.Text = newTextBuilder.ToString();
                // the newlines shift the text by 4 characters
                FocusTextBox(textBox, caretIndex + startCharsLength + 4);
            }
        }

        private void WrapSelectionWithLink(TextBox textBox)
        {
            // [GitHub](http://github.com)
            if (textBox.SelectionLength > 0)
            {
                string oldText = textBox.Text;
                int selectionStartIndex = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                string textBefore = oldText.Substring(0, selectionStartIndex);
                string selectionText = oldText.Substring(selectionStartIndex, selectionLength);
                string textAfter = oldText.Substring(selectionStartIndex + selectionLength);
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(textBefore)
                    .Append($"[{selectionText}]({selectionText})")
                    .Append(textAfter);
                textBox.Text = newTextBuilder.ToString();
                FocusTextBox(textBox, selectionStartIndex + 1, selectionLength);
            }
        }

        private void MakeBold(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters(sectionTxt, "**", "**");
        }

        private void MakeCode(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithLines(sectionTxt, "```", "```");
        }

        private void MakeLink(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithLink(sectionTxt);
        }

        private void MakeTask(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters(sectionTxt, "- [ ] ");
        }

        private void MakeKey(object sender, RoutedEventArgs e)
        {
            WrapSelectionWithCharacters(sectionTxt, "<kbd>", "</kbd>");
        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            SectionWindow sectionWindow = new();
            sectionWindow.Owner = this;
            if (sectionWindow.ShowDialog() == true)
            {
                allSectionsList.Add(sectionWindow.GetSection());
            }
        }

        private void AppendSelectedSection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is SectionTemplate selectedSection)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    ((sender as ListView).ItemsSource as ObservableCollection<SectionTemplate>).Remove(selectedSection);
                }
                else
                {
                    sectionTxt.DataContext = selectedSection;
                    FindNextPlaceholder(sectionTxt);
                }
            }
        }

        private void SaveData(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string appDirectory = Path.GetDirectoryName(sectionTemplatesPath);
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            SaveFileToYaml(sectionTemplatesPath, allSectionsList);
            SaveFileToYaml(readmeTemplatePath, selectedSectionsList);
        }

        private void SaveFileToYaml<T>(string filePath, ObservableCollection<T> list)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(list);
            File.WriteAllText(filePath, yaml);
        }

        private void AddSectionToReadme(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is SectionTemplate selectedSection)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    ((sender as ListView).ItemsSource as ObservableCollection<SectionTemplate>).Remove(selectedSection);
                }
                else if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    sectionTxt.DataContext = selectedSection;
                    FindNextPlaceholder(sectionTxt);
                }
                else
                {
                    if (selectedSectionsList.FirstOrDefault(i => i.Name == selectedSection.Name) == null)
                    {
                        selectedSectionsList.Add(new SectionTemplate(selectedSection));
                        //selectedSectionsListView.ScrollIntoView(selectedSectionsListView.SelectedItem);
                    }
                }
            }
        }

        private void UpdateReadmeOutput(object sender, TextChangedEventArgs e)
        {
            StringBuilder readmeBuilder = new();

            foreach (SectionTemplate section in selectedSectionsList)
            {
                readmeBuilder.AppendLine(section.Body).AppendLine();
            }

            readmeTxt.Text = readmeBuilder.ToString();
        }

        private void CopyReadme(object sender, RoutedEventArgs e)
        {
            CopyReadmeToClipboard();
        }

        private void CopyReadmeToClipboard()
        {
            Match match = GetPlaceholderMatch(readmeTxt.Text);

            if (match.Success)
            {
                MessageBox.Show("Readme still contains placeholders", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            Clipboard.SetText(readmeTxt.Text);
        }
    }
}
