using MediatR;
using SW.Dao;
using SW.Api.Requests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SW.Api.Handlers
{
    public class CharacterDeletionHandler : IRequestHandler<CharacterDeletionCommand, CharacterDeletionResponse>
    {
        private readonly ICharacterFacade _facade;

        public CharacterDeletionHandler(ICharacterFacade characterFacade)
        {
            _facade = characterFacade;
        }
        public Task<CharacterDeletionResponse> Handle(CharacterDeletionCommand request, CancellationToken cancellationToken)
        {
            var response = new CharacterDeletionResponse();
            var facadeErrors = _facade.TryDelete(request.CharacterId);
            if (facadeErrors.Any())
            {
                foreach (var error in facadeErrors)
                {
                    response.WithError(error.Key, error.Value);
                }
                
            }
            return Task.FromResult(response);
        }
    }
}
