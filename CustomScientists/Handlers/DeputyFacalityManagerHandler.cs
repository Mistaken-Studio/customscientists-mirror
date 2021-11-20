// -----------------------------------------------------------------------
// <copyright file="DeputyFacalityManagerHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Mistaken.API;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Mistaken.CustomScientists.Classes;
using UnityEngine;

namespace Mistaken.CustomScientists.Handlers
{
    internal class DeputyFacalityManagerHandler : Mistaken.API.Diagnostics.Module
    {
        public DeputyFacalityManagerHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => throw new NotImplementedException();

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.Escaping -= this.Player_Escaping;
        }

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Player.Escaping += this.Player_Escaping;
        }

        internal static DoorVariant EscapeLock { get; set; }

        private void Player_Escaping(EscapingEventArgs ev)
        {
            if (API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).Check(ev.Player))
            {
                if (!MapPlus.IsLCZDecontaminated())
                {
                    ev.Player.SetGUI("cc_deputy_escape", PseudoGUIPosition.MIDDLE, "<size=200%>Nie możesz uciec przed dekontaminacją LCZ</size>", 1 / 60f);
                    ev.IsAllowed = false;
                }
            }
        }

        private void Server_RoundStarted()
        {
            var scientists = RealPlayers.Get(RoleType.Scientist).ToList();
            if (scientists.Count < 4)
                return;
            scientists = scientists.Where(x => !API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.ZONE_MANAGER).Check(x)).ToList();
            EscapeLock = UnityEngine.Object.Instantiate(DoorUtils.GetPrefab(DoorUtils.DoorType.HCZ_BREAKABLE), new Vector3(170, 984, 20), Quaternion.identity);
            GameObject.Destroy(EscapeLock.GetComponent<DoorEventOpenerExtension>());
            if (EscapeLock.TryGetComponent<Scp079Interactable>(out var scp079Interactable))
                GameObject.Destroy(scp079Interactable);
            EscapeLock.transform.localScale = new Vector3(1.7f, 1.5f, 1f);
            if (EscapeLock is BasicDoor door)
                door._portalCode = 1;
            EscapeLock.NetworkActiveLocks |= (ushort)DoorLockReason.AdminCommand;
            (EscapeLock as BreakableDoor)._brokenPrefab = null;
            EscapeLock.gameObject.SetActive(false);
            API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).AddRole(scientists[UnityEngine.Random.Range(0, scientists.Count)]);
            this.CallDelayed(1, () =>
            {
                int rid = RoundPlus.RoundId;
                this.CallDelayed((60 * 12) - 10, () =>
                {
                    if (rid != RoundPlus.RoundId)
                        return;
                    foreach (var item in API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).TrackedPlayers)
                    {
                        if (!item.IsConnected)
                            continue;
                        ObjectDestroyMessage msg = new ObjectDestroyMessage
                        {
                            netId = DeputyFacalityManagerHandler.EscapeLock.netIdentity.netId,
                        };

                        // NetworkServer.SendToClientOfPlayer<ObjectDestroyMessage>(item.ReferenceHub.networkIdentity, msg);
                        item.ReferenceHub.networkIdentity.connectionToClient.Send<ObjectDestroyMessage>(msg);
                        if (DeputyFacalityManagerHandler.EscapeLock.netIdentity.observers.ContainsKey(item.Connection.connectionId))
                        {
                            DeputyFacalityManagerHandler.EscapeLock.netIdentity.observers.Remove(item.Connection.connectionId);
                            if (DeputyFacalityManager.RemoveFromVisList == null)
                                DeputyFacalityManager.RemoveFromVisList = typeof(NetworkConnection).GetMethod("RemoveFromVisList", BindingFlags.NonPublic | BindingFlags.Instance);
                            DeputyFacalityManager.RemoveFromVisList?.Invoke(item.Connection, new object[] { DeputyFacalityManagerHandler.EscapeLock.netIdentity, true });
                        }
                    }

                    GameObject.Destroy(DeputyFacalityManagerHandler.EscapeLock.gameObject);
                    DeputyFacalityManagerHandler.EscapeLock = null;
                }, "RemoveDoors");
            }, "RoundStartLate");
        }
    }
}
