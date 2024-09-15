namespace Exiled.Archiver;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

public class Program
{
    private const string CONFIG_FILE = "config.json";
    private static Config config;
    private static List<string> missingFiles = [];
    private static string BaseDirectory => Path.GetDirectoryName(Environment.ProcessPath);

    public static void Main(string[] args)
    {
        LoadConfig();

        try
        {
            Directory.CreateDirectory(config.Paths.ExiledPluginsDepsPath);
            Directory.CreateDirectory(config.Paths.NWPluginDepsPath);

            ProcessFiles(config.Plugins, BaseDirectory, config.Paths.ExiledPluginsPath);
            ProcessFiles(config.PluginsDependencies, BaseDirectory, config.Paths.ExiledPluginsDepsPath);
            ProcessFiles(config.NWDependencies, BaseDirectory, config.Paths.NWPluginDepsPath);

            string exiledLoaderPath = Path.Combine(BaseDirectory, "Exiled.Loader.dll");
            string nwPluginPath = Path.Combine(BaseDirectory, config.Paths.NWPluginPath, "Exiled.Loader.dll");

            if (File.Exists(exiledLoaderPath))
            {
                File.Copy(exiledLoaderPath, nwPluginPath, true);
            }
            else
            {
                missingFiles.Add(exiledLoaderPath);
            }

            CreateTarGZ(Path.Combine(BaseDirectory, "Exiled.tar.gz"), BaseDirectory);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
        finally
        {
            if (missingFiles.Count > 0)
            {
                foreach (string missingFile in missingFiles)
                    Console.WriteLine($"Missing file: {Path.GetFileName(missingFile)}");
                
                Console.WriteLine();
            }
            
            Console.WriteLine("The archive has been generated.");
            Console.ReadLine();
        }
    }

    private static void ProcessFiles(IEnumerable<string> files, string sourcePath, string destPath)
    {
        foreach (string str in files)
        {
            string fileName = Path.Combine(sourcePath, str + ".dll");
            string destFile = Path.Combine(destPath, str + ".dll");

            Directory.CreateDirectory(sourcePath);
            Directory.CreateDirectory(destPath);
            
            try
            {
                if (File.Exists(fileName))
                {
                    File.Copy(fileName, destFile, true);
                }
                else
                {
                    missingFiles.Add(fileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file {fileName}: {ex.Message}");
            }
        }
    }

    private static void CreateTarGZ(string tgzFilename, string sourceDirectory)
    {
        Stream outStream = File.Create(tgzFilename);
        Stream gzoStream = new GZipOutputStream(outStream);
        TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

        tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
        if (tarArchive.RootPath.EndsWith("/"))
            tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

        AddDirectoryFilesToTar(tarArchive, Path.Combine(sourceDirectory, "EXILED"), true);
        AddDirectoryFilesToTar(tarArchive, Path.Combine(sourceDirectory, "SCP Secret Laboratory"), true);

        tarArchive.Close();
    }
    
    private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
    {
        TarEntry tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
        tarArchive.WriteEntry(tarEntry, false);

        string[] filenames = Directory.GetFiles(sourceDirectory);
        foreach (string filename in filenames)
        {
            tarEntry = TarEntry.CreateEntryFromFile(filename);
            tarArchive.WriteEntry(tarEntry, true);
        }

        if (recurse)
        {
            string[] directories = Directory.GetDirectories(sourceDirectory);
            foreach (string directory in directories)
                AddDirectoryFilesToTar(tarArchive, directory, recurse);
        }
    }
    
    private static void LoadConfig()
    {
        try
        {
            if (!File.Exists(CONFIG_FILE))
            {
                Console.WriteLine("Config file not found, using default settings.");
                SetDefaultConfig();
                return;
            }

            string json = File.ReadAllText(CONFIG_FILE);
            config = JsonSerializer.Deserialize<Config>(json);

            if (config is null)
            {
                Console.WriteLine("Config file is empty or invalid, using default settings.");
                SetDefaultConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load config: {ex.Message}. Using default settings.");
            SetDefaultConfig();
        }
    }
    
    private static void SetDefaultConfig()
    {
        config = new Config
        {
            Paths = new Paths
            {
                ExiledPluginsDepsPath = Path.Combine(BaseDirectory, "EXILED", "Plugins", "dependencies"),
                NWPluginDepsPath = Path.Combine(BaseDirectory, "SCP Secret Laboratory", "PluginAPI", "plugins", "global", "dependencies"),
                ExiledPluginsPath = Path.Combine(BaseDirectory, "EXILED", "Plugins"),
                NWPluginPath = Path.Combine(BaseDirectory, "SCP Secret Laboratory", "PluginAPI", "plugins", "global"),
            },
        };

        try
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(BaseDirectory, CONFIG_FILE), json);
            Console.WriteLine("Config saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save config: {ex.Message}");
        }
    }
}
