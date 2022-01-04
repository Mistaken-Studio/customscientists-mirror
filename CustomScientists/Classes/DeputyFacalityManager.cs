// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Mistaken.API;
using Mistaken.API.CustomRoles;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.CustomScientists.Classes
{
    /// <inheritdoc/>
    public class DeputyFacalityManager : MistakenCustomRole
    {
        /// <summary>
        /// Gets the deputy facility manager instance.
        /// </summary>
        public static DeputyFacalityManager Instance { get; private set; }

        /// <inheritdoc/>
        public override MistakenCustomRoles CustomRole => MistakenCustomRoles.DEPUTY_FACILITY_MANAGER;

        /// <inheritdoc/>
        public override RoleType Role { get; set; } = RoleType.Scientist;

        /// <inheritdoc/>
        public override int MaxHealth { get; set; } = 100;

        /// <inheritdoc/>
        public override string Name { get; set; } = "Deputy Facility Manager";

        /// <inheritdoc/>
        public override string Description { get; set; } = "Twoim zadaniem jest pomoc w ochronie i odeskortowaniu <color=yellow>naukowców</color><br>. Nie możesz uciec przed dekontaminacją LCZ";

        /// <inheritdoc/>
        public override void AddRole(Player player)
        {
            base.AddRole(player);

            if (this.EscapeLock != null)
            {
                MethodInfo sendSpawnMessage = Server.SendSpawnMessage;
                if (sendSpawnMessage != null)
                {
                    if (player.Connection == null)
                        return;
                    sendSpawnMessage.Invoke(null, new object[]
                    {
                        this.EscapeLock.netIdentity,
                        player.Connection,
                    });
                }
            }
        }

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
        protected override string DisplayName => "<color=#bd1a47>Zastępca Dyrektora Placówki</color>";

        /// <inheritdoc/>
        protected override List<string> Inventory { get; set; } = new List<string>()
        {
            ItemType.Adrenaline.ToString(),
            ItemType.Medkit.ToString(),

            // ItemType.Radio.ToString(),
            ItemType.ArmorLight.ToString(),
            ((int)API.CustomItems.MistakenCustomItems.DEPUTY_FACILITY_MANAGER_KEYCARD).ToString(),
            ((int)API.CustomItems.MistakenCustomItems.SNAV_ULTIMATE).ToString(),
        };

        /// <inheritdoc/>
        protected override void UnSubscribeEvents()
        {
            base.UnSubscribeEvents();
            Exiled.Events.Handlers.Player.Escaping -= this.Player_Escaping;
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Map.Decontaminating -= this.Map_Decontaminating;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.Escaping += this.Player_Escaping;
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Map.Decontaminating += this.Map_Decontaminating;
        }

        private static MethodInfo RemoveFromVisList { get; set; } = null;

        private DoorVariant EscapeLock { get; set; }

        private void Player_Escaping(Exiled.Events.EventArgs.EscapingEventArgs ev)
        {
            if (this.Check(ev.Player))
            {
                if (!Map.IsLczDecontaminated)
                {
                    ev.Player.SetGUI("cc_deputy_escape", PseudoGUIPosition.MIDDLE, "<size=200%>Nie możesz uciec przed dekontaminacją LCZ</size>", /*1 / 60f*/5f);
                    ev.IsAllowed = false;
                }
            }
        }

        private void Map_Decontaminating(Exiled.Events.EventArgs.DecontaminatingEventArgs ev)
        {
            if (this.EscapeLock != null)
            {
                foreach (var item in this.TrackedPlayers)
                {
                    if (!item.IsConnected)
                        continue;

                    ObjectDestroyMessage msg = new ObjectDestroyMessage
                    {
                        netId = this.EscapeLock.netIdentity.netId,
                    };

                    // NetworkServer.SendToClientOfPlayer<ObjectDestroyMessage>(item.ReferenceHub.networkIdentity, msg);
                    item.Connection.Send<ObjectDestroyMessage>(msg);
                    if (this.EscapeLock.netIdentity.observers?.ContainsKey(item.Connection.connectionId) ?? false)
                    {
                        this.EscapeLock.netIdentity.observers.Remove(item.Connection.connectionId);
                        if (RemoveFromVisList == null)
                            RemoveFromVisList = typeof(NetworkConnection).GetMethod("RemoveFromVisList", BindingFlags.NonPublic | BindingFlags.Instance);
                        RemoveFromVisList?.Invoke(item.Connection, new object[] { this.EscapeLock.netIdentity, true });
                    }
                }

                GameObject.Destroy(this.EscapeLock.gameObject);
                this.EscapeLock = null;
            }

            foreach (var player in this.TrackedPlayers)
                player.SetGUI("cc_deputy_decontamination", PseudoGUIPosition.TOP, "<size=150%>Zostały tobie nadane dodatkowe uprawnienia</size>", /*1 / 60f*/10f);
        }

        private void Server_WaitingForPlayers()
        {
            this.EscapeLock = UnityEngine.Object.Instantiate(DoorUtils.GetPrefab(DoorUtils.DoorType.HCZ_BREAKABLE), new Vector3(170, 984, 20), Quaternion.identity);
            GameObject.Destroy(this.EscapeLock.GetComponent<DoorEventOpenerExtension>());
            if (this.EscapeLock.TryGetComponent<Scp079Interactable>(out var scp079Interactable))
                GameObject.Destroy(scp079Interactable);
            this.EscapeLock.transform.localScale = new Vector3(1.7f, 1.5f, 1f);
            if (this.EscapeLock is BasicDoor door)
                door._portalCode = 1;
            this.EscapeLock.NetworkActiveLocks |= (ushort)DoorLockReason.AdminCommand;
            (this.EscapeLock as BreakableDoor)._brokenPrefab = null;
            this.EscapeLock.gameObject.SetActive(false);
        }

        private void Server_RoundStarted()
        {
            var scientists = RealPlayers.Get(RoleType.Scientist).ToList();
            if (scientists.Count < 4)
                return;

            scientists = scientists.Where(x => !ZoneManager.Instance.Check(x)).ToList();
            this.AddRole(scientists[UnityEngine.Random.Range(0, scientists.Count)]);
        }
    }
}
