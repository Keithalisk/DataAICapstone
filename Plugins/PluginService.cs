using Microsoft.SemanticKernel;

namespace DataAICapstone.Plugins
{
    public interface IPluginService
    {
        IEnumerable<string> GetAvailablePlugins();
        string GetPluginDescription(string pluginName);
        IEnumerable<string> GetPluginFunctions(string pluginName);
    }

    public class PluginService : IPluginService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<PluginService> _logger;

        public PluginService(Kernel kernel, ILogger<PluginService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public IEnumerable<string> GetAvailablePlugins()
        {
            try
            {
                return _kernel.Plugins.Select(p => p.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available plugins");
                return Enumerable.Empty<string>();
            }
        }

        public string GetPluginDescription(string pluginName)
        {
            try
            {
                var plugin = _kernel.Plugins.FirstOrDefault(p => p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
                return plugin?.Description ?? "No description available";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plugin description for {PluginName}", pluginName);
                return "Error retrieving description";
            }
        }

        public IEnumerable<string> GetPluginFunctions(string pluginName)
        {
            try
            {
                var plugin = _kernel.Plugins.FirstOrDefault(p => p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
                return plugin?.Select(f => f.Name) ?? Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plugin functions for {PluginName}", pluginName);
                return Enumerable.Empty<string>();
            }
        }
    }
}