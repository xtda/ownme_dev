using System;
using System.Collections.Generic;
using System.Linq;
using Canvas.Helpers;
using Canvas.Model;
using Facebook;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;

namespace Canvas.Controller {
  public class Index : NancyModule {
    private readonly MongoCollection<Users> _user;

    public Index(MongoCollection<Users> user) {
      _user = user;
      var err = new Errors();
      Get["/"] = ctx => {
        return View["views/index"];
      };
      Get["/profile"] = ctx => {
        var profile = new UserProfile();
        if (Request.Session["user_id"] == null) {
          Request.Session["user_id"] = "1350215260";
        }
        string userId = Request.Session["user_id"].ToString();
        Users newuser =
          _user.AsQueryable().FirstOrDefault(e => e.UserData.UserId == userId);

        profile.User = newuser;

        IQueryable<Users> newImage =
          _user.AsQueryable().Where(e => e.ImageData.OwnerId == userId);

        profile.Images = newImage;
        return View["Views/profile", profile];
      };
      Get["/random/{number}"] = ctx => {
        this.EnableCors();
        int number;
        if (!int.TryParse(ctx.number.ToString(), out number)) {
          return HttpStatusCode.NotFound;
        }
        int count = _user.AsQueryable().Count();
        if (count == 0) {
          return HttpStatusCode.NotFound;
        }
        if (count < 10) {
          count = 0;
        }
        var random = new Random();

        var userQuery =
          _user.AsQueryable()
            .Skip(random.Next(0, count))
            .Take(number)
            .Select(
              e =>
                new {
                  Id = e.Id.ToString(),
                  e.UserData.UserId,
                  e.UserData.Name.FirstName,
                  e.UserData.Name.LastName,
                  e.ImageData.OwnerId,
                  e.ImageData.Price,
                  e.ImageData.OwnerHistory
                });
        return Response.AsJson(userQuery);
        //return HttpStatusCode.NotImplemented;
      };
      // Facebook canvas route
      // This route handles parsing and decoding the signed_request that facebook sends to confirm user indentiy 
      // See: https://developers.facebook.com/docs/reference/login/signed-request/ for more information
      // After it has decoded the signed request data it adds user_id and the oauth token to the session cookie for future use
      // It then queries the database to see if a user exists and redirects to the main route if it does
      // If no user is found it sets up a new user and redirects
      Post["/"] = ctx => {
        var facebookClient = new FacebookClient();
        dynamic signedRequest =
          facebookClient.ParseSignedRequest("54ad6adb424a91c73bc5ff1e2ded756e",
            Request.Form.signed_request);
        if (!signedRequest.ContainsKey("user_id")) {
          return View["Views/shared/oauthredirect"];
        }
        Request.Session["user_id"] = signedRequest.user_id;
        Request.Session["access_token"] = signedRequest.oauth_token;
        string userId = signedRequest.user_id;
        int userCount =
          _user.AsQueryable().Count(e => e.UserData.UserId == userId);

        if (userCount == 0) {
          var newUserFacebookClient =
            new FacebookClient(signedRequest.oauth_token);
          dynamic me = newUserFacebookClient.Get("/me");

          var newUser = new Users {
            UserData = new UserData {
              UserId = signedRequest.user_id,
              Gender = me.gender,
              DateOfBirth = me.birthday,
              Name = new Name {
                FirstName = me.first_name,
                LastName = me.last_name
              }
            },
            ImageData = new ImageData {
              OwnerId = signedRequest.user_id,
              OwnerHistory = new List<string>()
            }
          };
          _user.Save(newUser);
        }
        var bannedCheck =
          _user.AsQueryable().FirstOrDefault(e => e.UserData.UserId == userId);

        if (bannedCheck.UserData.Banned) {
          return View["Views/Banned"];
        }
        return Response.AsRedirect("/");
      };
    }
  }
}