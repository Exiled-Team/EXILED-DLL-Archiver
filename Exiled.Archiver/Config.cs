#pragma warning disable CS8600
#pragma warning disable CS8604


namespace Exiled.Archiver
{
    /// <summary>
    /// Represents the configuration settings for the application.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets the paths configuration.
        /// </summary>
        public Paths Paths { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin names for EXILED plugins.
        /// </summary>
        public List<string> Plugins { get; set; } = ["Exiled.CreditTags", "Exiled.CustomModules", "Exiled.Events", "Exiled.Permissions"];

        /// <summary>
        /// Gets or sets the list of dependency names for EXILED plugins.
        /// </summary>
        public List<string> PluginsDependencies { get; set; } = ["0Harmony", "System.ComponentModel.DataAnnotations", "Newtonsoft.Json"];

        /// <summary>
        /// Gets or sets the list of dependency names for NW plugins.
        /// </summary>
        public List<string> NWDependencies { get; set; } = ["Exiled.API", "SemanticVersioning", "Mono.Posix", "YamlDotNet"];
    }
}
