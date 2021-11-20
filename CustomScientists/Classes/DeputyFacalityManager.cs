// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mistaken.API.CustomRoles;

namespace Mistaken.CustomScientists.Classes
{
    /// <inheritdoc/>
    public class DeputyFacalityManager : Mistaken.API.CustomRoles.MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.DEPUTY_FACILITY_MANAGER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.Scientist;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Zastępca Dyrektora Placówki";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Twoim zadaniem jest pomoc ochronie w odeskortowaniu <color=yellow>naukowców</color><br>Nie możesz uciec przed dekontaminacją LCZ";

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);
            player.InfoArea = ~PlayerInfoArea.Role;

        }

        /// <inheritdoc/>
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override bool RemovalKillsPlayer { get; set; } = true;

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.Adrenaline.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorLight.ToString(),
            API.CustomItems.MistakenCustomItem.Get(API.CustomItems.MistakenCustomItems.DEPUTY_FACILITY_MANAGER_KEYCARD).Name,
        };
    }
}
