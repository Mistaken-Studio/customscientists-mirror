﻿// -----------------------------------------------------------------------
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
                (5000, (p1, p2) =>
                {
                    if (p1.Role != RoleType.Scientist && p2.Role != RoleType.Scientist)
                        return CompareResult.NO_ACTION;

                    var p1c = Classes.DeputyFacalityManager.Instance.Check(p1);
                    var p2c = Classes.DeputyFacalityManager.Instance.Check(p2);
                    if (p1c && p2c)
                        return CompareResult.SAME_RANK;
                    else if (p1c)
                    {
                        if (p2.Role == RoleType.Scientist)
                            return CompareResult.GIVE_ORDERS;
                        else if (Map.IsLczDecontaminated && p2.Role.Team == Team.MTF)
                            return CompareResult.GIVE_ORDERS;
                        else
                            return CompareResult.NO_ACTION;
                    }
                    else if (p2c)
                    {
                        if (p1.Role == RoleType.Scientist)
                            return CompareResult.FOLLOW_ORDERS;
                        else if (Map.IsLczDecontaminated && p1.Role.Team == Team.MTF)
                            return CompareResult.FOLLOW_ORDERS;
                        else
                            return CompareResult.NO_ACTION;
                    }
                    else
                        return CompareResult.NO_ACTION;
                }
            ));
            Log.Debug("Enabled CustomHierarchy integration.", PluginHandler.Instance.Config.VerbouseOutput);
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }
    }
}
