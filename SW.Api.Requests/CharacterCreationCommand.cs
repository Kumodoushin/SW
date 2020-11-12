using MediatR;
using SW.Api.Requests.Base;
using System;
using System.Collections.Generic;

namespace SW.Api.Requests
{
    public class CharacterCreationCommand : IRequest<CharacterCreationResponse>
    {
        public string Name { get; set; }
        public List<int> Episodes { get; set; } = new List<int>();
        public List<Guid> Friends { get; set; } = new List<Guid>();
    }
    public class CharacterCreationResponse : BaseResponse
    {

    }
}
