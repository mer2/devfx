using System;
using HTB.DevFx.Core;
using HTB.DevFx.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
	[TestClass]
	public class ConvertingTest
	{
		public enum Enum1
		{
			None = 0,
			E1,
			E2,
			E3,
			E4
		}

		[Flags]
		public enum Enum2
		{
			None = 0,
			E1,
			E2,
			E3,
			E4
		}

		[TestMethod]
		public void TestStringToEnum() {
			var e1 = Converting.StringToEnum<Enum1>("e1");
			Assert.AreEqual(Enum1.E1, e1);
			e1 = Converting.StringToEnum<Enum1>("e1,e2");
			Assert.AreEqual(Enum1.E3, e1);
			e1 = Converting.StringToEnum<Enum1>("e1,e2,none");
			Assert.AreEqual(Enum1.E3, e1);
			e1 = Converting.StringToEnum<Enum1>("e1,e2,unknown");
			Assert.AreEqual(Enum1.None, e1);
			e1 = Converting.StringToEnum<Enum1>("1");
			Assert.AreEqual(Enum1.E1, e1);

			var e2 = Converting.StringToEnum<Enum2>("e1,e2");
			Assert.AreEqual(Enum2.E1 | Enum2.E2, e2);
			e2 = Converting.StringToEnum<Enum2>("e1,e2,unknown");
			Assert.AreEqual(Enum2.None, e2);

			var e3 = Converting.StringToEnum(typeof(Enum1), "e1");
			Assert.AreEqual(Enum1.E1, e3);
			e3 = Converting.StringToEnum(typeof(Enum1), "e1,e2");
			Assert.AreEqual(Enum1.E3, e3);
			e3 = Converting.StringToEnum(typeof(Enum1), "e1,e2,none");
			Assert.AreEqual(Enum1.E3, e3);
			e3 = Converting.StringToEnum(typeof(Enum1), "e1,e2,unknown");
			Assert.AreEqual(Enum1.None, e3);

			var e4 = Converting.StringToEnum(typeof(Enum2), "e1,e2");
			Assert.AreEqual(Enum2.E1 | Enum2.E2, e4);
			e4 = Converting.StringToEnum(typeof(Enum2), "e1,e2,unknown");
			Assert.AreEqual(Enum2.None, e4);
		}

		[TestMethod]
		public void TestAOPResultConverting() {
			var ap = new AOPResult<int>();
			var ap1 = (IAOPResult<int>)ap;
			AOPResult aop = ap;
			aop.ResultAttachObject = 1F;
			Assert.IsTrue(aop.ResultAttachObject is int);
			Assert.IsTrue(ap1.ResultAttachObjectEx == 1);
		}
	}
}
