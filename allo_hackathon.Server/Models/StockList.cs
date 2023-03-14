namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class StockList(
    Guid OwnerId,
    string TokenName,
    double Cost,
    DateTime? CreatedAt,
    DateTime? DeletedAt = null);


