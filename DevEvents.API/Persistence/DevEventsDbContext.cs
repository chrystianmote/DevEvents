using DevEvents.API.Entities;

namespace DevEvents.API.Persistence
{
    public class DevEventsDbContext
    {
        public DevEventsDbContext()
        {
            DevEvents = new List<DevEvent>(); 
        }

        public List<DevEvent> DevEvents { get; set; }


    }
}
