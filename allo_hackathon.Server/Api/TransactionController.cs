using Microsoft.AspNetCore.Mvc;
using Server.Grains;
using Server.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Immutable;
using Orleans;
//using Server.Grains;

namespace Server.Silo.Api;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly IGrainFactory _factory;

    public TransactionController(IGrainFactory factory) => _factory = factory;

    [HttpGet("{uuid}")]
    public Task<Transaction> GetAsync([Required] Guid uuid) => _factory.GetGrain<ITransactionGrains>(uuid).GetAsync(uuid);

    [HttpGet("all")]
    public async Task<List<Transaction>> GetAllAsync() {
        List<Transaction> transactionList = new List<Transaction>();
        var transactions = await _factory.GetGrain<ITransactionManagerGrain>("Transaction").GetAllAsync();
        foreach(Guid t in transactions)
        {
            var temp = await _factory.GetGrain<ITransactionGrains>(t).GetAsync(t);
            transactionList.Add(temp);
        }
        return transactionList;
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] TransactionModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var transaction = new Transaction(model.Key, model.TokenId, DateTime.Now, model.SendFrom, model.SendTo, model.Cost, model.IsFarst);
        await _factory.GetGrain<ITransactionGrains>(model.Key).SetAsync(transaction);
        return Ok();
    }

    public record class TransactionModel(
    // トランザクションID
    [Required] Guid Key,
    [Required] Guid TokenId,
    [Required] DateTime? CreatedAt,
    // 誰から送られたか
    [Required] string SendFrom,
    // 誰に送られたか
    [Required] string SendTo,
    // いくら送金したか
    [Required] double Cost,
    [Required] bool IsFarst);
}

