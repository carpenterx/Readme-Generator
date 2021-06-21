namespace Readme_Generator.Models
{
    public class Snippet
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Snippet(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
