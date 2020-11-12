using System.Linq;
using System.Collections.ObjectModel;

namespace SW.Model
{
    public class Friends : ReadOnlyCollection<Friend>
    {
        public Friends(params Character[] characters) 
            : base(characters.Distinct()
                             .Select(x => new Friend { Id = x.Id, Name = x.Name })
                             .ToList())
        {

        }
        public Friends(params Friend[] friends) 
            : base(friends.Distinct()
                          .ToList())
        {

        }
    }
}
