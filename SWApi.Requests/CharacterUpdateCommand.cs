using MediatR;
using SW.Model;
using SWApi.Requests.Base;
using System;

namespace SWApi.Requests
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
