# DataAI Capstone - AI Chat Assistant

A modern C# .NET Blazor Server application featuring Semantic Kernel AI integration with plugin orchestration for intelligent chat assistance.

## 🚀 Features

- **Interactive Chat Interface**: Modern, responsive Blazor Server UI with real-time messaging
- **Semantic Kernel Integration**: Direct integration with Microsoft Semantic Kernel for AI capabilities
- **Plugin Architecture**: Extensible plugin system with multiple built-in capabilities:
  - **Math Plugin**: Mathematical calculations (add, subtract, multiply, divide, square root, power, etc.)
  - **Text Plugin**: Text processing (case conversion, word counting, string manipulation, etc.)
  - **Time Plugin**: Date and time operations (current time, date calculations, formatting, etc.)
  - **File Plugin**: Safe file operations (read, write, list files within a secure directory)
- **Function Calling**: Automatic plugin function invocation based on user queries
- **Session Management**: Maintains conversation context across multiple interactions

## 🏗️ Architecture

### Core Components

- **Chat Service**: Direct integration with Semantic Kernel for processing user messages
- **Plugin System**: Modular functions that extend AI capabilities
- **Chat Interface**: Blazor Server-side UI components for real-time chat interaction
- **Session Management**: Conversation history and context preservation

### Technology Stack

- **Framework**: .NET 9.0 with Blazor Server
- **AI/ML**: Microsoft Semantic Kernel with OpenAI connectors
- **Plugins**: Native C# plugin functions with Semantic Kernel integration
- **Communication**: Real-time updates using Blazor Server rendering
- **Architecture**: Simplified service-oriented design with dependency injection

## 🛠️ Setup

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- OpenAI API key (recommended for full functionality)

### Configuration

1. **Clone and Build**:
   ```bash
   git clone <repository-url>
   cd DataAICapstone
   dotnet build
   ```

2. **Configure API Keys**:
   Update `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-openai-api-key-here"
     },
     "FileOperations": {
       "WorkingDirectory": "C:\\temp\\AIAssistant"
     }
   }
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

4. **Access the Chat Interface**:
   Navigate to the URL displayed in console (typically `http://localhost:5077`)

## 💬 Usage

### Available Capabilities

The AI assistant can help with various tasks through its plugin system:

**Mathematical Operations**:
- "Calculate 15 + 27"
- "What's the square root of 144?"
- "What's 2 to the power of 8?"

**Text Processing**:
- "Convert 'hello world' to uppercase"
- "Count the words in this sentence"
- "Reverse the text 'semantic kernel'"

**Date and Time**:
- "What's the current time?"
- "Add 30 days to 2024-01-15"
- "How many days between 2024-01-01 and 2024-12-31?"

**File Operations**:
- "Create a file called notes.txt with 'Hello World'"
- "List all .txt files"
- "Read the contents of notes.txt"

