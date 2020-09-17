using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheTable.Models;
using Microsoft.Extensions.Configuration;


namespace CacheTable.Services
{
    /// <summary>
    /// a bruit force airtable data caching service
    /// </summary>
    public class AirTableService : AirTableBase
    {
        private IEnumerable<App> _apps;
        private IEnumerable<Table> _tables;

        public AirTableService(IConfiguration configuration) : base(configuration) {}

        public async Task<IEnumerable<App>> GetAppsAsync()
        {
            return _apps ??= await GetTableAsync<App>("Apps");
        }

        public async Task<App> GetAppAsync(string name)
        {
            var list = await GetAppsAsync();
            return list.SingleOrDefault(a => string.Equals(a.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<IEnumerable<Table>> GetTablesAsync()
        {
            return _tables ??= await GetTableAsync<Table>("Tables");
        }

        public async Task<Table> GetTableAsync(string name)
        {
            var list = await GetTablesAsync();
            return list.SingleOrDefault(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void Flush()
        {
            _apps = null;
            _tables = null;
        }
    }
}
