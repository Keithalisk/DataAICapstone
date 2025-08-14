# Copilot Instructions

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

This is a C# .NET Blazor Server application with Semantic Kernel AI orchestration capabilities. The project includes:

## Project Structure
- **AI Orchestration Layer**: Uses Microsoft Semantic Kernel for managing multiple AI agents
- **Chat Interface**: Blazor Server-side UI components for real-time chat interaction
- **Agent Architecture**: Multiple specialized agents including:
  - MSLearn MCP (Model Context Protocol) server agent
  - Azure SQL NL2SQL (Natural Language to SQL) agent
  - Agent coordinator/dispatcher for routing requests

## Key Technologies
- **Framework**: .NET 9.0+ with Blazor Server
- **AI/ML**: Microsoft Semantic Kernel with OpenAI connectors
- **Database**: Azure SQL Database with Microsoft.Data.SqlClient
- **Communication**: Real-time SignalR for chat functionality
- **Architecture**: Agent-based design pattern with dependency injection

## Coding Guidelines
- Follow SOLID principles and dependency injection patterns
- Implement proper error handling and logging
- Use async/await patterns for all AI and database operations
- Maintain separation of concerns between UI, orchestration, and agent layers
- Implement proper configuration management for API keys and connection strings
- Use proper exception handling for AI model failures and database connectivity issues

## Security Considerations
- Never hardcode API keys or connection strings
- Implement proper input validation and sanitization
- Use secure communication protocols for external service calls
- Implement rate limiting and usage monitoring for AI services
