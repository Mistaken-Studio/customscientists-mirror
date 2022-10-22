// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Toys;
using MEC;
using Mirror;
using Mistaken.API;
using Mistaken.API.Components;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.CustomScientists.Classes
{
    /// <inheritdoc/>
    [CustomRole(RoleType.Scientist)]
    public sealed class DeputyFacalityManager : MistakenCustomRole
    {
        /// <summary>
        /// Gets the Deputy Facility Manager instance.
        /// </summary>
        public static DeputyFacalityManager Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.DEPUTY_FACILITY_MANAGER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.Scientist;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = PluginHandler.Instance.Translation.DeputyFacilityManager;

        /// <inheritdoc/>
        public override string Description { get; set; } = PluginHandler.Instance.Translation.DeputyFacilityManagerDescription;

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
            ItemType.Adrenaline.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorLight.ToString(),
            ((int)API.CustomItems.MistakenCustomItems.DEPUTY_FACILITY_MANAGER_KEYCARD).ToString(),
            ((int)API.CustomItems.MistakenCustomItems.SNAV_ULTIMATE).ToString(),
        };

        /// <inheritdoc/>
        public override string DisplayName => $"<color=#bd1a47>{this.Name}</color>";

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();
            Instance = this;
        }

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);
            if(PluginHandler.CustomHierarchyIntegrationEnabled)
                CustomHierarchyIntegration.UpdateHierarchy(player);
            if (escapeLock?.Base == null)
            {
                UnityEngine.Debug.LogError("Tried to spawn null object for DeputyFacilityManager");
                return;
            }

            Server.SendSpawnMessage.Invoke(null, new object[] { escapeLock.Base.netIdentity, player.Connection, });
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.Escaping += this.Player_Escaping;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating += this.Map_Decontaminating;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.Escaping -= this.Player_Escaping;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating -= this.Map_Decontaminating;
        }

        private static Primitive escapeLock;

        private void Server_WaitingForPlayers()
        {
            if (escapeLock?.Base != null)
                return;

            escapeLock = Primitive.Create(new Vector3(170.15f, 986f, 20f), new Vector3(0f, 0f, 90f), new Vector3(6f, 4f, 1f), false);
            escapeLock.Type = PrimitiveType.Quad;
            escapeLock.Color = new Color(255f, 255f, 255f, 53f);

            void OnEnter(Player player)
            {
                if (!this.Check(player))
                    return;

                if (Map.IsLczDecontaminated)
                    return;

                player.SetGUI("DeputyFacilityManager_InformEscape", PseudoGUIPosition.MIDDLE, "<size=200%>Nie możesz uciec przed dekontaminacją LCZ</size>", 5f);
            }

            InRange.Spawn(new Vector3(170.15f, 987f, 18f), new Vector3(4f, 6f, 4f), OnEnter);
        }

        private void Player_Escaping(Exiled.Events.EventArgs.EscapingEventArgs ev)
        {
            if (!this.Check(ev.Player))
                return;

            if (Map.IsLczDecontaminated)
                return;

            ev.Player.SetGUI("DeputyFacilityManager_InformEscape", PseudoGUIPosition.MIDDLE, "<size=200%>Nie możesz uciec przed dekontaminacją LCZ</size>", 5f);
            ev.IsAllowed = false;
        }

        private void Map_Decontaminating(Exiled.Events.EventArgs.DecontaminatingEventArgs ev)
        {
            if (escapeLock?.Base == null)
            {
                UnityEngine.Debug.LogError("Tried to remove null object for DeputyFacilityManagers");
                return;
            }

            foreach (var player in this.TrackedPlayers)
            {
                if (!player.IsConnected())
                    continue;

                player.Connection.Send(new ObjectDestroyMessage() { netId = escapeLock.Base.netId });
                player.SetGUI("DeputyFacilityManager_InformDecontamination", PseudoGUIPosition.TOP, "<size=150%>Zostały tobie nadane dodatkowe uprawnienia</size>", 10f);
            }
        }

        private void Server_RoundStarted()
        {
            Timing.CallDelayed(1.3f, () =>
            {
                var scientists = RealPlayers.Get(RoleType.Scientist).ToArray();
                if (scientists.Length < 3)
                    return;

                scientists = scientists.Where(x => !Registered.Any(y => y.Check(x))).ToArray();
                this.AddRole(scientists[UnityEngine.Random.Range(0, scientists.Length)]);
            });
        }
    }
}
