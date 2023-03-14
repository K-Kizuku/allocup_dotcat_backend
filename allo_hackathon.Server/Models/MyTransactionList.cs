namespace Server.Models;


[Immutable]
[GenerateSerializer]
public record class MyTransactionList(
    // トランザクションID
    Guid Key,
    string TokenName,
    DateTime? CreatedAt,
    bool IsSend,
    // 誰と取引したか
    string OtherName,
    // いくら取引したか
    double Cost);