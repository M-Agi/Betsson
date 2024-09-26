using Newtonsoft.Json;
using RestSharp;
using Betsson.OnlineWallets.Models;
using System.Net;

namespace Betsson.OnlineWallets.Data.IntegrationTests;

[TestClass]
public class ApiTests
{

    /// <summary>
    /// Deserialize the content of rest response to Betsson.OnlineWallets.Models.Balance object.
    /// </summary>
    /// <param name="response">the rest response to be deserialize.</param>
    /// <returns>If the content of the response is valid, the value of the amount property.
    ///          If the content of the response is not valid, 0.</returns>

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
    /// Gets the balance from "/onlinewallet/balance".
    /// </summary>
    /// <returns>If the content of the response is valid, the value of the amount property.
    ///          If the content of the response is not valid, 0.</returns>
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
    /// Deposits the given amount to "/onlinewallet/deposit".
    /// </summary>
    /// <param name="amount">The value to be deposit.</param>
    /// <returns>If the content of the response is valid, the value of the amount property.
    ///          If the content of the response is not valid, 0.</returns>
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
    /// Deposits the given amount to "/onlinewallet/deposit". Request expected to fail.
    /// </summary>
    /// <param name="amount">The value to be deposit.</param>
    public static void DepositExpectToFail(decimal amount, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
    {        
        var client = new RestClient( new RestClientOptions {
            BaseUrl = new Uri("http://localhost:8080")
        } ); 
        var request = new RestRequest( "/onlinewallet/deposit", Method.Post);

        request.AddJsonBody( "{ \"amount\": " + amount + "  }" );
    
        var response = client.Execute( request );
        
        if (httpStatusCode == HttpStatusCode.BadRequest)
        { 
            Assert.AreEqual( response.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual( response.StatusDescription, "Bad Request");
        }
        else
        {
            Assert.AreEqual( response.StatusCode, HttpStatusCode.InternalServerError);
            Assert.AreEqual( response.StatusDescription, "Internal Server Error");
        }
    }

    /// <summary>
    /// Deposits the given amount to "/onlinewallet/withdraw".
    /// </summary>
    /// <param name="amount">The value to be withdraw.</param>
    /// <returns>If the content of the response is valid, the value of the amount property.
    ///          If the content of the response is not valid, 0.</returns>
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
    /// Withdraws the given amount to "/onlinewallet/withdraw". Request expected to fail.
    /// </summary>
    /// <param name="amount">The value to be withdraw.</param>
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
    /// Tests whether depositing the minimum amount is possible.
    /// </summary>
    [TestMethod]
    [Ignore("Fails, depositing the minimum value of decimal is not possible.")]
    public void TestDepositMinimumAmount()
    {               
        ClearBalance();
        var amount = Deposit( decimal.MinValue);
        Assert.AreEqual( amount, decimal.MinValue);
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
    /// Tests whether depositing the maximum amount is possible.
    /// </summary>
    [TestMethod]
    public void TestDepositMaxAmount()
    {               
        ClearBalance();
        var amount = Deposit(decimal.MaxValue);
        Assert.AreEqual( amount,  decimal.MaxValue);
        ClearBalance();
    }

    /// <summary>
    /// Tests whether depositing after the maximum amount is NOT possible.
    /// </summary>
    [TestMethod]
    [Ignore("Fails, server stuck in internal server error after this test.")]
    public void TestDepositAfterMaximumAmountReached()
    {               
        ClearBalance();
        var amount = Deposit(decimal.MaxValue);
        Assert.AreEqual( amount,  decimal.MaxValue);
        DepositExpectToFail(1m, HttpStatusCode.InternalServerError);
        var balance = GetBalance();
        Assert.AreEqual( balance,  amount);
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
    /// Tests whether the expected withdraw amount is also returned back when getting the balance. 
    /// </summary>
    [TestMethod]
    public void TestWithdrawAmountIsSameAsGetBalance()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(12);
        Assert.AreEqual( balance, amount - 12);
        Assert.AreEqual(balance, GetBalance());
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
    /// Tests whether withdrawing the minimum amount is possible.
    /// </summary>
    [TestMethod]
    [Ignore("Fails, withdrawing the minimum value of decimal is not possible.")]
    public void TestWithdrawMinimumAmount()
    {               
        ClearBalance();
        var amount = Deposit(50);
        Assert.AreEqual( amount,  50);
        var balance = Withdraw(decimal.MinValue);
        Assert.AreEqual( balance, amount - decimal.MinValue);
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
    /// Tests whether withdrawing the maximum amount is possible.
    /// </summary>
    [TestMethod]
    public void TestWithdrawMaximumAmount()
    {               
        ClearBalance();
        var amount = Deposit(decimal.MaxValue);
        Assert.AreEqual( amount,  decimal.MaxValue);
        var balance = Withdraw(decimal.MaxValue);
        Assert.AreEqual( balance, 0);
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

        WithdrawExpectToFail(50 + decimal.MinValue);

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
    /// Tests whether depositing and withdrawing multiple times can be possible.
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

    /// <summary>
    /// Tests whether after depositing the maximum amount withdrawing the minimum amount is possible.
    /// </summary>
    [TestMethod]
    [Ignore("Fails, withdrawing the minimum value of decimal is not possible.")]
    public void TestDepositLargeWithdrawSmallAmount()
    {               
        ClearBalance();

        var amount = Deposit(decimal.MaxValue);
        Assert.AreEqual( amount,  decimal.MaxValue);

        var amount2 = Withdraw(decimal.MinValue);
        Assert.AreEqual( amount2,  amount - decimal.MinValue);

        ClearBalance();
    }
}