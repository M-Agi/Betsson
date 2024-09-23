using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Repositories;


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
}