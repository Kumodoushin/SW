using MediatR;
using SW.Api.Requests.Base;
using SW.Model;
using System;

namespace SW.Api.Requests
{
    

    public class CharacterUpdateCommand:IRequest<CharacterUpdateResponse>
    {
        public CharacterUpdateCommand(Guid characterId,CharacterUpdateForm characterUpdateForm)
        {
            CharacterId = characterId;
            CharacterUpdateForm = characterUpdateForm;
        }

        public Guid CharacterId { get; }
        public CharacterUpdateForm CharacterUpdateForm { get; }
    }

    public class CharacterUpdateResponse:BaseResponse
    {
    }
}
