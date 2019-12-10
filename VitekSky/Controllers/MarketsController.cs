using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VitekSky.Data;
using VitekSky.Models;

namespace VitekSky.Controllers
{
    public class MarketsController : Controller
    {
        private readonly BusinessContext _context;

        public MarketsController(BusinessContext context)
        {
            _context = context;
        }

        // GET: Markets
        public async Task<IActionResult> Index()
        {
            var businessContext = _context.Markets.Include(m => m.Administrator);
            return View(await businessContext.ToListAsync());
        }

        // GET: Markets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string query = "SELECT * FROM Market WHERE MarketID = {0}";
            var market = await _context.Markets
                .FromSql(query, id)
                .Include(m => m.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MarketID == id);
            if (market == null)
            {
                return NotFound();
            }

            return View(market);
        }

        // GET: Markets/Create
        public IActionResult Create()
        {
            ViewData["ProductGuideID"] = new SelectList(_context.ProductGuides, "ID", "FullName");
            return View();
        }

        // POST: Markets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarketID,Name,Budget,StartDate,ProductGuideID,RowVersion")] Market market)
        {
            if (ModelState.IsValid)
            {
                _context.Add(market);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductGuideID"] = new SelectList(_context.ProductGuides, "ID", "FullName", market.ProductGuideID);
            return View(market);
        }

        // GET: Markets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var market = await _context.Markets
                .Include(i => i.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MarketID == id);

            if (market == null)
            {
                return NotFound();
            }
            ViewData["ProductGuideID"] = new SelectList(_context.ProductGuides, "ID", "FullName", market.ProductGuideID);
            return View(market);
        }

        // POST: Markets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketToUpdate = await _context.Markets.Include(i => i.Administrator).FirstOrDefaultAsync(m => m.MarketID == id);

            if (marketToUpdate == null)
            {
                Market deletedMarket = new Market();
                await TryUpdateModelAsync(deletedMarket);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["ProductGuideID"] = new SelectList(_context.ProductGuides, "ID", "FullName", deletedMarket.ProductGuideID);
                return View(deletedMarket);
            }

            _context.Entry(marketToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Market>(
                marketToUpdate,
                "",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.ProductGuideID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Market)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The market was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Market)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.ProductGuideID != clientValues.ProductGuideID)
                        {
                            ProductGuide databaseProductGuide = await _context.ProductGuides.FirstOrDefaultAsync(i => i.ID == databaseValues.ProductGuideID);
                            ModelState.AddModelError("InstructorID", $"Current value: {databaseProductGuide?.FullName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        marketToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["ProductGuideID"] = new SelectList(_context.ProductGuides, "ID", "FullName", marketToUpdate.ProductGuideID);
            return View(marketToUpdate);
        }

        // GET: Markets/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var market = await _context.Markets
                .Include(m => m.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MarketID == id);
            if (market == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(market);
        }

        // POST: Markets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Market market)
        {
            try
            {
                if (await _context.Markets.AnyAsync(m => m.MarketID == market.MarketID))
                {
                    _context.Markets.Remove(market);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = market.MarketID });
            }
        }

        private bool MarketExists(int id)
        {
            return _context.Markets.Any(e => e.MarketID == id);
        }
    }
}
