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
    public class ZoneManagerKeycard : MistakenCustomKeycard
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
        public override string Name { get; set; } = "Karta Zarządcy Strefy";

        /// <inheritdoc/>
        public override string DisplayName => "<color=#217a7b>Karta Zarządcy Strefy</color>";

        /// <inheritdoc/>
        public override string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }
    }
}
