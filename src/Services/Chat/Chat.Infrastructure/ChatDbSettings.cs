﻿namespace Chat.Infrastructure;

public class ChatDbSettings
{
    public string DefaultConnection { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string ServersCollectionName { get; set; } = default!;
    public string UsersCollectionName { get; set; } = default!;
    public string MessagesCollectionName { get; set; } = default!;
}
