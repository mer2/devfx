/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	public abstract class ConfigService : ConfigServiceBase
	{
		#region Default Instance

		internal static IConfigService Default {
			get { return new DefaultConfigService(); }
		}

		internal class DefaultConfigService : ConfigService
		{
		}
		
		#endregion Default Instance

		#region Current Instance

		public static IConfigService Current {
			get { return DevFx.ObjectService.GetObject<IConfigService>(); }
		}

		#endregion Current Instance
	}
}
