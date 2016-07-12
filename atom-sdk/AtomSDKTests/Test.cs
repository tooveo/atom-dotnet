using NUnit.Framework;

namespace ironsource {
    namespace test {
        [TestFixture]
        public class Test1
        {
            [Test]
            public void TestCase1 ()
            {
                //ironsource.IronSourceAtomTracker tracker = new ironsource.IronSourceAtomTracker();


                long currentTimer = Utils.GetCurrentMilliseconds ();

                //Assert.True(false);
            }

            [Test]
            public void TestCase2 ()
            {
                ironsource.IronSourceAtomTracker tracker = new IronSourceAtomTracker ();


                //long currentTimer  = ironsource.Utils.GetCurrentMilliseconds ();

                //Assert.True(false);
            }
        }
    }
}

