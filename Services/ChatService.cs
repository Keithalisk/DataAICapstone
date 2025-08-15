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
                        Content = "I'm currently running without an AI language model. To enable full functionality, please configure your Azure OpenAI API key in appsettings.json.\n\n" +
                                 "However, I can still help you with movie review functions like:\n" +
                                 "• Browse available movies for review\n" +
                                 "• Search movie reviews using semantic similarity\n" +
                                 "• Add new movie reviews with automatic embedding\n" +
                                 "• Filter movies by genre, year, or rating\n\n" +
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
                    "You are a specialized Movie Review Assistant powered by advanced semantic search capabilities. " +
                    "You have access to a PostgreSQL database with vector embeddings that allows you to:\n\n" +
                    "1. **Search movie reviews semantically** - Find reviews based on natural language queries using vector similarity\n" +
                    "2. **Browse available movies** - Show users what movies are available for review with filtering options\n" +
                    "3. **Add new movie reviews** - Help users submit reviews that are automatically embedded for future search\n" +
                    "4. **Filter and compare** - Advanced filtering by genre, year, rating, and comparison of search methods\n\n" +
                    "Key capabilities:\n" +
                    "- Semantic search using Azure OpenAI embeddings for natural language understanding\n" +
                    "- Vector similarity matching for finding related content\n" +
                    "- Real-time embedding generation for new reviews\n" +
                    "- Comprehensive movie database with metadata\n\n" +
                    "Always encourage users to explore movie discovery, add their own reviews, and leverage the semantic search " +
                    "to find exactly what they're looking for. When users ask about movies, reviews, or want to add content, " +
                    "use the available PostgreSQL semantic search functions to provide accurate, relevant results."
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
                // AutoInvokeKernelFunctions allows the LLM to intelligently decide whether to:
                // 1. Call available plugin functions when they're needed for the user's request
                // 2. Respond directly using its own trained knowledge when appropriate
                // This gives the AI flexibility to handle both movie database queries AND general conversation
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
                Content = "🎬 Welcome to your **Movie Review Assistant**! \n\n" +
                         "I'm powered by advanced semantic search using PostgreSQL vector embeddings and Azure OpenAI. Here's what I can help you with:\n\n" +
                         "🔍 **Discover Movies & Reviews**\n" +
                         "• *\"What movies can I review?\"* - Browse available movies with filters\n" +
                         "• *\"Find action movies from 2020 or later\"* - Smart filtering by genre, year, rating\n" +
                         "• *\"Search for reviews about great special effects\"* - Semantic search through reviews\n\n" +
                         "⭐ **Add Your Reviews**\n" +
                         "• *\"I want to review [IMDB_ID] with title 'Amazing Film' and text '...'\"* - Add reviews with auto-embedding\n" +
                         "• Your reviews become instantly searchable by others!\n\n" +
                         "🎯 **Smart Search Features**\n" +
                         "• *\"Find romantic comedies with good acting\"* - Natural language understanding\n" +
                         "• *\"Show me highly rated thriller movies\"* - Vector similarity matching\n" +
                         "• *\"Compare semantic vs keyword search\"* - Advanced search analysis\n\n" +
                         "**Try asking**: *\"What sci-fi movies can I review?\"* or *\"Find reviews mentioning plot twists\"*\n\n" +
                         "What movie adventure would you like to start with? 🍿",
                Role = MessageRole.System,
                AgentName = "Movie Review Assistant"
            };
            
            session.Messages.Add(welcomeMessage);

            _logger.LogInformation("Created new chat session: {SessionId}", session.Id);
            
            return session;
        }
    }
}
