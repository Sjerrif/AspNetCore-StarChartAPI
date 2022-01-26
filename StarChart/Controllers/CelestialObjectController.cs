using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;
using System.Linq;

namespace StarChart.Controllers {
  [Route(""), ApiController]
  public class CelestialObjectController : ControllerBase {
    private readonly ApplicationDbContext _context;

    public CelestialObjectController(ApplicationDbContext context) {
      _context = context;
    }

    [HttpGet("{id:int}", Name = "GetById")]
    public IActionResult GetById(int id) {
      var celestialObj = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
      if(celestialObj == null) {
        return NotFound();
      }

      celestialObj.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();

      return Ok(celestialObj);
    }

    [HttpGet("{name}")]
    public IActionResult GetByName(string name)
    {
        var celestialObjs = _context.CelestialObjects.Where(e => e.Name == name);
        if (!celestialObjs.Any()) {
          return NotFound();
        }

        foreach(var celestialObj in celestialObjs)
        {
          celestialObj.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObj.Id).ToList();
        }
        return Ok(celestialObjs.ToList());
    }

    [HttpGet]
    public IActionResult GetAll() {
      var celestialObjs = _context.CelestialObjects.ToList();
      foreach(var celestialObj in celestialObjs)
      {
        celestialObj.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObj.Id).ToList();
      }
      return Ok(celestialObjs);
    }


    [HttpPost]
    public IActionResult Create([FromBody] CelestialObject celestialObject)
    {
      _context.CelestialObjects.Add(celestialObject);
      _context.SaveChanges();

      return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CelestialObject celestialObject)
    {
      var existingObject = _context.CelestialObjects.Find(id);
      if (existingObject == null) {
        return NotFound();
      }

      existingObject.Name = celestialObject.Name;
      existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
      existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
      _context.CelestialObjects.Update(existingObject);
      _context.SaveChanges();
      return NoContent();
    }

    [HttpPatch("{id}/{name}")]
    public IActionResult RenameObject(int id, string name)
    {
      var existingObject = _context.CelestialObjects.Find(id);
      if (existingObject == null) {
        return NotFound();
      }

      existingObject.Name = name;
      _context.CelestialObjects.Update(existingObject);
      _context.SaveChanges();

      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var celestialObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
      if (!celestialObjects.Any()) {
        return NotFound();
      }

      _context.CelestialObjects.RemoveRange(celestialObjects);
      _context.SaveChanges();

      return NoContent();
      }
  }
}
