using HTB.DevFx;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
	internal interface IInitClassSetting
	{
		string Value { get; }
	}

	internal class InitClassSetting : ConfigSettingElement, IInitClassSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Value = this.GetSetting("value");
		}
		public string Value { get; set; }
	}

	internal class InitClass : IInitializable<IInitClassSetting>, ISettingRequired<InitClassSetting>
	{
		public IInitClassSetting Setting { get; private set; }

		void IInitializable<IInitClassSetting>.Init(IInitClassSetting setting) {
			this.Setting = setting;
		}

		InitClassSetting ISettingRequired<InitClassSetting>.Setting {
			set { this.Setting = value; }
		}
	}

	internal class InitClass1 : IInitializable<IInitClassSetting>
	{
		public IInitClassSetting Setting { get; private set; }

		public void Init(IInitClassSetting setting) {
			this.Setting = new InitClassSetting { Value = "InitClass1.Init self" };
		}

		void IInitializable<IInitClassSetting>.Init(IInitClassSetting setting) {
			this.Setting = setting;
		}
	}

	internal class InitClass2 : ISettingRequired<IInitClassSetting>
	{
		public IInitClassSetting Setting { get; private set; }
		IInitClassSetting ISettingRequired<IInitClassSetting>.Setting {
			set { this.Setting = value; }
		}
	}

	[TestClass]
	public class SettingRequiredExtenderTest
	{
		[TestMethod]
		public void TestMethod1() {
			var svr = ObjectService.GetObject<InitClass>();
			Assert.IsNotNull(svr);
			Assert.IsNotNull(svr.Setting);
			Assert.AreEqual("123456", svr.Setting.Value);
			var svr1 = ObjectService.GetObject<InitClass1>();
			Assert.IsNotNull(svr1);
			Assert.IsNotNull(svr1.Setting);
			Assert.AreEqual("1:123456", svr1.Setting.Value);
			var svr2 = ObjectService.GetObject<InitClass2>();
			Assert.IsNotNull(svr2);
			Assert.IsNotNull(svr2.Setting);
			Assert.AreEqual("2:123456", svr2.Setting.Value);
		}
	}
}
