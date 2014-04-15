using System;
using System.Collections.Generic;
using Canvas.Model;
using MongoDB.Driver;

namespace DevDbInit {
  internal class Program {
    static DateTime RandomDay()
    {
      DateTime start = new DateTime(1995, 1, 1);
      Random gen = new Random();

      int range = (DateTime.Today - start).Days;
      return start.AddDays(gen.Next(range));
    }
    private static void Main(string[] args) {
      string connectionString = "mongodb://localhost";
      var client = new MongoClient(connectionString);
      MongoServer server = client.GetServer();
      MongoDatabase database = server.GetDatabase("ownme_dev");
      MongoCollection<Users> collection = database.GetCollection<Users>("users");

      if (!database.CollectionExists("users")) {
        database.CreateCollection("users");
      }
      List<string> history = new List<string>();
      List<string> useridHistory = new List<string>();
      Random rnd = new Random();
      for (var i = 0; i < 40000; i++) {
        int number = rnd.Next(100000, 1000000);
        int price = 1;
        int useridrand = rnd.Next(0, useridHistory.Count);
        int ran2 = rnd.Next(0, useridrand);
        int balance = 10000;
        string gender = "female";
        string curowner = number.ToString();
        useridHistory.Add(number.ToString());
        history.Add(number.ToString());
        int sex = rnd.Next(0, 10);
        if (sex  % 2 == 0) {
          gender = "male";
        }
        if (ran2 % 2 == 0)
        {
          history.Add(useridHistory[useridrand]);
          curowner = useridHistory[useridrand];
          price = rnd.Next(10, 100000);
          balance = balance + price;
        }
        var User = new Users {
          UserData = new UserData {
            UserId = number.ToString(),
            Name = new Name {
              FirstName = number.ToString(),
              LastName = number.ToString()
            },
            DateOfBirth = RandomDay().ToString(),
            Balance = balance,
            Gender = gender,
          },
          ImageData = new ImageData {
            OwnerId = curowner,
            Price = price,
            OwnerHistory = history
          }
        };
        
        collection.Insert(User);
        /*Console.WriteLine("UserId: " + User.UserData.UserId);
        Console.WriteLine("Date Of Birth: " + User.UserData.DateOfBirth);
        Console.WriteLine("Image Owner: " + User.ImageData.OwnerId);
        Console.WriteLine("Image Price: " + User.ImageData.Price);*/
        history.Clear();
      }
    }
  }
}