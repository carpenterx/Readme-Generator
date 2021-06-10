using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Readme_Generator.Models
{
    public class SectionTemplate
    {
        public string Name { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Body { get; set; }

        public SectionTemplate(SectionTemplate section)
        {
            Name = section.Name;
            Body = section.Body;
        }

        public SectionTemplate(string sectionName, string sectionBody)
        {
            Name = sectionName;
            Body = sectionBody;
        }

        public SectionTemplate()
        {
            Name = "";
            Body = "";
        }
    }
}
