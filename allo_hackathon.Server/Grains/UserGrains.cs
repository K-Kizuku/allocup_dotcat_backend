using Orleans.Runtime;
using Server.Models;
using Server.Grains;
using System.Collections.Immutable;
using static Server.Grains.UserGrain;

namespace Server.Grains;

public class UserGrain : Grain, IUserGrains
{
    private readonly ILogger<Grain> _logger;
    private readonly IPersistentState<State> _state;

    private string GrainType => nameof(UserGrain);
    private string GrainKey = "Users";

    public UserGrain(
        ILogger<UserGrain> logger,
        [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }

    public Task<Users> GetAsync(Guid guid)
    {
        return Task.FromResult(_state.State.Users[guid]);
    }

    public Task<List<Users>> GetAllAsync()
    {
        var UserList = new List<Users>(_state.State.Users.Values);
        return Task.FromResult(UserList);

    }

    public async Task SetAsync(Users users)
    {
        // ensure the key is consistent
        if ("Users" != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // save the item
        _state.State.Users.Add(users.Key, users);
        await _state.WriteStateAsync();

        //// register the item with its owner list
        //await GrainFactory.GetGrain<ITodoManagerGrain>(item.OwnerKey)
        //    .RegisterAsync(item.Key);

        //// for sample debugging
        //_logger.LogInformation(
        //    "{@GrainType} {@GrainKey} now contains {@Todo}",
        //    GrainType, GrainKey, item);

        // notify listeners - best effort only
        //this.GetStreamProvider("MemoryStreams")
        //    .GetStream<TodoNotification>(StreamId.Create(nameof(ITodoGrain), item.OwnerKey))
        //    .OnNextAsync(new TodoNotification(item.Key, item))
        //    .Ignore();
    }

    //public async Task ClearAsync()
    //{
    //    // fast path for already cleared state
    //    if (_state.State.Item is null) return;

    //    // hold on to the keys
    //    var itemKey = _state.State.Item.Key;
    //    var ownerKey = _state.State.Item.OwnerKey;

    //    // unregister from the registry
    //    await GrainFactory.GetGrain<ITodoManagerGrain>(ownerKey)
    //        .UnregisterAsync(itemKey);

    //    // clear the state
    //    await _state.ClearStateAsync();

    //    // for sample debugging
    //    _logger.LogInformation(
    //        "{@GrainType} {@GrainKey} is now cleared",
    //        GrainType, GrainKey);

    //    // notify listeners - best effort only
    //    this.GetStreamProvider("MemoryStreams")
    //        .GetStream<TodoNotification>(StreamId.Create(nameof(ITodoGrain), itemKey))
    //        .OnNextAsync(new TodoNotification(itemKey, null))
    //        .Ignore();

    //    // no need to stay alive anymore
    //    DeactivateOnIdle();
    //}

    [GenerateSerializer]
    public class State
    {
        [Id(0)]
        public Users? User { get; set; }
        //public HashSet<Users> Users { get; set; } = new();
        public Dictionary<Guid, Users> Users = new Dictionary<Guid, Users>();
    }
}

