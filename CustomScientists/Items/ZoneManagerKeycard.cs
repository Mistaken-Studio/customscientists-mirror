// -----------------------------------------------------------------------
// <copyright file="ZoneManagerKeycard.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Mistaken.API.CustomItems;

namespace Mistaken.CustomScientists.Items
{
    /// <inheritdoc/>
    [CustomItem(ItemType.KeycardZoneManager)]
    public sealed class ZoneManagerKeycard : MistakenCustomKeycard
    {
        /// <summary>
        /// Gets the zone manager keycard instance.
        /// </summary>
        public static ZoneManagerKeycard Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.ZONE_MANAGER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardZoneManager;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.ZoneManagerKeycard;

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#217a7b>{this.Name}</color>";

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.ZoneManagerKeycardDescription;

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new();

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }
    }
}
