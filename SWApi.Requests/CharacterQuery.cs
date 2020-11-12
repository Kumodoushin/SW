using MediatR;
using SW.Model;
using SWApi.Requests.Base;
using System;

namespace SWApi.Requests
{
    public class CharacterQuery : IRequest<CharacterQueryResponse>
    {
        public CharacterQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class CharacterQueryResponse:BaseResponse<Character>
    {
        
    }
}
