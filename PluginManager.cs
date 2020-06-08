using BIF.SWE1.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace myWebServer
{
    public class PluginManager : IPluginManager
    {

        private List<IPlugin> plugins;

        /// <summary>
        /// Returns a list of all plugins. Never returns null.
        /// </summary>
        public IEnumerable<IPlugin> Plugins
        {
            get { return plugins; }
        }

        private static PluginManager instance;

        ///<summary>
        /// Plugin Manager constructor. Initialize plugin List
        ///</summary>
        private PluginManager()
        {
            plugins = new List<IPlugin>();
        }

        ///<summary>
        /// Creates or returns singleton instance
        ///</summary>
        public static PluginManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PluginManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Adds a new plugin. If the plugin was already added, nothing will happen.
        /// </summary>
        /// <param name="plugin"></param>
        public void Add(IPlugin plugin)
        {
            if (!plugins.Contains(plugin))
                plugins.Add(plugin);
        }

        /// <summary>
        /// Adds a new plugin by type name. If the plugin was already added, nothing will happen.
        /// Throws an exeption, when the type cannot be resoled or the type does not implement IPlugin.
        /// </summary>
        /// <param name="plugin"></param>
        public void Add(string plugin)
        {
            try
            {
                Assembly DLLPlugin = Assembly.LoadFile(plugin);
                foreach (Type type in DLLPlugin.GetExportedTypes())
                {
                    Console.WriteLine(type.FullName);
                    IPlugin lPlugin = (IPlugin)Activator.CreateInstance(type);
                    plugins.Add(lPlugin);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// Clears all plugins
        /// </summary>
        public void Clear()
        {
            plugins.Clear();
        }

    }
}