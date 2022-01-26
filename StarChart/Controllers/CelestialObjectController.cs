using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using System.Linq;

namespace StarChart.Controllers {
  [Route(""), ApiController]
  public class CelestialObjectController : ControllerBase {
    private readonly ApplicationDbContext _context;

    public CelestialObjectController(ApplicationDbContext context) {
      _context = context;
    }

    [HttpGet("{id:int}")]
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
    public IActionResult GetAll(int id) {
      var celestialObjs = _context.CelestialObjects.ToList();
      foreach(var celestialObj in celestialObjs)
      {
        celestialObj.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObj.Id).ToList();
      }
      return Ok(celestialObjs);
    }
  }
}
