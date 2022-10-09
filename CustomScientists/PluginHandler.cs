// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.Updater.API.Config;

namespace Mistaken.CustomScientists
{
    internal sealed class PluginHandler : Plugin<Config, Translations>, IAutoUpdateablePlugin
    {
        public override string Author => "Mistaken Devs";

        public override string Name => "CustomScientists";

        public override string Prefix => "MCScientists";

        public override PluginPriority Priority => PluginPriority.Default;

        public override Version RequiredExiledVersion => new(5, 2, 2);

        public AutoUpdateConfig AutoUpdateConfig => new()
        {
            Type = this.Config.SourceType,
            Url = this.Config.Url,
        };

        public override void OnEnabled()
        {
            Instance = this;
            Events.Handlers.CustomEvents.LoadedPlugins += this.CustomEvents_LoadedPlugins;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Events.Handlers.CustomEvents.LoadedPlugins -= this.CustomEvents_LoadedPlugins;
            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }

        private void CustomEvents_LoadedPlugins()
        {
            if (Exiled.Loader.Loader.Plugins.Any(x => x.Name == "CustomHierarchy"))
                CustomHierarchyIntegration.EnableCustomHierarchyIntegration();
        }
    }
}
