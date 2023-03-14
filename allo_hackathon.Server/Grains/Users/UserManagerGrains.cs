using Orleans.Runtime;
using Server.Models;
using System.Collections.Immutable;

namespace Server.Grains;

public class UserManagerGrain : Grain, IUserManagerGrain
{
    private readonly IPersistentState<UsersState> _state;

    private string GrainKey = "Users";

    public UserManagerGrain(
        [PersistentState("UsersState")] IPersistentState<UsersState> state) => _state = state;

    public async Task RegisterAsync(Guid userId, string name, string token)
    {
        _state.State.Users.Add(userId);
        _state.State.Name2Id.Add(name, userId);
        _state.State.Token.Add(token);
        await _state.WriteStateAsync();
    }

    public async Task UnregisterAsync(Guid userId, string name)
    {
        _state.State.Users.Remove(userId);
        _state.State.Name2Id.Remove(name);
        await _state.WriteStateAsync();
    }

    public Task<List<Guid>> GetAllAsync() =>
        Task.FromResult(new List<Guid>(_state.State.Name2Id.Values));

    public Task<List<Guid>> GetPageAsync(int page)
    {
        List<Guid> data = new List<Guid>(_state.State.Name2Id.Values);
        List<Guid> pagedData = data.Skip(page * 20).Take(20).ToList();
        return Task.FromResult(pagedData);
    }

    public async Task<string> GetUserNameAsync(Guid guid)
    {
        var temp = await GrainFactory.GetGrain<IUserGrains>(guid).GetAsync(guid);
        return temp.UserName;
    }

    public Task<Guid> GetUserIdAsync(string name)
    {
        if (!_state.State.Name2Id.ContainsKey(name))
        {
            throw new InvalidOperationException();
        }
        return Task.FromResult(_state.State.Name2Id[name]);
    }

    public Task<List<Guid>> SerchUserAsync(string serch)
    {
        List<string> data = new List<string>(_state.State.Name2Id.Keys);
        List<Guid> res = new List<Guid>();
        foreach(var name in data)
        {
            if (name.Contains(serch))
            {
                res.Add(_state.State.Name2Id[name]);
            }
        }
        return Task.FromResult(res);
    }

    public Task<bool> CheckReqAsync(Guid guid, string name, string token)
    {
        if (_state.State.Name2Id.ContainsKey(name))
        {
            return Task.FromResult(true);
        }
        if(_state.State.Token.IndexOf(token) >= 0)
        {
            return Task.FromResult(true);
        }
        if (_state.State.Users.Contains(guid))
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    [GenerateSerializer]
    public class UsersState
    {
        [Id(0)]
        public List<string> Token { get; set; } = new();
        public HashSet<Guid> Users { get; set; } = new();
        public SortedDictionary<string, Guid> Name2Id { get; set; } = new();
    }
}

