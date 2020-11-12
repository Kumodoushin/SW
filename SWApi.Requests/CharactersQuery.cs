using MediatR;
using SW.Model;
using SWApi.Requests.Base;

namespace SWApi.Requests
{
    public class CharactersQuery : IRequest<CharactersQueryResponse>
    {
    }

    public class CharactersQueryResponse:BaseResponse<Characters>
    {
    }
}
