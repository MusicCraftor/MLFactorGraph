using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using MySql.Data.MySqlClient;

namespace ResearchUtils
{
    public class DatabaseConnection
    {
        public DatabaseConnection(string connectionStr)
        {
            connStr = connectionStr;
            conn = null;
        }

        public bool TestConnection()
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                conn.Close();
            }
            catch (MySqlException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool ConnectDatabase()
        {
            bool caughtException = false;
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
            }
            catch (Exception)
            {
                caughtException = true;
                conn = null;
            }
            finally
            {
            }

            return !caughtException;
        }

        public void DisconnectDatabase()
        {
            conn.Close();
            conn = null;
        }

        public Int32[] ExecuteQueries(string[] queries)
        {
            Int32 parallelCount = 10000;
            MySqlCommand command = null;
            MySqlTransaction trans = null;
            MySqlDataReader reader = null;
            List<Int32> failedQueries = new List<Int32>();

            if (queries == null || queries.Length == 0)
            {
                return new Int32[]{ -1 };
            }

            try
            {
                if (conn == null)
                {
                    conn = new MySqlConnection(connStr);
                }

                conn.Open();
                command = new MySqlCommand();
                command.Connection = conn;
                trans = conn.BeginTransaction();
                command.Transaction = trans;

                List<Int32> pendingQueries = new List<Int32>();
                for (int i = 0; i < queries.Length; i++)
                {
                    try
                    {
                        pendingQueries.Add(i);
                        command.CommandText = queries[i];
                        command.ExecuteNonQuery();
                        if (i > 0 && (i % parallelCount == 0 || i == queries.Length - 1))
                        {
                            trans.Commit();
                            pendingQueries.Clear();
                            trans = conn.BeginTransaction();
                        }
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine("[ExecuteQuery] MySql Exception: " + e.Message);
                        if (trans != null)
                        {
                            trans.Rollback();
                        }
                        failedQueries.AddRange(pendingQueries);
                        pendingQueries.Clear();
                        trans = conn.BeginTransaction();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[ExecuteQuery] Exception: " + e.Message);
                        if (trans != null)
                        {
                            trans.Rollback();
                        }
                        failedQueries.AddRange(pendingQueries);
                        pendingQueries.Clear();
                        trans = conn.BeginTransaction();
                    }
                    finally
                    {
                    }
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ExecuteQuery] MySql Exception: " + e.Message);
                return new Int32[]{ -1 };
            }
            catch (Exception e)
            {
                Console.WriteLine("[ExecuteQuery] Exception: " + e.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            if (failedQueries.Count == 0)
                return null;

            return failedQueries.ToArray();
        }

        public bool PollingExecuteQuery(string query, int n = 1)
        {
            bool result = false;
            int i = 0;
            if (query == "")
            {
                return false;
            }

            while (result == false && i < n)
            {
                result = ExecuteQuery(query);
                i++;
            }

            return result;
        }

        public bool ExecuteQuery(string query)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            bool result = true;

            if (query == "")
            {
                return false;
            }

            try
            {
                if (conn == null)
                {
                    conn = new MySqlConnection(connStr);
                }
                command = new MySqlCommand(query, conn);

                conn.Open();

                reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ExecuteQuery] MySql Exception: " + e.Message);
                result = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ExecuteQuery] Exception: " + e.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return result;
        }

        public Int64 PollingExecuteQuery_Count(string query_withCount, int n = 1)
        {
            Int64 result = -1;
            int i = 0;
            if (query_withCount == "")
            {
                return -1;
            }

            while (result == -1 && i < n)
            {
                result = ExecuteQuery_Count(query_withCount);
                i++;
            }

            return result;
        }

        public Int64 ExecuteQuery_Count(string query_withCount)
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            Int64 result = 0;

            if (query_withCount == "")
            {
                return -1;
            }

            try
            {
                if (conn == null)
                {
                    conn = new MySqlConnection(connStr);
                }
                command = new MySqlCommand(query_withCount, conn);

                conn.Open();

                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    result = (Int64)reader.GetValue(0);
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ExecuteQuery] MySql Exception: " + e.Message);
                result = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ExecuteQuery] Exception: " + e.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return result;
        }

        public List<T> PollingExecuteQuery_ObjectResult<T>(string query, Int64 startIndex = 0, Int64 resultNumber = 0, int n = 1) where T : class, new()
        {
            List<T> result = null;
            int i = 0;
            if (query == "")
            {
                return null;
            }

            while (result == null && i < n)
            {
                result = ExecuteQuery_ObjectResult<T>(query, startIndex, resultNumber);
                i++;
            }

            return result;
        }

        public List<T> ExecuteQuery_ObjectResult<T>(string query, Int64 startIndex, Int64 resultNumber) where T : class, new()
        {
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            List<T> result = null;

            if (query == "")
            {
                return result;
            }

            try
            {
                if (conn == null)
                {
                    conn = new MySqlConnection(connStr);
                }

                if (resultNumber != 0)
                {
                    query += " LIMIT " + startIndex + "," + resultNumber;
                }
                command = new MySqlCommand(query, conn);

                conn.Open();

                reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                result = new List<T>();

                while (reader.Read())
                {
                    T record = new T();
                    foreach (var property in record.GetType().GetProperties())
                    {
                        try
                        {
                            int i = reader.GetOrdinal(property.Name);
                            property.SetValue(record, reader.GetValue(i));
                        }
                        catch (IndexOutOfRangeException)
                        {
                        }
                    }
                    result.Add(record);
                }

                //if (conn.State == ConnectionState.Open)
                    //conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ExecuteQuery] MySql Exception: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ExecuteQuery] Exception: " + e.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return result;
        }

        private string connStr;
        private MySqlConnection conn;
    }
}
