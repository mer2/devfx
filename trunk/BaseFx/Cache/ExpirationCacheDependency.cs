/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Cache
{
	/// <summary>
	/// 时间过期的过期策略（包括相对时间过期、绝对时间过期）
	/// </summary>
	[Serializable]
	public class ExpirationCacheDependency : CacheDependency
	{
		/// <summary>
		/// 构造函数（绝对时间过期方式）
		/// </summary>
		/// <param name="expiration">绝对过期时间</param>
		public ExpirationCacheDependency(DateTime expiration) {
			this.AbsoluteExpiration = expiration;
			this.SlidingExpiration = TimeSpan.MaxValue;
			this.Sliding = false;
		}

		/// <summary>
		/// 构造函数（相对时间过期方式）
		/// </summary>
		/// <param name="expiration">相对过期时间</param>
		public ExpirationCacheDependency(TimeSpan expiration) {
			this.SlidingExpiration = expiration;
			this.AbsoluteExpiration = DateTime.Now.Add(expiration);
			this.Sliding = true;
		}

		/// <summary>
		/// 是否为相对过期
		/// </summary>
		public bool Sliding { get; set; }

		/// <summary>
		/// 绝对过期时间
		/// </summary>
		public DateTime AbsoluteExpiration { get; private set; }

		/// <summary>
		/// 相对过期时间
		/// </summary>
		public TimeSpan SlidingExpiration { get; private set; }

		/// <summary>
		/// 是否已过期
		/// </summary>
		protected override bool IsExpired {
			get { return (this.AbsoluteExpiration < DateTime.Now); }
		}

		/// <summary>
		/// 重置缓存策略（相当于重新开始缓存），针对于相对时间过期策略有效
		/// </summary>
		protected override void Reset() {
			if(this.Sliding) {
				this.AbsoluteExpiration = DateTime.Now.Add(this.SlidingExpiration);
			}
		}
	}
}
