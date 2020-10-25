﻿using PDCore.Enums;
using PDCore.Web.Helpers.ExceptionHandling;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace PDCore.Web.Handlers
{
    /// <summary>
    /// Obsługa wyjątków na poziomie aplikacji
    /// </summary>
    public class LogExceptionHandler : ExceptionHandler
    {
        //public bool AllowMultiple => true;

        //public async Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        //{
        //    await ErrorLogService<T>.LogAsync(actionExecutedContext.Exception, LogType.Error);


        //    Exception exception = actionExecutedContext.Exception;

        //    HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

        //    if (exception is HttpException httpException)
        //    {
        //        httpStatusCode = (HttpStatusCode)httpException.GetHttpCode();
        //    }

        //    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(httpStatusCode, exception.Message);
        //}

        public override async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var result = await HttpApplicationErrorHandler.HandleExceptionAsync(context.Exception, LogType.Error);

            var response = context.Request.CreateErrorResponse(result.Item2, result.Item1);

            context.Result = new ResponseMessageResult(response);
        }

        //private class TextPlainErrorResult : IHttpActionResult
        //{
        //    public TextPlainErrorResult()
        //    {
        //    }

        //    public TextPlainErrorResult(HttpRequestMessage request, string content, HttpStatusCode httpStatusCode)
        //    {
        //        Request = request;
        //        Content = content;
        //        HttpStatusCode = httpStatusCode;
        //    }

        //    public HttpRequestMessage Request { get; set; }

        //    public string Content { get; set; }

        //    public HttpStatusCode HttpStatusCode { get; set; }


        //    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        //    {
        //        HttpResponseMessage response =
        //                         new HttpResponseMessage(HttpStatusCode)
        //                         {
        //                             Content = new StringContent(Content),
        //                             RequestMessage = Request
        //                         };

        //        return Task.FromResult(response);
        //    }
        //}
    }
}
