namespace ChatApp.Application.Dto;

public class MessageGetDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Author { get; set; }
    public DateTime Timestamp { get; set; }
}
