using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Data.Models;

namespace Betsson.OnlineWallets.UnitTests;

[TestClass]
public class UnitTest1
{

    [TestMethod]
    public void TestGetBalanceFirstTime()
    {
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository();
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var task = onlineWalletService.GetBalanceAsync();
        task.Wait();
        Assert.AreEqual(0, task.Result.Amount);
    }

    [TestMethod]
    public void TestGetBalanceNullOnlineWalletEntry()
    {
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(null);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var task = onlineWalletService.GetBalanceAsync();
        task.Wait();
        Assert.AreEqual(0, task.Result.Amount);
    }

    [TestMethod]
    public void TestGetBalanceOnlineWalletEntryFilled()
    {
        var mockOnlineWalletEntry = new OnlineWalletEntry()
        {
            BalanceBefore = 0.36M,
            Amount = 54546564654684654.36M,
        };

        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(mockOnlineWalletEntry);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var task = onlineWalletService.GetBalanceAsync();
        task.Wait();
        Assert.AreEqual(mockOnlineWalletEntry.BalanceBefore + mockOnlineWalletEntry.Amount, task.Result.Amount);
    }

    [TestMethod]
    public void TestGetBalanceOnlineWalletEntryFilledWithNegaitve()
    {
        var mockOnlineWalletEntry = new OnlineWalletEntry()
        {
            BalanceBefore = -0.36M,
            Amount = -54546564654684654.36M,
        };
        
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(mockOnlineWalletEntry);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var task = onlineWalletService.GetBalanceAsync();
        task.Wait();
        Assert.AreEqual(mockOnlineWalletEntry.BalanceBefore + mockOnlineWalletEntry.Amount, task.Result.Amount);
    }

    [TestMethod]
    public void TestDepositFundsFirstDeposit()
    {
        var mockOnlineWalletEntry = new OnlineWalletEntry();        
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(mockOnlineWalletEntry);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var deposit = new Models.Deposit();
        deposit.Amount = 500M;
        var task = onlineWalletService.DepositFundsAsync(deposit);
        task.Wait();
        Assert.AreEqual(mockOnlineWalletEntry.BalanceBefore + deposit.Amount, task.Result.Amount);
    }

    [TestMethod]
    public void TestDepositFundsNotFirstDeposit()
    {
        var mockOnlineWalletEntry = new OnlineWalletEntry()
        {
            Amount = 54.36M,
        };
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(mockOnlineWalletEntry);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var deposit = new Models.Deposit();
        deposit.Amount = 500M;
        var task = onlineWalletService.DepositFundsAsync(deposit);
        task.Wait();
        Assert.AreEqual(mockOnlineWalletEntry.BalanceBefore + deposit.Amount, task.Result.Amount);
    }

    [TestMethod]
    public void TestDepositFundsNDepositZero()
    {
        var mockOnlineWalletEntry = new OnlineWalletEntry()
        {
            Amount = 54.36M,
        };
        IOnlineWalletRepository mockOnlineWalletRepository = new MockOnlineWalletRepository(mockOnlineWalletEntry);
        OnlineWalletService onlineWalletService = new OnlineWalletService(mockOnlineWalletRepository);
        var deposit = new Models.Deposit();
        deposit.Amount = 0M;
        var task = onlineWalletService.DepositFundsAsync(deposit);
        task.Wait();
        Assert.AreEqual(mockOnlineWalletEntry.BalanceBefore + deposit.Amount, task.Result.Amount);
    }
}