using MediatR;
using SW.Model.Helpers;

namespace SW.Api.Requests.Base
{

    public abstract class PaginatedRequest<T> : IRequest<T>
    {
        public PaginationOptions PaginationOptions { get; set; }
    }
    

    public abstract class PaginatedResponse<T> : BaseResponse<T>
    {        
        public int Total { get; set; }
        public int CurrentPage { get; set; }
    }
}
