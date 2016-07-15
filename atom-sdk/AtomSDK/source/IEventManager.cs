using System;

namespace ironsource {
    public interface IEventManager {
        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="eventObject">Event object.</param>
        void addEvent(Event eventObject);

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <returns>
        /// <see cref="Event"/> The event.
        /// </returns>
        /// <param name="stream">
        /// <see cref="string"/> Stream.
        /// </param>
        Event getEvent(string stream);
    }
}