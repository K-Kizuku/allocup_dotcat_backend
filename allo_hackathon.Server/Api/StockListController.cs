using Microsoft.AspNetCore.Mvc;
using Server.Grains;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Immutable;
using Orleans;
//using Server.Grains;

namespace Server.Silo.Api;

[ApiController]
[Route("api/stock_list")]
public class StockListController : ControllerBase
{
    private readonly IGrainFactory _factory;

    public StockListController(IGrainFactory factory) => _factory = factory;

    [HttpGet("{name}")]
    public async Task<List<StockList>> GetAsync([Required] string name)
    {
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(name);
        return await _factory.GetGrain<IStockListGrains>(uuid).GetAsync();
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] StockListModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var stockList = new StockList(model.Title, model.Cost, DateTime.Now);
        await _factory.GetGrain<IStockListGrains>(model.OwnerKey).SetAsync(stockList, model.OwnerKey);
        return Ok();
    }

    //[HttpPost("edit")]
    //public Task<>

    public record class StockListModel(
    [Required] Guid OwnerKey,
    [Required] string Title,
    [Required] double Cost,
    [Required] DateTime? CreatedAt,
    [Required] DateTime? DeletedAt = null);
}

