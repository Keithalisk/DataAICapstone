# DataAI Capstone - Movie Review Semantic Search

A modern C# .NET Blazor Server application featuring Semantic Kernel AI integration with PostgreSQL vector search for intelligent movie review discovery and management.

## 🚀 Features

- **Interactive Chat Interface**: Modern, responsive Blazor Server UI with real-time messaging
- **Semantic Kernel Integration**: Direct integration with Microsoft Semantic Kernel for AI capabilities
- **Movie Review System**: Complete movie review platform with intelligent search capabilities:
  - **Semantic Search**: Natural language search through movie reviews using vector embeddings
  - **Browse Movies**: View available movies with filtering by genre, year, and rating
  - **Add Reviews**: Submit new movie reviews with automatic embedding generation
  - **Vector Similarity**: Advanced PostgreSQL with pgvector for semantic similarity matching
- **Real-time Embedding**: Automatic vector embedding generation using Azure OpenAI
- **Session Management**: Maintains conversation context across multiple interactions

## 🏗️ Architecture

### Core Components

- **Chat Service**: Direct integration with Semantic Kernel for processing user messages
- **PostgreSQL Semantic Search Plugin**: Advanced vector search capabilities for movie reviews
- **Chat Interface**: Blazor Server-side UI components for real-time chat interaction
- **Vector Database**: PostgreSQL with pgvector extension for similarity search

### Technology Stack

- **Framework**: .NET 9.0 with Blazor Server
- **AI/ML**: Microsoft Semantic Kernel with Azure OpenAI connectors
- **Database**: PostgreSQL with pgvector and azure_ai extensions
- **Vector Search**: Semantic similarity using text-embedding-ada-002
- **Communication**: Real-time updates using Blazor Server rendering
- **Architecture**: Plugin-based design with dependency injection

## 🛠️ Setup

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- PostgreSQL database with pgvector extension
- Azure OpenAI API key for embedding generation

### Configuration

1. **Clone and Build**:
   ```bash
   git clone <repository-url>
   cd DataAICapstone
   dotnet build
   ```

2. **Configure Database and API Keys**:
   Update `appsettings.json`:
   ```json
   {
     "AzureOpenAI": {
       "ApiKey": "your-azure-openai-api-key-here",
       "Endpoint": "your-azure-openai-endpoint"
     },
     "ConnectionStrings": {
       "PostgreSQL": "your-postgresql-connection-string-here"
     }
   }
   ```

3. **Setup Database**:
   - Ensure PostgreSQL has pgvector and azure_ai extensions installed
   - Import your movie and review data
   - Generate embeddings for existing reviews

4. **Run the Application**:
   ```bash
   dotnet run
   ```

5. **Access the Chat Interface**:
   Navigate to the URL displayed in console (typically `http://localhost:5077`)

## 💬 Usage

### Available Capabilities

The Movie Review Assistant can help with various movie-related tasks:

**Browse Available Movies**:
- "What movies can I review?"
- "Show me action movies from 2020 or later"
- "List sci-fi movies with high ratings"

**Semantic Movie Review Search**:
- "Find reviews about movies with great special effects"
- "Search for romantic comedies with good acting"
- "Show me reviews mentioning plot twists"
- "Find highly rated thriller movies"

**Add Movie Reviews**:
- "I want to review tt1234567 with title 'Amazing Film' and text 'This movie was incredible...'"
- First use the browse function to get IMDB IDs, then add your review

**Advanced Search Features**:
- Filter by genre, year range, or minimum rating
- Semantic similarity matching using vector embeddings
- Compare semantic vs keyword search results
- Get recent highly-rated movies by type

**Example Workflow**:
1. "What action movies can I review?" → Browse available action films
2. "I want to review tt1745960..." → Add your review for Top Gun: Maverick
3. "Find reviews about amazing aerial sequences" → Your review becomes searchable!

## 🔧 Development

### Project Structure

```
DataAICapstone/
├── Components/            # Blazor components
│   ├── Pages/
│   │   └── Chat.razor     # Main chat interface
│   └── Layout/            # Layout components
├── Models/                # Data models
│   └── ChatModels.cs      # Chat session and message models
├── Plugins/               # Plugin implementations
│   └── PostgreSQLSemanticSearchPlugin.cs  # Movie review search functionality
├── Services/              # Business services
│   └── ChatService.cs     # Semantic Kernel chat service
├── sql/                   # Database scripts
│   ├── semanticSearch.sql # Vector search queries
│   └── createEmbeddings.sql # Embedding generation
└── Program.cs             # Application startup and configuration
```

### Key Plugin Functions

The PostgreSQL Semantic Search Plugin provides:

1. **SearchMovieReviewsAsync**: Natural language search through reviews
2. **SearchMovieReviewsWithFiltersAsync**: Advanced search with filters
3. **GetAvailableMoviesAsync**: Browse movies available for review
4. **AddMovieReviewAsync**: Add new reviews with automatic embedding
5. **GetRecentHighRatedReviewsAsync**: Find recent highly-rated movies
6. **CompareSemanticVsKeywordSearchAsync**: Compare search methodologies

### Adding New Functionality

1. Add new methods to `PostgreSQLSemanticSearchPlugin.cs`
2. Decorate with `[KernelFunction]` and `[Description]` attributes
3. The Semantic Kernel will automatically make functions available

Example function:
```csharp
[KernelFunction]
[Description("Get movie recommendations based on user preferences")]
public async Task<string> GetMovieRecommendationsAsync(
    [Description("Preferred genres")] string genres,
    [Description("Minimum rating")] int minRating)
{
    // Implementation using vector similarity
}
```

### Security Features

- Vector embeddings generated server-side using Azure OpenAI
- PostgreSQL parameterized queries prevent SQL injection
- Input validation and sanitization for all user inputs
- Safe movie review content filtering
- API key configuration management

## 🚀 Deployment

The application can be deployed to:
- Azure App Service (recommended for Azure OpenAI integration)
- Azure Container Instances
- Docker containers
- Any hosting platform supporting .NET 9.0 and PostgreSQL connectivity

### Environment Variables

For production deployment, configure:
- `AzureOpenAI__ApiKey`: Your Azure OpenAI API key
- `AzureOpenAI__Endpoint`: Your Azure OpenAI endpoint
- `ConnectionStrings__PostgreSQL`: PostgreSQL connection string

## 📝 Notes

- Vector embeddings are generated using Azure OpenAI's text-embedding-ada-002 model
- PostgreSQL requires pgvector and azure_ai extensions for full functionality
- Movie reviews are automatically embedded and immediately searchable
- Conversation history is maintained in memory (consider Redis for production scaling)
- Database supports both existing review search and new review addition

## 🎬 Sample Interactions

**Discovery**:
```
User: "What movies can I review?"
Assistant: [Shows list of available movies with IMDB IDs, genres, years, ratings]

User: "Show me action movies from 2020 or later"
Assistant: [Filtered list of recent action movies]
```

**Search**:
```
User: "Find reviews about movies with amazing visual effects"
Assistant: [Semantic search results with similarity scores]

User: "Search for sci-fi movies with time travel themes"
Assistant: [Related reviews using vector similarity]
```

**Review Addition**:
```
User: "I want to review tt1745960 with title 'Outstanding Sequel' and text 'Top Gun Maverick delivers incredible aerial cinematography...'"
Assistant: [Confirms review added with automatic embedding generation]
```

## 🤝 Contributing

This is a capstone project demonstrating modern .NET development with AI integration and vector databases. Feel free to extend the search capabilities or add new movie-related functions.
