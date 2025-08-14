# Semantic Kernel Plugins

This directory contains Semantic Kernel plugins that provide external tools and functions for your AI agents.

## Available Plugins

### PostgreSQLSemanticSearchPlugin
The core plugin for this application, providing PostgreSQL-based semantic search capabilities:
- `SearchMovieReviewsAsync` - Search for movie reviews using semantic similarity
- `SearchMovieReviewsWithFiltersAsync` - Search with additional filters (year, genre, rating)
- `GetRecentHighRatedReviewsAsync` - Get recent highly-rated movie reviews
- `CompareSemanticVsKeywordSearchAsync` - Compare semantic vs traditional keyword search

This plugin leverages:
- **Azure OpenAI embeddings** for semantic understanding
- **PostgreSQL pgvector extension** for efficient vector similarity search
- **Real-time embedding generation** through PostgreSQL's azure_ai extension

## How It Works

1. **Natural Language Queries**: Users can search using natural language like "action movies with great special effects"
2. **Embedding Generation**: PostgreSQL calls Azure OpenAI to create embeddings from the query text
3. **Vector Similarity**: The query embedding is compared against pre-stored review embeddings using cosine similarity
4. **Ranked Results**: Results are returned ranked by semantic similarity score

## Configuration

The PostgreSQL plugin requires:
- Azure OpenAI configuration (endpoint, API key, embedding model)
- PostgreSQL connection with vector and azure_ai extensions enabled
- Pre-populated movie review embeddings in the database

See `sql/createEmbeddings.sql.template` for database setup instructions.

## Adding New Plugins

To add a new plugin:

1. Create a new class in this directory
2. Add methods decorated with `[KernelFunction]` attribute
3. Use `[Description]` attributes to describe functions and parameters
4. Register the plugin in `Program.cs` using `kernelBuilder.Plugins.AddFromObject()` or `AddFromType()`

Example:
```csharp
public class MyPlugin
{
    [KernelFunction]
    [Description("Does something useful")]
    public string DoSomething([Description("Input parameter")] string input)
    {
        return $"Processed: {input}";
    }
}
```

## Security Notes

- Database connections use secure connection strings stored in appsettings
- Sensitive configuration files are excluded from version control via .gitignore
- API keys and database credentials should never be committed to the repository
