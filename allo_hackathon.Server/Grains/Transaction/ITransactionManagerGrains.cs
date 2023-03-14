using System.Collections.Immutable;
using Server.Models;
namespace Server.Grains;

public interface ITransactionManagerGrain : IGrainWithStringKey
{
    Task RegisterAsync(Transaction transaction);
    Task UnregisterAsync(Guid transactionKey);

    Task<List<Guid>> GetAllAsync();
    Task<List<MyTransactionList>> GetMyTransactionListsAsync(string name);
}

