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
        }
    }
}