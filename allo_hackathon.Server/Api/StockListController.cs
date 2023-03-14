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
        Guid guid = Guid.NewGuid();
        var stockList = new StockList(guid, model.Title, model.Cost, DateTime.Now);
        await _factory.GetGrain<IStockListGrains>(model.OwnerKey).SetAsync(stockList, model.OwnerKey);
        return Ok(stockList);
    }

    [HttpPost("edit")]
    public async Task<ActionResult> EditAsync([FromBody] StockListEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var stockList = new StockList(model.Key, model.Title, model.Cost, DateTime.Now);
        await _factory.GetGrain<IStockListGrains>(model.OwnerKey).SetAsync(stockList, model.OwnerKey);
        return Ok(stockList);
    }

    public record class StockListModel(
    [Required] Guid OwnerKey,
    [Required] string Title,
    [Required] double Cost);

    public record class StockListEditModel(
    [Required] Guid Key,
    [Required] Guid OwnerKey,
    [Required] string Title,
    [Required] double Cost);
}

