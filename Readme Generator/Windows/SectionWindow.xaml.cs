using MahApps.Metro.Controls;
using Readme_Generator.Models;
using System.Windows;

namespace Readme_Generator.Windows
{
    /// <summary>
    /// Interaction logic for SectionWindow.xaml
    /// </summary>
    public partial class SectionWindow : MetroWindow
    {
        private SectionTemplate section;

        public SectionWindow()
        {
            InitializeComponent();
        }

        public SectionTemplate GetSection()
        {
            return section;
        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            section = new SectionTemplate(nameTxt.Text, bodyTxt.Text);
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }
    }
}
