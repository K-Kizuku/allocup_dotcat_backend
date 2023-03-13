using Microsoft.AspNetCore.Mvc;
using Server.Grains;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using Orleans;
//using Server.Grains;

namespace Server.Silo.Api;

[ApiController]
[Route("api/user")]
public class TodoController : ControllerBase
{
    private readonly IGrainFactory _factory;

    public TodoController(IGrainFactory factory) => _factory = factory;

    [HttpGet("{uuid}")]
    public Task<Users> GetAsync([Required] Guid uuid) => _factory.GetGrain<IUserGrains>("Users").GetAsync(uuid);

    [HttpGet("all")]
    public Task<List<Users>> GetAllAsync() => _factory.GetGrain<IUserGrains>("Users").GetAllAsync();

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] UsersModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new Users(model.Key, DateTime.Now, model.Name, false,new TokenItem[0], new TokenItem[0], new TokenItem[0], 0.0,null);
        await _factory.GetGrain<IUserGrains>("Users").SetAsync(user);
        return Ok();
    }
    public record class UsersModel(
    [Required] Guid Key,
    [Required] string Name);
}
