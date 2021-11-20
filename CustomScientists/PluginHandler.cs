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
        public override string Name => "";

        /// <inheritdoc/>
        public override string Prefix => "M";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Higher;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 11, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            // new Handler(this);

            API.Diagnostics.Module.OnEnable(this);
            Events.Handlers.CustomEvents.LoadedPlugins += this.CustomEvents_LoadedPlugins;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            API.Diagnostics.Module.OnDisable(this);
            Events.Handlers.CustomEvents.LoadedPlugins -= this.CustomEvents_LoadedPlugins;
            base.OnDisabled();
        }

        /// <summary>
        /// Gets a value indicating whether Custom Hierarchy plugin is available.
        /// </summary>
        internal static bool CustomHierarchyAvailable { get; private set; } = false;

        internal static PluginHandler Instance { get; private set; }

        private void CustomEvents_LoadedPlugins()
        {
            if (Exiled.Loader.Loader.Plugins.Any(x => x.Name == "CustomHierarchy"))
            {
                PluginHandler.CustomHierarchyAvailable = true;
                Log.Info("Enabling additional features :)");
#pragma warning disable SA1118 // Parameter should not span multiple lines
                Mistaken.CustomHierarchii.HierarchyHandler.CustomPlayerComperers.Add(
                    "dfm_comparer",
                    (1, (Player p1, Player p2) =>
                            {
                                if (p1.Role != RoleType.Scientist && p2.Role != RoleType.Scientist)
                                    return CustomHierarchii.HierarchyHandler.CompareResult.NO_ACTION;
                                var p1c = API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).Check(p1);
                                var p2c = API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).Check(p2);
                                if (p1c && p2c)
                                    return CustomHierarchii.HierarchyHandler.CompareResult.SAME_RANK;
                                else if (p1c)
                                {
                                    if (p2.Role == RoleType.Scientist)
                                        return CustomHierarchii.HierarchyHandler.CompareResult.GIVE_ORDERS;
                                    else if (Mistaken.API.MapPlus.IsLCZDecontaminated() && p2.Team == Team.MTF)
                                        return CustomHierarchii.HierarchyHandler.CompareResult.GIVE_ORDERS;
                                    else
                                        return CustomHierarchii.HierarchyHandler.CompareResult.NO_ACTION;
                                }
                                else if (p2c)
                                {
                                    if (p1.Role == RoleType.Scientist)
                                        return CustomHierarchii.HierarchyHandler.CompareResult.FOLLOW_ORDERS;
                                    else if (Mistaken.API.MapPlus.IsLCZDecontaminated() && p1.Team == Team.MTF)
                                        return CustomHierarchii.HierarchyHandler.CompareResult.FOLLOW_ORDERS;
                                    else
                                        return CustomHierarchii.HierarchyHandler.CompareResult.NO_ACTION;
                                }
                                else
                                {
                                    return CustomHierarchii.HierarchyHandler.CompareResult.NO_ACTION;
                                }
                            }));
#pragma warning restore SA1118 // Parameter should not span multiple lines
            }
        }
    }
}
