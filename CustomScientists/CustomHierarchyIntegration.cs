// -----------------------------------------------------------------------
// <copyright file="CustomHierarchyIntegration.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.CustomScientists.Handlers;
using static Mistaken.CustomHierarchy.HierarchyHandler;

namespace Mistaken.CustomScientists
{
    internal static class CustomHierarchyIntegration
    {
        internal static void EnableCustomHierarchyIntegration()
        {
            PluginHandler.CustomHierarchyAvailable = true;
            Log.Info("Enabling CustomHierarchy integration.");
#pragma warning disable SA1118 // Parameter should not span multiple lines
            Mistaken.CustomHierarchy.HierarchyHandler.CustomPlayerComperers.Add(
                "dfm_comparer",
                (
                    5000,
                    (Func<Player, Player, CompareResult>)((Player p1, Player p2) =>
                    {
                        if (p1.Role != RoleType.Scientist && p2.Role != RoleType.Scientist)
                            return CompareResult.NO_ACTION;
                        var p1c = API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).Check(p1);
                        var p2c = API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER).Check(p2);
                        if (p1c && p2c)
                            return CompareResult.SAME_RANK;
                        else if (p1c)
                        {
                            if (p2.Role == RoleType.Scientist)
                                return CompareResult.GIVE_ORDERS;
                            else if (Mistaken.API.MapPlus.IsLCZDecontaminated() && p2.Team == Team.MTF)
                                return CompareResult.GIVE_ORDERS;
                            else
                                return CompareResult.NO_ACTION;
                        }
                        else if (p2c)
                        {
                            if (p1.Role == RoleType.Scientist)
                                return CompareResult.FOLLOW_ORDERS;
                            else if (Mistaken.API.MapPlus.IsLCZDecontaminated() && p1.Team == Team.MTF)
                                return CompareResult.FOLLOW_ORDERS;
                            else
                                return CompareResult.NO_ACTION;
                        }
                        else
                        {
                            return CompareResult.NO_ACTION;
                        }
                    })));
            Log.Info("Enabled CustomHierarchy integration.");
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }
    }
}
