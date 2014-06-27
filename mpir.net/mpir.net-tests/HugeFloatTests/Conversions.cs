/*
Copyright 2014 Alex Dyachenko

This file is part of the MPIR Library.

The MPIR Library is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published
by the Free Software Foundation; either version 3 of the License, or (at
your option) any later version.

The MPIR Library is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
License for more details.

You should have received a copy of the GNU Lesser General Public License
along with the MPIR Library.  If not, see http://www.gnu.org/licenses/.  
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MPIR.Tests.HugeFloatTests
{
    [TestClass]
    public class Conversions
    {
        private static HugeFloat AlmostOne;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            AlmostOne = new HugeFloat(0.99999);
        }

        [TestMethod]
        public void FloatToStringDecimal()
        {
            var n = "-234293847562.98357462983476598345623984756";
            using (var a = new HugeFloat(n))
            {
                FloatAssert.AreEqual(n, a);
            }
        }

        [TestMethod]
        public void FloatToStringHex()
        {
            var s = "-23429abcdef29835746298.3fedcba34562";
            using (var a = new HugeFloat(s, 16))
            {
                Assert.AreEqual("-0.23429abcdef298357462983fedcba34562@16", a.ToString(16, true));
                Assert.AreEqual("-0.23429ABCDEF298357462983FEDCBA34562@16", a.ToString(16));

                Assert.AreEqual("-0.23429abcdef298357462983fedcba34562@22", a.ToString(16, true, true));
                Assert.AreEqual("-0.23429ABCDEF298357462983FEDCBA34562@22", a.ToString(16, false, true));
            }
        }

        [TestMethod]
        public void FloatFromUlong()
        {
            using (var a = new HugeFloat())
            {
                ulong b = 0xF84739ABCDEF4876;
                a.SetTo(b);
                FloatAssert.AreEqual(b + ".", a);
            }
        }

        [TestMethod]
        public void FloatFromLong()
        {
            using (var a = new HugeFloat())
            {
                long b = -0x784739ABCDEF4876;
                a.SetTo(b);
                FloatAssert.AreEqual(b + ".", a);
            }
        }

        [TestMethod]
        public void FloatToAndFromDouble()
        {
            using (var a = new HugeFloat())
            {
                a.SetTo(-123.25);

                double c = a.ToDouble();
                Assert.IsTrue(c.Equals(-123.25));
                Assert.IsTrue(a.Equals(-123.25));
            }
        }

        [TestMethod]
        public void FloatToAndFromFloat()
        {
            using (var a = new HugeFloat())
            {
                a.SetTo(-123.125f);

                double c = a.ToDouble();
                Assert.IsTrue(-123.125 == c);
                Assert.IsTrue(-123.125 == a);
                Assert.IsTrue(-123.125f == a);
            }
        }

        [TestMethod]
        public void FloatToAndFromUlong()
        {
            using(var a = new HugeFloat())
            {
                ulong b = 0xF84739ABCDEF4876;
                a.SetTo(b);
                FloatAssert.AreEqual(b + ".", a);

                a.Value = -a;
                ulong c = a.ToUlong();
                Assert.AreEqual(b, c);
            }
        }

        [TestMethod]
        public void FloatToAndFromLong()
        {
            using(var a = new HugeFloat())
            {
                long b = -0x784739ABCDEF4876;
                a.SetTo(b);
                FloatAssert.AreEqual(b + ".", a);

                long c = a.ToLong();
                Assert.AreEqual(b, c);
            }
        }

        [TestMethod]
        public void FloatToAndFromDoubleOutExp()
        {
            using(var a = new HugeFloat())
            {
                a.SetTo(-123.45e20);

                long exp;
                a.Value = a + a;
                double c = a.ToDouble(out exp);

                Assert.AreEqual(75, exp);
                c *= Math.Pow(2, exp);

                Assert.IsTrue(a + 10000000000 >= c);
                Assert.IsTrue(a - 10000000000 <= c);
            }
        }

        [TestMethod]
        public void FloatFromString()
        {
            using (var a = new HugeFloat())
            {
                var n = "98762934876529834765234123.984761";
                a.SetTo(n);
                FloatAssert.AreEqual(n, a);
            }
        }

        [TestMethod]
        public void FloatFromStringExpDecimal()
        {
            using(var a = new HugeFloat())
            {
                var n = "0.12354523094527035843ABCDEF54@10";
                
                a.SetTo(n, 16);
                Assert.AreEqual("0.12354523094527035843ABCDEF54@10", a.ToString(16, false, true));
                Assert.AreEqual("0.12354523094527035843ABCDEF54@A", a.ToString(16, false, false));

                a.SetTo(n, 16, false);
                Assert.AreEqual("0.12354523094527035843ABCDEF54@16", a.ToString(16, false, true));
                Assert.AreEqual("0.12354523094527035843ABCDEF54@10", a.ToString(16, false, false));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FloatFromInvalidString()
        {
            using (var a = new HugeFloat())
            {
                a.SetTo("12345A");
            }
        }

        [TestMethod]
        public void FloatToStringTruncated()
        {
            using (var d = new HugeInt("115756986668303657898962467957"))
            using (var e = new HugeInt(d ^ 10))
            {
                var s = e.ToString(10);
                using (var a = HugeFloat.Allocate(1200))
                {
                    a.SetTo(s);
                    Assert.AreEqual(256, a.ToString().Replace("0.", "").Replace("@291", "").Length);
                    Assert.AreEqual(291, a.ToString(10).Replace("0.", "").Replace("@291", "").Length);
                }
            }
        }

        [TestMethod]
        public void FloatFitsLong()
        {
            using (var a = new HugeFloat(long.MaxValue))
            using (var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsLong());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsLong());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsLong());

                a.SetTo(long.MinValue);
                Assert.IsTrue(a.FitsLong());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsLong());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsLong());
            }
        }

        [TestMethod]
        public void FloatFitsUlong()
        {
            using(var a = new HugeFloat(ulong.MaxValue))
            using(var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsUlong());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsUlong());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsUlong());

                a.SetTo(ulong.MinValue);
                Assert.IsTrue(a.FitsUlong());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsUlong());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsUlong());
            }
        }

        [TestMethod]
        public void FloatFitsInt()
        {
            using(var a = new HugeFloat(int.MaxValue))
            using(var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsInt());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsInt());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsInt());

                a.SetTo(int.MinValue);
                Assert.IsTrue(a.FitsInt());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsInt());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsInt());
            }
        }

        [TestMethod]
        public void FloatFitsUint()
        {
            using(var a = new HugeFloat(uint.MaxValue))
            using(var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsUint());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsUint());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsUint());

                a.SetTo(uint.MinValue);
                Assert.IsTrue(a.FitsUint());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsUint());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsUint());
            }
        }

        [TestMethod]
        public void FloatFitsShort()
        {
            using(var a = new HugeFloat(short.MaxValue))
            using(var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsShort());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsShort());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsShort());

                a.SetTo(short.MinValue);
                Assert.IsTrue(a.FitsShort());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsShort());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsShort());
            }
        }

        [TestMethod]
        public void FloatFitsUshort()
        {
            using(var a = new HugeFloat(ushort.MaxValue))
            using(var b = new HugeFloat())
            {
                Assert.IsTrue(a.FitsUshort());

                b.Value = a + AlmostOne;
                Assert.IsTrue(b.FitsUshort());

                b.Value = a + 1;
                Assert.IsFalse(b.FitsUshort());

                a.SetTo(ushort.MinValue);
                Assert.IsTrue(a.FitsUshort());

                b.Value = a - AlmostOne;
                Assert.IsTrue(b.FitsUshort());

                b.Value = a - 1;
                Assert.IsFalse(b.FitsUshort());
            }
        }
    }
}
