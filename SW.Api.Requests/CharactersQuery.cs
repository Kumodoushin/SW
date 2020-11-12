using MediatR;
using SW.Model;
using SW.Api.Requests.Base;

namespace SW.Api.Requests
{
    public class CharactersQuery : IRequest<CharactersQueryResponse>
    {
    }

    public class CharactersQueryResponse:BaseResponse<Characters>
    {
    }
}
