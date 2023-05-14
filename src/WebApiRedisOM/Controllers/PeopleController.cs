﻿using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Searching;
using WebApiRedisOM.Models;

namespace WebApiRedisOM.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly RedisConnectionProvider _provider;
    private readonly RedisCollection<Person> _people;

    public PeopleController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _people = (RedisCollection<Person>?) provider.RedisCollection<Person>();
    }

    [HttpPost]
    public async Task<Person> AddPerson([FromBody] Person person)
    {
        await _people.InsertAsync(person);
        return person;
    }

    [HttpGet("filterAge")]
    public IList<Person> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
    {
        return _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
    }

    [HttpGet("filterGeo")]
    public IList<Person> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius,
        [FromQuery] string unit)
    {
        return _people.GeoFilter(x => x.Address!.Location, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit))
            .ToList();
    }

    [HttpGet("filterName")]
    public async Task<IList<Person>> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
    {
        return _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
    }

    [HttpGet("postalCode")]
    public IList<Person> FilterByPostalCode([FromQuery] string postalCode)
    {
        return _people.Where(x => x.Address!.PostalCode == postalCode).ToList();
    }

    [HttpGet("fullText")]
    public IList<Person> FilterByPersonalStatement([FromQuery] string text)
    {
        return _people.Where(x => x.PersonalStatement == text).ToList();
    }

    [HttpGet("streetName")]
    public IList<Person> FilterByStreetName([FromQuery] string streetName)
    {
        return _people.Where(x => x.Address!.StreetName == streetName).ToList();
    }

    [HttpGet("skill")]
    public IList<Person> FilterBySkill([FromQuery] string skill)
    {
        return _people.Where(x => x.Skills.Contains(skill)).ToList();
    }

    [HttpPatch("updateAge/{id}")]
    public async Task<IActionResult> UpdateAge([FromRoute] string id, [FromBody] int newAge)
    {
        foreach (var person in _people.Where(x => x.Id == id))
        {
            person.Age = newAge;
        }

        await _people.SaveAsync();
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson([FromRoute] string id)
    {
        var person = await _people.FindByIdAsync(id);

        if (person != null)
            await _people.DeleteAsync(person);

        return NoContent();
    }
}