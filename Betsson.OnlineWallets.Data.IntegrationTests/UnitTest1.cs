using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Betsson.OnlineWallets.Models;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{
    public static RestResponse GetBalance()
    {
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } );        

        var request = new RestRequest( "/onlinewallet/balance");

        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return response;
    }

    public static RestResponse Deposit(decimal amount)
    {
        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/deposit", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return response;
    }

    public static RestResponse Withdraw(decimal amount)
    {
        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/withdraw", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        return response;
    }

    public static void ClearBalance()
    {
        var response = GetBalance();
        if (response.Content != "{\"amount\":0}")
        {
            var json = JsonConvert.DeserializeObject<Balance>(response.Content);
            Withdraw(json.Amount);
        }

    }

    [TestMethod]
    public void TestGetBalanceFirsttime()
    {
        ClearBalance();
        var response = GetBalance();
        Assert.AreEqual( response.Content,  "{\"amount\":0}" );
    }


    [TestMethod]
    public void TestDepositValidValue()
    {               
        ClearBalance();
        var response = Deposit(50);
        Assert.AreEqual( response.Content,  "{\"amount\":50}" );
        ClearBalance();
    }
}