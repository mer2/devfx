/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using DevFx.Configuration;
using DevFx.Utils;
[assembly: ConfigResource("res://DevFx.Data.Settings.Settings.config", Index = 2000)]

namespace DevFx.Data.Settings
{
	[SettingObject("~/data", Required = true)]
	internal class DataServiceSetting : ConfigSettingElement, IDataServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Debug = this.GetSetting("debug", false);
			this.ResultHandlerFactoryContext = this.GetSetting<ResultHandlerFactoryContextSetting>("results");
			this.ConnectionStrings = this.GetSettings<ConnectionStringSetting>("connectionStrings", null).ToArray();
			this.DataStorageContext = this.GetSetting<DataStorageContextSetting>("dataStorages");
			var contexts = this.GetSettings<StatementContextSetting>("statements").ToArray();
			if(contexts != null && contexts.Length > 0) {
				foreach(var ctx in contexts) {
					ctx.SetDataStorageNameIfNull(this.DataStorageContext?.DefaultStorageName);
				}
			}
			this.StatementContexts = contexts;
		}

		public bool Debug { get; private set; }
		public IResultHandlerFactoryContextSetting ResultHandlerFactoryContext { get; private set; }
		public IConnectionStringSetting[] ConnectionStrings { get; private set; }
		public IDataStorageContextSetting DataStorageContext { get; private set; }
		public IStatementContextSetting[] StatementContexts { get; private set; }
	}

	internal class ConnectionStringSetting : ConfigSettingElement, IConnectionStringSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting<string>("name", required:true);
			this.ConnectionString = this.GetSetting<string>("connectionString", required: true);
			this.ProviderName = this.GetSetting<string>("providerName", required: true);
		}

		public string Name { get; private set; }
		public string ConnectionString { get; private set; }
		public string ProviderName { get; private set; }
	}

	internal class DataStorageContextSetting : ConfigSettingElement, IDataStorageContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.DefaultStorageName = this.GetSetting<string>("defaultStorage");
			this.DataStorages = this.GetSettings<DataStorageSetting>(null).ToArray();
		}

		public string DefaultStorageName { get; private set; }
		public IDataStorageSetting[] DataStorages { get; private set; }
	}

	internal class DataStorageSetting : ConfigSettingElement, IDataStorageSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting<string>("name");
			this.ConnectionName = this.GetSetting<string>("connectionName");
			this.StorageTypeName = this.GetSetting<string>("type");
		}

		public string Name { get; private set; }
		public string ConnectionName { get; private set; }
		public string StorageTypeName { get; private set; }
	}

	internal class StatementContextSetting : ConfigSettingElement, IStatementContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting<string>("name");
			this.DataStorageName = this.GetSetting<string>("dataStorage");
			this.Statements = this.GetSettings<StatementSetting>("add").ToArray();
		}

		public string Name { get; private set; }
		public string DataStorageName { get; private set; }
		public IStatementSetting[] Statements { get; private set; }

		public void SetDataStorageNameIfNull(string name) {
			if(this.DataStorageName == null) {
				this.DataStorageName = name;
				if(this.Statements != null && this.Statements.Length > 0) {
					foreach(StatementSetting statement in this.Statements) {
						statement.SetDataStorageNameIfNull(name);
					}
				}
			}
		}
	}

	internal class StatementSetting : ConfigSettingElement, IStatementSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting<string>("name");
			this.CommandType = this.GetSetting("sqlType", CommandType.Text);
			this.Timeout = this.GetSetting("timeout", 30);
			this.ResultTypeName = this.GetSetting<string>("resultType");
			this.ResultHandlerName = this.GetSetting<string>("resultHandler");
			this.AutoParameters = this.GetSetting("autoParameters", true);
			this.Parameters = this.GetSettings<ParameterSetting>("parameters", null).ToArray();
			this.StatementText = this.GetSetting<StatementTextSetting>("sql");
			this.DataStorageName = this.GetSetting<string>("dataStorage");
			this.CommandBuilderTypeName = this.GetSetting<string>("builderType");
		}

		public string Name { get; private set; }
		public CommandType CommandType { get; private set; }
		public int Timeout { get; private set; }
		public string ResultTypeName { get; private set; }
		public string ResultHandlerName { get; private set; }
		public bool AutoParameters { get; private set; }
		public IParameterSetting[] Parameters { get; private set; }
		public IStatementTextSetting StatementText { get; private set; }
		public string DataStorageName { get; private set; }
		public string CommandBuilderTypeName { get; private set; }

		public void SetDataStorageNameIfNull(string name) {
			if (this.DataStorageName == null) {
				this.DataStorageName = name;
			}
		}
	}

	//用于仅由Sql组成的Sql状态配置
	internal class SqlStatementSetting : IStatementSetting
	{
		public SqlStatementSetting(string name, string sql, string storageName, string parserName = null) {
			this.Name = name;
			this.CommandType = CommandType.Text;
			this.Timeout = 30;
			this.ResultTypeName = null;
			this.ResultHandlerName = null;
			this.AutoParameters = true;
			this.Parameters = null;
			this.StatementText = new SqlStatementTextSetting(sql, parserName);
			this.DataStorageName = storageName;
			this.CommandBuilderTypeName = null;
		}

		public string Name { get; }
		public CommandType CommandType { get; internal set; }
		public int Timeout { get; internal set; }
		public string ResultTypeName { get; internal set; }
		public string ResultHandlerName { get; internal set; }
		public bool AutoParameters { get; }
		public IParameterSetting[] Parameters { get; }
		public IStatementTextSetting StatementText { get; }
		public string DataStorageName { get; set; }
		public string CommandBuilderTypeName { get; }
	}

	//配合SqlStatementSetting
	internal class SqlStatementTextSetting : IStatementTextSetting
	{
		public SqlStatementTextSetting(string sql, string parseName) {
			this.CommandText = sql;
			this.ParseName = parseName;
			if (!string.IsNullOrEmpty(parseName)) {
				this.Parser = ObjectService.Current.GetObject<ICommandTextParser>(parseName);
			}
		}

		public string CommandText { get; }
		public string ParseName { get; }
		public ICommandTextParser Parser { get; }
	}

	internal class StatementTextSetting : ConfigSettingElement, IStatementTextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.CommandText = this.ConfigSetting.Value.Value;
			this.ParseName = this.GetSetting<string>("parser");
			if (!string.IsNullOrEmpty(this.ParseName)) {
				this.Parser = ObjectService.Current.GetObject<ICommandTextParser>(this.ParseName);
			}
		}

		public string CommandText { get; private set; }
		public string ParseName { get; private set; }
		public ICommandTextParser Parser { get; private set; }
	}

	internal class ParameterSetting : ConfigSettingElement, IParameterSetting
	{
		public override IConfigSetting ConfigSetting {
			get => base.ConfigSetting;
			protected set {
				var setting = value;
				var refName = setting.GetSetting<string>("ref");
				if (!string.IsNullOrEmpty(refName)) {
					var refSetting = setting.GetChildSetting("../../../templates/parameters/" + refName);
					if (refSetting != null) {
						setting = setting.CopyFrom(refSetting);
					}
				}
				base.ConfigSetting = setting;
			}
		}
		
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting<string>("name");
			this.ParameterName = this.GetSetting<string>("parameterName");
			if (string.IsNullOrEmpty(this.ParameterName)) {
				this.ParameterName = "@"+  this.Name;//TODO：这里的参数名统一加@前缀？
			}
			this.ParameterTypeName = this.GetSetting<string>("type");
			this.IsInline = this.GetSetting("inline", false);
			this.Expandable = this.GetSetting("expandable", false);
			this.DbTypeName = this.GetSetting<string>("dbType");
			this.Direction = this.GetSetting("direction", ParameterDirection.Input);
			this.IsNullable = this.GetSetting("nullable", true);
			this.Size = this.GetSetting("size", 0);
			this.Scale = this.GetSetting<byte>("scale", 0);
			this.Precision = this.GetSetting<byte>("precision", 0);
		}

		public string Name { get; private set; }
		public string ParameterName { get; private set; }
		public string ParameterTypeName { get; private set; }
		public bool IsInline { get; private set; }
		public bool Expandable { get; private set; }
		public string DbTypeName { get; private set; }
		public ParameterDirection Direction { get; private set; }
		public bool IsNullable { get; private set; }
		public int Size { get; private set; }
		public byte Scale { get; private set; }
		public byte Precision { get; private set; }

		public object GetDefaultValue(Type type) {
			var value = this.GetSetting<string>("defaultValue");
			return !string.IsNullOrEmpty(value) && type != null ? Converting.GetConvert(value).TryToObject(type) : null;
		}
	}

	internal class ResultHandlerFactoryContextSetting : ConfigSettingElement, IResultHandlerFactoryContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.FactoryTypeName = this.GetSetting<string>("handlerFactory");
			this.ModuleEnabled = this.GetSetting("moduleEnabled", false);
			this.DefaultHandlerName = this.GetSetting<string>("defaultHandler");
		}

		public string FactoryTypeName { get; private set; }
		public bool ModuleEnabled { get; private set; }
		public string DefaultHandlerName { get; private set; }
	}
}
