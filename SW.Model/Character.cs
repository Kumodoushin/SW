using System;

namespace SW.Model
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Episodes Episodes { get; set; }
        public Friends Friends { get; set; }
    }
}
