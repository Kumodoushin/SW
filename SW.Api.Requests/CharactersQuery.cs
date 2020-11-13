using SW.Model;
using SW.Api.Requests.Base;

namespace SW.Api.Requests
{
    public class CharactersQuery : PaginatedRequest<CharactersQueryResponse>
    {
    }

    public class CharactersQueryResponse: PaginatedResponse<Characters>
    {
    }
}
