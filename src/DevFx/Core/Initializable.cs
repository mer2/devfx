namespace DevFx.Core
{
	public abstract class Initializable<TSetting> : IInitializable<TSetting> where TSetting : class
	{
		protected TSetting Setting { get; set; }
		protected virtual void OnInit(TSetting setting) {
		}

		void IInitializable<TSetting>.Init(TSetting setting) {
			this.Setting = setting;
			this.OnInit(setting);
		}
	}
}
