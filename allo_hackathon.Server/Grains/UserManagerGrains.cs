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

    public async Task RegisterAsync(Guid userId)
    {
        _state.State.Users.Add(userId);
        await _state.WriteStateAsync();
    }

    public async Task UnregisterAsync(Guid userId)
    {
        _state.State.Users.Remove(userId);
        await _state.WriteStateAsync();
    }

    public Task<List<Guid>> GetAllAsync() =>
        Task.FromResult(new List<Guid>(_state.State.Users));

    public async Task<string> GetUserNameAsync(Guid guid)
    {
        var temp = await GrainFactory.GetGrain<IUserGrains>(guid).GetAsync(guid);
        return temp.Name;
    }

    [GenerateSerializer]
    public class UsersState
    {
        [Id(0)]
        public HashSet<Guid> Users { get; set; } = new();
    }
}

