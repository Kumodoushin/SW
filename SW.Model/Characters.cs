using System.Collections.ObjectModel;
using System.Linq;

namespace SW.Model
{
    public class Characters : ReadOnlyCollection<Character>
    {
        public Characters(params Character[] characters) 
            : base(characters.Distinct()
                             .ToArray())
        {

        }
    }
}
