using MediatR;
using SWApi.Requests.Base;
using System;

namespace SWApi.Requests
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
