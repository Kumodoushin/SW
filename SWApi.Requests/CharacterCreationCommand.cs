using MediatR;
using SWApi.Requests.Base;
using System;
using System.Collections.Generic;

namespace SWApi.Requests
{
    public class CharacterCreationCommand : IRequest<CharacterCreationResponse>
    {
        public string Name { get; set; }
        public List<int> Episodes { get; set; }
        public List<Guid> Friends { get; set; }
    }
    public class CharacterCreationResponse : BaseResponse
    {

    }
}
