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

    public async Task RegisterAsync(Guid userId, string name)
    {
        _state.State.Users.Add(userId);
        _state.State.Name2Id.Add(name, userId);
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

    [GenerateSerializer]
    public class UsersState
    {
        [Id(0)]
        public HashSet<Guid> Users { get; set; } = new();
        public SortedDictionary<string, Guid> Name2Id { get; set; } = new();
    }
}

