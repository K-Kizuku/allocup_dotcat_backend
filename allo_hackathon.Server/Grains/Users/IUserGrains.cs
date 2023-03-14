using System.Collections.Immutable;
using Server.Models;

namespace Server.Grains;

public interface IUserGrains : IGrainWithGuidKey
{
    Task SetAsync(Users users);

    Task<Users> GetAsync(Guid guid);

    Task<List<string>> AddFollowAsync(string myName, string followName);
    Task AddFollowerAsync(string myName, string followName);
    Task<List<string>> GetFollowsAsync();
    Task<List<string>> GetFollowersAsync();
    Task<List<string>> RemoveFollowAsync(string myName, string followName);
    Task RemoveFollowerAsync(string myName, string followName);
    Task RemoveTokenAsync(string tokenName, double cost);
    Task AddTokenAsync(string tokenName, double cost);
}

