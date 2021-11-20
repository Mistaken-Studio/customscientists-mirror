// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManagerKeycard.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs;
using Mistaken.API;
using Mistaken.API.CustomItems;
using UnityEngine;

namespace Mistaken.CustomScientists.Items
{
    /// <inheritdoc/>
    public class DeputyFacalityManagerKeycard : MistakenCustomItem
    {
        /// <inheritdoc/>
        public override MistakenCustomItems CustomItem => MistakenCustomItems.DEPUTY_FACILITY_MANAGER_KEYCARD;

        /// <inheritdoc/>
        public override ItemType Type { get; set; } = ItemType.KeycardFacilityManager;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Karta Zastępcy Dyrektora Placówki";

        /// <inheritdoc/>
        public override string Description { get; set; } = "well";

        /// <inheritdoc/>
        public override string DisplayName { get; set; } = "<color=#bd1a47>karta Zastępcy Dyrektora Placówki</color>";

        /// <inheritdoc/>
        public override float Weight { get; set; } = 0.5f;

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

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
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
        }

        private void Player_InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (MapPlus.IsLCZDecontaminated())
                return;
            var type = ev.Door.Type;
            if (type == DoorType.GateA || type == DoorType.GateB)
            {
                foreach (var player in RealPlayers.List.Where(p => p.Id != ev.Player.Id && p.Role == RoleType.Scientist))
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
