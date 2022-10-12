using BenchmarkDotNet.Attributes;
using Bogus;

namespace Warehouse.Benchmarks
{
    [MemoryDiagnoser(false)]
    public class Casting
    {
        //[Benchmark]
        //public Person HardCast()
        //{
        //    var antHardCast = (Person) StaticObject.Person;
        //    return antHardCast;
        //}

        //[Benchmark]
        //public Person SafeCast()
        //{
        //    var antHardCast = StaticObject.Person as Person;
        //    return antHardCast!;
        //}

        //[Benchmark]
        //public Person MatchCast()
        //{
        //    if (StaticObject.Person is Person person)
        //    {
        //        return person;
        //    }

        //    return null!;
        //}


        [Benchmark]
        public List<Person> OfType()
        {
            return StaticObject.People.OfType<Person>().ToList();
        }

        [Benchmark]
        public List<Person> HardCast_TypeOf()
        {
            return StaticObject.People
                .Where(x => x.GetType() == typeof(Person)) 
                .Select(x => (Person)x)
                .ToList();
        }

        [Benchmark]
        public List<Person> HardCast_As()
        {
            return StaticObject.People
                .Where(x => x as Person is not null)
                .Select(x => (Person)x)
                .ToList();
        }

        [Benchmark]
        public List<Person> HardCast_Is()
        {
            return StaticObject.People
                .Where(x => x is Person)
                .Select(x => (Person)x)
                .ToList();
        }

        [Benchmark]
        public List<Person> Cast_As()
        {
            return StaticObject.People
                .Where(x => x as Person is not null)
                .Cast<Person>()
                .ToList();
        }

        [Benchmark]
        public List<Person> Cast_Is()
        {
            return StaticObject.People
                .Where(x => x is Person)
                .Cast<Person>()
                .ToList();
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class StaticObject
    {

        static StaticObject()
        {
            //Randomizer.Seed = new Random(11);

            var personFaker = new Faker<Person>()
                .RuleFor(p => p.Id, faker => faker.IndexFaker)
                .RuleFor(p => p.Name, faker => faker.Person.FullName);

            People  = new List<object>(personFaker.Generate(10_000));
        }

        public static List<object> People { get; }
    }
}
