namespace Server.Models;


[Immutable]
[GenerateSerializer]
public record class Transaction(
    // トランザクションID
    Guid Key,
    string TokenName,
    DateTime? CreatedAt,
    // 誰から送られたか
    string SendFrom,
    // 誰に送られたか
    string SendTo,
    // いくら送金したか
    double Cost,
    bool IsFarst);

