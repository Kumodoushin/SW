using SW.Model;
using System;
using System.Collections.Generic;

namespace SW.Dao
{
    public interface ICharacterFacade
    {
        Characters Query();
        Character QueryById(Guid id);

        (bool success, Dictionary<string, string> facadeErrors) TryAdd(Character character);

        Dictionary<string, string> TryUpdate(Guid characterId, CharacterUpdateForm characterForm);

        Dictionary<string, string> TryDelete(Guid characterId);
    }
}
