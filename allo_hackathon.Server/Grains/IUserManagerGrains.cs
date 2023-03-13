using System.Collections.Immutable;

namespace Server.Grains;

public interface IUserManagerGrain : IGrainWithStringKey
{
    Task RegisterAsync(Guid userId);
    Task UnregisterAsync(Guid userId);

    Task<List<Guid>> GetAllAsync();
}
