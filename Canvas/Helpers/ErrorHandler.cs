using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace Canvas.Helpers {
  public class ErrorHandler : DefaultViewRenderer, IStatusCodeHandler {
    private readonly Dictionary<HttpStatusCode, string> _errors =
      new Dictionary<HttpStatusCode, string> {
        {HttpStatusCode.InternalServerError, "Errors/500.html"},
        {HttpStatusCode.NotFound, "Errors/404.html"}
      };

    public ErrorHandler(IViewFactory factory) : base(factory) {}

    #region IStatusCodeHandler Members

    public bool HandlesStatusCode(HttpStatusCode statusCode,
      NancyContext context) {
      return _errors.ContainsKey(statusCode);
    }

    public void Handle(HttpStatusCode statusCode, NancyContext context) {
      KeyValuePair<HttpStatusCode, string> error =
        _errors.First(x => x.Key == statusCode);
      Response response = RenderView(context, error.Value);
      response.StatusCode = error.Key;
      context.Response = response;
    }

    #endregion
  }
}