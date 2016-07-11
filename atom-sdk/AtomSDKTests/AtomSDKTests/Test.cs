using NUnit.Framework;
using System;
namespace AtomSDKTests {
    [TestFixture]
    public class Test1 {
        [Test]
        public void TestCase1() {
            //ironsource.IronSourceAtomTracker tracker = new ironsource.IronSourceAtomTracker();


            long currentTimer  = ironsource.Utils.GetCurrentMilliseconds ();

            //Assert.True(false);
        }

        [Test]
        public void TestCase2() {
            ironsource.IronSourceAtomTracker tracker = new ironsource.IronSourceAtomTracker();


            //long currentTimer  = ironsource.Utils.GetCurrentMilliseconds ();

            //Assert.True(false);
        }
    }
}

