using System;
using System.Linq;
using Canvas.Helpers;
using Canvas.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;

namespace Canvas.Controller {
  public class User : NancyModule {
    private readonly MongoCollection<Users> _user;

    public User(MongoCollection<Users> user) : base("/user/") {
      _user = user;
      var err = new Errors();
      Get["/{id}"] = ctx => {
        Int64 userId;
        if (!Int64.TryParse(ctx.id.ToString(), out userId)) {
          return HttpStatusCode.NotFound;
        }
        Users uploader =
          user.AsQueryable()
            .FirstOrDefault(e => e.UserData.UserId == userId.ToString());
        if (uploader == null) {
          return HttpStatusCode.NotFound;
        }
        string ownerId = uploader.ImageData.OwnerId;
        Users owner =
          user.AsQueryable().FirstOrDefault(e => e.UserData.UserId == ownerId);
        var model = new UserImage();
        model.Uploader = uploader.UserData;
        model.Owner = owner.UserData;
        model.Image = uploader.ImageData;
        //dynamic me = new FacebookClient(Request.Session["access_token"].ToString());
        return View["Views/user", model];
      };
      Post["/{id}/buy"] = ctx => {
        this.EnableCors();
        Int64 userId;
        if (!Int64.TryParse(ctx.id.ToString(), out userId)) {
          return Response.AsJson(err.InvalidUser());
        }
        Int64 buyPrice = 0;
        if (!Int64.TryParse(Request.Form.price.ToString(), out buyPrice)) {
          return Response.AsJson(err.NoBuyPrice());
        }
        if (Request.Session["user_id"] == null) {
          Request.Session["user_id"] = "1350215260";
          // this is my facebook userid added this just for testing
        }
        string sessionuserid = Request.Session["user_id"].ToString();
        //  "1350215260";
        double percentage = 0.0;
        double postercut = 0.0;
        double ownercut = 0.0;
        Users userData =
          _user.AsQueryable()
            .FirstOrDefault(e => e.UserData.UserId == userId.ToString());
        Users sessionUser =
          _user.AsQueryable()
            .FirstOrDefault(e => e.UserData.UserId == sessionuserid);

        string ownerId = userData.ImageData.OwnerId;

        if (sessionUser.UserData.Balance < buyPrice) {
          return Response.AsJson(err.NotEnoughFunds());
        }
        if (userData.ImageData.Price > 1 || userData.ImageData.Price < 1000) {
          percentage = 0.3;
        }
        else if (userData.ImageData.Price > 1000 ||
                 userData.ImageData.Price < 10000) {
          percentage = 0.2;
        }
        else if (userData.ImageData.Price > 10000 ||
                 userData.ImageData.Price < 100000) {
          percentage = 0.1;
        }
        else {
          percentage = 0.05;
        }

        if (Request.Form.price < userData.ImageData.Price + (userData.ImageData.Price*percentage)) {
          return Response.AsJson(err.BidToLow(Convert.ToInt64(userData.ImageData.Price*percentage) + userData.ImageData.Price));
        }
        if (sessionUser.UserData.UserId == userData.UserData.UserId) {
          ownercut = 0;
          postercut = Request.Form.price;
        }
        else {
          postercut = (Request.Form.price - userData.ImageData.Price)*0.1;
          ownercut = (Request.Form.price - userData.ImageData.Price) - postercut;

          var ownerData =
            _user.AsQueryable()
              .FirstOrDefault(e => e.UserData.UserId == ownerId);

          ownerData.UserData.Balance = ownerData.UserData.Balance +
                                       Convert.ToInt64(ownercut);

          _user.Save(ownerData);
        }
        sessionUser.UserData.Balance = sessionUser.UserData.Balance -
                                       Request.Form.price;

        if (userData.ImageData.OwnerId == userData.UserData.UserId) {
          Int64 newbalance = Request.Form.price;
          userData.UserData.Balance = userData.UserData.Balance + newbalance;
        }
        else {
          userData.UserData.Balance = userData.UserData.Balance +
                                      Convert.ToInt64(postercut);
        }
        userData.ImageData.OwnerHistory.Add(ownerId);
        userData.ImageData.OwnerId = sessionuserid;
        userData.ImageData.Price = Request.Form.price;
        _user.Save(sessionUser);
        _user.Save(userData);
        return Response.AsJson(err.BuildResponse(userData.UserData.UserId,"purchased"));

      };
    }
  }
}