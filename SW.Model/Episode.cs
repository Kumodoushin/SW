using Ardalis.SmartEnum;

namespace SW.Model
{
    public class Episode : SmartEnum<Episode>
    {

        public static Episode NewHope = new Episode(1, "New Hope");
        public static Episode EmpireStrikesBack = new Episode(2, "Empire Strikes Back");
        public static Episode ReturnOfTheJedi = new Episode(3, "The return of the Jedi");
        private Episode(int id, string name) : base(name, id)
        {

        }
    }
}
