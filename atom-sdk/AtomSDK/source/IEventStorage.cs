using System;

namespace ironsource {
    /// <summary>
    /// Interface for providing a generic way of storing events in a backlog before they are sent to Atom.
    /// </summary>
    public interface IEventStorage {
        /// <summary>
        /// Add an event.
        /// </summary>
        /// <param name="eventObject">Event object.</param>
        void addEvent(Event eventObject);

        /// <summary>
        /// Get one event from data store
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