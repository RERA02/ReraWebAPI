using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ReraRepo.Helper
{
    public static class Helper
    {
        public static async Task<DataSet> FillAsync(this SqlCommand sqlCommand)
        {
            return await Task.Run(() =>
            {
                try
                {
                    DataSet ds = new DataSet();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(ds);
                    }
                    return ds;
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }
            
        public static async Task<DataTable> FillAsync_DataTable(this SqlCommand sqlCommand)
        {
            return await Task.Run(() =>
            {
                try
                {
                    DataTable dataTable = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        da.Fill(dataTable);
                    }
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }


        //public static async Task<DataSet> FillAsync(this SqlCommand sqlCommand)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        DataTable dt = null;
        //        using (var reader = await sqlCommand.ExecuteReaderAsync())
        //        {
        //            int tableIndex = 0;
        //            do
        //            {
        //                dt = new DataTable();
        //                if (tableIndex == 0)
        //                {
        //                    dt.TableName = "Table";
        //                }
        //                else
        //                {
        //                    dt.TableName = $"Table{tableIndex}";
        //                }
        //                tableIndex++;
        //                var r = reader;
        //                dt.Copy().Load(r); // Load current result set into DataTable
        //                //ds.Tables.Add(dt);
        //                dt = null;
        //            } while (await reader.NextResultAsync()); // Move to next result set
        //        }
        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        throw; // Preserve original stack trace
        //    }
        //}
        //public static async Task<DataTable> FillAsync_DataTable(this SqlCommand sqlCommand)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable("Table");
        //        using (var reader = await sqlCommand.ExecuteReaderAsync())
        //        {
        //            dt.Load(reader); // Load current result set into DataTable                    
        //        }
        //        return dt;
        //    }
        //    catch (Exception)
        //    {
        //        throw; // Preserve original stack trace
        //    }
        //}



        public static String GetSqlExecutableQuery(this SqlCommand sc)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                Boolean FirstParam = true;

                sb.AppendLine($"use {sc.Connection.Database};");
                switch (sc.CommandType)
                {
                    case CommandType.StoredProcedure:
                        foreach (SqlParameter sp in sc.Parameters)
                        {
                            if (sp.Direction == ParameterDirection.Output)
                            {
                                var sizeStr = sp.Size > 0 ? $"({sp.Size})" : string.Empty;
                                sb.AppendLine($"declare {sp.ParameterName} {sp.SqlDbType.ToString()}{sizeStr} = null;");
                            }
                        }
                        sb.AppendLine();
                        sb.AppendLine($"exec [{sc.CommandText}]");
                        foreach (SqlParameter sp in sc.Parameters)
                        {
                            if (sp.Direction != ParameterDirection.ReturnValue)
                            {
                                sb.Append((FirstParam) ? "\t" : "\t, ");
                                if (FirstParam)
                                {
                                    FirstParam = false;
                                }
                                if (sp.Direction == ParameterDirection.Input)
                                {
                                    sb.AppendLine($"{sp.ParameterName} = '{sp.Value}'");
                                }
                                else
                                {
                                    sb.AppendLine($"{sp.ParameterName} = {sp.ParameterName} output");
                                }
                            }
                        }
                        sb.AppendLine(";");
                        foreach (SqlParameter sp in sc.Parameters)
                        {
                            if (sp.Direction == ParameterDirection.Output)
                            {
                                sb.AppendLine($"select '{sp.ParameterName}' = convert(varchar, {sp.ParameterName});");
                            }
                        }
                        break;
                    case CommandType.Text:
                        sb.AppendLine(sc.CommandText);
                        foreach (SqlParameter param in sc.Parameters)
                        {
                            sb.Replace(param.ParameterName, param.Value.ToString());
                        }
                        break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class SqlHelper
    {
        public async Task<DataSet> FillAsync(SqlCommand sqlCommand)
        {
            //return  Task.Run(() =>
            //{
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            //}).GetAwaiter().GetResult();
        }
        public async Task<DataTable> FillAsync_DataTable(SqlCommand sqlCommand)
        {
            //return Task.Run(() =>
            //{
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                {
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            // }).GetAwaiter().GetResult();
        }
        public async Task<DataTable> FillAsync_ExecuteReaderDT(SqlCommand sqlCommand)
        {
            try
            {
                SqlDataReader result = await sqlCommand.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(result);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<DataSet> FillAsync_ExecuteReaderDS(SqlCommand sqlCommand)
        {
            try
            {
                using (var reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    DataSet dataSet = new DataSet();

                    do
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataSet.Tables.Add(dt);
                    }
                    while (!reader.IsClosed && await reader.NextResultAsync());

                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
