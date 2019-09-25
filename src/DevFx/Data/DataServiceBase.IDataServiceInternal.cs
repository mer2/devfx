namespace DevFx.Data
{
	partial class DataServiceBase : IDataServiceInternal
	{
		bool IDataServiceInternal.StatementExists(string statementName) {
			var statement = this.GetStatementSetting(statementName, false);
			return statement != null;
		}
	}
}
