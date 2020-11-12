using MediatR;
using SW.Model;
using SWApi.Dao;
using SWApi.Requests;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{

    public class CharacterQueryHandler : IRequestHandler<CharacterQuery, CharacterQueryResponse>
    {
        private readonly ICharacterFacade _facade;

        public CharacterQueryHandler(ICharacterFacade facade)
        {
            _facade = facade;
        }

        public Task<CharacterQueryResponse> Handle(CharacterQuery request, CancellationToken cancellationToken)
        {
            var character = _facade.QueryById(request.Id);

            if (character is null)
            {
                return Task.FromResult(new CharacterQueryResponse { Id = request.Id, Errors = new Dictionary<string, string> { { nameof(Character), $"Character with id {request.Id} was not found." } } });                
            }
            return Task.FromResult(new CharacterQueryResponse { Id = request.Id, Data = character });
        }
    }

    
}
