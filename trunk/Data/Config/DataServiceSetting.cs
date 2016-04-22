/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Data;
using HTB.DevFx.Config;
using HTB.DevFx.Utils;

[assembly: ConfigResource("res://HTB.DevFx.Data.Config.htb.devfx.data.config", Index = 0)]

namespace HTB.DevFx.Data.Config
{
	internal class DataServiceSetting : ConfigSettingElement, IDataServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Debug = this.GetSetting("debug", false);
			this.ResultHandlerFactoryContext = this.GetSetting<ResultHandlerFactoryContextSetting>("results");
			this.DataStorageContext = this.GetSetting<DataStorageContextSetting>("dataStorages");
		}

		public bool Debug { get; private set; }
		public IResultHandlerFactoryContextSetting ResultHandlerFactoryContext { get; private set; }
		public IDataStorageContextSetting DataStorageContext { get; private set; }

		public IStatementContextSetting[] StatementContexts {
			get { return this.GetCachedSettingsWithInitializer<StatementContextSetting>("statements", x => x.DataStorageName = x.DataStorageName ?? this.DataStorageContext.DefaultStorageName); }
		}
	}

	internal class DataStorageContextSetting : ConfigSettingElement, IDataStorageContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.DefaultStorageName = this.GetSetting("defaultStorage");
			this.DataStorages = this.GetSettings<DataStorageSetting>(null).ToArray();
		}

		public string DefaultStorageName { get; private set; }
		public IDataStorageSetting[] DataStorages { get; private set; }
	}

	internal class DataStorageSetting : ConfigSettingElement, IDataStorageSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting("name");
			this.ConnectionString = this.GetSetting("connectionString");
			this.StorageTypeName = this.GetSetting("type");
		}

		public string Name { get; private set; }
		public string ConnectionString { get; private set; }
		public string StorageTypeName { get; private set; }
	}

	internal class StatementContextSetting : ConfigSettingElement, IStatementContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting("name");
			this.DataStorageName = this.GetSetting("dataStorage");
		}

		public string Name { get; private set; }
		public string DataStorageName { get; set; }

		public IStatementSetting[] Statements {
			get { return this.GetCachedSettingsWithInitializer<StatementSetting>("add", x => x.DataStorageName = x.DataStorageName ?? this.DataStorageName); }
		}
	}

	internal class StatementSetting : ConfigSettingElement, IStatementSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting("name");
			this.CommandType = this.GetSetting("commandType", CommandType.Text);
			this.Timeout = this.GetSetting("timeout", 30);
			this.ResultTypeName = this.GetSetting("resultType");
			this.ResultHandlerName = this.GetSetting("resultHandler");
			this.Parameters = this.GetSettings<ParameterSetting>("parameters", null).ToArray();
			this.StatementText = this.GetSetting<StatementTextSetting>("commandText");
			this.DataStorageName = this.GetSetting("dataStorage");
			this.CommandBuilderTypeName = this.GetSetting("builderType");
		}

		public string Name { get; private set; }
		public CommandType CommandType { get; private set; }
		public int Timeout { get; private set; }
		public string ResultTypeName { get; private set; }
		public string ResultHandlerName { get; private set; }
		public IParameterSetting[] Parameters { get; private set; }
		public IStatementTextSetting StatementText { get; private set; }
		public string DataStorageName { get; set; }
		public string CommandBuilderTypeName { get; private set; }
	}

	internal class StatementTextSetting : ConfigSettingElement, IStatementTextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.CommandTextParser = this.GetObject<ICommandTextParser>("parser");
		}

		private ICommandTextParser CommandTextParser { get; set; }
		public string GetCommandText(IStatementSetting statement, IDictionary parameters) {
			return this.CommandTextParser.GetCommandText(statement, parameters);
		}
	}

	internal class DynamicTextSetting : ConfigSettingElement, IDynamicTextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.ParameterName = this.GetSetting("parameter");
			this.IsNull = this.GetSetting<bool?>("null");
			this.IsPresent = this.GetSetting<bool?>("present");
			this.DynamicTexts = this.GetSettings<DynamicTextSetting>("if").ToArray();
		}

		public string ParameterName { get; private set; }
		public bool? IsNull { get; private set; }
		public bool? IsPresent { get; private set; }
		public IDynamicTextSetting[] DynamicTexts { get; private set; }
	}

	internal class ParameterSetting : ConfigSettingElement, IParameterSetting
	{
		public override IConfigSetting ConfigSetting {
			get { return base.ConfigSetting; }
			protected set {
				var setting = value;
				var refName = setting.GetSetting("ref");
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
			this.Name = this.GetSetting("name");
			this.ParameterName = this.GetSetting("parameterName");
			this.ParameterTypeName = this.GetSetting("type");
			this.IsInline = this.GetSetting("inline", false);
			this.Expandable = this.GetSetting("expandable", false);
			this.DbTypeName = this.GetSetting("dbType");
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
			return this.GetObjectContext("defaultValue", () => {
				var value = this.GetSetting("defaultValue");
				return !string.IsNullOrEmpty(value) ? Converting.GetConvert(value).TryToObject(type) : null;
			});
		}
	}

	internal class PropertyMapSetting : ConfigSettingElement, IPropertyMapSetting
	{
		protected override void OnConfigSettingChanged() {
			this.PropertyName = this.GetSetting("name");
			this.ColumnName = this.GetSetting("column");
			this.DefaultValue = this.GetObject("defaultValue", false);
		}

		public string PropertyName { get; private set; }
		public string ColumnName { get; private set; }
		public object DefaultValue { get; private set; }
	}

	internal class ObjectMapSetting : ConfigSettingElement, IObjectMapSetting
	{
		protected override void OnConfigSettingChanged() {
			this.IncludeProperties = this.GetSetting("includeProperties", "*");
			this.ExcludeProperties = this.GetSetting("excludeProperties");
			this.TypeTranslatorName = this.GetSetting("typeTranslator");
			this.IgnoreCase = this.GetSetting("ignoreCase", true);
			this.Properties = this.GetSettings<PropertyMapSetting>("property").ToArray();
		}

		public string IncludeProperties { get; private set; }
		public string ExcludeProperties { get; private set; }
		public string TypeTranslatorName { get; private set; }
		public bool IgnoreCase { get; private set; }
		public IPropertyMapSetting[] Properties { get; private set; }
	}

	internal class ResultHandlerFactoryContextSetting : ConfigSettingElement, IResultHandlerFactoryContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.FactoryTypeName = this.GetSetting("handlerFactory");
			this.ModuleEnabled = this.GetSetting("moduleEnabled", false);
			this.ResultModules = this.GetSettings<ResultModuleSetting>("modules", null).ToArray();
			this.ResultHandlerContext = this.GetSetting<ResultHandlerContextSetting>("handlers");
		}

		public string FactoryTypeName { get; private set; }
		public bool ModuleEnabled { get; private set; }
		public IResultModuleSetting[] ResultModules { get; private set; }
		public IResultHandlerContextSetting ResultHandlerContext { get; private set; }
	}

	internal class ResultHandlerContextSetting : ConfigSettingElement, IResultHandlerContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.DefaultHandlerName = this.GetSetting("defaultHandler");
			this.ResultHandlers = this.GetSettings<ResultHandlerSetting>(null).ToArray();
		}

		public string DefaultHandlerName { get; private set; }
		public IResultHandlerSetting[] ResultHandlers { get; private set; }
	}

	internal class ResultHandlerSetting : ConfigSettingElement, IResultHandlerSetting
	{
		protected override void OnConfigSettingChanged() {
			this.ResultTypeName = this.GetSetting("type");
			this.ResultHandlerName = this.GetSetting("handler");
		}

		public string ResultTypeName { get; private set; }
		public string ResultHandlerName { get; private set; }
	}

	internal class ResultModuleSetting: ConfigSettingElement, IResultModuleSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
			this.Enabled = this.GetSetting("enabled", true);
		}

		public string Name { get; private set; }
		public string TypeName { get; private set; }
		public bool Enabled { get; private set; }
	}
}
