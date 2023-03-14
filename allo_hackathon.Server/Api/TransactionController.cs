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

    [HttpGet("my/{name}")]
    public async Task<List<MyTransactionList>> GetAsync([Required] string name)
    {
        var uuid = await _factory.GetGrain<IUserManagerGrain>("Users").GetUserIdAsync(name);
        return await _factory.GetGrain<ITransactionManagerGrain>("Transaction").GetMyTransactionListsAsync(name);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] TransactionModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Guid guid = Guid.NewGuid();
        var transaction = new Transaction(guid, model.TokenName, DateTime.Now, model.SendFrom, model.SendTo, model.Cost, model.IsFarst);
        await _factory.GetGrain<ITransactionGrains>(guid).SetAsync(transaction);
        return Ok(transaction);
    }

    public record class TransactionModel(
    // トランザクションID
    [Required] string TokenName,
    // 誰から送られたか
    [Required] string SendFrom,
    // 誰に送られたか
    [Required] string SendTo,
    // いくら送金したか
    [Required] double Cost,
    [Required] bool IsFarst);
}

