namespace ZenBlog.Domain.DTOs.SystemDTOs;

public sealed record EmailParameters(
    string Host,
    int Port,
    bool EnableSsl,
    string From,
    string Username,
    string AppPassword);
