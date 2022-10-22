// -----------------------------------------------------------------------
// <copyright file="ZoneManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Mistaken.API;
using Mistaken.API.CustomRoles;
using UnityEngine;

namespace Mistaken.CustomScientists.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.Scientist)]
    public sealed class ZoneManager : MistakenCustomRole
    {
        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.ZONE_MANAGER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.Scientist;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.ZoneManager;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.ZoneManagerDescription;

        /// <inheritdoc/>
        public override string CustomInfo { get; set; }

        /// <inheritdoc/>
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        public override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Medkit.ToString(),
            ((int)API.CustomItems.MistakenCustomItems.ZONE_MANAGER_KEYCARD).ToString(),
            ((int)API.CustomItems.MistakenCustomItems.SNAV_3000).ToString(),
        };

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#217a7b>{this.Name}</color>";

        /// <inheritdoc/>
        public override void Init()
        {
            Instance = this;
            base.Init();
        }

        internal static ZoneManager Instance { get; set; }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
        }

        /// <inheritdoc/>
        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            MEC.Timing.CallDelayed(1f, () =>
            {
                player.Position = API.Utilities.Room.Get(Room.List.First(x => x.Type == RoomType.LczClassDSpawn)).Neighbors[0].ExiledRoom.Position + (Vector3.up * 2f);
                if (PluginHandler.CustomHierarchyIntegrationEnabled)
                    CustomHierarchyIntegration.UpdateHierarchy(player);
            });
        }

        private void Server_RoundStarted()
        {
            MEC.Timing.CallDelayed(1.2f, () =>
            {
                var scientists = RealPlayers.Get(RoleType.Scientist).ToArray();
                if (scientists.Length < 2)
                    return;

                scientists = scientists.Where(x => !Registered.Any(y => y.Check(x))).ToArray();
                this.AddRole(scientists[UnityEngine.Random.Range(0, scientists.Length)]);
            });
        }
    }
}
