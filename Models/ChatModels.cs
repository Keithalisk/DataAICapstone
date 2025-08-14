using System.ComponentModel.DataAnnotations;

namespace DataAICapstone.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public MessageRole Role { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? AgentName { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public enum MessageRole
    {
        User,
        Assistant,
        System
    }

    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<ChatMessage> Messages { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }
}
