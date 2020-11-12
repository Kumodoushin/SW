﻿using MediatR;
using SW.Model;
using SWApi.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace SWApi.Handlers
{
    public class CharactersQueryHandler : IRequestHandler<CharactersQuery, CharactersQueryResponse>
    {
        private readonly CharactersFacade _facade;

        public CharactersQueryHandler(CharactersFacade facade)
        {
            _facade = facade;
        }

        public Task<CharactersQueryResponse> Handle(CharactersQuery request, CancellationToken cancellationToken)
        {
            var result = _facade.Query();
            return Task.FromResult(new CharactersQueryResponse { Data=result});
        }
    }

    
}