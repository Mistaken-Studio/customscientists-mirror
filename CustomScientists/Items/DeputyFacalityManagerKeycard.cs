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
        public override SpawnProperties SpawnProperties { get; set; } = new();

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
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += this.Player_ActivatingWarheadPanel;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= this.Player_ActivatingWarheadPanel;
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
                bool isScientistClose = RealPlayers.List.Any((x) =>
                {
                    return x.Id != ev.Player.Id && x.Role.Type == RoleType.Scientist && Vector3.Distance(x.Position, ev.Player.Position) < 10f;
                });

                ev.IsAllowed = isScientistClose;
            }
            else if (type == DoorType.Scp106Primary || type == DoorType.Scp106Secondary)
            {
                ev.IsAllowed = false;
            }
        }

        private void Player_ActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (Map.IsLczDecontaminated)
                return;

            if (!this.Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;
        }
    }
}
