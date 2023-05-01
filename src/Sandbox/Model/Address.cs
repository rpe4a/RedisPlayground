namespace MyApp.Model;

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public int Number { get; set; }

    public override string ToString()
    {
        return $"{nameof(Street)}: {Street}, {nameof(City)}: {City}, {nameof(Number)}: {Number}";
    }
}