using MediatR;
using SW.Model;
using SWApi.Dao;
using SWApi.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{
    public class CharactersQueryHandler : IRequestHandler<CharactersQuery, CharactersQueryResponse>
    {
        private readonly ICharacterFacade _facade;

        public CharactersQueryHandler(ICharacterFacade facade)
        {
            _facade = facade;
        }

        public Task<CharactersQueryResponse> Handle(CharactersQuery request, CancellationToken cancellationToken)
        {
            var result = _facade.Query();
            return Task.FromResult(new CharactersQueryResponse { Data=result});
        }
    }

    
}
