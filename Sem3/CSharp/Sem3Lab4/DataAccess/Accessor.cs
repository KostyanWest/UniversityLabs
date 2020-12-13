using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Sem3Lab4.Models;

namespace Sem3Lab4.DataAccess
{
	public class AccessorSettings
	{
		public readonly string connectionString;

		public AccessorSettings (string connectionString)
		{
			this.connectionString = connectionString;
		}
	}

	public class Accessor : IDisposable
	{
		private SqlConnection connection;
		private bool disposed;

		public Accessor (AccessorSettings settings)
		{
			connection = new SqlConnection (settings.connectionString);
			connection.Open ();
		}

		public List<TElement> GetTable<TElement> (GetTableParams @params) where TElement : new()
		{
			if (disposed)
			{
				throw new ObjectDisposedException ("Accessor");
			}

			SqlCommand command = new SqlCommand (@params.ProcedureName, connection) {
				CommandType = CommandType.StoredProcedure
			};
			SqlParameter countParam = new SqlParameter ("@count", @params.Count);
			command.Parameters.Add (countParam);
			SqlDataReader reader = command.ExecuteReader ();

			List<TElement> list = new List<TElement> ();
			while (reader.Read ())
			{
				TElement element = new TElement ();
				foreach (PropertyInfo property in typeof (TElement).GetProperties ())
				{
					var value = reader[property.Name];
					if (value.GetType () != typeof (DBNull))
					{
						property.SetValue (element, value);
					}
					else
					{
						property.SetValue (element, null);
					}
				}
				list.Add (element);
			}

			reader.Close ();
			return list;
		}

		public object InsertElement (InsertElementParams @params)
		{
			if (disposed)
			{
				throw new ObjectDisposedException ("Accessor");
			}

			SqlCommand command = new SqlCommand (@params.ProcedureName, connection) {
				CommandType = CommandType.StoredProcedure
			};
			if (@params.Data != null)
			{
				foreach (PropertyInfo property in @params.Data.GetType ().GetProperties ())
				{
					var value = property.GetValue (@params.Data);
					SqlParameter param = new SqlParameter ("@" + property.Name, value ?? DBNull.Value);
					command.Parameters.Add (param);
				}
			}
			return command.ExecuteScalar ();
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
			{
				return;
			}

			disposed = true;
			if (disposing)
			{
				connection.Dispose ();
				GC.SuppressFinalize (this);
			}
		}

		~Accessor ()
		{
			Dispose (false);
		}
	}
}
