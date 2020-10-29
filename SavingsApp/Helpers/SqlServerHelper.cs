using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

namespace SavingsApp.Helpers
{
  public class SqlServerHelper
    {
        #region Singleton
        private static SqlServerHelper instance = null;
        private static readonly object padlock = new object();

        public SqlServerHelper()
        {

        }

        public static SqlServerHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SqlServerHelper();

                        }
                    }
                }
                return instance;
            }

        }
            #endregion
        #region Methods
        public string MakeConnectionString(string dataSource, string initialCatalog, bool useLoginPassword, string userID, string password, int timeout = 10)
        {
            string connectionString = string.Empty;

            if (useLoginPassword)
            {
                connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout={4};", dataSource, initialCatalog, userID, password, timeout);
            }
            else
            {
                connectionString = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=True;Connect Timeout={2};", dataSource, initialCatalog, timeout);
            }

            return connectionString;
        }

        public IList<string[]> ExecuteQuery(string queryString, SqlConnection cnn)
        {
            IList<string[]> result = new List<string[]>();
            using (SqlCommand command = new SqlCommand(queryString, cnn))
            {
                result = ExecuteCommand(command);
            };
            return result;
        }


        private static string[] GetRowFromReader(SqlDataReader rdr, int columnCount)
        {
            string[] result = new string[columnCount]; ;

            for (int i = 0; i < columnCount; i++)
            {
                result[i] = rdr[i].ToString();

            }

            return result;
        }

        public IList<string[]> ExecuteCommand(SqlCommand command)
        {
            IList<string[]> result = new List<string[]>();
            using (var rdr = command.ExecuteReader())
            {
                int columnCount = 0;

                try
                {
                    while (rdr.Read())
                    {
                        if (columnCount == 0)
                        {
                            columnCount = rdr.FieldCount;
                        }

                        result.Add(GetRowFromReader(rdr, columnCount));

                    }
                }
                finally
                {
                    rdr.Close();
                }
            }
            return result;
        }


        public IList<string[]> GetExpensesList(SqlConnection cnn,string value)
        {
            #region queryString

            string queryString = null;

            if (!String.IsNullOrEmpty(value))
            {
                queryString = String.Format(@"
             SELECT 
             Kwota,
            [Nazwa Transakcji],
            [Rodzaj Transakcji],
            CONVERT(date ,data)
            FROM [Kasa].[dbo].[Wydatkii]
			
			Where (DATENAME(MONTH,Data)) like '{0}'", value);
            }
            else
            {
                queryString = @"SELECT 
             Kwota,
            [Nazwa Transakcji],
            [Rodzaj Transakcji],
            CONVERT(date ,data)
            FROM [Kasa].[dbo].[Wydatkii]";
            }
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }

        public IList<string[]> GetMonthsList(SqlConnection cnn)
        {
            #region queryString
            string queryString = String.Format(@"
            WITH CTE AS(
            SELECT Distinct(DATENAME(MONTH,s.Data)) as 'MonthNane' FROM dbo.Wydatkii S) 
            SELECT  MonthNane   from cte order by  MONTH(CONVERT(NVARCHAR, MonthNane)
            + ' ' + CONVERT(NVARCHAR, YEAR(GETDATE()))) asc;");
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }

        public IList<string[]>GetTransactionsList(SqlConnection cnn)
        {
            #region queryString

            string queryString = String.Format(@"
                SELECT 
                DISTINCT [Nazwa Transakcji] 
                FROM 
                dbo.Wydatkii
                ");

            #endregion
            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }
        public IList<string[]> GetSummaryCosts(SqlConnection cnn,string value)
        {
            #region queryString
            string queryString = string.Empty;
            if (value!=null)
            queryString = String.Format(@"
            WITH CTE AS(
            select
            przychod=  (SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
	        where s.[Rodzaj Transakcji] = 'Przychod' and  FORMAT(s.data,'MMMM')= '{0}'),
	        koszty=(SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
	        where s.[Rodzaj Transakcji] = 'Wydatek' and  FORMAT(s.data,'MMMM')= '{0}')
            )
            select przychod,koszty, przychod - koszty as roznica  from cte;",value);
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }

        

        #endregion

    }
}
