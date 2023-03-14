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

    public async Task<Users> GetAsync(Guid guid)
    {
        await _state.ReadStateAsync();
        return await Task.FromResult(_state.State.User);
    }

    public async Task SetAsync(Users users)
    {
        // ensure the key is consistent
        if (users.Key != GrainKey)
        {
            throw new InvalidOperationException();
        }

        // save the item
        _state.State.User = users;
        await _state.WriteStateAsync();

        // register the item with its owner list
        await GrainFactory.GetGrain<IUserManagerGrain>("Users")
            .RegisterAsync(users.Key, users.UserName, users.TokenName);

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

    public Task<List<string>> GetFollowsAsync() => Task.FromResult(_state.State.User.Follows);

    public Task<List<string>> GetFollowersAsync() => Task.FromResult(_state.State.User.Followers);


    public async Task<List<string>> AddFollowAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.User.Follows.Add(followName);
        var followUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(followName);
        await GrainFactory.GetGrain<IUserGrains>(followUuid).AddFollowerAsync(followName, myName);
        //await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        await _state.WriteStateAsync();
        return await this.GetFollowsAsync();
    }

    public async Task AddFollowerAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.User.Followers.Add(followName);
        //return await this.GetFollowersAsync();
    }

    public async Task<List<string>> RemoveFollowAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.User.Follows.Remove(followName);
        var followUuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(followName);
        await GrainFactory.GetGrain<IUserGrains>(followUuid).RemoveFollowerAsync(followName, myName);
        //await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        await _state.WriteStateAsync();
        return await this.GetFollowsAsync();
    }

    public async Task RemoveFollowerAsync(string myName, string followName)
    {
        var uuid = await GrainFactory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(myName);
        if (uuid != GrainKey)
        {
            throw new InvalidOperationException();
        }
        _state.State.User.Followers.Remove(followName);
        //return await this.GetFollowersAsync();
    }

    public async Task AddTokenAsync(string tokenName, double cost)
    {
        if (!_state.State.User.TokenList.ContainsKey(tokenName))
        {
            _state.State.User.TokenList.Add(tokenName, cost);
        }
        else
        {
            _state.State.User.TokenList[tokenName] += cost;
        }
        await _state.WriteStateAsync();
    }

    public async Task RemoveTokenAsync(string tokenName, double cost)
    {
        if (!_state.State.User.TokenList.ContainsKey(tokenName))
        {
            return;
        }
        else
        {
            _state.State.User.TokenList[tokenName] -= cost;
        }
        await _state.WriteStateAsync();
    }

    [GenerateSerializer]
    public class UserState
    {
        [Id(0)]
        public Users User { get; set; }
    }
}

