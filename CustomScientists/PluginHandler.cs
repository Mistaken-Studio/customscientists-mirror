// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace Mistaken.CustomScientists
{
    /// <inheritdoc/>
    public class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "CustomScientists";

        /// <inheritdoc/>
        public override string Prefix => "MCScientists";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Default;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(3, 7, 2);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            new Items.DeputyFacalityManagerKeycard().TryRegister();
            new Items.ZoneManagerKeycard().TryRegister();
            new Classes.DeputyFacalityManager().TryRegister();
            new Classes.ZoneManager().TryRegister();
            Events.Handlers.CustomEvents.LoadedPlugins += this.CustomEvents_LoadedPlugins;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Items.DeputyFacalityManagerKeycard.Instance.TryUnregister();
            Items.ZoneManagerKeycard.Instance.TryUnregister();
            Classes.DeputyFacalityManager.Instance.TryUnregister();
            Classes.ZoneManager.Instance.TryUnregister();
            Events.Handlers.CustomEvents.LoadedPlugins -= this.CustomEvents_LoadedPlugins;
            base.OnDisabled();
        }

        /// <summary>
        /// Gets or sets a value indicating whether Custom Hierarchy plugin is available.
        /// </summary>
        internal static bool CustomHierarchyAvailable { get; set; } = false;

        internal static PluginHandler Instance { get; private set; }

        private void CustomEvents_LoadedPlugins()
        {
            if (Exiled.Loader.Loader.Plugins.Any(x => x.Name == "CustomHierarchy"))
                CustomHierarchyIntegration.EnableCustomHierarchyIntegration();
        }
    }
}
