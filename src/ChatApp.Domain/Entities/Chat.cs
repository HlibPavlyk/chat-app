namespace ChatApp.Domain.Entities;

public class Chat
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    
    public ICollection<Message> Messages { get; set; }
}
