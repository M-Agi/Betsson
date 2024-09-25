using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Betsson.OnlineWallets.Models;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{
    public static decimal GetBalance()
    {
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } );        

        var request = new RestRequest( "/onlinewallet/balance");

        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        var json = JsonConvert.DeserializeObject<Balance>(response.Content);
        return json.Amount;
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
        var json = JsonConvert.DeserializeObject<Balance>(response.Content);
        return json.Amount;
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
        var json = JsonConvert.DeserializeObject<Balance>(response.Content);
        return json.Amount;
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
}