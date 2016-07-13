using NUnit.Framework;
using System;

namespace ironsource {
    namespace test {        
        [TestFixture()]
        public class Response_Tests {
            [Test()]
            public void ResponseProperties_Test() {
                string expectedError = "test error";
                string expectedData = "test data";
                int expectedStatus = 200;

                Response response = new Response(expectedError, expectedData, expectedStatus);

                Assert.AreEqual(expectedError, response.error);
                Assert.AreEqual(expectedData, response.data);
                Assert.AreEqual(expectedStatus, response.status);
            }           
        }
    }
}