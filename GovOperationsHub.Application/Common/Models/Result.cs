using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Application.Common.Models
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Success(T data, string message = "تمت العملية بنجاح.")
            => new() { Succeeded = true, Data = data, Message = message };

        public static Result<T> Failure(IEnumerable<string> errors, string message = "حدث خطأ أثناء تنفيذ العملية.")
            => new() { Succeeded = false, Errors = errors.ToList(), Message = message };

        public static Result<T> Failure(string error, string message = "حدث خطأ أثناء تنفيذ العملية.")
            => new() { Succeeded = false, Errors = new() { error }, Message = message };
    }
}
