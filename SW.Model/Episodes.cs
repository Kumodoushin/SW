using System.Linq;
using System.Collections.ObjectModel;

namespace SW.Model
{
    public class Episodes : ReadOnlyCollection<Episode>
    {
        public Episodes(params Episode[] episodes) 
            : base(episodes.Distinct()
                           .ToArray())
        {

        }
    }
}
