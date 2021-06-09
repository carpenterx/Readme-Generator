﻿using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Readme_Generator.Models
{
    public class SectionTemplate
    {
        public string Name { get; set; }
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Body { get; set; }
    }
}
