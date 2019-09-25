/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DevFx.Utils;

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置节实现
	/// </summary>
	public abstract partial class ConfigSetting : IConfigSetting
	{
		#region static fields

		/// <summary>
		/// 配置节集合的设定，形如：{tag:'add', key:'name', nullable:'false'}
		/// </summary>
		public const string ConfigSetPropertyName = "configSet";

		/// <summary>
		/// 配置节来源的设定，形如：{file:'app.config', node:'/configuration/customSetting', provider:'DevFx.Configuration.XmlConfig, DevFx'}
		/// </summary>
		public const string ConfigSourcePropertyName = "configSource";

		#endregion static fields

		#region fields

		/// <summary>
		/// 是否只读
		/// </summary>
		private bool @readonly;
		/// <summary>
		/// 此配置节实际名称
		/// </summary>
		private string settingName;
		/// <summary>
		/// 配置值
		/// </summary>
		protected SettingValue value;
		/// <summary>
		/// 父配置节
		/// </summary>
		protected ConfigSetting parent;
		/// <summary>
		/// 配置属性
		/// </summary>
		protected SettingProperty property;
		/// <summary>
		/// 子配置节
		/// </summary>
		protected ConfigSettingCollection childSettings;
		/// <summary>
		/// 配置节命令集合
		/// </summary>
		protected ConfigSettingCollection operatorSettings;
		/// <summary>
		/// 配置节版本
		/// </summary>
		protected int version;

		#endregion fields

		#region abstract methods

		/// <summary>
		/// 创建配置节实例
		/// </summary>
		/// <returns></returns>
		protected abstract ConfigSetting CreateConfigSetting();

		/// <summary>
		/// 创建配置值
		/// </summary>
		/// <param name="name">配置值名</param>
		/// <param name="value">配置值</param>
		/// <param name="readonly">是否只读</param>
		/// <param name="values">多值</param>
		/// <returns>SettingValue</returns>
		protected abstract SettingValue CreateSettingValue(string name, string value, bool @readonly, object[] values);

		/// <summary>
		/// 创建配置属性实例
		/// </summary>
		/// <param name="readonly">是否只读</param>
		/// <returns>SettingProperty</returns>
		protected abstract SettingProperty CreateSettingProperty(bool @readonly);

		#endregion abstract methods

		#region members

		/// <summary>
		/// 转换成字符串
		/// </summary>
		/// <param name="sb"><see cref="StringBuilder"/></param>
		/// <param name="layerIndex">所处层次</param>
		protected virtual void ToString(StringBuilder sb, int layerIndex) {
			var layerString = new string('\t', layerIndex);
			sb.AppendFormat("{0}<{1}", layerString, this.SettingName);
			if (this.Property.Count > 0) {
				sb.AppendFormat(" {0}", this.Property);
			}
			if (this.childSettings.Count > 0) {
				sb.AppendFormat(">{0}\r\n", HttpUtility.HtmlAttributeEncode(this.Value.Value));
				foreach (var setting in this.childSettings.Values) {
					setting.ToString(sb, layerIndex + 1);
				}
				sb.AppendFormat("{0}</{1}>\r\n", layerString, this.SettingName);
			} else {
				if (this.Value.Value != null) {
					sb.AppendFormat(">{0}</{1}>\r\n", HttpUtility.HtmlAttributeEncode(this.Value.Value), this.SettingName);
				} else {
					sb.AppendLine(" />");
				}
			}
		}

		/// <summary>
		/// 创建配置节实例
		/// </summary>
		/// <param name="setting">被复制的配置节</param>
		/// <param name="deep">是否深度复制</param>
		/// <returns>配置节</returns>
		protected virtual ConfigSetting CreateConfigSetting(ConfigSetting setting, bool deep) {
			var newSetting = this.CreateConfigSetting();
			newSetting.@readonly = setting.ReadOnly;
			newSetting.settingName = setting.settingName;
			if (deep) {
				newSetting.value = setting.Value.Clone();
				newSetting.property = setting.Property.Clone(this.@readonly, true);
				newSetting.childSettings = setting.childSettings.Clone(newSetting);
				newSetting.operatorSettings = setting.operatorSettings.Clone(newSetting);
			} else {
				newSetting.value = setting.Value;
				newSetting.property = setting.Property;
				newSetting.childSettings = setting.childSettings;
				newSetting.operatorSettings = setting.operatorSettings;
			}
			return newSetting;
		}

		/// <summary>
		/// 当前配置节是否只读
		/// </summary>
		public virtual bool ReadOnly {
			get { return this.@readonly; }
			protected set { this.@readonly = value; }
		}

		/// <summary>
		/// 此配置节的名（逻辑名称，可能会根据配置命令而变化）
		/// </summary>
		public virtual string Name {
			get { return this.Value.Name; }
		}

		/// <summary>
		/// 此配置节实际名称（原始名称，可能为操作命令，比如“Add”等）
		/// </summary>
		public virtual string SettingName {
			get {
				if (!string.IsNullOrEmpty(this.settingName)) {
					return this.settingName;
				}
				return this.Name;
			}
			protected set { this.settingName = value; }
		}

		/// <summary>
		/// 此配置节的名/值
		/// </summary>
		public virtual SettingValue Value {
			get { return this.value; }
			set {
				if(this.ReadOnly) {
					throw new ConfigException("配置节只读");
				}
				this.value = value;
			}
		}

		/// <summary>
		/// 包含此配置节的父配置节
		/// </summary>
		public virtual ConfigSetting Parent {
			get { return this.parent; }
			set {
				if(this.parent != value) {
					this.parent = value;
					this.configSet = null;
				}
			}
		}

		/// <summary>
		/// 此配置节包含的子配置节数目
		/// </summary>
		public virtual int Children {
			get { return this.childSettings.Count; }
		}

		/// <summary>
		/// 配置节属性
		/// </summary>
		public virtual SettingProperty Property {
			get { return this.property; }
			set {
				if(this.ReadOnly) {
					throw new ConfigException("配置节只读");
				}
				this.property = value;
			}
		}

		/// <summary>
		/// 配置节版本
		/// </summary>
		public virtual int Version {
			get { return this.version; }
			set { this.version = value; }
		}

		#region ConfigFile

		/// <summary>
		/// 配置节内容来源
		/// </summary>
		internal class ConfigSourceInternal : IConfigSourceSetting
		{
			/// <summary>
			/// 配置节内容来源文件
			/// </summary>
			public string File { get; set; }
			/// <summary>
			/// 配置节内容来源文件中的节点
			/// </summary>
			public string Node { get; set; }
			/// <summary>
			/// 配置节内容提供者
			/// </summary>
			public string Provider { get; set; }

			private string source;
			/// <summary>
			/// 原始的配置信息
			/// </summary>
			string IConfigSourceSetting.Source {
				get { return this.source; }
			}

			/// <summary>
			/// 缺省实例
			/// </summary>
			public static ConfigSourceInternal Default {
				get { return new ConfigSourceInternal(); }
			}

			/// <summary>
			/// 从字符串中创建配置节内容来源
			/// </summary>
			/// <param name="configSource">内容来源字符串</param>
			/// <returns>配置节内容来源实例</returns>
			public static ConfigSourceInternal Create(string configSource) {
				var instance = WebHelper.FromJsonLite<ConfigSourceInternal>(configSource, (c, n, v) => {
					switch (n.ToLower()) {
						case "file":
							c.File = v;
							break;
						case "node":
							c.Node = v;
							break;
						case "provider":
							c.Provider = v;
							break;
					}
				}) ?? Default;
				instance.source = configSource;
				return instance;
			}
		}

		private ConfigSourceInternal configSource;
		/// <summary>
		/// 配置节的内容来源
		/// </summary>
		public virtual IConfigSourceSetting ConfigSource {
			get {
				if (this.configSource == null) {
					var configSource = this.Property.TryGetPropertyValue(ConfigSourcePropertyName);
					this.configSource = ConfigSourceInternal.Create(configSource);
				}
				return this.configSource;
			}
		}

		/// <summary>
		/// 获取此配置节的内容来源
		/// </summary>
		public virtual string ConfigFile {
			get { return this.ConfigSource.File; }
		}

		/// <summary>
		/// 获取此配置节内容来源的节点
		/// </summary>
		public virtual string ConfigNode {
			get { return this.ConfigSource.Node; }
		}

		/// <summary>
		/// 获取此配置节内容来源的提供者，此提供者必须有一个静态方法为<c>GetConfigSetting(IConfigSourceSetting sourceSetting)</c>，返回值是<see cref="IConfigSetting"/>实例
		/// </summary>
		public virtual string ConfigProvider {
			get { return this.ConfigSource.Provider; }
		}

		#endregion

		#region ConfigSet

		/// <summary>
		/// 集合配置
		/// </summary>
		internal class ConfigSetInternal : IConfigSetSetting
		{
			/// <summary>
			/// 配置节集合添加动作的标签
			/// </summary>
			public string Tag { get; set; } = "set";

			private string op = "set";
			/// <summary>
			/// 配置节集合动作类型名称
			/// </summary>
			public string Op {
				get { return this.op; }
				set {
					if (Converting.StringToEnum<ConfigSettingOperator>(value) != 0) {
						this.op = value;
					}
				}
			}

			/// <summary>
			/// 配置节集合键值属性名
			/// </summary>
			public string Key { get; set; } = "name";

			/// <summary>
			/// 配置节集合键值是否允许空
			/// </summary>
			public bool Nullable { get; set; }

			private string source;

			/// <summary>
			/// 缺省实例
			/// </summary>
			public static ConfigSetInternal Default {
				get { return new ConfigSetInternal(); }
			}

			/// <summary>
			/// 创建集合配置实例
			/// </summary>
			/// <param name="configSet">集合配置字符串</param>
			/// <returns>集合配置实例</returns>
			public static ConfigSetInternal Create(string configSet) {
				var instance = WebHelper.FromJsonLite<ConfigSetInternal>(configSet, (c, n, v) => {
					switch (n) {
						case "tag":
							c.Tag = v;
							break;
						case "key":
							c.Key = v;
							break;
						case "op":
							c.Op = v;
							break;
						case "nullable":
							c.Nullable = Convert.ToBoolean(v);
							break;
					}
				}) ?? Default;
				instance.source = configSet;
				return instance;
			}

			#region IConfigSetSetting Members

			ConfigSettingOperator IConfigSetSetting.Operator {
				get { return Converting.StringToEnum<ConfigSettingOperator>(this.Op); }
			}

			string IConfigSetSetting.KeyName {
				get { return this.Key; }
			}

			bool IConfigSetSetting.KeyNullable {
				get { return this.Nullable; }
			}

			string IConfigSetSetting.Source {
				get { return this.source; }
			}

			#endregion
		}

		private ConfigSetInternal configSet;
		/// <summary>
		/// 获取此配置对应的集合配置
		/// </summary>
		public virtual IConfigSetSetting ConfigSet {
			get {
				if(this.configSet == null) {
					string configSet = null;
					if(this.Parent != null) {
						configSet = this.Parent.Property.TryGetPropertyValue(ConfigSetPropertyName, string.Empty);
					}
					this.configSet = ConfigSetInternal.Create(configSet);
				}
				return this.configSet;
			}
		}

		/// <summary>
		/// 是否为配置节集合添加动作的标签（默认为add）
		/// </summary>
		public virtual bool IsConfigOperatorTag {
			get { return this.ConfigSet.Tag == this.SettingName; }
		}

		/// <summary>
		/// 配置节集合键值属性名（默认为name）
		/// </summary>
		public virtual string ConfigOperatorKey {
			get { return this.ConfigSet.KeyName; }
		}

		/// <summary>
		/// 配置节集合键值是否允许空
		/// </summary>
		public virtual bool ConfigKeyNullable {
			get { return this.ConfigSet.KeyNullable; }
		}

		/// <summary>
		/// 此节是否为配置节命令
		/// </summary>
		protected virtual ConfigSettingOperator SettingOperator {
			get {
				return this.IsConfigOperatorTag ? this.ConfigSet.Operator : Converting.StringToEnum<ConfigSettingOperator>(this.SettingName);
			}
		}

		#endregion

		/// <summary>
		/// 获取配置节的名称（逻辑名称，可能会根据配置命令而变化）
		/// </summary>
		/// <returns></returns>
		public virtual string GetName() {
			var name = this.Name;
			if (this.IsConfigOperatorTag) {
				var newName = this.Property.TryGetPropertyValue(this.ConfigOperatorKey);
				if(this.ConfigSet.KeyNullable || !string.IsNullOrEmpty(newName)) {
					this.Value.SetName(newName);
					name = newName;
				}
			}
			return name;
		}

		/// <summary>
		/// 增加对配置节路径的"@"支持，表明使用配置节名（非逻辑名）来匹配
		/// </summary>
		internal virtual ConfigSetting GetChildSettingInternal(string settingName) {
			if (!string.IsNullOrEmpty(settingName) && settingName.StartsWith("@")) {
				settingName = settingName.Substring(1);
				foreach (var setting in this.childSettings.Values) {
					if (setting.SettingName == settingName) {
						return setting;
					}
				}
				return null;
			}
			return this.childSettings[settingName];
		}

		/// <summary>
		/// 获取/设置子配置节
		/// </summary>
		/// <param name="childSettingName">子配置节名</param>
		/// <remarks>
		/// 如果不存在，将返回<c>null</c><br />
		/// 如果设置时存在相同的节，则替换
		/// </remarks>
		public virtual ConfigSetting this[string childSettingName] {
			get { return this.GetChildSettingInternal(childSettingName); }
		}

		/// <summary>
		/// 获取子配置节
		/// </summary>
		/// <param name="childSettingIndex">子配置节顺序</param>
		/// <remarks>
		/// 如果不存在，将返回null
		/// </remarks>
		public virtual ConfigSetting this[int childSettingIndex] {
			get { return this.childSettings[childSettingIndex]; }
		}

		/// <summary>
		/// 获取所有子配置节
		/// </summary>
		/// <returns>配置节数组</returns>
		public virtual ConfigSetting[] GetChildSettings() {
			return this.childSettings.CopyToArray();
		}

		/// <summary>
		/// 按XPath方式获取配置节
		/// </summary>
		/// <param name="xpath">XPath</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// XPath为类似XML的XPath，形如<c>framework/modules/module"</c><br />
		/// 如果有相同的配置节，则返回第一个配置节
		/// </remarks>
		public virtual ConfigSetting GetChildSetting(string xpath) {
			string[] settingNames = null;
			if(xpath != null) {
				if(xpath.StartsWith("/")) {
					xpath = xpath.Substring(1);
				} else if(xpath.StartsWith("~/")) {
					xpath = ConfigHelper.RootSettingName + xpath.Substring(1);
				}
				settingNames = xpath.Split('/');
			}
			return this.GetChildSetting(settingNames);
		}

		/// <summary>
		/// 按多级方式获取配置节
		/// </summary>
		/// <param name="settingNames">多级的配置节名</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// 多级的配置节名，形如有如下配置：
		///		<code>
		///			&lt;app1&gt;
		///				&lt;app2&gt;
		///					&lt;app3&gt;&lt;/app3&gt;
		///				&lt;/app2&gt;
		///			&lt;/app1&gt;
		///		</code>
		///	则按顺序传入，比如<c>GetChildSetting("app1", "app2", "app3")</c>，此时返回名为<c>app3</c>的配置节<br />
		/// "."表示当前配置节，".."表示上级配置节
		/// </remarks>
		public virtual ConfigSetting GetChildSetting(params string[] settingNames) {
			var setting = this;
			if(settingNames != null && settingNames.Length > 0) {
				for(var i = 0; setting != null && i < settingNames.Length; i++) {
					var name = settingNames[i];
					switch (name) {
						case ".":
						case null:
							break;
						case "..":
							setting = setting.Parent;
							break;
						default:
							setting = setting[name];
							break;
					}
				}
			}
			return setting;
		}

		/// <summary>
		/// 获取指定名称的子配置节
		/// </summary>
		/// <param name="childSettingName">子配置节名称</param>
		/// <returns>配置节数组</returns>
		public virtual ConfigSetting[] GetChildSettings(string childSettingName) {
			var values = this.childSettings.Values;
			if (childSettingName == null) {
				return values;
			}
			var list = new List<ConfigSetting>();
			foreach (var setting in values) {
				if (setting.SettingName == childSettingName) {
					list.Add(setting);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 克隆此配置节
		/// </summary>
		/// <param name="readonly">是否只读</param>
		/// <param name="deep">是否深层次的克隆</param>
		/// <returns>配置节</returns>
		public virtual ConfigSetting Clone(bool @readonly, bool deep) {
			ConfigSetting setting = this.CreateConfigSetting(this, deep);
			setting.@readonly = @readonly;
			return setting;
		}

		/// <summary>
		/// 克隆此配置节
		/// </summary>
		/// <returns>配置节</returns>
		public virtual ConfigSetting Clone() {
			return this.Clone(this.ReadOnly, true);
		}

		/// <summary>
		/// 合并配置节
		/// </summary>
		/// <param name="setting">需被合并的配置节</param>
		/// <returns>合并后的配置节</returns>
		public virtual ConfigSetting Merge(ConfigSetting setting) {
			if(setting == null || string.Compare(this.Name, setting.Name, StringComparison.OrdinalIgnoreCase) != 0) {
				return this;
			}
			this.Version++;
			this.Property.Merge(setting.Property);
			this.value = setting.Value.Clone(this.ReadOnly);

			foreach(var configSetting in setting.operatorSettings.Values) {
				this.operatorSettings.Add(configSetting).Parent = this;
			}

			Compile(this, setting.operatorSettings);
			return this;
		}

		internal virtual ConfigSetting CopyFrom(ConfigSetting setting) {
			this.Version++;
			this.Property.CopyFrom(setting.Property);

			var opts = this.operatorSettings;
			this.operatorSettings =	setting.operatorSettings.Clone(this);
			this.childSettings = setting.childSettings.Clone(this);
			Compile(this, opts);

			return this;
		}

		/// <summary>
		/// 获取配置命令集合（为后续配置合并提供支持）
		/// </summary>
		/// <returns>配置节集合</returns>
		public virtual ConfigSettingCollection GetOperatorSettings() {
			return this.operatorSettings.Clone(this);
		}

		/// <summary>
		/// 编译本配置节，将执行一些配置命令，具有配置命令的配置节需执行本方法后才可以使用
		/// </summary>
		/// <param name="current">当前配置节</param>
		/// <param name="settings">配置命令集合</param>
		/// <returns>编译后的配置节</returns>
		protected static ConfigSetting Compile(ConfigSetting current, ConfigSettingCollection settings) {
			if (settings.Count > 0) {
				var currentSettings = current.childSettings;
				for (var i = 0; i < settings.Count; i++) {
					var setting = settings[i].Clone();
					setting.Parent = current;
					var logicName = setting.GetName();
					switch(setting.SettingOperator) {
						case ConfigSettingOperator.Add:
							if (currentSettings.Contains(logicName)) {
								throw new ConfigException(string.Format("已存在子配置节 {0}", logicName));
							}
							currentSettings.Add(setting).Parent = current;
							break;
						case ConfigSettingOperator.Remove:
							currentSettings.Remove(logicName);
							break;
						case ConfigSettingOperator.Move:
							var moveSetting = currentSettings[logicName];
							if(moveSetting != null) {
								currentSettings.Remove(logicName);
								currentSettings.Add(moveSetting);
							}
							break;
						case ConfigSettingOperator.Clear:
							currentSettings.Clear();
							break;
						case ConfigSettingOperator.Update:
							if (currentSettings.Contains(logicName)) {
								currentSettings[logicName].Merge(setting);
							}
							break;
						default:
							if (currentSettings.Contains(logicName)) {
								currentSettings[logicName].Merge(setting);
							} else {
								currentSettings.Add(setting).Parent = current;
							}
							break;
					}
				}
			}
			return current;
		}

		/// <summary>
		/// 获取根配置节
		/// </summary>
		/// <returns>配置节</returns>
		public virtual ConfigSetting GetRootSetting() {
			var root = this;
			while(root.Parent != null) {
				root = root.Parent;
			}
			return root;
		}

		/// <summary>
		/// 转换成字符串格式
		/// </summary>
		/// <returns>字符串</returns>
		public override string ToString() {
			var sb = new StringBuilder();
			this.ToString(sb, 0);
			return sb.ToString();
		}

		#endregion members

		#region ICloneable Members

		object ICloneable.Clone() {
			return this.Clone(this.ReadOnly, true);
		}

		#endregion

		#region IConfigSetting Members

		bool IConfigSetting.ReadOnly {
			get { return this.ReadOnly; }
		}

		string IConfigSetting.Name {
			get { return this.Name; }
		}

		string IConfigSetting.SettingName {
			get { return this.SettingName; }
		}

		ISettingValue IConfigSetting.Value {
			get { return this.Value; }
		}

		IConfigSetting IConfigSetting.Parent {
			get { return this.Parent; }
		}

		int IConfigSetting.Children {
			get { return this.Children; }
		}

		ISettingProperty IConfigSetting.Property {
			get { return this.Property; }
		}

		int IConfigSetting.Version {
			get { return this.Version; }
		}

		IConfigSetting IConfigSetting.this[string childSettingName] {
			get { return this[childSettingName]; }
		}

		IConfigSetting IConfigSetting.this[int childSettingIndex] {
			get { return this[childSettingIndex]; }
		}

		IConfigSetting[] IConfigSetting.GetChildSettings() {
			return this.GetChildSettings();
		}

		IConfigSetting[] IConfigSetting.GetChildSettings(string childSettingName) {
			return this.GetChildSettings(childSettingName);
		}

		IConfigSetting IConfigSetting.GetChildSetting(string xpath) {
			return this.GetChildSetting(xpath);
		}

		IConfigSetting IConfigSetting.GetChildSetting(params string[] settingName) {
			return this.GetChildSetting(settingName);
		}

		IConfigSetting IConfigSetting.Clone(bool @readonly, bool deep) {
			return this.Clone(@readonly, deep);
		}

		IConfigSetting IConfigSetting.GetRootSetting() {
			return this.GetRootSetting();
		}

		IConfigSetting IConfigSetting.Merge(IConfigSetting setting) {
			return this.Merge((ConfigSetting)setting);
		}

		IConfigSettingCollection IConfigSetting.GetOperatorSettings() {
			return this.GetOperatorSettings();
		}

		IConfigSetting IConfigSetting.SetParent(IConfigSetting parent) {
			this.Parent = (ConfigSetting)parent;
			return this;
		}

		string IConfigSetting.GetName() {
			return this.GetName();
		}

		#endregion
	}
}