namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class TokenItem(
    Guid Key,
    double Token,
    DateTime CreatedAt);

