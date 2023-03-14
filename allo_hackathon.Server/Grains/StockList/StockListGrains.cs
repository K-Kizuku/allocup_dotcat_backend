using Orleans.Runtime;
using Server.Models;
using Server.Grains;
using System.Collections.Immutable;
using Azure.Storage.Blobs.Models;

namespace Server.Grains;

public class StockListGrain : Grain, IStockListGrains
{
    private readonly ILogger<Grain> _logger;
    private readonly IPersistentState<StockListState> _state;

    private string GrainType => nameof(UserGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public StockListGrain(
        ILogger<StockListGrain> logger,
        [PersistentState("StockListState", "strage")] IPersistentState<StockListState> state)
    {
        _logger = logger;
        _state = state;
    }

    public Task<List<StockList>> GetAsync()
    {
        return Task.FromResult(_state.State.stockList);
    }



    public async Task SetAsync(StockList stockList, Guid guid)
    {
        // ensure the key is consistent
        if (guid != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // save the item
        _state.State.stockList.Add(stockList);
        await _state.WriteStateAsync();

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
    public class StockListState
    {
        [Id(0)]
        public List<StockList> stockList { get; set; }
    }
}



