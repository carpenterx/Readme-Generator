using System.Collections.ObjectModel;

namespace Readme_Generator.Models
{
    class ReadmeTemplate
    {
        public string Name { get; set; }
        public ObservableCollection<SectionTemplate> Sections { get; set; }
    }
}
