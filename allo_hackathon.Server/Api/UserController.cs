using Microsoft.AspNetCore.Mvc;
using Server.Grains;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using Orleans;
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

    [HttpGet("all")]
    public async Task<List<ResponseUsers>> GetAllAsync()
    {
        List<ResponseUsers> userList = new List<ResponseUsers>();
        var users = await _factory.GetGrain<IUserManagerGrain>("Users").GetAllAsync();
        foreach(Guid id in users)
        {
            var temp = await _factory.GetGrain<IUserGrains>(id).GetAsync(id);
            userList.Add(new ResponseUsers(temp.CreatedAt, temp.UserName, temp.TokenName, temp.IsReceived, temp.TokenList, temp.Follows, temp.Followers, temp.MyToken, temp.DeletedAt));
        }
        return userList;
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

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UsersModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new Users(model.Key, DateTime.Now, model.UserName, model.TokenName, false, new Dictionary<string, double>(), new List<string>(), new List<string>(), 0.0, null);
        await _factory.GetGrain<IUserGrains>(model.Key).SetAsync(user);
        return Ok(user);
    }
    public record class UsersModel(
    [Required] Guid Key,
    [Required] string UserName,
    [Required] string TokenName);

    public record class FollowModel(
    [Required] string myName,
    [Required] string otherName);
}
