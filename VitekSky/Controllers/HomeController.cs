using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VitekSky.Models;
using Microsoft.EntityFrameworkCore;
using VitekSky.Data;
using VitekSky.Models.BusinessViewModels;
using System.Data.Common;

namespace VitekSky.Controllers
{
    public class HomeController : Controller
    {
        private readonly BusinessContext _context;

        public HomeController(BusinessContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> About()
        {
            List<SubscriptionDateGroup> groups = new List<SubscriptionDateGroup>();
            var conn = _context.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    string query = "SELECT SubscriptionDate, COUNT(*) AS CustomerCount "
                        + "FROM Person "
                        + "WHERE Discriminator = 'Customer' "
                        + "GROUP BY SubscriptionDate";
                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new SubscriptionDateGroup { SubscriptionDate = reader.GetDateTime(0), CustomerCount = reader.GetInt32(1) };
                            groups.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return View(groups);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
