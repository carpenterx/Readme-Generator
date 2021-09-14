﻿using Readme_Generator.Models;
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
using Microsoft.Win32;
using MahApps.Metro.Controls;
using ControlzEx.Theming;

namespace Readme_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static readonly string APPLICATION_FOLDER = "Readme Generator";
        private static readonly string SECTION_TEMPLATES_FILE = "section templates.yml";
        private static readonly string README_TEMPLATE_FILE = "readme template.yml";
        private static readonly string SNIPPETS_FILE = "snippets.yml";
        private readonly string sectionTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, SECTION_TEMPLATES_FILE);
        private readonly string readmeTemplatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, README_TEMPLATE_FILE);
        private readonly string snippetsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, SNIPPETS_FILE);

        private ObservableCollection<SectionTemplate> allSectionsList = new();
        private ObservableCollection<SectionTemplate> selectedSectionsList = new();

        private ObservableCollection<Snippet> snippetsList = new();

        public MainWindow()
        {
            InitializeComponent();

            LoadSectionTemplates();

            LoadSnippets();
        }

        private void LoadSectionTemplates()
        {
            allSectionsList = LoadFileToList<SectionTemplate>(sectionTemplatesPath);
            selectedSectionsList = LoadFileToList<SectionTemplate>(readmeTemplatePath);

            allSectionsListView.ItemsSource = allSectionsList;
            selectedSectionsListView.ItemsSource = selectedSectionsList;
        }

        private void LoadSnippets()
        {
            snippetsList = LoadFileToList<Snippet>(snippetsPath);

            snippetsListView.ItemsSource = snippetsList;
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

        private void SelectSectionFromReadmeSections(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.SelectedItem is SectionTemplate selectedSection)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    // Delete
                    (listView.ItemsSource as ObservableCollection<SectionTemplate>).Remove(selectedSection);
                    sectionTxt.DataContext = null;
                    UpdateReadmeOutput();
                }
                else
                {
                    // Edit/display
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
            SaveFileToYaml(snippetsPath, snippetsList);
        }

        private void SaveFileToYaml<T>(string filePath, ObservableCollection<T> list)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(list);
            File.WriteAllText(filePath, yaml);
        }

        private void SelectSectionFromAllSections(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is SectionTemplate selectedSection)
            {
                if (Keyboard.IsKeyDown(Key.LeftAlt))
                {
                    // Delete
                    ((sender as ListView).ItemsSource as ObservableCollection<SectionTemplate>).Remove(selectedSection);
                }
                else if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    // Edit/display
                    sectionTxt.DataContext = selectedSection;
                    FindNextPlaceholder(sectionTxt);
                }
                else
                {
                    // Add
                    if (selectedSectionsList.FirstOrDefault(i => i.Name == selectedSection.Name) == null)
                    {
                        selectedSectionsList.Add(new SectionTemplate(selectedSection));
                        sectionTxt.DataContext = selectedSection;
                    }
                }
            }
        }

        private void TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            UpdateReadmeOutput();
        }

        private void UpdateReadmeOutput()
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

        private void InsertSnippet(TextBox textBox, string snippet)
        {
            if (textBox.SelectionLength > 0)
            {
                string oldText = textBox.Text;
                int selectionStartIndex = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                int startCharsLength = snippet.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, selectionStartIndex))
                    .Append(snippet)
                    .Append(oldText.Substring(selectionStartIndex + selectionLength));
                textBox.Text = newTextBuilder.ToString();
                FocusTextBox(textBox, selectionStartIndex + startCharsLength, selectionLength);
            }
            else
            {
                string oldText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                int startCharsLength = snippet.Length;
                StringBuilder newTextBuilder = new();
                newTextBuilder
                    .Append(oldText.Substring(0, caretIndex))
                    .Append(snippet)
                    .Append(oldText.Substring(caretIndex));
                textBox.Text = newTextBuilder.ToString();
                FocusTextBox(textBox, caretIndex + startCharsLength);
            }
        }

        private void AddSnippetClick(object sender, RoutedEventArgs e)
        {
            SnippetWindow snippetWindow = new();
            snippetWindow.Owner = this;
            if (snippetWindow.ShowDialog() == true)
            {
                snippetsList.Add(snippetWindow.GetSnippet());
            }
        }

        private void SnippetClickedHandler(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            Snippet snippet = item.DataContext as Snippet;
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                snippetsList.Remove(snippet);
            }
            else
            {
                InsertSnippet(sectionTxt, snippet.Value);
            }
        }

        private void DeleteSnippetClick(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedItem is Snippet selectedSnippet)
            {
                snippetsList.Remove(selectedSnippet);
            }
        }

        private void SaveReadmeTemplateClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Readme Template";
            dlg.FileName = "Readme template"; // Default file name
            dlg.DefaultExt = ".yml"; // Default file extension
            dlg.Filter = "Yaml documents (.yml)|*.yml"; // Filter files by extension

            if (dlg.ShowDialog() == true)
            {
                SaveFileToYaml(dlg.FileName, selectedSectionsList);
            }
        }

        private void LoadReadmeTemplateClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Load Readme Template";
            openFileDialog.Filter = "Yaml documents (.yml)|*.yml";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedSectionsList = LoadFileToList<SectionTemplate>(openFileDialog.FileName);
                selectedSectionsListView.ItemsSource = selectedSectionsList;
            }
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            ToggleSwitch themeToggle = (ToggleSwitch)sender;
if (themeToggle.IsOn)
{
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Teal");
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light.Teal");
            }
        }
    }
}
