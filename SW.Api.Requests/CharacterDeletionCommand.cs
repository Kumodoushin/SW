using MediatR;
using SW.Api.Requests.Base;
using System;

namespace SW.Api.Requests
{
    public class CharacterDeletionCommand : IRequest<CharacterDeletionResponse>
    {
        public CharacterDeletionCommand(Guid characterId)
        {
            CharacterId = characterId;
        }
        public Guid CharacterId { get; }
    }

    public class CharacterDeletionResponse : BaseResponse
    {
    }
}
