using Orleans.Runtime;
using Server.Models;
using Server.Grains;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Server.Grains;

public class TransactionGrain : Grain, ITransactionGrains
{
    private readonly ILogger<Grain> _logger;
    private readonly IPersistentState<TransactionState> _state;

    private string GrainType => nameof(UserGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public TransactionGrain(
        ILogger<UserGrain> logger,
        [PersistentState("TransactionState", "strage")] IPersistentState<TransactionState> state)
    {
        _logger = logger;
        _state = state;
    }

    public Task<Transaction> GetAsync(Guid guid)
    {
        return Task.FromResult(_state.State.Transaction);
    }



    public async Task SetAsync(Transaction transaction)
    {
        // ensure the key is consistent
        if (transaction.Key != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // save the item
        _state.State.Transaction = transaction;
        var fromUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(transaction.SendFrom);
        var toUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(transaction.SendTo);
        await GrainFactory.GetGrain<IUserGrains>(fromUuid).RemoveTokenAsync(transaction.TokenName, transaction.Cost);
        await GrainFactory.GetGrain<IUserGrains>(toUuid).AddTokenAsync(transaction.TokenName, transaction.Cost);


        await _state.WriteStateAsync();



        // register the item with its owner list
        await GrainFactory.GetGrain<ITransactionManagerGrain>("Transaction")
            .RegisterAsync(transaction);



        //// for sample debugging
        //_logger.LogInformation(
        //    "{@GrainType} {@GrainKey} now contains {@Todo}",
        //    GrainType, GrainKey, item);

        // notify listeners - best effort only
        //this.GetStreamProvider("MemoryStreams")
        //    .GetStream<TodoNotification>(StreamId.Create(nameof(ITodoGrain), item.OwnerKey))
        //    .OnNextAsync(new TodoNotification(item.Key, item))
        //    .Ignore();
    }

    [GenerateSerializer]
    public class TransactionState
    {
        [Id(0)]
        public Transaction Transaction { get; set; }
    }
}



