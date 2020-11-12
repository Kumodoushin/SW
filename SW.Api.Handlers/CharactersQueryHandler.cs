using MediatR;
using SW.Dao;
using SW.Api.Requests;
using System.Threading;
using System.Threading.Tasks;
using SW.Model;

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
            Characters result;
            if (request?.PaginationOptions != null)
            {
                int pageNr;
                int charactersCount;
                (result, pageNr, charactersCount) = _facade.QueryPaginated(request.PaginationOptions);
                return Task.FromResult(new CharactersQueryResponse { Data = result,Total=charactersCount,CurrentPage=pageNr });
            }
            else
            {
                result = _facade.Query();
                return Task.FromResult(new CharactersQueryResponse { Data = result });
            }
            

            
        }
    }

    
}
