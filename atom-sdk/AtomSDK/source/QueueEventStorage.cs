using System;

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ironsource {
    /// <summary>
    /// Queue event manager.
    /// </summary>
    public class QueueEventStorage: IEventStorage {
        private ConcurrentDictionary<string, ConcurrentQueue<Event>> events_;

        /// <summary>
        /// Initializes a new instance of the QueueEventManager
        /// </summary>
        public QueueEventStorage() {
            events_ = new ConcurrentDictionary<string, ConcurrentQueue<Event>>();
        }

        /// <summary>
        /// Add event to storage
        /// </summary>
        /// <param name="eventObject">
        /// <see cref="Event"/> event for adding to storage
        /// </param>
        void IEventStorage.addEvent(Event eventObject) {   
            if (!events_.ContainsKey(eventObject.stream_)) {
                events_.TryAdd(eventObject.stream_, new ConcurrentQueue<Event>());
            }

            events_[eventObject.stream_].Enqueue(eventObject);
        }

        /// <summary>
        /// Get Event from stogare
        /// </summary>
        /// <returns>
        /// <see cref="Event"/> event from storage
        /// </returns>
        /// <param name="stream">
        /// <see cref="string"/> stream name
        /// </param>
        Event IEventStorage.getEvent(string stream) {
            Event eventObject = null;
            if (events_.ContainsKey(stream)) {
                events_[stream].TryDequeue(out eventObject);
            }

            return eventObject;
        }
    }
}