using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Betsson.OnlineWallets.Models;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>

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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param> <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
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

    /// <summary>
    /// Helper function to set the balanace back to 0.
    /// </summary>
    public static void ClearBalance()
    {
        var amount = GetBalance();
        Withdraw(amount);
    }

    /// <summary>
    /// Tests whether the balance is 0 for the first time.
    /// </summary>
    [TestMethod]
    public void TestGetBalanceFirsttime()
    {
        ClearBalance();
        var amount = GetBalance();
        Assert.AreEqual( amount, 0 );
    }

    /// <summary>
    /// Tests whether the expected deposited amount is returned back.
    /// </summary>
    [TestMethod]
    public void TestDepositValidValue()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether the expected deposited amounts are returned back.
    /// </summary>
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

    /// <summary>
    /// Tests whether the expected deposited amount is alse returned back when getting the balance. 
    /// </summary>
    [TestMethod]
    public void TestDepositAmountIsSameAsGetBalance()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        Assert.AreEqual(amount, GetBalance());
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing 0 is possible.
    /// </summary>
    [TestMethod]
    public void TestDepositZero()
    {               
        ClearBalance();
        var amount = Deposit(0);
        Assert.AreEqual( amount, 0);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing small amount is possible.
    /// </summary>
    [TestMethod]
    public void TestDepositSmallAmount()
    {               
        ClearBalance();
        var amount = Deposit( 0.00000000000000000001m);
        Assert.AreEqual( amount, 0.00000000000000000001m);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing large amount is possible.
    /// </summary>
    [TestMethod]
    public void TestDepositLargeAmount()
    {               
        ClearBalance();
        var amount = Deposit(9999999999999999999999999999m);
        Assert.AreEqual( amount,  9999999999999999999999999999m);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing negative amount is NOT possible.
    /// </summary>
    [TestMethod]
    public void TestDepositInvalidValue()
    {               
        ClearBalance();
        DepositExpectToFail(-50);
        var amount = GetBalance();
        Assert.AreEqual( amount,  0);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing negative amount for the second time is NOT possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether depositing negative amount for the second time is NOT possible, but after that depositing valid amount is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing valid amount is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing multiple times is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing 0 is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing everything is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing small amount is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing large amount is possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing negative amount is NOT possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing negative amount for the second time is NOT possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing valid amount is possible after trying to withdraw negative amount.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing more than the balance is NOT possible.
    /// </summary>
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

    /// <summary>
    /// Tests whether withdrawing more than the balance for the second time is NOT possible.
    /// </summary>
    [TestMethod]
    public void TestWithdrawTooMuchForSecondTime()
    {               
        ClearBalance();

        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);

        var amount2 = Withdraw(30);
        Assert.AreEqual( amount2,  amount - 30);

        WithdrawExpectToFail(30);

        var balance = GetBalance();
        Assert.AreEqual( amount2,  amount - 30);

        ClearBalance();
    }

    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void TestDepositDepositWithdrawwithdrawDeposit()
    {               
        ClearBalance();

        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);

        var amount2 = Deposit(10);
        Assert.AreEqual( amount2,  50 + 10);

        var amount3 = Withdraw(30);
        Assert.AreEqual( amount3,  amount2 - 30);

        var amount4 = Withdraw(5);
        Assert.AreEqual( amount4,  amount3 - 5);

        var amount5 = Deposit(15);
        Assert.AreEqual( amount5,  amount4 + 15);

        var balance = GetBalance();
        Assert.AreEqual( balance,  amount4 + 15);

        ClearBalance();
    }
}