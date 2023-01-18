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

        public IList<string[]> GetExpensesList(SqlConnection cnn, string MonthName,string YearName)
        {
            #region queryString

            string queryString = null;

            if (!String.IsNullOrEmpty(MonthName) && !String.IsNullOrEmpty(YearName))
            {
                queryString = String.Format(@"
             SELECT 
             Kwota,
            [Nazwa Transakcji],
            [Rodzaj Transakcji],
            CONVERT(date ,data)
            FROM [Kasa].[dbo].[Wydatkii]
			
			Where (DATENAME(MONTH,Data)) like '{0}' AND YEAR(Data) = {1}", MonthName,YearName);
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
             WITH CTE AS
             (
                SELECT DISTINCT(DATENAME(MONTH,s.Data)) AS 'MonthName' ,
                CONVERT (NVARCHAR,YEAR(S.DATA)) AS 'YearName' 
                FROM dbo.Wydatkii S 
             )
            SELECT  MonthName + ' ' +YearName 
            FROM  CTE 
            ORDER BY CONVERT(INT,YEAR(YearName)),  MONTH(CONVERT(NVARCHAR, MonthName)
            + ' ' + CONVERT(NVARCHAR, YEAR(GETDATE()))) ASC; 
		  ");
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }

        public IList<string[]> GetStatistic(SqlConnection cnn)
        {
            #region queryString

            string queryString = String.Format(@"
            WITH CTE AS(
            SELECT DISTINCT 
            DATENAME(MONTH,DATA) AS MIESIAC, 
            MONTH(Data) AS MIES, YEAR(data) AS ROK, 
            PRZYCHOD= ISNULL( (SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
            WHERE s.[Rodzaj Transakcji] = 'Przychod' AND MONTH(K.DATA) = MONTH(S.DATA) AND YEAR(K.DATA) = YEAR(S.DATA) ),0),
            KOSZTY= ISNULL( (SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
            WHERE s.[Rodzaj Transakcji] = 'Wydatek'  AND MONTH(S.DATA) = MONTH(K.DATA) AND YEAR(K.DATA) = YEAR(S.DATA)),0 ) From dbo.Wydatkii K)
            SELECT C.MIESIAC+ ' '+CONVERT(nvarchar, c.ROK),C.przychod , C.koszty , ISNULL( (C.przychod - C.koszty),0) as 'Roznica'  FROM CTE C ORDER BY ROK, MIES ;

            ");
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
	        where s.[Rodzaj Transakcji] = 'Przychod' AND  FORMAT(s.data,'MMMM')= '{0}'),
	        koszty=(SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
	        where s.[Rodzaj Transakcji] = 'Wydatek'  AND  FORMAT(s.data,'MMMM')= '{0}')
            )
            select przychod,koszty, przychod - koszty as roznica  from cte;",value);
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }


        public IList<string[]> GetSummaryCosts(SqlConnection cnn, string MonthName,string YearName)
        {
            #region queryString
            string queryString = string.Empty;
            if (MonthName != null && YearName !=null)
                queryString = String.Format(@"
            WITH CTE AS(
            SELECT
            przychod= ISNULL( (SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
	        where s.[Rodzaj Transakcji] = 'Przychod' AND  FORMAT(s.data,'MMMM')= '{0}' AND YEAR(S.DATA) ={1}),0),
	        koszty= ISNULL ((SELECT SUM(s.Kwota) FROM [Kasa].[dbo].[Wydatkii] s 
	        where s.[Rodzaj Transakcji] = 'Wydatek'  AND  FORMAT(s.data,'MMMM')= '{0}' AND YEAR(S.DATA) ={1}),0)
            )
            SELECT przychod,koszty, przychod - koszty as roznica  FROM CTE;", MonthName,YearName);
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }


        public IList<string []> GetCategoryCosts(SqlConnection cnn)
        {
            #region queryString
            string queryString = string.Empty;
           
                queryString = String.Format(@"
                WITH CTE AS(
                SELECT 
                CASE WHEN SUM(W.kwota) < 100 THEN 'Pozostałe'
		        ELSE  W.[Nazwa Transakcji] END AS 'Nazwa Transakcji',
                SUM(W.kwota) 'Kwota' 
                FROM dbo.Wydatkii W 
                WHERE W.[Rodzaj Transakcji] = 'Wydatek'
                GROUP BY W.[Nazwa Transakcji]             
                )   
                SELECT  C.[Nazwa Transakcji] ,SUM(C.Kwota)   
                FROM CTE C GROUP BY C.[Nazwa Transakcji] ORDER BY SUM(C.Kwota) DESC;
                ");
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }

        public IList<string[]> GetCategoryCostsByMonth(SqlConnection cnn, string MonthName, string YearName)
        {
            #region queryString
            string queryString = string.Empty;


            queryString = String.Format(@"
                SELECT 
                W.[Nazwa Transakcji] as 'Nazwa Transakcji',
                SUM(W.kwota) 'Kwota' 
                FROM dbo.Wydatkii W 
                WHERE W.[Rodzaj Transakcji] = 'Wydatek'   AND  FORMAT(W.data,'MMMM')= '{0}' AND YEAR(W.DATA) ={1}
                GROUP BY W.[Nazwa Transakcji]
                ORDER BY SUM(W.Kwota) DESC", MonthName, YearName);
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }


        public IList<string[]> GetGeneralSummary(SqlConnection cnn)
        {
            #region queryString
            string queryString = string.Empty;

            queryString = String.Format(@"
                 WITH CTE AS(
                 SELECT 
                 PRZYCHODY= (SELECT SUM(KWOTA) FROM dbo.Wydatkii WHERE [Rodzaj Transakcji] = 'Przychod'),
                 WYDATKI= (SELECT SUM(KWOTA) FROM dbo.Wydatkii WHERE [Rodzaj Transakcji] = 'Wydatek')
                 )
                 SELECT K.PRZYCHODY, K.WYDATKI,  K.PRZYCHODY  - K.WYDATKI AS 'OGÓŁEM', YEAR(GETDATE()) AS 'ROK' FROM CTE K;
                ");
            #endregion

            IList<string[]> result = ExecuteQuery(queryString, cnn);

            return result;
        }




        #endregion

    }
}
