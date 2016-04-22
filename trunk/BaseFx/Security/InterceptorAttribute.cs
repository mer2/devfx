/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Security;
using System.Security.Permissions;

namespace HTB.DevFx.Security
{
	/// <summary>
	/// 拦截属性
	/// </summary>
	[Serializable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class InterceptorAttribute : CodeAccessSecurityAttribute, IPermission
	{
		/// <summary>
		/// 保护构造方法，用于调用基类的构造方法
		/// </summary>
		/// <param name="action"><see cref="SecurityAction"/></param>
		protected InterceptorAttribute(SecurityAction action) : base(action) { }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="action">拦截动作，目前只有<see cref="InterceptorAction.Demand"/></param>
		public InterceptorAttribute(InterceptorAction action) : this((SecurityAction)action) {
		}

		/// <summary>
		/// 拦截执行的方法
		/// </summary>
		protected virtual void Demand() {
		}

		#region Overrides of SecurityAttribute

		///<summary>
		///When overridden in a derived class, creates a permission object that can then be serialized into binary form and persistently stored along with the <see cref="T:System.Security.Permissions.SecurityAction" /> in an assembly's metadata.
		///</summary>
		///<returns>
		///A serializable permission object.
		///</returns>
		public override IPermission CreatePermission() {
			return this;
		}

		#endregion

		#region IPermission Members

		IPermission IPermission.Copy() {
			throw new NotImplementedException();
		}

		void IPermission.Demand() {
			this.Demand();
		}

		IPermission IPermission.Intersect(IPermission target) {
			throw new NotImplementedException();
		}

		bool IPermission.IsSubsetOf(IPermission target) {
			throw new NotImplementedException();
		}

		IPermission IPermission.Union(IPermission target) {
			throw new NotImplementedException();
		}

		#endregion

		#region ISecurityEncodable Members

		void ISecurityEncodable.FromXml(SecurityElement e) {
			throw new NotImplementedException();
		}

		SecurityElement ISecurityEncodable.ToXml() {
			throw new NotImplementedException();
		}

		#endregion
	}
}
