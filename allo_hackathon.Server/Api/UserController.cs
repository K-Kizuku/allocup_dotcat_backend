using Microsoft.AspNetCore.Mvc;
using Server.Grains;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using Orleans;
using System.Text.Json;
using System.Reflection;
//using Server.Grains;

namespace Server.Silo.Api;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IGrainFactory _factory;

    public UserController(IGrainFactory factory) => _factory = factory;

    [HttpGet("{uuid}")]
    public Task<Users> GetAsync([Required] Guid uuid) => _factory.GetGrain<IUserGrains>(uuid).GetAsync(uuid);

    [HttpGet("serch/{serch}")]
    public async Task<ActionResult> GetSerchUserAsync([Required] string serch)
    {
        List<Guid> users = await _factory.GetGrain<IUserManagerGrain>("Users").SerchUserAsync(serch);
        List<ResponseUsers> userList = new List<ResponseUsers>();
        foreach (Guid id in users)
        {
            var temp = await _factory.GetGrain<IUserGrains>(id).GetAsync(id);
            userList.Add(new ResponseUsers(temp.CreatedAt, temp.UserName, temp.TokenName, temp.IsReceived, temp.TokenList, temp.Follows, temp.Followers, temp.MyToken, temp.DeletedAt));
        }
        return Ok(userList);
    }

    [HttpGet("all")]
    public async Task<ActionResult> GetAllAsync()
    {
        List<ResponseUsers> userList = new List<ResponseUsers>();
        var users = await _factory.GetGrain<IUserManagerGrain>("Users").GetAllAsync();
        foreach(Guid id in users)
        {
            var temp = await _factory.GetGrain<IUserGrains>(id).GetAsync(id);
            userList.Add(new ResponseUsers(temp.CreatedAt, temp.UserName, temp.TokenName, temp.IsReceived, temp.TokenList, temp.Follows, temp.Followers, temp.MyToken, temp.DeletedAt));
        }

        return Ok(userList);
    }

    [HttpGet("page/{page}")]
    public async Task<ActionResult> GetPagedUsersAsync([Required] string page)
    {
        List<Guid> users = await _factory.GetGrain<IUserManagerGrain>("Users").GetPageAsync(int.Parse(page));
        List<ResponseUsers> userList = new List<ResponseUsers>();
        foreach (Guid id in users)
        {
            var temp = await _factory.GetGrain<IUserGrains>(id).GetAsync(id);
            userList.Add(new ResponseUsers(temp.CreatedAt, temp.UserName, temp.TokenName, temp.IsReceived, temp.TokenList, temp.Follows, temp.Followers, temp.MyToken, temp.DeletedAt));
        }
        return Ok(userList);
    }

    [HttpPost("follow")]
    public async Task<ActionResult> PostFollowsAsync([FromBody] FollowModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(model.myName);
        var res = await _factory.GetGrain<IUserGrains>(uuid).AddFollowAsync(model.myName,model.otherName);
        return Ok(res);
    }

    [HttpDelete("follow")]
    public async Task<ActionResult> DeleteFollowsAsync([FromBody] FollowModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(model.myName);
        var res = await _factory.GetGrain<IUserGrains>(uuid).RemoveFollowAsync(model.myName, model.otherName);
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UsersModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(await _factory.GetGrain<IUserManagerGrain>("Users").CheckReqAsync(model.Key, model.UserName, model.TokenName)){
            return BadRequest(model);
        }

        var user = new Users(model.Key, DateTime.Now, model.UserName, model.TokenName, false, new Dictionary<string, double>() { { model.TokenName, 10} }, new List<string>(), new List<string>(), 10.0, null);
        await _factory.GetGrain<IUserGrains>(model.Key).SetAsync(user);
        return Ok(user);
    }

    [HttpPost("add_token")]
    public async Task<ActionResult> PostAddTokenAsync([FromBody] TokenAddModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(model.myName);
        await _factory.GetGrain<IUserGrains>(uuid).AddTokenAsync(model.myName, model.cost);
        return Ok();
    }

    [HttpGet("debug/{name}")]
    public async Task<ActionResult> DebugAsync([Required] string name)
    {
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(name);
        return Ok(uuid);
    }

    public record class UsersModel(
    [Required] Guid Key,
    [Required] string UserName,
    [Required] string TokenName);

    public record class FollowModel(
    [Required] string myName,
    [Required] string otherName);

    public record class TokenAddModel(
    [Required] string myName,
    [Required] string tokenName,
    [Required] double cost);
}
