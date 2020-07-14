using ArgumentParser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Text;

namespace SqlClient
{
	class Program
	{
		public static List<string> driverList = new List<string>();
		public static Dictionary<string, string> driverDict = new Dictionary<string, string>();


		static void Main(string[] args)
		{
			ArgParse argparse = new ArgParse
			(
					new ArgItem("driver", "d", true, "ODBC Driver: mssql,postgresql,firebird,mssql", "mssql", ArgParse.ArgParseType.String),
					new ArgItem("username", "u", true, "username", "", ArgParse.ArgParseType.String),
					new ArgItem("password", "p", true, "password", "", ArgParse.ArgParseType.String),
					new ArgItem("host", "i", true, "the host name of the server", "", ArgParse.ArgParseType.String),
					new ArgItem("catalog", "c", true, "The catalog or database", "", ArgParse.ArgParseType.String),
					new ArgItem("sql", "s", true, "The sql statement to execute", "", ArgParse.ArgParseType.String),
					new ArgItem("output", "o", false, "Output type: csv,json", "json", ArgParse.ArgParseType.String),
					new ArgItem("url", "e", false, "url where data will be posted", "", ArgParse.ArgParseType.String)
			);

			argparse.parse(args);

			driverDict.Add("mssql", "SQL Server");
			driverDict.Add("mysql", "mysql");
			driverDict.Add("firebird", "firebird");
			driverDict.Add("postgresql", "postgresql");

			string driver = argparse.Get<string>("driver");
			string username = argparse.Get<string>("username");
			string password = argparse.Get<string>("password");
			string host = argparse.Get<string>("host");
			string database = argparse.Get<string>("catalog");
			string sql = argparse.Get<string>("sql");
			string output = argparse.Get<string>("output");
			string url = argparse.Get<string>("url");

			ListODBCDrivers();

			if(DriverExists(driver))
			{
				string connectionString = string.Format(getConnectionString(driver), host, database, username, password);

				OdbcConnection connection = new OdbcConnection(connectionString);
				connection.Open();

				OdbcCommand cmd = new OdbcCommand(sql, connection);
				OdbcDataReader dataReader = cmd.ExecuteReader();
				DataTable dt = new DataTable("table");
				dt.Load(dataReader);

				string result = "";
				if (output.ToLower() == "csv")
				{
					result = DataTableToCsv(dt);
				}

				if (output.ToLower() == "json")
				{
					result = DataTableToJson(dt);
				}

				if (url.Length == 0)
				{
					Console.WriteLine(DataTableToCsv(dt));
				}
				else
				{
					WebClient wc = new WebClient();
					wc.UploadString(url, result);
				}


				connection.Close();
			}
			else
			{
				Console.WriteLine(string.Format("ODBC Drive {0} does not exist", driver));
			}
		}

		public static string DataTableToCsv(DataTable dt)
		{
			return ToCSV(dt);
		}

		public static string DataTableToJson(DataTable dt)
		{
			return DataTableToJSONWithStringBuilder(dt);
		}

		private static string getConnectionString(string driver)
		{
			string cs = "";
			switch (driver.ToLower())
			{
				case "mysql":
					string driverString = GetMySqlDriver();
					cs = "Driver={{"+driverString+"}};Server={0};Database={1};Uid={2};Pwd={3};";
					break;
				case "postgres":
					cs = "Driver={{PostgreSQL}};Server={0};Database={1};Uid={2};Pwd={3};";
					break;
				case "firebird":
					cs = "Driver=Firebird/InterBase(r) driver;DBNAME={0}:{1};UID={2};PWD={3};";
					break;
				default:
					cs = "Driver={{SQL Server}};Server={0};Database={1};Uid={2};Pwd={3};";
					break;
			}

			return cs;
		}

		public static bool DriverExists(string driver)
		{
			if(!driverDict.ContainsKey(driver.ToLower()))
			{
				return false;
			}

			string key = driverDict[driver];

			foreach (string d in driverList)
			{
				if(d.ToLower().Contains(key))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetMySqlDriver()
		{
			string cs = "";

			foreach (string d in driverList)
			{
				if (d.ToLower().Contains("mysql"))
				{
					return d;
				}
			}

			return cs;
		}

		public static List<string> ListODBCDrivers()
		{
			//https://stackoverflow.com/questions/6457973/odbc-driver-list-from-net
			Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
			if (reg != null)
			{
				reg = reg.OpenSubKey("ODBC");
				if (reg != null)
				{
					reg = reg.OpenSubKey("ODBCINST.INI");
					if (reg != null)
					{

						reg = reg.OpenSubKey("ODBC Drivers");
						if (reg != null)
						{
							foreach (string sName in reg.GetValueNames())
							{
								driverList.Add(sName);
							}
						}
						try
						{
							reg.Close();
						}
						catch {}
					}
				}
			}

			return driverList;
		}

		private static string DataTableToJSONWithStringBuilder(DataTable table)
		{
			//https://www.c-sharpcorner.com/UploadFile/9bff34/3-ways-to-convert-datatable-to-json-string-in-Asp-Net-C-Sharp/
			var JSONString = new StringBuilder();
			if (table.Rows.Count > 0)
			{
				JSONString.Append("[");
				for (int i = 0; i < table.Rows.Count; i++)
				{
					JSONString.Append("{");
					for (int j = 0; j < table.Columns.Count; j++)
					{
						if (j < table.Columns.Count - 1)
						{
							JSONString.Append("\"" + table.Columns[j].ColumnName.ToString().Replace("\"", "\\\"") + "\":" + "\"" + table.Rows[i][j].ToString().Replace("\"", "\\\"") + "\",");
						}
						else if (j == table.Columns.Count - 1)
						{
							JSONString.Append("\"" + table.Columns[j].ColumnName.ToString().Replace("\"", "\\\"") + "\":" + "\"" + table.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"");
						}
					}
					if (i == table.Rows.Count - 1)
					{
						JSONString.Append("}");
					}
					else
					{
						JSONString.Append("},");
					}
				}
				JSONString.Append("]");
			}
			return JSONString.ToString();
		}

		public static string ToCSV(DataTable dt)
		{
			string result = "";
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				result += dt.Columns[i];
				if (i < dt.Columns.Count - 1)
				{
					result += ",";
				}
			}
			
			result += "\n";

			foreach (DataRow dr in dt.Rows)
			{
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					if (!Convert.IsDBNull(dr[i]))
					{
						string value = dr[i].ToString();
						if (value.Contains(","))
						{
							value = String.Format("\"{0}\"", value.Replace("\"", "\\\""));
							result += value;
						}
						else
						{
							result += dr[i].ToString();
						}
					}
					if (i < dt.Columns.Count - 1)
					{
						result += ",";
					}
				}
				result += "\n";
			}

			return result;
		}
	}
}
