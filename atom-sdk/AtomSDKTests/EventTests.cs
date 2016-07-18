using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

namespace ironsource {
    namespace test {        
        [TestFixture()]
        public class Event_Tests {
            [Test()]
            public void Constructor_Test() {
                string expectedStream = "test stream";
                string expectedData = "test data";
                string expectedAuth = "test auth";

                Event eventObject = new Event(expectedStream, expectedData, expectedAuth); 
             
                Assert.AreEqual(eventObject.stream_, expectedStream);
                Assert.AreEqual(eventObject.data_, expectedData);
                Assert.AreEqual(eventObject.authKey_, expectedAuth);
            }
        }
    }
}
