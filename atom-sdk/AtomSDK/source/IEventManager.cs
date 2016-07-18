using System;

namespace ironsource {
    /// <summary>
    /// Interface for store data
    /// </summary>
    public interface IEventManager {
        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="eventObject">Event object.</param>
        void addEvent(Event eventObject);

        /// <summary>
        /// Get one the event from store.
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