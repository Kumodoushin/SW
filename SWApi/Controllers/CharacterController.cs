using Ardalis.SmartEnum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SWApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly CharactersFacade _facade;
        
        public CharacterController(CharactersFacade facade)
        {
            _facade = facade;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CharacterForm characterForm)
        {
            var existingOnes=_facade.Query();

            var errors = new Dictionary<string,string>();
            var newId = Guid.NewGuid();
            
            if (existingOnes.Count(x => x.Name == characterForm.Name) > 0)
            {                
                errors.Add(nameof(characterForm.Name), $"A character with name {characterForm.Name} already exists.");                
            }

            var existingGuids = existingOnes.Select(x => x.Id);
            if (!characterForm.Friends.TrueForAll(x => existingGuids.Contains(x)))
            {
                errors.Add(nameof(characterForm.Friends), "Some of provided friends don't exist.");
            }

            if (!characterForm.Episodes.TrueForAll(x => Episode.List.Count(y => y.Value == x) == 1))
            {
                errors.Add(nameof(characterForm.Episodes), "Some of provided episodes don't exist.");
            }

            if (errors.Count==0)
            {
                var (success, facadeErrors) =
                  _facade.TryAdd(
                      new Character
                      {
                          Id = newId,
                          Name = characterForm.Name,
                          Episodes = 
                            new Episodes(
                                Episode.List
                                       .Where(x => characterForm
                                                        .Episodes
                                                        .Contains(x.Value))
                                       .ToArray()),
                          Friends = 
                            new Friends(
                                existingOnes.Where(x => characterForm
                                                            .Friends
                                                            .Contains(x.Id))
                                            .ToArray())
                      });
                if (success)
                {
                    return Accepted(new { Id = newId });
                }
                foreach (var error in facadeErrors)
                {
                    errors.Add(error.Key, error.Value);
                }
            }            
            return BadRequest(errors);
        }
        
        [HttpGet("{characterId:guid?}")]
        public IActionResult Get([FromRoute] Guid characterId)
        {
            if (characterId == Guid.Empty)
            {
                return Ok(_facade.Query());
            }
            var requestedCharacter = _facade.QueryById(characterId);
            if(requestedCharacter != null)
            {
                return Ok(requestedCharacter);
            }            
            return NotFound(new { Id = characterId, error = $"Character with id {characterId} was not found." });
        }
        
        [HttpPatch("{characterId:guid}")]
        public IActionResult Patch([FromRoute] Guid characterId, [FromBody] CharacterUpdateForm characterForm)
        {
            var updatedCharacter = _facade.QueryById(characterId);
            if (updatedCharacter is null)
            {
                return NotFound(new {Id= characterId, error = $"Character with id {characterId} was not found." });
            }
            if (characterForm.Episodes.Any(x => Episode.List.All(y=>y.Value!=x)))
            {
                return BadRequest(new {Id=characterId, error= $"Some invalid episodes provided." });
            }
            (bool success,Dictionary<string,string> facadeErrors) = _facade.TryUpdate(characterId, characterForm);
            if (success)
            {
                return Ok();
            }
            var errors = new Dictionary<string,string>();
            foreach (var error in facadeErrors)
            {
                errors.Add(error.Key, error.Value);
            }
            return BadRequest(errors);            
        }
        
        [HttpDelete("{characterId:guid}")]
        public IActionResult Delete([FromRoute] Guid characterId)
        {
            (bool success, Dictionary<string, string> facadeErrors) = _facade.TryDelete(characterId);
            if (success)
            {
                return Ok();
            }
            return NotFound(new { Id = characterId, Errors= facadeErrors });            
        }
    }

    public class CharactersFacade
    {
        private List<Character> _characters;

        public CharactersFacade()
        {
            var luke = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Luke Skywalker",
                Episodes = new Episodes(Episode.NewHope, Episode.EmpireStrikesBack, Episode.ReturnOfTheJedi )
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
            leia.Friends = new Friends( luke, han, annoyance, hacker );
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
            return new Characters(_characters.ToArray());
        }
        public Character QueryById(Guid id)
        {
            return _characters.FirstOrDefault(x => x.Id == id);
        }

        public (bool success,Dictionary<string,string> facadeErrors) TryAdd(Character character)
        {
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

        public (bool success, Dictionary<string, string> facadeErrors) TryUpdate(Guid characterId, CharacterUpdateForm characterForm)
        {
            var updatedCharacter = _characters.SingleOrDefault(x => x.Id == characterId);
            var errors = new Dictionary<string, string>();
            if(updatedCharacter is null)
            {
                errors.Add(nameof(Character), $"Character with {characterId} does not exist.");                
            }
            if (_characters.Any(x=>x.Id != characterId && x.Name == characterForm.Name))
            {
                errors.Add(nameof(characterForm.Name), $"Character with {characterForm.Name} does exist, can't have doppelgangers.");
            }
            if (characterForm.Friends.Any(x=>_characters.All(y => y.Id != x)))
            {
                errors.Add(nameof(characterForm.Friends), $"Some of provided friends do not exist.");
            }
            if (errors.Any())
            {
                return (false, errors);
            }
            updatedCharacter.Name = characterForm.Name;
            updatedCharacter.Episodes = new Episodes(Episode.List.Where(x => characterForm.Episodes.Contains(x.Value)).ToArray());
            updatedCharacter.Friends = new Friends(_characters.Where(x => characterForm.Friends.Contains(x.Id)).ToArray());
            return (true, new Dictionary<string, string>());
        }

        public (bool success, Dictionary<string, string> facadeErrors) TryDelete(Guid characterId)
        {            
            var success=_characters.Remove(_characters.SingleOrDefault(x => x.Id == characterId));
            if (success)
            {
                foreach (var chr in _characters.Where(x => x.Id != characterId && x.Friends.Any(y=>y.Id==characterId)))
                {
                    chr.Friends = new Friends(chr.Friends.Where(x => x.Id != characterId).ToArray());
                }
                return (success, new Dictionary<string, string>());
            }
            return (success, new Dictionary<string, string>(new[] { new KeyValuePair<string, string>(nameof(Character), "Character does not exist") }));
        }
    }

    public class CharacterForm
    {
        public string Name { get; set; }
        public List<int> Episodes { get; set; }
        public List<Guid> Friends { get; set; }
    }

    public class CharacterUpdateForm
    {
        public string Name { get; set; }
        public List<int> Episodes { get; set; }
        public List<Guid> Friends { get; set; }
    }


    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Episodes Episodes { get; set; }
        public Friends Friends { get; set; }
    }
    public class Characters: ReadOnlyCollection<Character>
    {
        public Characters(params Character[] characters):base(characters.Distinct().ToArray())
        {

        }
    }


    public class Friend
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Friends : ReadOnlyCollection<Friend> 
    {
        public Friends(params Character[] characters):base(characters.Distinct().Select(x=>new Friend { Id=x.Id,Name=x.Name}).ToList())
        {

        }
        public Friends(params Friend[] friends) : base(friends.Distinct().ToList())
        {

        }
    }


    public class Episode:SmartEnum<Episode>
    {

        public static Episode NewHope = new Episode(1, "New Hope");
        public static Episode EmpireStrikesBack = new Episode(2, "Empire Strikes Back");
        public static Episode ReturnOfTheJedi = new Episode(3, "The return of the Jedi");
        private Episode(int id,string name):base(name,id)
        {

        }
    }
    public class Episodes:ReadOnlyCollection<Episode>
    {
        public Episodes(params Episode[] episodes):base(episodes.Distinct().ToArray())
        {

        }
    }
}
