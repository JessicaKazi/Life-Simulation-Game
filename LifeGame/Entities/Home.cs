using System.Collections.Generic;

namespace LifeGame.Entities;

public class District
{
    public string Name { get; }
    public string Description { get; }

    public District(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static readonly IReadOnlyList<District> Catalog = new List<District>
    {
        new("Downtown",   "Busy city centre - offices, restaurants, apartments."),
        new("Industrial", "Factories and warehouses. Cheap, but grim."),
        new("Waterfront", "Expensive scenic area - beaches, restaurants."),
        new("Suburbs",    "Quiet residential area - houses, parks, schools."),
        new("Old Town",   "Historic district - markets, older buildings."),
    };
}

public class Home
{
    public string Address { get; }
    public District District { get; }
    public decimal WeeklyRent { get; }
    public int Quality { get; }
    public string Description { get; }

    public Home(string address, District district, decimal weeklyRent, int quality, string description)
    {
        Address = address;
        District = district;
        WeeklyRent = weeklyRent;
        Quality = quality;
        Description = description;
    }

    public static readonly IReadOnlyList<Home> Listings = new List<Home>
    {
        new("Industrial Flat 3B",    District.Catalog[1], 60m,  1, "A cramped one-room flat above a factory."),
        new("Old Town Studio",       District.Catalog[4], 90m,  2, "A small studio in the historic quarter."),
        new("Downtown Apartment 12", District.Catalog[0], 140m, 3, "A modern one-bedroom in the city centre."),
        new("Suburban House",        District.Catalog[3], 220m, 4, "A family home with a garden and driveway."),
        new("Waterfront Loft",       District.Catalog[2], 400m, 5, "A luxury loft overlooking the beach."),
    };
}   