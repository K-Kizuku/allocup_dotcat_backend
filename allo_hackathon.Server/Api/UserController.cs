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
    public async Task<List<Users>> GetAllAsync() {
        List<Users> userList = new List<Users>();
        var users = await _factory.GetGrain<IUserManagerGrain>("Users").GetAllAsync();
        foreach(Guid id in users)
        {
            var temp = await _factory.GetGrain<IUserGrains>(id).GetAsync(id);
            userList.Add(temp);
        }
        return userList;
    } 

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UsersModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new Users(model.Key, DateTime.Now, model.Name, false,new TokenItem[0], new TokenItem[0], new TokenItem[0], 0.0,null);
        await _factory.GetGrain<IUserGrains>(model.Key).SetAsync(user);
        return Ok();
    }
    public record class UsersModel(
    [Required] Guid Key,
    [Required] string Name);
}
