namespace Server.Models;

[Immutable]
[GenerateSerializer]
public record class Users(
    Guid Key,
    DateTime? CreatedAt,
    string UserName,
    string TokenName,
    // 1日1回のトークンを受け取ったかどうか
    bool IsReceived,
    // 今持ってるトークンのリスト
    Dictionary<string, double> TokenList,
    // 自分がフォローしている人のリスト
    List<string> Follows,
    // 自分がフォローされている人のリスト
    List<string> Followers,
    // 自分のトークンの合計数
    double? MyToken,
    DateTime? DeletedAt = null);

[Immutable]
[GenerateSerializer]
public record class ResponseUsers(
    DateTime? CreatedAt,
    string UserName,
    string TokenName,
    // 1日1回のトークンを受け取ったかどうか
    bool IsReceived,
    // 今持ってるトークンのリスト
    Dictionary<string, double> TokenList,
    // 自分がフォローしている人のリスト
    List<string> Follows,
    // 自分がフォローされている人のリスト
    List<string> Followers,
    // 自分のトークンの合計数
    double? MyToken,
    DateTime? DeletedAt = null);

