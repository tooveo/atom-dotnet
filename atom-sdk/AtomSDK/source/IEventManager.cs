using System;

namespace ironsource {
    public interface IEventManager {
        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="eventObject">Event object.</param>
        void addEvent(Event eventObject);

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="stream">Stream.</param>
        Event getEvent(string stream);
    }
}