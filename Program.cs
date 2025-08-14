using DataAICapstone.Components;
using DataAICapstone.Services;
using DataAICapstone.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Semantic Kernel services with plugins
builder.Services.AddSingleton<Kernel>(serviceProvider =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    
    // Only add OpenAI if API key is configured
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var openAiApiKey = configuration["OpenAI:ApiKey"];
    var openAiModel = configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
    var openAiEndpoint = configuration["OpenAI:Endpoint"];
    var provider = configuration["OpenAI:Provider"] ?? "OpenAI";
    
    if (!string.IsNullOrEmpty(openAiApiKey) && openAiApiKey != "your-openai-api-key-here" && openAiApiKey != "your-actual-openai-api-key-here")
    {
        if (provider.Equals("AzureOpenAI", StringComparison.OrdinalIgnoreCase))
        {
            // Azure OpenAI configuration - use the newer method signature
            if (!string.IsNullOrEmpty(openAiEndpoint))
            {
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: openAiModel,
                    endpoint: openAiEndpoint,
                    apiKey: openAiApiKey);
            }
            else
            {
                throw new InvalidOperationException("Azure OpenAI endpoint is required when using AzureOpenAI provider.");
            }
        }
        else
        {
            // Standard OpenAI configuration
            if (!string.IsNullOrEmpty(openAiEndpoint))
            {
                // Custom OpenAI-compatible endpoint
                kernelBuilder.AddOpenAIChatCompletion(openAiModel, openAiApiKey, httpClient: new HttpClient { BaseAddress = new Uri(openAiEndpoint) });
            }
            else
            {
                // Standard OpenAI
                kernelBuilder.AddOpenAIChatCompletion(openAiModel, openAiApiKey);
            }
        }
    }
    
    // Add plugins to kernel
    kernelBuilder.Plugins.AddFromType<MathPlugin>();
    kernelBuilder.Plugins.AddFromType<TextPlugin>();
    kernelBuilder.Plugins.AddFromType<TimePlugin>();
    
    // Add FilePlugin with configuration
    var filePlugin = new FilePlugin(configuration);
    kernelBuilder.Plugins.AddFromObject(filePlugin, "FilePlugin");
    
    // Add PostgreSQL Semantic Search Plugin with configuration and logger
    var logger = serviceProvider.GetRequiredService<ILogger<PostgreSQLSemanticSearchPlugin>>();
    var pgSearchPlugin = new PostgreSQLSemanticSearchPlugin(configuration, logger);
    kernelBuilder.Plugins.AddFromObject(pgSearchPlugin, "PostgreSQLSemanticSearch");
    
    return kernelBuilder.Build();
});

// Register chat service and plugin service
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPluginService, PluginService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
