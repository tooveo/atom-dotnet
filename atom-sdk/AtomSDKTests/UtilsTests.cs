using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

namespace ironsource {
    namespace test {        
        [TestFixture()]
        public class Utils_Tests {
            [Test()]
            public void DictionaryToJson_Test() {
                string expectedStr = "{\"test 1\": \"data 1\",\"test 2\": \"data 2\"}";

                Dictionary<string, string> testDict = new Dictionary<string, string>();
                testDict.Add("test 1", "data 1");
                testDict.Add("test 2", "data 2");

                Assert.AreEqual(expectedStr, Utils.DictionaryToJson(testDict));
            }

            [Test()]
            public void ListToJson_Test() {
                string expectedStr = "[{\"test\": \"data 1\"},{\"test\": \"data 2\"}]";

                List<string> testList = new List<string>();
                testList.Add("{\"test\": \"data 1\"}");
                testList.Add("{\"test\": \"data 2\"}");

                Assert.AreEqual(expectedStr, Utils.ListToJson(testList));
            }

            [Test()]
            public void EncodeHmac_Test() {
                string expectedStr = "1861387e46c3001593a644f3ade069a38bbf9a3220e82da5280a1bae1c44e4dc";

                string testInput = "{\"test\": \"data 1\"}";
                string testKey = "FefwefFESRWEfewrvw";

                Assert.AreEqual(expectedStr, Utils.EncodeHmac(testInput,
                                                    Encoding.ASCII.GetBytes(testKey)));
            }

            [Test()]
            public void Base64Encode_Test() {
                string expectedStr = "eyJ0ZXN0IjogImRhdGEgMSJ9";

                string testData = "{\"test\": \"data 1\"}";

                string resultData = Utils.Base64Encode(testData);

                Assert.AreEqual(expectedStr, resultData);
            }

            [Test()]
            public void EscapeStringValue_Test() {
                string expectStr = "{\\\"test\\\": \\\"data 1\\\"}";

                string testData = "{\"test\": \"data 1\"}";

                Assert.AreEqual(expectStr, Utils.EscapeStringValue(testData));
            }

            [Test()]
            public void CurrentTimeTest_Test() {
                long previousTime = Utils.GetCurrentMilliseconds();
                Thread.Sleep(10);
                long currentTime = Utils.GetCurrentMilliseconds();

                Assert.AreNotEqual(currentTime, previousTime);
            }
        }
    }
}
