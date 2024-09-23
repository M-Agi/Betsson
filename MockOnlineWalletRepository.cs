using System;
using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;

namespace Betsson.OnlineWallets.UnitTests;

    public class MockOnlineWalletRepository : IOnlineWalletRepository
    {
        public OnlineWalletEntry mockOnlineWalletEntry = new OnlineWalletEntry();
        
        public async Task<OnlineWalletEntry?> GetLastOnlineWalletEntryAsync()
        {
            return await Task.FromResult(mockOnlineWalletEntry);
            //return await new Task<OnlineWalletEntry?>(() => { return this.mockOnlineWalletEntry; });
        }

        public Task InsertOnlineWalletEntryAsync(OnlineWalletEntry mockOnlineWalletEntry)
        {
            return Task.CompletedTask;
        }
    }
