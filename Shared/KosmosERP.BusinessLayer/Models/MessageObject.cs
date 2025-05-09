﻿using System.Text.Json;

namespace KosmosERP.BusinessLayer.Models;

public class MessageObject
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public DateTime created_on { get; set; } = DateTime.UtcNow;
    public string body { get; set; }
    public string object_type { get; set; }

    public MessageObject()
    {
        body = "";
        object_type = "";
    }

    public MessageObject(object content)
    {
        body = JsonSerializer.Serialize(content);
    }
}
