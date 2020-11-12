using System;
using System.Collections.Generic;

namespace SW.Model
{
    public class CharacterUpdateForm
    {
        public string Name { get; set; }
        public List<int> Episodes { get; set; } = new List<int>();
        public List<Guid> Friends { get; set; } = new List<Guid>();
    }
}
