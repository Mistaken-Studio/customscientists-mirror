// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManagerKeycard.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using Mistaken.API;
using Mistaken.API.CustomItems;
using Mistaken.API.Extensions;
using UnityEngine;

namespace Mistaken.CustomScientists.Items
{
    /// <inheritdoc/>
    [CustomItem(ItemType.KeycardFacilityManager)]
    public sealed class DeputyFacalityManagerKeycard : MistakenCustomKeycard
    {
        /// <summary>
        /// Gets the deputy facality manager keycard instance.
        /// </summary>
        public static DeputyFacalityManagerKeycard Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.DEPUTY_FACILITY_MANAGER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardFacilityManager;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.DeputyFacilityManagerKeycard;

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#bd1a47>{this.Name}</color>";

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.DeputyFacilityManagerKeycardDescription;

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

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
        }

        private void Player_InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (Map.IsLczDecontaminated)
                return;

            if (!this.Check(ev.Player.CurrentItem))
            {
                if (!ev.Player.TryGetSessionVariable<ItemPickupBase>(SessionVarType.THROWN_ITEM, out var item))
                    return;

                if (item is not KeycardPickup keycard)
                    return;

                if (!this.Check(Pickup.Get(keycard)))
                    return;
            }

            var type = ev.Door.Type;
            if (type == DoorType.GateA || type == DoorType.GateB)
            {
                foreach (var player in RealPlayers.List.Where(p => p.Id != ev.Player.Id && p.Role.Type == RoleType.Scientist))
                {
                    if (Vector3.Distance(player.Position, ev.Player.Position) < 10)
                    {
                        ev.IsAllowed = true;
                        return;
                    }
                }

                ev.IsAllowed = false;
                return;
            }
            else if (type == DoorType.NukeSurface || type == DoorType.Scp106Primary || type == DoorType.Scp106Secondary)
            {
                ev.IsAllowed = false;
                return;
            }
        }
    }
}
