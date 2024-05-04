using System.Net;

namespace DemoAPI.Models
{
    public class ResponseModel<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Status { get; set; }

        public string Error { get; set; }

        public string ErrorId { get; set; }

        public string Message { get; set; }

        public string MessageCode { get; set; }

        public T Result { get; set; }
    }

    public class ResponseModelPaginated<T> : ResponseModel<T>
    {
        public int? TotalRecords { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
