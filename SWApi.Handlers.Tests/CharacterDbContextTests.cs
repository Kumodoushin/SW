using System.Threading;
using Xunit;
using SWApi.Requests;
using System.Linq;
using System.Collections.Generic;
using System;
using SWApi.Dao;
using Microsoft.EntityFrameworkCore;

namespace SWApi.Handlers.Tests
{
    public class CharacterDbContextTests
    {
        private readonly string _connectionString="Data Source=(LocalDb)\\.;Initial Catalog=StarWars;Integrated Security=True;Connect Timeout=60";
        [Fact]
        public void QueryingCollectionHandler_ReturnsAllCharactersFromFacade()
        {
            var facade = 
                new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options);
            var characters = facade.Query();
            Assert.NotNull(characters);
            Assert.NotEmpty(characters);
            var handler = new CharactersQueryHandler(facade);
            var result = handler.Handle(new CharactersQuery(), new CancellationToken()).GetAwaiter().GetResult();
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.True(result.IsSuccessful);
            Assert.Equal(characters.Count,result.Data.Count);
        }
        
        [Fact]
        public void QueryingSingleHandler_ReturnsSpecificCharacterFromFacade()
        {
            var facade =
                new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options);
            var characterIds = facade.Query().Select(x=>x.Id).ToList();
            Assert.NotNull(characterIds);
            Assert.NotEmpty(characterIds);
            var handler = new CharacterQueryHandler(facade);
            var results = new List<CharacterQueryResponse>();
            foreach(var id in characterIds)
            {
                results.Add(handler.Handle(new CharacterQuery(id), new CancellationToken()).GetAwaiter().GetResult());
            }
            
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            foreach (var result in results)
            {
                Assert.True(result.IsSuccessful);
                Assert.Contains(result.Id, characterIds);
            }
        }

        [Fact]
        public void SubsequentQueryReturnsTheSameValues()
        {
            var facade =
                new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options);
            var characterIds = facade.Query().Select(x => x.Id).ToList();
            Assert.NotNull(characterIds);
            Assert.NotEmpty(characterIds);
            var handler = new CharacterQueryHandler(facade);
            
            foreach (var id in characterIds)
            {
                var result1= handler.Handle(new CharacterQuery(id), new CancellationToken()).GetAwaiter().GetResult();
                var result2 = handler.Handle(new CharacterQuery(id), new CancellationToken()).GetAwaiter().GetResult();
                Assert.NotSame(result1, result2);
                Assert.True(result1.IsSuccessful);
                Assert.True(result2.IsSuccessful);
                
                Assert.Equal(result1.Data.Id, result2.Data.Id);
                Assert.Equal(result1.Data.Name, result2.Data.Name);
                Assert.Equal(result1.Data.Episodes.Count, result2.Data.Episodes.Count);
                Assert.Equal(result1.Data.Friends.Count, result2.Data.Friends.Count);

                var res1episodes = result1.Data.Episodes.OrderBy(x => x.Value).ToList();
                var res2episodes = result2.Data.Episodes.OrderBy(x => x.Value).ToList();                
                for (int i=0;i<res1episodes.Count;i++)
                {
                    Assert.Equal(res1episodes[i], res2episodes[i]);
                }

                var res1friends = result1.Data.Friends.OrderBy(x => x.Id).ToList();
                var res2friends = result2.Data.Friends.OrderBy(x => x.Id).ToList();
                for (int i = 0; i < res1friends.Count; i++)
                {
                    Assert.Equal(res1friends[i].Id, res2friends[i].Id);
                    Assert.Equal(res1friends[i].Name, res2friends[i].Name);
                }

            }

        }
    
        [Fact]
        public void FullLifecycle()
        {           
            var creationHandler = 
                new CharacterCreationHandler(new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options));
            string oldName = $"Random character with guid name {Guid.NewGuid()}";
            var creationResult =
                creationHandler.Handle(new CharacterCreationCommand() { Name = oldName, }, new CancellationToken())
                               .GetAwaiter()
                               .GetResult();
            Assert.True(creationResult.IsSuccessful);

            var queryHandler = 
                new CharacterQueryHandler(new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options));
            var queryResult = 
                queryHandler.Handle(new CharacterQuery(creationResult.Id), new CancellationToken())
                            .GetAwaiter()
                            .GetResult();
            Assert.True(queryResult.IsSuccessful);

            string newName = $"New name with guid {Guid.NewGuid()}";
            var updateHandler = 
                new CharacterUpdateHandler(new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options));
            var updateResult =
                updateHandler.Handle(new CharacterUpdateCommand(creationResult.Id, new CharacterUpdateForm() { Name = newName }), new CancellationToken())
                             .GetAwaiter()
                             .GetResult();
            Assert.True(updateResult.IsSuccessful);

            var updatedQueryResult =
                queryHandler.Handle(new CharacterQuery(creationResult.Id), new CancellationToken())
                            .GetAwaiter()
                            .GetResult();
            Assert.True(updatedQueryResult.IsSuccessful);

            Assert.Equal(queryResult.Data.Id, updatedQueryResult.Data.Id);
            Assert.Equal(newName, updatedQueryResult.Data.Name);
            Assert.NotEqual(queryResult.Data.Name, updatedQueryResult.Data.Name);

            var deleteHandler = 
                new CharacterDeletionHandler(new CharactersContext(new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options));
            var deletionResult =
                deleteHandler.Handle(new CharacterDeletionCommand(creationResult.Id), new CancellationToken())
                             .GetAwaiter()
                             .GetResult();

            Assert.True(deletionResult.IsSuccessful);
            var requeryResult =
                queryHandler.Handle(new CharacterQuery(creationResult.Id), new CancellationToken())
                            .GetAwaiter()
                            .GetResult();
            Assert.False(requeryResult.IsSuccessful);
            var deletionRetryResult =
                deleteHandler.Handle(new CharacterDeletionCommand(creationResult.Id), new CancellationToken())
                             .GetAwaiter()
                             .GetResult();
            Assert.False(deletionRetryResult.IsSuccessful);
        }
    }
}
