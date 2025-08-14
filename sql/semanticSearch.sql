-- Semantic Search Queries for Movie Reviews
-- All embeddings are now populated!

-- 1. Basic semantic search: Find reviews about "action movies with great special effects"
WITH query_embedding AS (
    SELECT azure_openai.create_embeddings(
        'text-embedding-ada-002',
        'action movies with great special effects and amazing visuals'
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
ORDER BY review_embedding <=> q.embedding
LIMIT 10;

-- 2. Search for "emotional dramas about family relationships"
WITH query_embedding AS (
    SELECT azure_openai.create_embeddings(
        'text-embedding-ada-002',
        'emotional drama about family relationships and personal growth'
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
ORDER BY review_embedding <=> q.embedding
LIMIT 10;

-- 3. Search for "funny comedies that made me laugh"
WITH query_embedding AS (
    SELECT azure_openai.create_embeddings(
        'text-embedding-ada-002',
        'hilarious comedy that made me laugh out loud, funny and entertaining'
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
ORDER BY review_embedding <=> q.embedding
LIMIT 10;

-- 4. Search for "disappointing sequels that ruined the franchise"
WITH query_embedding AS (
    SELECT azure_openai.create_embeddings(
        'text-embedding-ada-002',
        'disappointing sequel that ruined the franchise, bad follow-up movie'
    ) as embedding
)
SELECT 
    m.movie_title,
    mr.review_title,
    mr.review_rating,
    m.genre,
    m.release_year,
    1 - (mr.review_embedding <=> q.embedding) as similarity_score
FROM movie_reviews mr
JOIN movies m ON mr.imdb_id = m.imdb_id
CROSS JOIN query_embedding q
WHERE mr.review_embedding IS NOT NULL
ORDER BY mr.review_embedding <=> q.embedding
LIMIT 10;

-- 5. Advanced: Semantic search with filters
-- Find "thrilling movies" from 2020 or later
WITH query_embedding AS (
    SELECT azure_openai.create_embeddings(
        'text-embedding-ada-002',
        'thrilling suspenseful movie that kept me on the edge of my seat'
    ) as embedding
)
SELECT 
    m.movie_title,
    mr.review_title,
    mr.review_rating,
    m.genre,
    m.release_year,
    m.rating as movie_rating,
    1 - (mr.review_embedding <=> q.embedding) as similarity_score
FROM movie_reviews mr
JOIN movies m ON mr.imdb_id = m.imdb_id
CROSS JOIN query_embedding q
WHERE mr.review_embedding IS NOT NULL
  AND m.release_year >= 2020
  AND mr.review_rating >= 7  -- Only positive reviews
ORDER BY mr.review_embedding <=> q.embedding
LIMIT 15;

-- 6. Compare traditional keyword search vs semantic search
-- Traditional keyword search
SELECT 
    m.movie_title,
    mr.review_title,
    mr.review_rating,
    'Keyword Search' as search_type
FROM movie_reviews mr
JOIN movies m ON mr.imdb_id = m.imdb_id
WHERE mr.review_text ILIKE '%action%' 
  AND mr.review_text ILIKE '%special effects%'
LIMIT 5;

-- Note: Run the semantic search queries above to compare results!
