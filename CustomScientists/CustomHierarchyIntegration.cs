// -----------------------------------------------------------------------
// <copyright file="CustomHierarchyIntegration.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;

using static Mistaken.CustomHierarchy.HierarchyHandler;

namespace Mistaken.CustomScientists
{
    internal static class CustomHierarchyIntegration
    {
        internal static void EnableCustomHierarchyIntegration()
        {
            PluginHandler.CustomHierarchyAvailable = true;
            Log.Debug("Enabling CustomHierarchy integration.", PluginHandler.Instance.Config.VerbouseOutput);
#pragma warning disable SA1118 // Parameter should not span multiple lines
            CustomPlayerComperers.Add(
                "dfm_comparer",
                (
                    5000,
                    (Func<Player, Player, CompareResult>)((Player p1, Player p2) =>
                    {
                        if (p1.Role != RoleType.Scientist && p2.Role != RoleType.Scientist)
                            return CompareResult.NO_ACTION;

                        var dfmRole = API.CustomRoles.MistakenCustomRole.Get(API.CustomRoles.MistakenCustomRoles.DEPUTY_FACILITY_MANAGER);
                        var p1c = dfmRole.Check(p1);
                        var p2c = dfmRole.Check(p2);
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
                            return CompareResult.NO_ACTION;
                    })));
            Log.Debug("Enabled CustomHierarchy integration.", PluginHandler.Instance.Config.VerbouseOutput);
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }
    }
}
