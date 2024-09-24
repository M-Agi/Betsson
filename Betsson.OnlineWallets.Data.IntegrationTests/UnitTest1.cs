using RestSharp;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } );        

        var request = new RestRequest( "/onlinewallet/balance");

        var response = client.ExecuteGet( request );
        
        Assert.AreEqual( response.StatusCode, System.Net.HttpStatusCode.OK);
        Assert.AreEqual( response.StatusDescription, "OK");
        Assert.AreEqual( response.Content,  "{\"amount\":50}" );
    }
}