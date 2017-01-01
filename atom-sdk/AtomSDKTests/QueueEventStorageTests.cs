using NUnit.Framework;
using System;

namespace ironsource
{
    namespace test
    {
        [TestFixture ()]
        public class QueueEventStorage_Tests
        {
            [Test ()]
            public void Constructor_Test ()
            {
                IEventStorage eventStorage = new QueueEventStorage ();

                Assert.AreEqual (eventStorage.getEvent (""), null);
            }

            [Test ()]
            public void AddEvent_Test ()
            {
                IEventStorage eventStorage = new QueueEventStorage ();
                string streamName = "test stream";

                Event expectedEvent = new Event (streamName, "test data", "test auth");
                eventStorage.addEvent (expectedEvent);

                Event resultEvent = eventStorage.getEvent (streamName);

                Assert.AreEqual (expectedEvent.stream_, resultEvent.stream_);
                Assert.AreEqual (expectedEvent.data_, resultEvent.data_);
                Assert.AreEqual (expectedEvent.authKey_, resultEvent.authKey_);
            }
        }
    }
}