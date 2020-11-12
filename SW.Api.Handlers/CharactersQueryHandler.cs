using MediatR;
using SW.Dao;
using SW.Api.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace SW.Api.Handlers
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
