using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.ValueObjects
{
    public class Price : ValueObject
    {
        // For Entity Framework Core
        protected Price() { }

        public Price(int amount, MoneyUnit unit)
        {
            if (MoneyUnit.UnSpecified == unit)
                throw new ApplicationException("You must supply a valid money unit!");

            Amount = amount;

            Unit = unit;
        }


        public int Amount { get; protected set; }


        public MoneyUnit Unit { get; protected set; } = MoneyUnit.UnSpecified;


        public bool HasValue => (Unit != MoneyUnit.UnSpecified);


        public override string ToString()
        {
            return
                Unit != MoneyUnit.UnSpecified ?
                    Amount + " " + MoneySymbols.GetSymbol(Unit) :
                    Amount.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new List<object> { Amount, Unit};
        }
    }
}