**General Assistance**:
- Ask questions about any topic
- Request help with various tasks
- The AI will automatically invoke appropriate plugin functions when needed

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
│   ├── MathPlugin.cs      # Mathematical operations
│   ├── TextPlugin.cs      # Text processing functions
│   ├── TimePlugin.cs      # Date/time operations
│   ├── FilePlugin.cs      # File system operations
│   └── PluginService.cs   # Plugin management service
├── Services/              # Business services
│   └── ChatService.cs     # Semantic Kernel chat service
└── Program.cs             # Application startup and configuration
```

### Adding New Plugins

1. Create a new plugin class in the `Plugins` folder
2. Add methods decorated with `[KernelFunction]` and `[Description]` attributes
3. Register the plugin in `Program.cs` using `kernelBuilder.Plugins.AddFromType<YourPlugin>()`
4. The Semantic Kernel will automatically make functions available to the AI

Example plugin method:
```csharp
[KernelFunction]
[Description("Multiply two numbers")]
public double Multiply(
    [Description("The first number")] double a,
    [Description("The second number")] double b)
{
    return a * b;
}
```

### Security Features

- File operations are restricted to a safe working directory
- Input validation and sanitization
- Safe plugin function execution
- API key configuration management

## 🚀 Deployment

The application can be deployed to:
- Azure App Service
- IIS
- Docker containers
- Any hosting platform supporting .NET 9.0

## 📝 Notes

- The application works with or without OpenAI API keys (limited functionality without)
- All file operations are sandboxed to a safe directory
- Plugin functions are automatically discovered and made available to the AI
- Conversation history is maintained in memory (consider persistence for production)

## 🤝 Contributing

This is a capstone project demonstrating modern .NET development with AI integration. Feel free to extend the plugin capabilities or add new plugin functions.

## 🚀 Features

- **Interactive Chat Interface**: Modern, responsive Blazor Server UI with real-time messaging
- **Multi-Agent Architecture**: Intelligent routing between specialized AI agents
- **Microsoft Learn Integration**: Access to Microsoft Learn documentation and resources via MCP server
- **Natural Language to SQL**: Convert natural language queries to SQL and execute against Azure SQL Database
- **Semantic Kernel Orchestration**: Advanced AI coordination using Microsoft Semantic Kernel

## 🏗️ Architecture

### Core Components

- **AI Orchestration Layer**: Uses Microsoft Semantic Kernel for managing multiple AI agents
- **Chat Interface**: Blazor Server-side UI components for real-time chat interaction
- **Agent System**: Multiple specialized agents including:
  - **MSLearn MCP Agent**: Accesses Microsoft Learn documentation through Model Context Protocol
  - **Azure SQL NL2SQL Agent**: Converts natural language to SQL queries and executes them safely
  - **Agent Coordinator**: Intelligent routing and selection of appropriate agents

### Technology Stack

- **Framework**: .NET 9.0 with Blazor Server
- **AI/ML**: Microsoft Semantic Kernel with OpenAI connectors
- **Database**: Azure SQL Database with Microsoft.Data.SqlClient
- **Communication**: Real-time updates using Blazor Server rendering
- **Architecture**: SOLID principles with dependency injection

## 🛠️ Setup

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- Azure SQL Database (optional - will use sample data if not configured)
- OpenAI API key (optional - basic functionality works without it)

### Configuration

1. **Clone and Build**:
   ```bash
   git clone <repository-url>
   cd DataAICapstone
   dotnet build
   ```

2. **Configure API Keys** (Optional):
   Update `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your-openai-api-key-here"
     },
     "ConnectionStrings": {
       "DefaultConnection": "your-azure-sql-connection-string-here"
     }
   }
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

4. **Access the Chat Interface**:
   Navigate to `https://localhost:7XXX` (port will be displayed in console)

## 💬 Usage

### Chat Commands

The AI assistant can help with various tasks:

**Microsoft Learn Queries**:
- "Find Azure documentation"
- "Show me .NET tutorials"
- "Microsoft Learn courses on Blazor"

**Database Queries**:
- "Show me all customers"
- "Count total orders"
- "Find customers from Seattle"
- "Average order amount by city"

**General Assistance**:
- Ask any general questions and the system will route to the most appropriate agent

### Agent Selection

The system automatically selects the best agent based on your query:
- Keywords like "learn", "documentation", "tutorial" → MSLearn MCP Agent
- Keywords like "data", "customers", "orders", "count" → Azure SQL NL2SQL Agent

## 🔧 Development

### Project Structure

```
DataAICapstone/
├── Agents/                 # AI agent implementations
│   ├── IAgent.cs          # Agent interface and orchestrator
│   ├── AgentOrchestrator.cs
│   ├── MSLearnMCPAgent.cs # Microsoft Learn agent
│   └── AzureSqlNL2SQLAgent.cs # SQL agent
├── Components/            # Blazor components
│   ├── Pages/
│   │   └── Chat.razor     # Main chat interface
│   └── Layout/            # Layout components
├── Models/                # Data models
│   └── ChatModels.cs      # Chat and agent models
├── Services/              # Business services
│   └── ChatService.cs     # Chat management service
└── Program.cs             # Application startup
```

### Adding New Agents

1. Implement the `IAgent` interface
2. Register the agent in `Program.cs`
3. The orchestrator will automatically include it in agent selection

### Security Features

- SQL injection protection (only SELECT queries allowed)
- Input validation and sanitization
- Safe query execution with timeouts
- API key configuration management

## 🚀 Deployment

The application can be deployed to:
- Azure App Service
- IIS
- Docker containers
- Any hosting platform supporting .NET 9.0

## 📝 Notes

- The application includes sample data when database connections are not configured
- OpenAI integration is optional - basic agent routing works without it
- All database queries are read-only for security
- The MCP integration currently uses simulated responses (can be extended with real MCP server)

## 🤝 Contributing

This is a capstone project demonstrating advanced .NET development with AI integration. Feel free to extend the agent capabilities or add new specialized agents.
