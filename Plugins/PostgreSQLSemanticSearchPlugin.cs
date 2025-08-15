using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Data;
using Npgsql;
using System.Text.Json;

namespace DataAICapstone.Plugins
{
    /// <summary>
    /// PostgreSQL Semantic Search plugin for finding similar movie reviews using vector embeddings
    /// </summary>
    public class PostgreSQLSemanticSearchPlugin
    {
        private readonly string _connectionString;
        private readonly ILogger<PostgreSQLSemanticSearchPlugin> _logger;

        public PostgreSQLSemanticSearchPlugin(IConfiguration configuration, ILogger<PostgreSQLSemanticSearchPlugin> logger)
        {
            _connectionString = configuration.GetConnectionString("PostgreSQL") 
                ?? throw new InvalidOperationException("PostgreSQL connection string not found");
            _logger = logger;
        }

        [KernelFunction]
        [Description("Search for movie reviews using semantic similarity based on natural language queries")]
        public async Task<string> SearchMovieReviewsAsync(
            [Description("Natural language query to search for (e.g., 'action movies with great special effects')")] string query,
            [Description("Maximum number of results to return (default: 10)")] int limit = 10,
            [Description("Minimum similarity threshold (0.0 to 1.0, default: 0.7)")] double minSimilarity = 0.7)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    WITH query_embedding AS (
                        SELECT azure_openai.create_embeddings(
                            'text-embedding-ada-002',
                            @query
                        )::vector(1536) as embedding
                    )
                    SELECT 
                        movie_title,
                        review_title,
                        review_rating,
                        genre,
                        release_year,
                        ROUND((1 - (review_embedding <=> q.embedding))::numeric, 4) as similarity_score
                    FROM movie_review_text_for_embedding
                    CROSS JOIN query_embedding q
                    WHERE review_embedding IS NOT NULL
                      AND (1 - (review_embedding <=> q.embedding)) >= @minSimilarity
                    ORDER BY review_embedding <=> q.embedding
                    LIMIT @limit";

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@query", query);
                command.Parameters.AddWithValue("@limit", limit);
                command.Parameters.AddWithValue("@minSimilarity", minSimilarity);

