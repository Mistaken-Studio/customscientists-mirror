// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Exiled.API.Interfaces;
using Mistaken.Updater.API.Config;

namespace Mistaken.CustomScientists
{
    internal class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        [Description("Auto Update Settings")]
        public SourceType SourceType { get; set; } = SourceType.DISABLED;

        public string Url { get; set; } = string.Empty;
    }
}
