using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Canvas.Model {
  public class Name {
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }

  public class UserData {
    public UserData() {
      Balance = 10000;
      Banned = false;
      BannedReason = String.Empty;
    }

    public string UserId { get; set; }
    public Name Name { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Location { get; set; }
    public long Balance { get; set; }
    public bool Banned { get; set; }
    public string BannedReason { get; set; }
  }

  public class ImageData {
    public ImageData() {
      Price = 1;
    }
    
    public string OwnerId { get; set; }
    public List<string> OwnerHistory { get; set; }
    public long Price { get; set; }
  }

  public class Users {
    public ObjectId Id { get; set; }
    public UserData UserData { get; set; }
    public ImageData ImageData { get; set; }
  }
}