using System;
using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;

namespace Betsson.OnlineWallets.UnitTests;

    public class MockOnlineWalletRepository : IOnlineWalletRepository
    {
        public OnlineWalletEntry? mockOnlineWalletEntry;

        public MockOnlineWalletRepository() {
            this.mockOnlineWalletEntry = new OnlineWalletEntry();
        }

        public MockOnlineWalletRepository(OnlineWalletEntry? mockOnlineWalletEntry)
        {
            this.mockOnlineWalletEntry = mockOnlineWalletEntry;
        }
        
        public async Task<OnlineWalletEntry?> GetLastOnlineWalletEntryAsync()
        {
            return await Task.FromResult(mockOnlineWalletEntry);
        }

        public Task InsertOnlineWalletEntryAsync(OnlineWalletEntry mockOnlineWalletEntry)
        {
            this.mockOnlineWalletEntry.BalanceBefore = this.mockOnlineWalletEntry.Amount;
            this.mockOnlineWalletEntry.Amount = this.mockOnlineWalletEntry.Amount + mockOnlineWalletEntry.Amount;
            return Task.CompletedTask;
        }
    }
