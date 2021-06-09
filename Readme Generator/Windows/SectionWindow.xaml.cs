using Readme_Generator.Models;
using System.Windows;

namespace Readme_Generator.Windows
{
    /// <summary>
    /// Interaction logic for SectionWindow.xaml
    /// </summary>
    public partial class SectionWindow : Window
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
            section = new SectionTemplate
            {
                Name = nameTxt.Text,
                Body = bodyTxt.Text,
            };
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }
    }
}
