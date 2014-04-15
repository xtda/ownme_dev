using Nancy;

namespace Canvas.Helpers {
  public static class NancyExtensions {
    public static void EnableCors(this NancyModule module) {
      module.After.AddItemToEndOfPipeline(
        x => {
          x.Response.WithHeader("Access-Control-Allow-Origin", "*");
          x.Response.WithHeader("Access-Control-Allow-Credentials", "true");
          x.Response.WithHeader("Access-Control-Allow-Methods",
            "GET POST PUT DELETE OPTIONS");
        }
        );
    }
  }
}