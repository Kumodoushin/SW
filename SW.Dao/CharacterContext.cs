using Microsoft.EntityFrameworkCore;
using SW.Model;
using SW.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SW.Dao
{
    public class CharactersContext:DbContext, ICharacterFacade
    {
        internal DbSet<CharacterDao> Characters { get; set; }
        internal DbSet<FriendDao> Friends { get; set; }
        internal DbSet<CharacterFriend> CharacterFriends { get; set; }
        internal DbSet<CharacterEpisode> CharacterEpisodes { get; set; }
        public CharactersContext(DbContextOptions options) : base(options){ }

        public Characters Query()
        {
            return
                new Characters(
                    Characters.AsNoTracking().Include(x => x.Friends).Include(x => x.Episodes)
                    .Select(x => new Character
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Episodes = 
                            new Episodes(x.Episodes
                                          .Select(y => y.Episode)
                                          .ToArray()),
                        Friends = 
                            new Friends(x.Friends
                                         .Select(y=> new Friend 
                                                    { 
                                                        Id=y.FriendId,
                                                        Name=y.FriendName
                                                    })
                                         .ToArray())
                    })
                    .ToArray());
        }
        public (Characters data, int pageNr, int charactersCount) QueryPaginated(PaginationOptions paginationOptions)
        {
            int count = Characters.Count();
            int lastPage = 0;
            int pageSize = paginationOptions.PageSize < 1 ? 1 : paginationOptions.PageSize;
            if (count% pageSize != 0)
            {
                lastPage = count / pageSize + 1;
            }
            else
            {
                lastPage = count / pageSize;
            }

            int pageNr = lastPage < paginationOptions.PageNumber?lastPage:paginationOptions.PageNumber;
            
            return (                
                new Characters(
                    Characters.AsNoTracking().Include(x => x.Friends).Include(x => x.Episodes)
                    .Skip(pageSize * (pageNr-1))
                    .Take(pageSize)
                    .Select(x => new Character
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Episodes =
                            new Episodes(x.Episodes
                                          .Select(y => y.Episode)
                                          .ToArray()),
                        Friends =
                            new Friends(x.Friends
                                         .Select(y => new Friend
                                         {
                                             Id = y.FriendId,
                                             Name = y.FriendName
                                         })
                                         .ToArray())
                    })
                    .ToArray())
                , pageNr, count);
        }

        public Character QueryById(Guid id)
        {
            var dbCharacter=
                Characters
                    .AsNoTracking()
                    .Include(x => x.Friends)
                    .Include(x => x.Episodes)
                    .FirstOrDefault(x => x.Id == id);
            if (dbCharacter is null)
            {
                return null;
            }
            return
                new Character
                    {
                        Id = dbCharacter.Id,
                        Name = dbCharacter.Name,
                        Episodes =
                            new Episodes(dbCharacter.Episodes
                                                    .Select(y => y.Episode)
                                                    .ToArray()),
                        Friends =
                            new Friends(dbCharacter.Friends
                                                   .Select(y => new Friend
                                                   {
                                                       Id = y.FriendId,
                                                       Name = y.FriendName
                                                   })
                                                   .ToArray())
                    };
        }

        public (Guid newCharacterId, Dictionary<string, string> facadeErrors) TryAdd(Character character)
        {

            try
            {
                var friends = 
                    this.Characters
                        .Where(x => character.Friends.Select(y => y.Id).Contains(x.Id))
                        .ToList();
                if (character.Friends.Any(x => !friends.Select(y => y.Id).Contains(x.Id)))
                {
                    return (Guid.Empty, new Dictionary<string, string> { { nameof(Friends), "Some friends do not exist!" } });
                }
                
                var chr = new CharacterDao()
                {
                    Name = character.Name,
                    
                };
                this.Characters.Add(chr);

                if (SaveChanges() > 0)
                {
                    var chrFriends = character.Friends.Select(x => new CharacterFriend { CharacterId = chr.Id, FriendId = x.Id });
                    CharacterFriends.AddRange(chrFriends);
                    chr.Episodes =
                        character
                            .Episodes
                            .Select(x => new CharacterEpisode { CharacterId = chr.Id, Episode = x })
                            .ToList();
                    SaveChanges();
                    return (chr.Id, new Dictionary<string, string>());
                }
                return (Guid.Empty, new Dictionary<string, string> { { "database error", "Error when saving." } });
            }
            catch (Exception ex)
            {
                return (Guid.Empty, new Dictionary<string, string>{ { "database error", ex.Message } });
            }
        }

        public Dictionary<string, string> TryDelete(Guid characterId)
        {
            var chr=this.Characters.FirstOrDefault(x => x.Id == characterId);
            if(chr is null)
            {
                return new Dictionary<string, string> { { "database error", "Character does not exist" } };
            }
            var removedFriends = 
                CharacterFriends
                    .Where(x => x.FriendId == characterId || x.CharacterId==characterId)
                    .ToList();
            var removedEpisodes = 
                CharacterEpisodes
                    .Where(x => x.CharacterId == characterId)
                    .ToList();
            this.CharacterFriends
                .RemoveRange(removedFriends);
            this.CharacterEpisodes
                .RemoveRange(removedEpisodes);
            SaveChanges();
            try
            { 
                this.Characters.Remove(chr);
                SaveChanges();
            }
            catch (Exception ex)
            {
                this.CharacterFriends.AddRange(removedFriends);
                this.CharacterEpisodes.AddRange(removedEpisodes);
                SaveChanges();
                return new Dictionary<string, string> { { "database error", "There was an error when removing character. Aborted." } };
            }            
            return new Dictionary<string, string>();
        }

        public Dictionary<string, string> TryUpdate(Guid characterId, CharacterUpdateForm characterForm)
        {
            var chr =
                this
                .Characters
                .Include(x => x.Episodes)
                .Include(x => x.Friends)
                .FirstOrDefault(x => x.Id == characterId);
            if (chr is null)
            {
                return new Dictionary<string, string> { { nameof(Character), "Character does not exist" } };
            }

            try
            {
                chr.Name = characterForm.Name;

                var currentFriends = this.CharacterFriends.Where(x => x.CharacterId == chr.Id).ToList();
                var toDelete = 
                    currentFriends
                        .Where(x => !characterForm.Friends.Contains(x.FriendId))
                        .ToList();
                var toAdd =
                    characterForm
                        .Friends
                        .Where(x => !currentFriends.Select(y => y.FriendId).Contains(x))
                        .Select(x =>
                            new CharacterFriend
                            {
                                CharacterId = chr.Id,
                                FriendId = x,
                            })
                        .ToList();

                this.CharacterFriends.RemoveRange(toDelete);
                this.CharacterFriends.AddRange(toAdd);

                var currentEpisodes = this.CharacterEpisodes.Where(x => x.CharacterId == chr.Id).ToList();
                var episodesToDelete =
                    currentEpisodes
                        .Where(x => !characterForm.Episodes.Contains(x.Episode.Value))
                        .ToList();
                var episodesToAdd =
                    characterForm
                        .Episodes
                        .Where(x => !currentEpisodes.Select(y => y.Episode.Value).Contains(x))
                        .Select(x=> 
                            new CharacterEpisode 
                            {
                                CharacterId = chr.Id,
                                Episode = Episode.FromValue(x),
                            })
                        .ToList();
                this.CharacterEpisodes.RemoveRange(episodesToDelete);
                this.CharacterEpisodes.AddRange(episodesToAdd);

                SaveChanges();
                return new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string> { { "database error", ex.Message } };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("swapi");
            modelBuilder.Entity(
                typeof(CharacterDao), 
                x =>
                {
                    x.ToTable(nameof(Character));
                    x.HasKey(nameof(CharacterDao.Id));
                    x.HasMany(nameof(CharacterDao.Friends));
                    x.HasMany(nameof(CharacterDao.Episodes));
                });
            
            modelBuilder.Entity(
                typeof(CharacterFriend),
                x =>
                {
                    x.ToTable(nameof(Character) + nameof(Friend));
                    x.HasKey(nameof(CharacterFriend.CharacterId),nameof(CharacterFriend.FriendId));
                });

            modelBuilder.Entity<FriendDao>()
                .ToView(nameof(Friend) + "_uvw")
                .HasKey(nameof(CharacterFriend.CharacterId), nameof(CharacterFriend.FriendId));
            modelBuilder.Entity<FriendDao>()
                .HasOne<CharacterDao>().WithMany(x => x.Friends).HasForeignKey(x=>x.CharacterId);

            modelBuilder.Entity<CharacterEpisode>()
                .ToTable(nameof(CharacterEpisode))
                .HasOne<CharacterDao>().WithMany(x => x.Episodes).HasForeignKey(x => x.CharacterId);
            modelBuilder.Entity<CharacterEpisode>()
                .Property(x => x.Episode)
                .HasConversion(p => p.Value, v => Episode.FromValue(v));
        }

       
    }

    internal class CharacterDao
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<FriendDao> Friends { get; set; }
        public ICollection<CharacterEpisode> Episodes { get; set; }
    }
    public class CharacterFriend
    {
        public Guid CharacterId { get; set; }
        public Guid FriendId { get; set; }
    }

    public class FriendDao
    {
        public Guid CharacterId { get; set; }
        public Guid FriendId { get; set; }
        public string FriendName { get; set; }
    }

    public class CharacterEpisode
    {
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Episode Episode { get; set; }
    }
}
