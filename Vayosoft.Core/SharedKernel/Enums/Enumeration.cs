using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vayosoft.Core.SharedKernel.Enums
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; }

        public int Id { get; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ 31;
        }

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static T ParseId<T>(int id) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(id, "Id", item => item.Id == id);
            return matchingItem;
        }

        public static T ParseName<T>(string name) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(name, "Name", item 
                => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return matchingItem;
        }

        private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem != null) return matchingItem;
            var message = $"'{value}' is not a valid {description} in {typeof(T)}";
            throw new ApplicationException(message);
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}
