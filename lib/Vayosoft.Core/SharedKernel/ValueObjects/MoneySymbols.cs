using System.Collections.Generic;
using Vayosoft.Core.SharedKernel.Enums;

namespace Vayosoft.Core.SharedKernel.ValueObjects
{
    public static class MoneySymbols
    {
        private static readonly Dictionary<MoneyUnit, string> Symbols = new()
        {
            { MoneyUnit.UnSpecified, string.Empty },
            { MoneyUnit.Dollar, "$" },
            { MoneyUnit.Euro, "€" },
            { MoneyUnit.Shekel, "₪" }
        };

        public static string GetSymbol(MoneyUnit moneyUnit)
        {
            return Symbols[moneyUnit];
        }
    }
}
