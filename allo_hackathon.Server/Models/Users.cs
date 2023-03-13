namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class Users(
    Guid Key,
    DateTime? CreatedAt,
    string Name,
    // 1日1回のトークンを受け取ったかどうか
    bool IsReceived,
    // 今持ってるトークンのリスト
    TokenItem?[] TokenList,
    // 自分が送ったトークンのリスト
    TokenItem?[] SendTo,
    // 自分に送られたトークンのリスト
    TokenItem?[] SendFrom,
    // 自分のトークンの合計数
    double? MyToken,
    DateTime? DeletedAt = null);

