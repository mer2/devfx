/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
{
	public class ObjectWrap : IObjectWrap
	{
		protected ObjectWrap(object value) {
			this.Value = value;
		}

		public object Value { get; private set; }

		public void ChangeValue(object value) {
			this.Value = value;
		}

		public static IObjectWrap Wrap(object value) {
			return new ObjectWrap(value);
		}
	}
}
