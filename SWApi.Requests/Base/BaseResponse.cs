using System;
using System.Collections.Generic;
using System.Linq;

namespace SWApi.Requests.Base
{
    public abstract class BaseResponse
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public bool IsSuccessful => !Errors.Any();
        public BaseResponse WithError(string Key, string Value)
        {
            Errors.Add(Key, Value);
            return this;
        }
    }

    public abstract class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
