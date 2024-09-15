#pragma warning disable CS8600
#pragma warning disable CS8604
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Exiled.Archiver
{
    /// <summary>
    /// Contains paths configuration for plugin directories.
    /// </summary>
    public class Paths
    {
        /// <summary>
        /// Gets or sets the path to the EXILED plugins directory.
        /// </summary>
        public string ExiledPluginsPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the EXILED plugins dependencies directory.
        /// </summary>
        public string ExiledPluginsDepsPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the NW plugins directory.
        /// </summary>
        public string NWPluginPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the NW plugins dependencies directory.
        /// </summary>
        public string NWPluginDepsPath { get; set; }
    }
}
