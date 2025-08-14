# AI Model Endpoint Configuration Examples

This file shows examples of how to configure different AI model endpoints in your `appsettings.Development.json` file.

## 1. Standard OpenAI (Default)

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key-here",
    "Model": "gpt-4o-mini",
    "Endpoint": "",
    "Provider": "OpenAI"
  }
}
```

## 2. Azure OpenAI

```json
{
  "OpenAI": {
    "ApiKey": "your-azure-openai-key-here",
    "Model": "gpt-4o-mini",
    "Endpoint": "https://your-resource-name.openai.azure.com",
    "Provider": "AzureOpenAI"
  }
}
```

## 3. OpenAI-Compatible Endpoint (e.g., local or other providers)

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "Model": "gpt-4o-mini",
    "Endpoint": "https://your-custom-endpoint.com/v1",
    "Provider": "OpenAI"
  }
}
```

## 4. Local LLM (e.g., Ollama, LM Studio)

```json
{
  "OpenAI": {
    "ApiKey": "not-needed-for-local",
    "Model": "llama3.1",
    "Endpoint": "http://localhost:11434/v1",
    "Provider": "OpenAI"
  }
}
```

## Available Models by Provider

### OpenAI Models
- `gpt-4o-mini` (recommended - fast and cost-effective)
- `gpt-4o` (most capable)
- `gpt-4-turbo`
- `gpt-3.5-turbo`

### Azure OpenAI Models
Use the deployment name you configured in Azure:
- `gpt-4o-mini` (your deployment name)
- `gpt-4o` (your deployment name)
- `gpt-35-turbo` (your deployment name)

### Local Models (depends on what you have installed)
- `llama3.1`
- `codellama`
- `mistral`
- `phi3`

## Configuration Notes

- **Provider**: Set to "AzureOpenAI" for Azure, "OpenAI" for standard OpenAI or compatible endpoints
- **Endpoint**: Leave empty for standard OpenAI, provide full URL for others
- **Model**: For Azure OpenAI, use your deployment name, not the base model name
- **ApiKey**: Required for OpenAI and Azure OpenAI, may not be needed for local models

## Security Best Practices

1. Never commit API keys to source control
2. Use `appsettings.Development.json` for local development
3. Use environment variables or Azure Key Vault for production
4. Consider using managed identity for Azure deployments
