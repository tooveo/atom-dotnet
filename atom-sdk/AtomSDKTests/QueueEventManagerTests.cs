using NUnit.Framework;
using System;

namespace ironsource {
    namespace test {        
        [TestFixture()]
        public class QueueEventManager_Tests {
            [Test()]
            public void Constructor_Test() {  
                IEventManager eventManager = new QueueEventManager();

                Assert.AreEqual(eventManager.getEvent(""), null);
            }   

            [Test()]
            public void AddEvent_Test() {  
                IEventManager eventManager = new QueueEventManager();
                string streamName = "test stream";

                Event expectedEvent = new Event(streamName, "test data", "test auth");
                eventManager.addEvent(expectedEvent);

                Event resultEvent = eventManager.getEvent(streamName);

                Assert.AreEqual(expectedEvent.stream_, resultEvent.stream_);
                Assert.AreEqual(expectedEvent.data_, resultEvent.data_);
                Assert.AreEqual(expectedEvent.authKey_, resultEvent.authKey_);
            }   
        }
    }
}