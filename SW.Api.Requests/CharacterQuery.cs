using MediatR;
using SW.Model;
using SW.Api.Requests.Base;
using System;

namespace SW.Api.Requests
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
