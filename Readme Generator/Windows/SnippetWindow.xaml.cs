using MahApps.Metro.Controls;
using Readme_Generator.Models;
using System.Windows;

namespace Readme_Generator.Windows
{
    /// <summary>
    /// Interaction logic for SnippetWindow.xaml
    /// </summary>
    public partial class SnippetWindow : MetroWindow
    {
        private Snippet snippet;

        public SnippetWindow()
        {
            InitializeComponent();
        }

        public Snippet GetSnippet()
        {
            return snippet;
}

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            snippet = new Snippet(titleTxt.Text, contentTxt.Text);
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }
    }
}
