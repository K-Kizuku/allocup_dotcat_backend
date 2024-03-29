﻿using System.Collections.Immutable;

namespace Server.Grains;

public interface IUserManagerGrain : IGrainWithStringKey
{
    Task RegisterAsync(Guid userId, string name, string token);
    Task UnregisterAsync(Guid userId, string name);

    Task<List<Guid>> GetAllAsync();
    Task<string> GetUserNameAsync(Guid guid);
    Task<Guid> GetUserIdAsync(string name);
    Task<List<Guid>> GetPageAsync(int page);
    Task<List<Guid>> SerchUserAsync(string serch);
    Task<bool> CheckReqAsync(Guid guid, string name, string token);
    Task GiveTokenAsync();
}
