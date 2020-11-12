using MediatR;
using SW.Model;
using SWApi.Requests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{
    public class CharacterDeletionHandler : IRequestHandler<CharacterDeletionCommand, CharacterDeletionResponse>
    {
        private readonly CharactersFacade _facade;

        public CharacterDeletionHandler(CharactersFacade characterFacade)
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
