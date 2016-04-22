/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Reflection;
using System.Web.UI.WebControls;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data.Utils
{
	/// <summary>
	/// 关于对象和数据存储转换的一些有用的方法
	/// </summary>
	public sealed class DataTransfer
	{
		#region internal static members
		
		internal const BindingFlags ColumnBindingFlags = FieldMemberInfo.FieldBindingFlags;

		#endregion internal static members

		#region TransObject

		#region Objects to Data

		/// <summary>
		/// 对象到DataTable的转换
		/// </summary>
		/// <param name="objs">对象列表（类型一致的对象）</param>
		/// <param name="dt">DataTable</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		public static void TransObject<T>(T[] objs, DataTable dt, BindingFlags columnBindingFlags) {
			if(objs == null || objs.Length == 0 || dt == null) {
				return;
			}
			Type type = typeof(T);
			IFieldMemberInfo[] columns = FieldMemberInfo.GetFieldMembers(type, columnBindingFlags);
			for(int i = 0; i< objs.Length; i++) {
				DataRow dr = dt.NewRow();
				for(int k = 0; k < columns.Length; k++) {
					if(columns[k].CanRead) {
						dr[columns[k].Name] = columns[k].GetValue(objs[i]);
					}
				}
				dt.Rows.Add(dr);
			}
		}

		/// <summary>
		/// 对象到DataTable的转换
		/// </summary>
		/// <param name="objs">对象列表（类型一致的对象）</param>
		/// <param name="dt">DataTable</param>
		public static void TransObject<T>(T[] objs, DataTable dt) {
			TransObject<T>(objs, dt, ColumnBindingFlags);
		}

		/// <summary>
		/// 对象到DataTable的转换
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="dt">DataTable</param>
		public static void TransObject<T>(T obj, DataTable dt) {
			TransObject<T>(new T[] { obj }, dt);
		}

		/// <summary>
		/// 对象到DataTable的转换
		/// </summary>
		/// <param name="obj">对象</param>
		/// <param name="dr">DataRow</param>
		public static void TransObject<T>(T obj, DataRow dr) {
			if (obj == null ||  dr == null) {
				return;
			}
			Type type = typeof(T);
			IFieldMemberInfo[] columns = FieldMemberInfo.GetFieldMembers(type, ColumnBindingFlags);
			for (int k = 0; k < columns.Length; k++) {
				if (columns[k].CanRead) {
					dr[columns[k].Name] = columns[k].GetValue(obj);
				}
			}
		}

		#endregion Objects to Data

		#region Data to Objects

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		public static void TransObject<T>(DataTable dt, T[] objs, BindingFlags columnBindingFlags) {
			if(objs == null || objs.Length == 0 || dt == null || dt.Rows.Count == 0) {
				return;
			}
			TransObject<T>(dt.Select(), objs, columnBindingFlags);
		}

		/// <summary>
		/// DataRow到对象的转换
		/// </summary>
		/// <param name="drs">DataRow数组，必须是有相同架构（Schema）的DataRow</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		public static void TransObject<T>(DataRow[] drs, T[] objs) {
			TransObject<T>(drs, objs, ColumnBindingFlags);
		}

		/// <summary>
		/// DataRow到对象的转换
		/// </summary>
		/// <param name="drs">DataRow数组，必须是有相同架构（Schema）的DataRow</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		public static void TransObject<T>(DataRow[] drs, T[] objs, BindingFlags columnBindingFlags) {
			if(objs == null || objs.Length == 0 || drs == null || drs.Length == 0) {
				return;
			}
			int num = drs.Length;
			IDataRecord[] dataRecords = new DataRecord[num];
			for (int i = 0; i < num; i++) {
				dataRecords[i] = new DataRecord(drs[i]);
			}
			TransObject<T>(dataRecords, objs, columnBindingFlags);
		}

		/// <summary>
		/// IDataRecord到对象的转换
		/// </summary>
		/// <typeparam name="T">需要被转换的类型</typeparam>
		/// <param name="drs">IDataRecord数组</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		public static void TransObject<T>(IDataRecord[] drs, T[] objs, BindingFlags columnBindingFlags) {
			if (objs == null || objs.Length == 0 || drs == null || drs.Length == 0) {
				return;
			}
			int num = drs.Length;
			if (objs.Length < num) {
				num = objs.Length;
			}
			Type type = typeof(T);
			IFieldMemberInfo[] columns = FieldMemberInfo.GetFieldMembers(type, columnBindingFlags);
			for (int i = 0; i < num; i++) {
				IDataRecord dr = drs[i];
				for (int k = 0; k < columns.Length; k++) {
					if (columns[k].CanWrite) {
						columns[k].SetValue(objs[i], dr[columns[k].Name]);
					}
				}
			}
		}

		/// <summary>
		/// IDataRecord到对象的转换
		/// </summary>
		/// <typeparam name="T">需要被转换的类型</typeparam>
		/// <param name="drs">IDataRecord数组</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		public static void TransObject<T>(IDataRecord[] drs, T[] objs) {
			TransObject<T>(drs, objs, ColumnBindingFlags);
		}

		/// <summary>
		/// IDataRecord到对象的转换
		/// </summary>
		/// <typeparam name="T">需要被转换的类型</typeparam>
		/// <param name="drs">IDataRecord数组</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		/// <returns>转换后的对象列表</returns>
		public static T[] TransObject<T>(IDataRecord[] drs, BindingFlags columnBindingFlags) {
			if (drs == null || drs.Length == 0) {
				return null;
			}
			Type type = typeof(T);
			int num = drs.Length;
			Array objs = Array.CreateInstance(type, num);
			for (int i = 0; i < num; i++) {
				objs.SetValue(TypeHelper.CreateObject(type, null, true), i);
			}
			TransObject<T>(drs, (T[])objs, columnBindingFlags);
			return (T[])objs;
		}

		/// <summary>
		/// IDataRecord到对象的转换
		/// </summary>
		/// <typeparam name="T">需要被转换的类型</typeparam>
		/// <param name="drs">IDataRecord数组</param>
		/// <returns>转换后的对象列表</returns>
		public static T[] TransObject<T>(IDataRecord[] drs) {
			return TransObject<T>(drs, ColumnBindingFlags);
		}
		
		/// <summary>
		/// IDataRecord到对象的转换
		/// </summary>
		/// <typeparam name="T">需要被转换的类型</typeparam>
		/// <param name="dr">IDataRecord</param>
		/// <returns>转换后的对象</returns>
		public static T TransObject<T>(IDataRecord dr) {
			T[] objs = TransObject<T>(new IDataRecord[] {dr});
			if(objs != null && objs.Length > 0) {
				return objs[0];
			} else {
				return default(T);
			}
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="type">需要转换的对象类型</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static Array TransObject(DataTable dt, Type type, BindingFlags columnBindingFlags, params object[] parameters) {
			if(dt == null || dt.Rows.Count == 0) {
				return null;
			}
			int num = dt.Rows.Count;
			Array objs = Array.CreateInstance(type, num);
			for(int i = 0; i < num; i++) {
				objs.SetValue(TypeHelper.CreateObject(type, null, true, parameters), i);
			}

			TransObject(dt, (object[])objs, columnBindingFlags);
			return objs;
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static T[] TransObject<T>(DataTable dt, BindingFlags columnBindingFlags, params object[] parameters) {
			if (dt == null || dt.Rows.Count == 0) {
				return null;
			}
			int num = dt.Rows.Count;
			T[] objs = new T[num];
			Type type = typeof(T);
			for (int i = 0; i < num; i++) {
				objs[i] = (T)TypeHelper.CreateObject(type, null, true, parameters);
			}

			TransObject<T>(dt, objs, columnBindingFlags);

			return objs;
		}

		/// <summary>
		/// DataRow到对象的转换
		/// </summary>
		/// <param name="drs">DataRow数组，必须是有相同架构（Schema）的DataRow</param>
		/// <param name="type">需要转换的对象类型</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static Array TransObject(DataRow[] drs, Type type, BindingFlags columnBindingFlags, params object[] parameters) {
			if(drs == null || drs.Length == 0) {
				return null;
			}
			int num = drs.Length;
			Array objs = Array.CreateInstance(type, num);
			for(int i = 0; i < num; i++) {
				objs.SetValue(TypeHelper.CreateObject(type, null, true, parameters), i);
			}

			TransObject(drs, (object[])objs, columnBindingFlags);
			return objs;
		}

		/// <summary>
		/// DataRow到对象的转换
		/// </summary>
		/// <param name="drs">DataRow数组，必须是有相同架构（Schema）的DataRow</param>
		/// <param name="columnBindingFlags">BindingFlags</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static T[] TransObject<T>(DataRow[] drs, BindingFlags columnBindingFlags, params object[] parameters) {
			if (drs == null || drs.Length == 0) {
				return null;
			}
			int num = drs.Length;
			T[] objs = new T[num];
			Type type = typeof(T);
			for (int i = 0; i < num; i++) {
				objs[i] = (T)TypeHelper.CreateObject(type, null, true, parameters);
			}

			TransObject<T>(drs, objs, columnBindingFlags);

			return objs;
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="type">需要转换的对象类型</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static Array TransObject(DataTable dt, Type type, params object[] parameters) {
			return TransObject(dt, type, ColumnBindingFlags, parameters);
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="parameters">对象类型构造函数的参数列表</param>
		/// <returns>转换后的对象列表</returns>
		public static T[] TransObject<T>(DataTable dt, params object[] parameters) {
			return TransObject<T>(dt, ColumnBindingFlags, parameters);
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="objs">需要被填充的对象列表（类型一致的对象，不允许空引用）</param>
		public static void TransObject(DataTable dt, object[] objs) {
			TransObject(dt, objs, ColumnBindingFlags);
		}

		/// <summary>
		/// DataTable到对象的转换
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="obj">需要被填充的对象（不允许空引用）</param>
		public static void TransObject(DataTable dt, object obj) {
			TransObject(dt, new object[] { obj });
		}

		#endregion Data to Objects

		#endregion TransObject

		#region TransWebFormDataToEntityProperty

		/// <summary>
		/// WebForm数据到实体属性的转换
		/// </summary>
		/// <param name="form">被转换的WebForm实体（Page的派生类）</param>
		/// <param name="entity">实体（不允许空引用），支持<see cref="DataRow"/>、<see cref="IDataParameterCollection"/></param>
		/// <example>
		///		<code>
		///			public class myPage : System.Web.UI.Page
		///			{
		///				[Column("Title")]
		///				protected TextBox txtTitle;
		///				[Column("Description")]
		///				protected TextBox txtDescription;
		///				
		///				private void btnSubmit_Click(object sender, EventArgs e) {
		///					YourEntity entity = new YourEntity();
		///					DataTransfer.TransWebFormDataToEntityProperty(this, entity);
		///				}
		///			}
		///		</code>
		/// </example>
		public static void TransWebFormDataToEntityProperty(object form, object entity) {
			if(form == null || entity == null) {
				return;
			}
			Type formType = form.GetType();
			IFieldMemberInfo[] columns = FieldMemberInfo.GetFieldMembers(formType, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, true);
			for(int i = 0; i < columns.Length; i++) {
				if(columns[i].CanRead) {
					string stringValue;
					object fieldObject = columns[i].GetValue(form);
					if(fieldObject is TextBox) {
						stringValue = (fieldObject as TextBox).Text;
					} else if(fieldObject is DropDownList) {
						stringValue = (fieldObject as DropDownList).SelectedValue;
					} else if(fieldObject is Label) {
						stringValue = (fieldObject as Label).Text;
					} else if(fieldObject is HyperLink) {
						stringValue = (fieldObject as HyperLink).Text;
					} else {
						continue;
					}
					object value = stringValue;
					if(columns[i].Column.ColumnType != null) {
						try {
							value = Convert.ChangeType(stringValue, columns[i].Column.ColumnType);
						} catch {
							if(columns[i].Column.DefaultValue != null) {
								value = columns[i].Column.DefaultValue;
							} else {
								continue;
							}
						}
					}
					if(entity is DataRow) {
						DataRow dr = (DataRow)entity;
						dr[columns[i].Name] = value;
					} else if (entity is IDataParameterCollection) {
						IDataParameterCollection pc = (IDataParameterCollection)entity;
						string pname = columns[i].Name.StartsWith("@") ? columns[i].Name : "@" + columns[i].Name;
						((IDataParameter)pc[pname]).Value = value;
					} else {
						Type entityType = entity.GetType();
						PropertyInfo property = entityType.GetProperty(columns[i].Name, ColumnBindingFlags);
						if(property != null && property.CanWrite) {
							property.SetValue(entity, value, null);
						}
					}
				}
			}
		}

		#endregion

		#region TransEntityPropertyToWebFormData
		
		/// <summary>
		/// 实体属性到WebForm数据的转换
		/// </summary>
		/// <param name="entity">实体（不允许空引用），支持<see cref="DataRow"/></param>
		/// <param name="form">被转换的WebForm实体（Page的派生类）</param>
		/// <example>
		///		<code>
		///			public class myPage : System.Web.UI.Page
		///			{
		///				[Column("Title")]
		///				protected TextBox txtTitle;
		///				[Column("Description")]
		///				protected TextBox txtDescription;
		///				
		///				private void Page_Load(object sender, EventArgs e) {
		///					YourEntity entity;
		///					//load entity data ...
		///					DataTransfer.TransEntityPropertyToWebFormData(entity, this);
		///				}
		///			}
		///		</code>
		/// </example>
		public static void TransEntityPropertyToWebFormData(object entity, object form) {
			if(form == null || entity == null) {
				return;
			}
			Type formType = form.GetType();
			Type entityType = entity.GetType();
			IFieldMemberInfo[] columns = FieldMemberInfo.GetFieldMembers(formType, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, true);
			for(int i = 0; i < columns.Length; i++) {
				if(columns[i].CanWrite) {
					object value = null;
					if(entity is DataRow) {
						DataRow dr = (DataRow)entity;
						value = dr[columns[i].Name];
					} else {
						PropertyInfo property = entityType.GetProperty(columns[i].Name, ColumnBindingFlags);
						if(property != null && property.CanRead) {
							value = property.GetValue(entity, null);
						}
					}
					if(value == null) {
						continue;
					}
					object fieldObject = columns[i].GetValue(form);
					string stringValue = (string)Convert.ChangeType(value, typeof(string));
					if(fieldObject is TextBox) {
						(fieldObject as TextBox).Text = stringValue;
					} else if(fieldObject is DropDownList) {
						DropDownList ddl = fieldObject as DropDownList;
						ddl.ClearSelection();
						WebHelper.SetDropDownListValueSafely(ddl, stringValue);
					} else if(fieldObject is Label) {
						(fieldObject as Label).Text = stringValue;
					} else if(fieldObject is HyperLink) {
						(fieldObject as HyperLink).Text = stringValue;
					} else {
						continue;
					}
				}
				
			}
		}

		#endregion
	}

	#region internal class

	internal class DataRecord : IDataRecord
	{
		public DataRecord(DataRow dr) {
			this.dr = dr;
		}

		private DataRow dr;

		#region IDataRecord Members

		public int FieldCount {
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool GetBoolean(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public byte GetByte(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
			throw new Exception("The method or operation is not implemented.");
		}

		public char GetChar(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataReader GetData(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public string GetDataTypeName(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public DateTime GetDateTime(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public decimal GetDecimal(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public double GetDouble(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public Type GetFieldType(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public float GetFloat(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public Guid GetGuid(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public short GetInt16(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public int GetInt32(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public long GetInt64(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public string GetName(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public int GetOrdinal(string name) {
			throw new Exception("The method or operation is not implemented.");
		}

		public string GetString(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public object GetValue(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public int GetValues(object[] values) {
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsDBNull(int i) {
			throw new Exception("The method or operation is not implemented.");
		}

		public object this[string name] {
			get { return this.dr[name]; }
		}

		public object this[int i] {
			get { return this.dr[i]; }
		}

		#endregion
	}

	#endregion internal class
}
