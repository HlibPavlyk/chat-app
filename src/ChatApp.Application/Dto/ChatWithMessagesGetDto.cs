namespace ChatApp.Application.Dto;

public class ChatWithMessagesGetDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public PagedResponse<MessageGetDto>? Messages { get; set; }
}