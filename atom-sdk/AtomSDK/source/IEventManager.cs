using System;

namespace ironsource {
    public interface IEventManager {
        void addEvent(Event eventObject);
        Event getEvent(string stream);
    }
}