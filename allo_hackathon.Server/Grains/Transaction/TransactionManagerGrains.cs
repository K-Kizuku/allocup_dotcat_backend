using Orleans.Runtime;
using System.Collections.Immutable;
using Server.Grains;
using Server.Models;

namespace Server.Grains;

public class TransactionManagerGrain : Grain, ITransactionManagerGrain
{
    private readonly IPersistentState<TransactionsState> _state;

    private string GrainKey = "Transactions";

    public TransactionManagerGrain(
        [PersistentState("TransactionsState", "strage")] IPersistentState<TransactionsState> state) => _state = state;

    public async Task RegisterAsync(Transaction transaction)
    {
        _state.State.Transactions.Add(transaction.Key);
        if (!_state.State.MyTransactionList.ContainsKey(transaction.SendFrom))
        {
            _state.State.MyTransactionList.Add(transaction.SendFrom, new List<MyTransactionList>() { new MyTransactionList(transaction.Key, transaction.TokenName, transaction.CreatedAt, false, transaction.SendTo, transaction.Cost) });
        }
        else
        {
            _state.State.MyTransactionList[transaction.SendFrom].Add(new MyTransactionList(transaction.Key, transaction.TokenName, transaction.CreatedAt, false, transaction.SendTo, transaction.Cost));
        }
        if (!_state.State.MyTransactionList.ContainsKey(transaction.SendTo))
        {
            _state.State.MyTransactionList.Add(transaction.SendTo, new List<MyTransactionList>() { new MyTransactionList(transaction.Key, transaction.TokenName, transaction.CreatedAt, true, transaction.SendFrom, transaction.Cost) });
        }
        else
        {
            _state.State.MyTransactionList[transaction.SendTo].Add(new MyTransactionList(transaction.Key, transaction.TokenName, transaction.CreatedAt, true, transaction.SendFrom, transaction.Cost));
        }
        await _state.WriteStateAsync();
    }

    public async Task UnregisterAsync(Guid transactionKey)
    {
        _state.State.Transactions.Remove(transactionKey);
        await _state.WriteStateAsync();
    }

    public Task<List<Guid>> GetAllAsync() =>
        Task.FromResult(new List<Guid>(_state.State.Transactions));

    public Task<List<MyTransactionList>> GetMyTransactionListsAsync(string name) => Task.FromResult(_state.State.MyTransactionList[name]);


    [GenerateSerializer]
    public class TransactionsState
    {
        [Id(0)]
        public HashSet<Guid> Transactions { get; set; } = new();
        public Dictionary<string, List<MyTransactionList>> MyTransactionList { get; set; } = new();
    }
}

