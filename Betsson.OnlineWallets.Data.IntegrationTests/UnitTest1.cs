using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Betsson.OnlineWallets.Models;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{


    public static decimal DeserializeResponse(RestResponse response)
    {
        if (response.Content != null)
        {
            var json = JsonConvert.DeserializeObject<Balance>(response.Content);
            if (json != null) {
                return json.Amount;
            }
        }
        Assert.Fail("Response Content is invalid!");
        return 0;
    }
    public static decimal GetBalance()
    {
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } );        

        var request = new RestRequest( "/onlinewallet/balance");

        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return DeserializeResponse(response);
    }

    public static decimal Deposit(decimal amount)
    {        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/deposit", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return DeserializeResponse(response);
    }

    public static void DepositExpectToFail(decimal amount)
    {        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/deposit", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        Assert.AreEqual( response.StatusDescription, "Bad Request");
    }


    public static decimal Withdraw(decimal amount)
    {
        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/withdraw", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return DeserializeResponse(response);
    }

    public static void WithdrawExpectToFail(decimal amount)
    {        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/withdraw", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        Assert.AreEqual( response.StatusDescription, "Bad Request");
    }

    public static void ClearBalance()
    {
        var amount = GetBalance();
        Withdraw(amount);
    }

    [TestMethod]
    public void TestGetBalanceFirsttime()
    {
        ClearBalance();
        var amount = GetBalance();
        Assert.AreEqual( amount, 0 );
    }


    [TestMethod]
    public void TestDepositValidValue()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositMultipleTimes()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var amount2 = Deposit(25);
        Assert.AreEqual( amount2,  75);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositAmountIsSameAsGetBalance()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        Assert.AreEqual(amount, GetBalance());
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositZero()
    {               
        ClearBalance();
        var amount = Deposit(0);
        Assert.AreEqual( amount, 0);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositSmallAmount()
    {               
        ClearBalance();
        var amount = Deposit( 0.00000000000000000001m);
        Assert.AreEqual( amount, 0.00000000000000000001m);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositLargeAmount()
    {               
        ClearBalance();
        var amount = Deposit(99999999999999999999999m);
        Assert.AreEqual( amount,  99999999999999999999999m);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositInvalidValue()
    {               
        ClearBalance();
        DepositExpectToFail(-50);
        var amount = GetBalance();
        Assert.AreEqual( amount,  0);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositFirstValidThanInvalidValue()
    {               
        ClearBalance();

        var amount = Deposit(999999m);
        Assert.AreEqual( amount,  999999m);

        DepositExpectToFail(-50);

        var balance = GetBalance();
        Assert.AreEqual( balance,  999999m);
        ClearBalance();
    }

    [TestMethod]
    public void TestDepositFirstValidThanInvalidThanValidValue()
    {               
        ClearBalance();

        var amount = Deposit(9);
        Assert.AreEqual( amount,  9);

        DepositExpectToFail(-50);

        var balance = GetBalance();
        Assert.AreEqual( balance,  9);

        var amount2 = Deposit(10);
        Assert.AreEqual( amount2,  9 + 10);

        ClearBalance();
    }


    [TestMethod]
    public void TestWithdrawValidValue()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(12);
        Assert.AreEqual( balance, amount - 12);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawMultipleTimes()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var amount2 = Withdraw(12);
        Assert.AreEqual( amount2, amount - 12);
        var amount3 = Withdraw(11);
        Assert.AreEqual( amount3, amount - 12 - 11);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawZero()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(0);
        Assert.AreEqual( balance, amount - 0);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawAll()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(amount);
        Assert.AreEqual( balance, 0);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawSmallAmount()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(0.000000000000000000000000001m);
        Assert.AreEqual( balance, amount - 0.000000000000000000000000001m);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawLargeAmount()
    {               
        ClearBalance();
        var amount = Deposit(9999999999999999999999999999m);
        Assert.AreEqual( amount,  9999999999999999999999999999m);
        var balance = Withdraw(9999999999999999999999999998m);
        Assert.AreEqual( balance, amount - 9999999999999999999999999998m);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawInvalidValue()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        WithdrawExpectToFail(-50);
        var balance = GetBalance();
        Assert.AreEqual( balance, 50);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawFirstValidThanInvalidValue()
    {               
        ClearBalance();

        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);

        var amount2 = Withdraw(30);
        Assert.AreEqual( amount2,  50-30);

        WithdrawExpectToFail(-5);

        var balance = GetBalance();
        Assert.AreEqual( balance, amount2);
        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawFirstValidThanInvalidThanValidValue()
    {               
        ClearBalance();

        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);

        var amount2 = Withdraw(30);
        Assert.AreEqual( amount2,  50-30);

        WithdrawExpectToFail(-5);

        var balance = GetBalance();
        Assert.AreEqual( balance, amount2);

        var amount3 = Withdraw(10);
        Assert.AreEqual( amount3,  balance - 10);

        ClearBalance();
    }

    [TestMethod]
    public void TestWithdrawTooMuch()
    {               
        ClearBalance();

        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);

        WithdrawExpectToFail(50.0000000000001m);

        var balance = GetBalance();
        Assert.AreEqual( balance, 50);

        ClearBalance();
    }
}