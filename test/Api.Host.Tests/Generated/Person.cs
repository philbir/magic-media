namespace MagicMedia.Api.Host.Tests
{
    public class Person
        : IPerson
    {
        public Person(
            System.Guid id, 
            string name, 
            System.DateTimeOffset? dateOfBirth, 
            string group)
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
            Group = group;
        }

        public System.Guid Id { get; }

        public string Name { get; }

        public System.DateTimeOffset? DateOfBirth { get; }

        public string Group { get; }
    }
}