                var results = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        movie_title = reader.GetString("movie_title"),
                        review_title = reader.GetString("review_title"),
                        review_rating = reader.IsDBNull("review_rating") ? (int?)null : reader.GetInt32("review_rating"),
                        genre = reader.GetString("genre"),
                        release_year = reader.GetInt32("release_year"),
                        similarity_score = reader.GetDecimal("similarity_score")
                    });
                }

                if (!results.Any())
                {
                    return $"No movie reviews found matching '{query}' with similarity >= {minSimilarity}";
                }

                return FormatSearchResults(query, results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing semantic search for query: {Query}", query);
                return $"Error searching for '{query}': {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Search for movie reviews with additional filters like year range or genre")]
        public async Task<string> SearchMovieReviewsWithFiltersAsync(
            [Description("Natural language query to search for")] string query,
            [Description("Minimum release year (optional)")] int? minYear = null,
            [Description("Maximum release year (optional)")] int? maxYear = null,
            [Description("Genre filter (optional, e.g., 'Action', 'Drama')")] string? genre = null,
            [Description("Minimum review rating (1-10, optional)")] int? minRating = null,
            [Description("Maximum number of results (default: 10)")] int limit = 10)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var whereConditions = new List<string> { "review_embedding IS NOT NULL" };
                var parameters = new List<NpgsqlParameter>
                {
                    new("@query", query),
                    new("@limit", limit)
                };

                if (minYear.HasValue)
                {
                    whereConditions.Add("release_year >= @minYear");
                    parameters.Add(new("@minYear", minYear.Value));
                }

                if (maxYear.HasValue)
                {
                    whereConditions.Add("release_year <= @maxYear");
                    parameters.Add(new("@maxYear", maxYear.Value));
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    whereConditions.Add("genre ILIKE @genre");
                    parameters.Add(new("@genre", $"%{genre}%"));
                }

                if (minRating.HasValue)
                {
                    whereConditions.Add("review_rating >= @minRating");
                    parameters.Add(new("@minRating", minRating.Value));
                }

                var sql = $@"
                    WITH query_embedding AS (
                        SELECT azure_openai.create_embeddings(
                            'text-embedding-ada-002',
                            @query
                        )::vector(1536) as embedding
                    )
                    SELECT 
                        movie_title,
                        review_title,
                        review_rating,
                        genre,
                        release_year,
                        ROUND((1 - (review_embedding <=> q.embedding))::numeric, 4) as similarity_score
                    FROM movie_review_text_for_embedding
                    CROSS JOIN query_embedding q
                    WHERE {string.Join(" AND ", whereConditions)}
                    ORDER BY review_embedding <=> q.embedding
                    LIMIT @limit";

                using var command = new NpgsqlCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                var results = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        movie_title = reader.GetString("movie_title"),
                        review_title = reader.GetString("review_title"),
                        review_rating = reader.IsDBNull("review_rating") ? (int?)null : reader.GetInt32("review_rating"),
                        genre = reader.GetString("genre"),
                        release_year = reader.GetInt32("release_year"),
                        similarity_score = reader.GetDecimal("similarity_score")
                    });
                }

                if (!results.Any())
                {
                    return $"No movie reviews found matching your criteria";
                }

                return FormatSearchResults(query, results, $"Filters: {GetFilterDescription(minYear, maxYear, genre, minRating)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing filtered semantic search for query: {Query}", query);
                return $"Error searching with filters: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Get recent highly-rated movie reviews using semantic search")]
        public async Task<string> GetRecentHighRatedReviewsAsync(
            [Description("Natural language query for movie type (e.g., 'thriller', 'romance')")] string movieType,
            [Description("Minimum year for recent movies (default: 2020)")] int minYear = 2020,
            [Description("Minimum rating (1-10, default: 8)")] int minRating = 8,
            [Description("Number of results (default: 5)")] int limit = 5)
        {
            return await SearchMovieReviewsWithFiltersAsync(
                movieType, 
                minYear, 
                null, 
                null, 
                minRating, 
                limit);
        }

        [KernelFunction]
        [Description("Compare semantic search results with traditional keyword search")]
        public async Task<string> CompareSemanticVsKeywordSearchAsync(
            [Description("Search query to compare")] string query,
            [Description("Keywords for traditional search (space-separated)")] string keywords)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Semantic search
                var semanticResults = await GetSemanticSearchResults(connection, query, 5);
                
                // Keyword search
                var keywordResults = await GetKeywordSearchResults(connection, keywords.Split(' '), 5);

                var comparison = new
                {
                    Query = query,
                    Keywords = keywords,
                    SemanticResults = semanticResults.Count,
                    KeywordResults = keywordResults.Count,
                    Semantic = semanticResults.Take(3),
                    Keyword = keywordResults.Take(3)
                };

                return JsonSerializer.Serialize(comparison, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing search methods for query: {Query}", query);
                return $"Error comparing search methods: {ex.Message}";
            }
        }

        private async Task<List<object>> GetSemanticSearchResults(NpgsqlConnection connection, string query, int limit)
        {
            var sql = @"
                WITH query_embedding AS (
                    SELECT azure_openai.create_embeddings(
                        'text-embedding-ada-002',
                        @query
                    )::vector(1536) as embedding
                )
                SELECT 
                    movie_title,
                    review_title,
                    review_rating,
                    ROUND((1 - (review_embedding <=> q.embedding))::numeric, 4) as similarity_score
                FROM movie_review_text_for_embedding
                CROSS JOIN query_embedding q
                WHERE review_embedding IS NOT NULL
                ORDER BY review_embedding <=> q.embedding
                LIMIT @limit";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@query", query);
            command.Parameters.AddWithValue("@limit", limit);

            var results = new List<object>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    movie_title = reader.GetString("movie_title"),
                    review_title = reader.GetString("review_title"),
                    similarity_score = reader.GetDecimal("similarity_score")
                });
            }

            return results;
        }

        private async Task<List<object>> GetKeywordSearchResults(NpgsqlConnection connection, string[] keywords, int limit)
        {
            var whereConditions = keywords.Select((_, i) => $"review_text ILIKE @keyword{i}").ToList();
            var sql = $@"
                SELECT 
                    m.movie_title,
                    mr.review_title,
                    mr.review_rating,
                    'keyword' as search_type
                FROM movie_reviews mr
                JOIN movies m ON mr.imdb_id = m.imdb_id
                WHERE {string.Join(" AND ", whereConditions)}
                LIMIT @limit";

            using var command = new NpgsqlCommand(sql, connection);
            for (int i = 0; i < keywords.Length; i++)
            {
                command.Parameters.AddWithValue($"@keyword{i}", $"%{keywords[i]}%");
            }
            command.Parameters.AddWithValue("@limit", limit);

            var results = new List<object>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new
                {
                    movie_title = reader.GetString("movie_title"),
                    review_title = reader.GetString("review_title"),
                    search_type = "keyword"
                });
            }

            return results;
        }

        [KernelFunction]
        [Description("Get a list of movies available for review")]
        public async Task<string> GetAvailableMoviesAsync(
            [Description("Optional genre filter (e.g., 'Action', 'Drama')")] string? genre = null,
            [Description("Optional minimum release year")] int? minYear = null,
            [Description("Optional maximum release year")] int? maxYear = null,
            [Description("Maximum number of movies to return (default: 20)")] int limit = 20)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var whereConditions = new List<string>();
                var parameters = new List<NpgsqlParameter> { new("@limit", limit) };

                if (!string.IsNullOrEmpty(genre))
                {
                    whereConditions.Add("genre ILIKE @genre");
                    parameters.Add(new("@genre", $"%{genre}%"));
                }

                if (minYear.HasValue)
                {
                    whereConditions.Add("release_year >= @minYear");
                    parameters.Add(new("@minYear", minYear.Value));
                }

                if (maxYear.HasValue)
                {
                    whereConditions.Add("release_year <= @maxYear");
                    parameters.Add(new("@maxYear", maxYear.Value));
                }

                var whereClause = whereConditions.Any() ? $"WHERE {string.Join(" AND ", whereConditions)}" : "";

                var sql = $@"
                    SELECT 
                        imdb_id,
                        title,
                        genre,
                        release_year,
                        rating,
                        (SELECT COUNT(*) FROM movie_reviews mr WHERE mr.imdb_id = m.imdb_id) as review_count
                    FROM movies m
                    {whereClause}
                    ORDER BY release_year DESC, title
                    LIMIT @limit";

                using var command = new NpgsqlCommand(sql, connection);
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }

                var movies = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    movies.Add(new
                    {
                        imdb_id = reader.GetString("imdb_id"),
                        title = reader.GetString("title"),
                        genre = reader.IsDBNull("genre") ? "Unknown" : reader.GetString("genre"),
                        release_year = reader.GetInt32("release_year"),
                        rating = reader.IsDBNull("rating") ? (decimal?)null : reader.GetDecimal("rating"),
                        review_count = reader.GetInt32("review_count")
                    });
                }

                if (!movies.Any())
                {
                    return "No movies found matching your criteria.";
                }

                var response = $"## Available Movies for Review\n";
                if (!string.IsNullOrEmpty(genre) || minYear.HasValue || maxYear.HasValue)
                {
                    var filters = new List<string>();
                    if (!string.IsNullOrEmpty(genre)) filters.Add($"Genre: {genre}");
                    if (minYear.HasValue) filters.Add($"Year >= {minYear}");
                    if (maxYear.HasValue) filters.Add($"Year <= {maxYear}");
                    response += $"**Filters**: {string.Join(", ", filters)}\n";
                }
                response += $"Found {movies.Count} movies:\n\n";

                foreach (dynamic movie in movies)
                {
                    // Handle the rating properly for dynamic objects
                    var rating = movie.rating as decimal?;
                    var ratingText = rating.HasValue ? $" | Rating: {rating.Value:F1}" : "";
                    response += $"🎬 **{movie.title}** ({movie.release_year})\n";
                    response += $"   IMDB ID: {movie.imdb_id} | Genre: {movie.genre}{ratingText}\n";
                    response += $"   Existing Reviews: {movie.review_count}\n\n";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available movies");
                return $"Error retrieving movies: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Add a new movie review to the database with automatic semantic embedding generation")]
        public async Task<string> AddMovieReviewAsync(
            [Description("IMDB ID of the movie to review (use GetAvailableMoviesAsync to see options)")] string imdbId,
            [Description("Review title/headline")] string reviewTitle,
            [Description("Full review text content")] string reviewText,
            [Description("Rating from 1-10")] int reviewRating)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Start a transaction for data consistency
                using var transaction = await connection.BeginTransactionAsync();

                object? movieDetails = null;
                int newReviewId = 0;

                try
                {
                    // First, verify the movie exists and get its details
                    var movieSql = @"
                        SELECT imdb_id, title, genre, release_year, rating
                        FROM movies 
                        WHERE imdb_id = @imdbId";

                    using (var movieCommand = new NpgsqlCommand(movieSql, connection, transaction))
                    {
                        movieCommand.Parameters.AddWithValue("@imdbId", imdbId);
                        using var movieReader = await movieCommand.ExecuteReaderAsync();
                        
                        if (await movieReader.ReadAsync())
                        {
                            movieDetails = new
                            {
                                imdb_id = movieReader.GetString("imdb_id"),
                                title = movieReader.GetString("title"),
                                genre = movieReader.IsDBNull("genre") ? "Unknown" : movieReader.GetString("genre"),
                                release_year = movieReader.GetInt32("release_year"),
                                rating = movieReader.IsDBNull("rating") ? (decimal?)null : movieReader.GetDecimal("rating")
                            };
                        }
                    }

                    if (movieDetails == null)
                    {
                        await transaction.RollbackAsync();
                        return $"❌ Movie with IMDB ID '{imdbId}' not found. Use GetAvailableMoviesAsync to see available movies for review.";
                    }

                    // Insert the review with automatic embedding generation
                    var insertReviewSql = @"
                        INSERT INTO movie_reviews (
                            imdb_id, 
                            review_title, 
                            review_text, 
                            review_rating,
                            review_embedding
                        )
                        VALUES (
                            @imdbId,
                            @reviewTitle,
                            @reviewText,
                            @reviewRating,
                            azure_openai.create_embeddings('text-embedding-ada-002', @reviewText)::vector(1536)
                        )
                        RETURNING review_id";

                    using (var reviewCommand = new NpgsqlCommand(insertReviewSql, connection, transaction))
                    {
                        reviewCommand.Parameters.AddWithValue("@imdbId", imdbId);
                        reviewCommand.Parameters.AddWithValue("@reviewTitle", reviewTitle);
                        reviewCommand.Parameters.AddWithValue("@reviewText", reviewText);
                        reviewCommand.Parameters.AddWithValue("@reviewRating", reviewRating);

                        newReviewId = (int)(await reviewCommand.ExecuteScalarAsync() ?? 0);
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                // Success! Build response outside of transaction scope
                dynamic movie = movieDetails;
                var movieTitle = (string)movie.title;
                _logger.LogInformation("Successfully added review {ReviewId} for movie '{MovieTitle}' (IMDB: {ImdbId})", 
                    newReviewId, movieTitle, imdbId);

                // Handle the rating properly for dynamic objects
                var movieRating = movie.rating as decimal?;
                var ratingText = movieRating.HasValue ? $" | Movie Rating: {movieRating.Value:F1}" : "";
                return $"✅ Successfully added review for '{movie.title}'!\n\n" +
                       $"**Review ID**: {newReviewId}\n" +
                       $"**IMDB ID**: {movie.imdb_id}\n" +
                       $"**Movie**: {movie.title} ({movie.release_year}) - {movie.genre}{ratingText}\n" +
                       $"**Review Title**: {reviewTitle}\n" +
                       $"**Your Rating**: {reviewRating}/10\n" +
                       $"**Review Length**: {reviewText.Length} characters\n\n" +
                       $"The review has been automatically embedded and is now searchable through semantic search!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding movie review for IMDB ID: {ImdbId}", imdbId);
                return $"❌ Error adding review for IMDB ID '{imdbId}': {ex.Message}";
            }
        }

        private static string FormatSearchResults(string query, List<object> results, string? additionalInfo = null)
        {
            var response = $"## Semantic Search Results for: '{query}'\n";
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                response += $"**{additionalInfo}**\n";
            }
            response += $"Found {results.Count} matching reviews:\n\n";

            foreach (dynamic result in results)
            {
                response += $"**{result.movie_title}** ({result.release_year}) - {result.genre}\n";
                response += $"Review: {result.review_title}\n";
                
                // Handle nullable review rating
                var rating = result.review_rating as int?;
                var ratingText = rating.HasValue ? $"{rating.Value}/10" : "No rating";
                response += $"Rating: {ratingText} | Similarity: {result.similarity_score:F3}\n\n";
            }

            return response;
        }

        private static string GetFilterDescription(int? minYear, int? maxYear, string? genre, int? minRating)
        {
            var filters = new List<string>();
            
            if (minYear.HasValue && maxYear.HasValue)
                filters.Add($"Years: {minYear}-{maxYear}");
            else if (minYear.HasValue)
                filters.Add($"Year >= {minYear}");
            else if (maxYear.HasValue)
                filters.Add($"Year <= {maxYear}");
            
            if (!string.IsNullOrEmpty(genre))
                filters.Add($"Genre: {genre}");
            
            if (minRating.HasValue)
                filters.Add($"Rating >= {minRating}");

            return filters.Any() ? string.Join(", ", filters) : "No filters";
        }
    }
}
