// -----------------------------------------------------------------------
// <copyright file="ZoneManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using Mistaken.API;
using Mistaken.API.CustomRoles;
using UnityEngine;

namespace Mistaken.CustomScientists.Classes
{
    /// <inheritdoc/>
    public class ZoneManager : MistakenCustomRole
    {
        /// <summary>
        /// Gets the zone manager instance.
        /// </summary>
        public static ZoneManager Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.ZONE_MANAGER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.Scientist;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Zone Manager";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Twoim zadaniem jest ucieczka z placówki";

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        /// <inheritdoc/>
        protected override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc/>
        protected override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc/>
        protected override bool RemovalKillsPlayer { get; set; } = false;

        /// <inheritdoc/>
        protected override string DisplayName => "<color=#217a7b>Zarządca Strefy Podwyższonego Ryzyka</color>";

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.Medkit.ToString(),

            // ItemType.Radio.ToString(),
            ((int)API.CustomItems.MistakenCustomItems.ZONE_MANAGER_KEYCARD).ToString(),
            ((int)API.CustomItems.MistakenCustomItems.SNAV_3000).ToString(),
        };

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
        }

        /// <inheritdoc/>
        protected override void UnSubscribeEvents()
        {
            base.UnSubscribeEvents();
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
        }

        private void Server_RoundStarted()
        {
            MEC.Timing.CallDelayed(1.2f, () =>
            {
                var scientists = RealPlayers.Get(RoleType.Scientist).ToList();
                if (scientists.Count < 2)
                    return;

                scientists = scientists.Where(x => !DeputyFacalityManager.Instance.Check(x)).ToList();
                ZoneManager.Instance.AddRole(scientists[UnityEngine.Random.Range(0, scientists.Count)]);
            });
        }

        private void Player_ChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason == SpawnReason.Escaped)
            {
                if (ZoneManager.Instance.Check(ev.Player))
                {
                    if (ev.NewRole == RoleType.NtfSpecialist)
                    {
                        ev.NewRole = RoleType.NtfPrivate;
                        if (ev.Items.Contains(ItemType.KeycardNTFOfficer))
                        {
                            ev.Items.Remove(ItemType.KeycardNTFOfficer);
                            ev.Items.Add(ItemType.KeycardNTFLieutenant);
                        }
                    }
                    else
                    {
                        ev.Items.Add(ItemType.GunCrossvec);
                        ev.Items.Add(ItemType.SCP207);
                        MEC.Timing.CallDelayed(1, () =>
                        {
                            ev.Player.Ammo[ItemType.Ammo9x19] += 100;
                        });
                    }
                }
            }
        }

        protected override void RoleAdded(Player player)
        {
            base.RoleAdded(player);
            MEC.Timing.CallDelayed(1.5f, () =>
            {
                player.Position = Map.Rooms.Where(x => x.Type == RoomType.HczChkpA || x.Type == RoomType.HczChkpB).First().Position + (Vector3.up * 1.5f);
            });
        }
    }
}
