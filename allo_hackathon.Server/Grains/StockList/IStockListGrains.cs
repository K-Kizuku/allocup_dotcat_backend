using Azure.Storage.Blobs.Models;
using Server.Models;

namespace Server.Grains;

public interface IStockListGrains : IGrainWithGuidKey
{
    Task SetAsync(StockList stockList, Guid guid);

    Task<List<StockList>> GetAsync();

    //Task<ImmutableArray<Transaction>> GetAllAsync();
}
