using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Readme_Generator.Models
{
    public class SectionTemplate : INotifyPropertyChanged
    {
        public string Name { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        //public string Body { get; set; }
        private string _body;
        public string Body
        {
            get => _body;
            set => SetField(ref _body, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

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
