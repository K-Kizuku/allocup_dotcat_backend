using Orleans.Runtime;
using Server.Models;
using Server.Grains;
using System.Collections.Immutable;
using static Server.Grains.UserGrain;

namespace Server.Grains;

public class UserGrain : Grain, IUserGrains
{
    private readonly ILogger<Grain> _logger;
    private readonly IPersistentState<UserState> _state;

    private string GrainType => nameof(UserGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public UserGrain(
        ILogger<UserGrain> logger,
        [PersistentState("UserState","strage")] IPersistentState<UserState> state)
    {
        _logger = logger;
        _state = state;
    }

    public Task<Users> GetAsync(Guid guid)
    {
        return Task.FromResult(_state.State.Users[guid]);
    }

    //public Task<List<Users>> GetAllAsync()
    //{
    //    var UserList = new List<Users>(_state.State.Users.Values);
    //    return Task.FromResult(UserList);

    //}

    public async Task SetAsync(Users users)
    {
        // ensure the key is consistent
        if (users.Key != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // save the item
        _state.State.Users.Add(users.Key, users);
        await _state.WriteStateAsync();

        // register the item with its owner list
        await GrainFactory.GetGrain<IUserManagerGrain>("Users")
            .RegisterAsync(users.Key, users.UserName);

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

    public Task<List<string>> GetFollowsAsync() => Task.FromResult(_state.State.Users[GrainKey].Follows);

    public Task<List<string>> GetFollowersAsync() => Task.FromResult(_state.State.Users[GrainKey].Followers);


    public async Task<List<string>> AddFollowAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.Users[uuid].Follows.Add(followName);
        var followUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(followName);
        await GrainFactory.GetGrain<IUserGrains>(followUuid).AddFollowerAsync(followName, myName);
        //await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        return await this.GetFollowsAsync();
    }

    public async Task AddFollowerAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.Users[uuid].Followers.Add(followName);
        //return await this.GetFollowersAsync();
    }

    public async Task<List<string>> RemoveFollowAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.Users[uuid].Follows.Remove(followName);
        var followUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(followName);
        await GrainFactory.GetGrain<IUserGrains>(followUuid).RemoveFollowerAsync(followName, myName);
        //await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        return await this.GetFollowsAsync();
    }

    public async Task RemoveFollowerAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.Users[uuid].Followers.Remove(followName);
        //return await this.GetFollowersAsync();
    }

    [GenerateSerializer]
    public class UserState
    {
        [Id(0)]
        //public Users User { get; set; }
        //public HashSet<Users> Users { get; set; } = new();
        public Dictionary<Guid, Users> Users = new Dictionary<Guid, Users>();
    }
}

