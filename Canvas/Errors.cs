using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Canvas
{
  public class ErrorResponse {
    public string Status = "Error";
    public string Message = string.Empty;
  }
  public class Errors {
      ErrorResponse err = new ErrorResponse();

    public ErrorResponse InvalidUser() {
      this.err.Message = "Invalid User Id";
      return err;
    }

    public ErrorResponse UserNotFound() {
      err.Message = "User Id not found";
      return err;
    }

    public ErrorResponse NotEnoughFunds() {
      err.Message = "Not enough funds to purchase";
      return err;
    }

    public ErrorResponse NoBuyPrice() {
      err.Message = "Please enter a price to bid";
      return err;
    }

    public ErrorResponse BidToLow(long input) {
      err.Message = "Your bid is to low it must be at least " + input;
      return err;
    }

    public ErrorResponse BuildResponse(string input,string status = "Error") {
      err.Status = status;
      err.Message = input;
      return err;
    }
  }
}