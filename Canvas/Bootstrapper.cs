using System.Configuration;
using System.Linq;
using Canvas.Model;
using MongoDB.Driver;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using Nancy.TinyIoc;

namespace Canvas {
  public class Bootstrapper : DefaultNancyBootstrapper {
    protected override void ApplicationStartup(TinyIoCContainer container,
      IPipelines pipelines) {
      base.ApplicationStartup(container, pipelines);
      CookieBasedSessions.Enable(pipelines);
    }

    protected override void ConfigureApplicationContainer(
      TinyIoCContainer container) {
      const string connectionString = "mongodb://localhost";
      var mongoClient = new MongoClient(connectionString);
      var mongoServer = mongoClient.GetServer();
      var database = mongoServer.GetDatabase("ownme_dev");

      if (!database.CollectionExists("users")) {
        database.CreateCollection("users");
      }
      base.ConfigureApplicationContainer(container);
      
      container.Register<MongoServer>(mongoServer);
      container.Register<MongoDatabase>(database);
      container.Register<MongoCollection<Users>>(database.GetCollection<Users>("users"));
    }
  }
}