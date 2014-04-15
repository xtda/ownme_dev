using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Canvas
{
  public class Utils
  {
    public static int CalculateAgeCorrect(DateTime birthDate) {
      var now = DateTime.Now;
      int age = now.Year - birthDate.Year;
      if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
      return age;
    }
  }
}