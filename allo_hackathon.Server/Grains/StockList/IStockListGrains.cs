using Azure.Storage.Blobs.Models;
using Server.Models;

namespace Server.Grains;

public interface IStockListGrains : IGrainWithGuidKey
{
    Task SetAsync(StockList stockList);

    Task<List<StockList>> GetAsync();

    //Task<ImmutableArray<Transaction>> GetAllAsync();
}
