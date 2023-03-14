namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class StockList(
    string Title,
    double Cost,
    DateTime? CreatedAt,
    DateTime? DeletedAt = null);


