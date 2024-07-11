﻿namespace ChatApp.Domain.Entities;

public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string Author { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    
    public Chat Chat { get; set; }
}

