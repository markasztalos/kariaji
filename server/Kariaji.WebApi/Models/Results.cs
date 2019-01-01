using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Kariaji.WebApi.Models
{
    public class CommonResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static CommonResult NewError(string message = null) =>
            new CommonResult { Message = message, Success = false };
        public static CommonResult NewSuccess(string message = null) =>
            new CommonResult { Message = message, Success = true };
    }

    public class CommonResult<T> : CommonResult
    {
        public T Result { get; set; }

        public static CommonResult<T> NewSuccess<T>(T result, string message = null) =>
            new CommonResult<T> { Result = result, Message = message, Success = true };
    }

}
