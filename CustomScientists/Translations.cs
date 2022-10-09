// -----------------------------------------------------------------------
// <copyright file="Translations.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CustomScientists
{
    internal class Translations : ITranslation
    {
        public string DeputyFacilityManager { get; set; } = "Zastępca Dyrektora Placówki";

        public string ZoneManager { get; set; } = "Zarządca Strefy Niskiego Ryzyka";

        public string DeputyFacilityManagerDescription { get; set; } = "Twoim zadaniem jest pomoc w ochronie i odeskortowaniu <color=yellow>naukowców</color><br>. Nie możesz uciec przed dekontaminacją <color=green>LCZ</color>";

        public string ZoneManagerDescription { get; set; } = "Twoim zadaniem jest ucieczka z placówki";

        public string DeputyFacilityManagerKeycard { get; set; } = "Karta Zastępcy Dyrektora Placówki";

        public string ZoneManagerKeycard { get; set; } = "Karta Zarządcy Strefy Niskiego Ryzyka";

        public string DeputyFacilityManagerKeycardDescription { get; set; } = "Karta Zastępcy Dyrektora Placówki";

        public string ZoneManagerKeycardDescription { get; set; } = "Karta Zarządcy Strefy Niskiego Ryzyka";
    }
}
