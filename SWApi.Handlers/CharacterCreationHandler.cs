using MediatR;
using SW.Model;
using SWApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{
    public class CharacterCreationHandler : IRequestHandler<CharacterCreationCommand, CharacterCreationResponse>
    {
        private readonly CharactersFacade _facade;

        public CharacterCreationHandler(CharactersFacade facade)
        {
            _facade = facade;
        }

        public Task<CharacterCreationResponse> Handle(CharacterCreationCommand request, CancellationToken cancellationToken)
        {
            var existingOnes = _facade.Query();

            var errors = new Dictionary<string, string>();
            var newId = Guid.NewGuid();

            if (existingOnes.Count(x => x.Name == request.Name) > 0)
            {
                errors.Add(nameof(request.Name), $"A character with name {request.Name} already exists.");
            }

            var existingGuids = existingOnes.Select(x => x.Id);
            if (!request.Friends.TrueForAll(x => existingGuids.Contains(x)))
            {
                errors.Add(nameof(request.Friends), "Some of provided friends don't exist.");
            }

            if (!request.Episodes.TrueForAll(x => Episode.List.Count(y => y.Value == x) == 1))
            {
                errors.Add(nameof(request.Episodes), "Some of provided episodes don't exist.");
            }

            if (errors.Count == 0)
            {
                var (success, facadeErrors) =
                  _facade.TryAdd(
                      new Character
                      {
                          Id = newId,
                          Name = request.Name,
                          Episodes =
                            new Episodes(
                                Episode.List
                                       .Where(x => request
                                                        .Episodes
                                                        .Contains(x.Value))
                                       .ToArray()),
                          Friends =
                            new Friends(
                                existingOnes.Where(x => request
                                                            .Friends
                                                            .Contains(x.Id))
                                            .ToArray())
                      });
                if (success)
                {
                    return Task.FromResult(new CharacterCreationResponse{ Id = newId });
                }
                foreach (var error in facadeErrors)
                {
                    errors.Add(error.Key, error.Value);
                }
            }
            return Task.FromResult(new CharacterCreationResponse { Errors = errors });

        }
    }

    
}
