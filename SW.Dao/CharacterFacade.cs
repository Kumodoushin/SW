using System;
using System.Collections.Generic;
using System.Linq;
using SW.Model;
using SW.Model.Helpers;

namespace SW.Dao
{  

    public class CharactersFacade: ICharacterFacade
    {
        private List<Character> _characters;

        public CharactersFacade()
        {
            var luke = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Luke Skywalker",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var anakin = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Darth Vader",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var han = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Han Solo",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var leia = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Leia Organa",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var tarkin = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Wilhuff Tarkin",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var annoyance = new Character
            {
                Id = Guid.NewGuid(),
                Name = "C-3PO",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var hacker = new Character
            {
                Id = Guid.NewGuid(),
                Name = "R2-D2",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };
            var brush = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Wookie",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi)
            };


            luke.Friends = new Friends(han, leia, hacker, annoyance);
            anakin.Friends = new Friends(tarkin);
            leia.Friends = new Friends(luke, han, annoyance, hacker);
            han.Friends = new Friends(brush, luke, leia, hacker);
            tarkin.Friends = new Friends(anakin);
            annoyance.Friends = new Friends(luke, han, leia, hacker, brush);
            hacker.Friends = new Friends(luke, han, leia, brush);
            brush.Friends = new Friends(han, luke, leia);
            _characters = new List<Character>()
            {
                luke,
                anakin,
                han,
                leia,
                tarkin,
                annoyance,
                brush,
                hacker,
            };
        }

        public Characters Query()
        {
            return
                new Characters(
                    _characters.Select(x =>
                        new Character
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Episodes = new Episodes(x.Episodes.Select(y => y).ToArray()),
                            Friends = new Friends(x.Friends.Select(y => new Friend() { Id = y.Id, Name = y.Name }).ToArray())
                        }).ToArray());
        }


        public (Characters data, int pageNr, int charactersCount) QueryPaginated(PaginationOptions paginationOptions)
        {
            int count = _characters.Count();
            int lastPage = 0;
            if (count % paginationOptions.PageSize != 0)
            {
                lastPage = count / paginationOptions.PageSize + 1;
            }
            else
            {
                lastPage = count / paginationOptions.PageSize;
            }

            int pageNr = lastPage < paginationOptions.PageNumber ? lastPage : paginationOptions.PageNumber;

            return
                (
                    new Characters(
                        _characters
                            .Skip(paginationOptions.PageSize * (pageNr - 1))
                            .Take(paginationOptions.PageSize)
                            .Select(x =>
                            new Character
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Episodes = new Episodes(x.Episodes.Select(y => y).ToArray()),
                                Friends = new Friends(x.Friends.Select(y => new Friend() { Id = y.Id, Name = y.Name }).ToArray())
                            }).ToArray())
                    , pageNr, count
                );
        }

        public Character QueryById(Guid id)
        {
            var chr = _characters.FirstOrDefault(x => x.Id == id);
            if (chr is null)
            {
                return null;
            }
            return new Character
            {
                Id = chr.Id,
                Name = chr.Name,
                Episodes = new Episodes(chr.Episodes.Select(y => y).ToArray()),
                Friends = new Friends(chr.Friends.Select(y => new Friend() { Id = y.Id, Name = y.Name }).ToArray())
            };
        }

        public (bool success, Dictionary<string, string> facadeErrors) TryAdd(Character character)
        {
            character.Id = Guid.NewGuid();
            var facadeErrors = new Dictionary<string, string>();
            bool result = false;
            if (_characters.Any(x => x.Id == character.Id))
            {
                facadeErrors.Add(nameof(Character.Id), "Character with Id exists.");
            }

            if (!facadeErrors.Any())
            {
                _characters.Add(character);
                result = true;
            }
            return (result, facadeErrors);
        }

        public Dictionary<string, string> TryUpdate(Guid characterId, CharacterUpdateForm characterForm)
        {
            var updatedCharacter = _characters.SingleOrDefault(x => x.Id == characterId);
            var errors = new Dictionary<string, string>();
            if (updatedCharacter is null)
            {
                errors.Add(nameof(Character), $"Character with {characterId} does not exist.");
            }
            if (_characters.Any(x => x.Id != characterId && x.Name == characterForm.Name))
            {
                errors.Add(nameof(characterForm.Name), $"Character with {characterForm.Name} does exist, can't have doppelgangers.");
            }
            if (characterForm.Friends.Any(x => _characters.All(y => y.Id != x)))
            {
                errors.Add(nameof(characterForm.Friends), $"Some of provided friends do not exist.");
            }
            if (errors.Any())
            {
                return errors;
            }
            updatedCharacter.Name = characterForm.Name;
            updatedCharacter.Episodes = new Episodes(Episode.List.Where(x => characterForm.Episodes.Contains(x.Value)).ToArray());
            updatedCharacter.Friends = new Friends(_characters.Where(x => characterForm.Friends.Contains(x.Id)).ToArray());
            return new Dictionary<string, string>();
        }

        public Dictionary<string, string> TryDelete(Guid characterId)
        {
            var success = _characters.Remove(_characters.SingleOrDefault(x => x.Id == characterId));
            if (success)
            {
                foreach (var chr in _characters.Where(x => x.Id != characterId && x.Friends.Any(y => y.Id == characterId)))
                {
                    chr.Friends = new Friends(chr.Friends.Where(x => x.Id != characterId).ToArray());
                }
                return new Dictionary<string, string>();
            }
            return new Dictionary<string, string> { { nameof(Character), "Character does not exist" } };
        }

    }
}
