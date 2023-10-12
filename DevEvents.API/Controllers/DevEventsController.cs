using DevEvents.API.Entities;
using DevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        public DevEventsController(DevEventsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //Traz todos os eventos onde não está deletado/cancelado 
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();
            return Ok(devEvents);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            //Traz todos o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            return devEvent == null ? NotFound() : Ok(devEvent);

        }

        [HttpPost]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            //retorna após a conferência do objeto cadastrado
            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent input)
        {
            //Traz todos o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound();

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            //Traz todos o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound();

            devEvent.Delete();

            return NoContent();
        }


        [HttpPost("{id}/speakers")]
        public IActionResult PostSpeaker(Guid idEvent, DevEventSpeaker speaker)
        {
            //Traz todos o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == idEvent);

            if (devEvent == null)
                return NotFound();

            devEvent.Speakers.Add(speaker);

            return NoContent();
        }
    }
}
