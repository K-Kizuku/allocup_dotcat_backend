using System.Collections.Immutable;

namespace Server.Grains;

public interface ITransactionManagerGrain : IGrainWithStringKey
{
    Task RegisterAsync(Guid transactionKey);
    Task UnregisterAsync(Guid transactionKey);

    Task<List<Guid>> GetAllAsync();
}

