using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AirtableApiClient;
using CacheTable.Models;
using Microsoft.Extensions.Configuration;

//using Markdig.Extensions.ListExtras;


namespace CacheTable.Services
{


    /// <summary>
    /// a bruit force airtable data caching service
    /// </summary>
    public abstract class AirTableBase
    {
        protected readonly string BaseId;
        protected readonly string AppKey;

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public AirTableBase(IConfiguration configuration)
        {
            BaseId = configuration["AirTable:BaseId"];
            AppKey = configuration["AirTable:AppKey"];
        }

  
        private void PopulateIds<T>(IEnumerable<AirtableRecord<T>> responseRecords) where T : IAirtable
        {
            foreach (var item in responseRecords)
            {
                item.Fields.Id = item.Id;
            }
        }

        public async Task<List<T>> GetTableAsync<T>(string tableName) where T : IAirtable
        {
            var list = await GetTableAsync<T>(AppKey, BaseId, tableName);
            return list;
        }

        /// <summary>
        /// overload to accept appkeys and baseid to access more tables.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AppKey"></param>
        /// <param name="BaseId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<T>> GetTableAsync<T>(string AppKey, string BaseId, string tableName,string view=null) where T : IAirtable
        {
            var table = new List<AirtableRecord<T>>();
            string offset = null;
            var retryCount = 0;

            await semaphore.WaitAsync(); // wait for your place in line

            using AirtableBase airtableBase = new AirtableBase(AppKey, BaseId);
            try
            {
                do
                {
                    Task<AirtableListRecordsResponse<T>> task =
                        airtableBase.ListRecords<T>(tableName: tableName, offset: offset,view:view);

                    var response = await task;

                    if (response.Success)
                    {
                        PopulateIds(response.Records);
                        table.AddRange(response.Records);
                        offset = response.Offset;
                    }
                    // look for timeouts and add retry logic. 
                    else if (response.AirtableApiError != null)
                    {
                        if (retryCount < 3)
                        {
                            retryCount++;
                            await Task.Delay(500); //pause half-a-sec
                          
                        }
                        else
                        {
                            // too many retrys
                            table = null;
                            break;
                        }
                    }

                } while (offset != null);
            }
            catch (Exception e)
            {

                table = null;
            }
            finally
            {
                semaphore.Release(); // let the next one in
            }

            List<T> list = null;
            if (table != null)
            {
                list = new List<T>();
                try
                {
                    list = (from c in table select c.Fields).ToList();
                }
                catch (Exception e)
                {
                    var err = e.InnerException;
                }
            }

            return list;
        }
    }
}
