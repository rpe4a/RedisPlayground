namespace MyApp.Model;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsFriend { get; set; }
    public Address Address { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(IsFriend)}: {IsFriend}, {nameof(Address)}: {Address}";
    }
}