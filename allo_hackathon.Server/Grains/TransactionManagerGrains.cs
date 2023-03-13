using Orleans.Runtime;
using System.Collections.Immutable;

namespace Server.Grains;

public class TransactionManagerGrain : Grain, ITransactionManagerGrain
{
    private readonly IPersistentState<TransactionsState> _state;

    private string GrainKey = "Transactions";

    public TransactionManagerGrain(
        [PersistentState("TransactionsState")] IPersistentState<TransactionsState> state) => _state = state;

    public async Task RegisterAsync(Guid transactionKey)
    {
        _state.State.Transactions.Add(transactionKey);
        await _state.WriteStateAsync();
    }

    public async Task UnregisterAsync(Guid transactionKey)
    {
        _state.State.Transactions.Remove(transactionKey);
        await _state.WriteStateAsync();
    }

    public Task<List<Guid>> GetAllAsync() =>
        Task.FromResult(new List<Guid>(_state.State.Transactions));

    [GenerateSerializer]
    public class TransactionsState
    {
        [Id(0)]
        public HashSet<Guid> Transactions { get; set; } = new();
    }
}

