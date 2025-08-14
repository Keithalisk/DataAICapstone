using DataAICapstone.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DataAICapstone.Services
{
    public interface IChatService
    {
        Task<ChatMessage> SendMessageAsync(string content, Guid? sessionId = null);
        Task<ChatSession> GetSessionAsync(Guid sessionId);
        Task<List<ChatSession>> GetRecentSessionsAsync(int count = 10);
        Task<ChatSession> CreateSessionAsync();
    }

    public class ChatService : IChatService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<ChatService> _logger;
        private readonly Dictionary<Guid, ChatSession> _sessions = new();

        public ChatService(
            Kernel kernel,
            ILogger<ChatService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<ChatMessage> SendMessageAsync(string content, Guid? sessionId = null)
        {
            try
            {
                // Get or create session
                var session = sessionId.HasValue 
                    ? await GetSessionAsync(sessionId.Value)
                    : await CreateSessionAsync();

                // Add user message to session
                var userMessage = new ChatMessage
                {
                    Content = content,
                    Role = MessageRole.User
                };
                session.Messages.Add(userMessage);
                session.LastActivity = DateTime.UtcNow;

                _logger.LogInformation("Processing user message in session {SessionId}: {Content}", 
                    session.Id, content);

                // Check if OpenAI service is available
                var chatCompletionService = _kernel.Services.GetService<IChatCompletionService>();
                if (chatCompletionService == null)
                {
                    // Fallback response when no OpenAI service is configured
                    var fallbackMessage = new ChatMessage
                    {
                        Content = "I'm currently running without an AI language model. To enable full functionality, please configure your OpenAI API key in appsettings.json or appsettings.Development.json.\n\n" +
                                 "However, I can still help you with plugin functions like:\n" +
                                 "• Math calculations (try asking me to calculate something)\n" +
                                 "• Text processing (case conversion, word counting)\n" +
                                 "• Date/time operations\n" +
                                 "• File operations\n\n" +
                                 "Note: Without the language model, I can only execute direct function calls, not natural language understanding.",
                        Role = MessageRole.Assistant,
                        AgentName = "System (No AI Model)"
                    };

                    session.Messages.Add(fallbackMessage);
                    session.LastActivity = DateTime.UtcNow;
                    return fallbackMessage;
                }

                // Get chat completion service
                chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Build chat history from session messages
                var chatHistory = new ChatHistory();
                
                // Add system message with plugin capabilities
                chatHistory.AddSystemMessage(
                    "You are a helpful AI assistant with access to various plugins and tools. " +
                    "You can help with mathematical calculations, text processing, date/time operations, " +
                    "and file operations. When users ask for something that might require these capabilities, " +
                    "feel free to use the available functions to provide accurate results."
                );

                // Add recent conversation history (last 10 messages to keep context manageable)
                foreach (var msg in session.Messages.TakeLast(10))
                {
                    if (msg.Role == MessageRole.User)
                        chatHistory.AddUserMessage(msg.Content);
                    else if (msg.Role == MessageRole.Assistant)
                        chatHistory.AddAssistantMessage(msg.Content);
                }

                // Configure execution settings to enable function calling
                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                // Get response from Semantic Kernel
                var result = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory, 
                    executionSettings, 
                    _kernel);

                // Create assistant response message
                var assistantMessage = new ChatMessage
                {
                    Content = result.Content ?? "I apologize, but I couldn't generate a response.",
                    Role = MessageRole.Assistant,
                    AgentName = "AI Assistant"
                };

                // Add assistant message to session
                session.Messages.Add(assistantMessage);
                session.LastActivity = DateTime.UtcNow;

                _logger.LogInformation("Generated response from Semantic Kernel");

                return assistantMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message: {Content}", content);
                
                var errorMessage = new ChatMessage
                {
                    Content = "I apologize, but I encountered an error while processing your message. Please try again.",
                    Role = MessageRole.Assistant,
                    AgentName = "System"
                };

                return errorMessage;
            }
        }

        public async Task<ChatSession> GetSessionAsync(Guid sessionId)
        {
            await Task.CompletedTask;
            
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                return session;
            }

            // If session doesn't exist, create a new one
            return await CreateSessionAsync();
        }

        public async Task<List<ChatSession>> GetRecentSessionsAsync(int count = 10)
        {
            await Task.CompletedTask;
            
            return _sessions.Values
                .OrderByDescending(s => s.LastActivity)
                .Take(count)
                .ToList();
        }

        public async Task<ChatSession> CreateSessionAsync()
        {
            await Task.CompletedTask;
            
            var session = new ChatSession();
            _sessions[session.Id] = session;

            // Add welcome message
            var welcomeMessage = new ChatMessage
            {
                Content = "Hello! I'm your AI assistant with access to various plugins and capabilities. I can help you with:\n\n" +
                         "• **Mathematical calculations** - Basic arithmetic, square roots, powers, etc.\n" +
                         "• **Text processing** - Case conversion, word counting, text manipulation\n" +
                         "• **Date and time operations** - Current time, date calculations, formatting\n" +
                         "• **File operations** - Reading, writing, and managing text files\n" +
                         "• **General questions** - I can help with various other tasks as well\n\n" +
                         "What can I help you with today?",
                Role = MessageRole.System,
                AgentName = "System"
            };
            
            session.Messages.Add(welcomeMessage);

            _logger.LogInformation("Created new chat session: {SessionId}", session.Id);
            
            return session;
        }
    }
}
