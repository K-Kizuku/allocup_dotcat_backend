namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class StockList(
    Guid Guid,
    string Title,
    double Cost,
    DateTime? CreatedAt,
    DateTime? DeletedAt = null);


