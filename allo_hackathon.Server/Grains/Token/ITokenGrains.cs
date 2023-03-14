using System.Collections.Immutable;
using Server.Models;

namespace Server.Grains;

public interface ITokenGrains : IGrainWithStringKey
{
    Task SetAsync(Users users);

    Task<Users> GetAsync(Guid guid);
}
