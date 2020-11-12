using SW.Model;
using SW.Model.Helpers;
using System;
using System.Collections.Generic;

namespace SW.Dao
{
    public interface ICharacterFacade
    {
        Characters Query();
        (Characters data,int pageNr,int charactersCount) QueryPaginated(PaginationOptions paginationOptions);
        Character QueryById(Guid id);

        (bool success, Dictionary<string, string> facadeErrors) TryAdd(Character character);

        Dictionary<string, string> TryUpdate(Guid characterId, CharacterUpdateForm characterForm);

        Dictionary<string, string> TryDelete(Guid characterId);
    }
}
