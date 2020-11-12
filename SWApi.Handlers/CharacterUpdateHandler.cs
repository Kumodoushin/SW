using MediatR;
using SW.Model;
using SWApi.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{
    public class CharacterUpdateHandler : IRequestHandler<CharacterUpdateCommand, CharacterUpdateResponse>
    {
        private readonly CharactersFacade _facade;

        public CharacterUpdateHandler(CharactersFacade facade)
        {
            _facade = facade;
        }
        public Task<CharacterUpdateResponse> Handle(CharacterUpdateCommand request, CancellationToken cancellationToken)
        {
            var response = new CharacterUpdateResponse { Id = request.CharacterId };
            var updatedCharacter = _facade.QueryById(request.CharacterId);
            if (updatedCharacter is null)
            {
                response.WithError("Not found",$"Character with id {request.CharacterId} was not found." );
            }
            if (request.CharacterUpdateForm.Episodes.Any(x => Episode.List.All(y => y.Value != x)))
            {
                response.WithError(nameof(request.CharacterUpdateForm.Episodes),$"Some invalid episodes provided.");
            }

            if (response.IsSuccessful)
            {
                Dictionary<string, string> facadeErrors = _facade.TryUpdate(request.CharacterId, request.CharacterUpdateForm);
                foreach (var error in facadeErrors)
                {
                    response.WithError(error.Key, error.Value);
                }
            }
            return Task.FromResult(response);
        }
    }
}
