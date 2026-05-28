using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RenewalTransFiles.Models;

namespace RenewalTransFiles.Services
{
    public class MenuService
    {
        private readonly SqlConnConfig _config;
        public MenuService(SqlConnConfig config)
        {
            _config = config;
        }
        public async Task<IEnumerable<MenuInfo>> GetMenuData()
        {
            IEnumerable<MenuInfo> menuInfos;

            using (var conn = new SqlConnection(_config.Value))
            {
                const string query = @"Select * From SITRenMenuInfo";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                try
                {
                    menuInfos = await conn.QueryAsync<MenuInfo>(query);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            return menuInfos;
        }
    }
}
