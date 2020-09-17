using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using CacheTable.Models;
using CacheTable.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CacheTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("text/csv")]
    public class CsvController : ControllerBase
    {
      

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AirTableService _airTable;

        public CsvController(ILogger<WeatherForecastController> logger, AirTableService airTable)
        {
            _logger = logger;
            _airTable = airTable;
        }

        // GET /api/FireRefuge/Shelter
        [HttpGet("{database}/{view}")]
        public async Task<IActionResult> Get(string database, string view)
        {
            var app = await _airTable.GetAppAsync(database);
            if (app == null)
            {
                return NotFound($"app {database} not found.");
            }

            var table = await _airTable.GetTableAsync(view);
            if (table == null)
            {
                return NotFound($"view {view} not found.");
            }

            var layer = await _airTable.GetTableAsync<Layer>(app.AppKey, app.BaseId,table.TableName,table.View);
            // open the other base

            return Ok(layer);
        }

    }
}
