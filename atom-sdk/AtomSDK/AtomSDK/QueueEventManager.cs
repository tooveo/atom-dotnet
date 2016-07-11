using System;

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ironsource {
    public class QueueEventManager: IEventManager {
        private ConcurrentDictionary<string, ConcurrentQueue<Event>> events_;

        public QueueEventManager() {
            events_ = new ConcurrentDictionary<string, ConcurrentQueue<Event>>();
        }
            
        void IEventManager.addEvent(Event eventObject) {   
            if (!events_.ContainsKey(eventObject.stream_)) {
                events_.TryAdd(eventObject.stream_, new ConcurrentQueue<Event>());
            }

            events_[eventObject.stream_].Enqueue(eventObject);
        }

        Event IEventManager.getEvent(string stream) {
            Event eventObject = null;
            if (events_.ContainsKey(stream)) {
                events_[stream].TryDequeue(out eventObject);
            }

            return eventObject;
        }
    }
}