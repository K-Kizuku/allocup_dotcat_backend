using System.Collections.Immutable;
using Server.Models;

namespace Server.Grains;

public interface ITransactionGrains : IGrainWithGuidKey
{
    Task SetAsync(Transaction transaction);

    Task<Transaction> GetAsync(Guid guid);

    //Task<ImmutableArray<Transaction>> GetAllAsync();
}

