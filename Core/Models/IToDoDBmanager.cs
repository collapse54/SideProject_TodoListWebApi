using System.Collections.Generic;

namespace Core.Models
{
    public interface IToDoDBmanager
    {
        List<Thing> GetThing(string UserName);
        bool NewThing(Thing thing, string UserName);
        public bool ChangeThingByTitle(Thing thing, string UserName);
        public bool RecycleByTitle(RecycleThing title, string UserName);
    }
}