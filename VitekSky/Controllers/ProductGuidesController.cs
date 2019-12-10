using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VitekSky.Data;
using VitekSky.Models;
using VitekSky.Models.BusinessViewModels;

namespace VitekSky.Controllers
{
    public class ProductGuidesController : Controller
    {
        private readonly BusinessContext _context;

        public ProductGuidesController(BusinessContext context)
        {
            _context = context;   
        }

        // GET: ProductGuides
        public async Task<IActionResult> Index(int? id, int? productID)
        {
            var viewModel = new ProductGuideIndexData();
        viewModel.ProductGuides=   await _context.ProductGuides
                  .Include(i => i.CountryAssignment)
                  .Include(i => i.ProductAssignments)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(i => i.Subscriptions)
                            .ThenInclude(i => i.Customer)
                  .Include(i => i.ProductAssignments)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(i => i.Market)
                  .AsNoTracking()
                  .OrderBy(i => i.LastName)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["ProductGuideID"] = id.Value;
                ProductGuide productGuide = viewModel.ProductGuides.Where(
                    i => i.ID == id.Value).Single();
                viewModel.Products = productGuide.ProductAssignments.Select(s => s.Product);
            }

            if (productID != null)
            {
                ViewData["ProductID"] = productID.Value;
                var selectedProduct = viewModel.Products.Where(x => x.ProductID == productID).Single();
                await _context.Entry(selectedProduct).Collection(x => x.Subscriptions).LoadAsync();
                foreach (Subscription subscription in selectedProduct.Subscriptions)
                {
                    await _context.Entry(subscription).Reference(x => x.Customer).LoadAsync();
                }
                viewModel.Subscriptions = selectedProduct.Subscriptions;
            }

            return View(viewModel);
        }

        // GET: ProductGuides/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGuide = await _context.ProductGuides
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productGuide == null)
            {
                return NotFound();
            }

            return View(productGuide);
        }

        // GET: ProductGuides/Create
        public IActionResult Create()
        {
            var productGuide = new ProductGuide();
            productGuide.ProductAssignments = new List<ProductAssignment>();
            PopulateAssignedProductData(productGuide);

            return View();
        }

        // POST: ProductGuides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryAssignment,LastName,FirstMidName,HireDate")] ProductGuide productGuide, string[] selectedProducts)
        {
            if (selectedProducts != null)
            {
                productGuide.ProductAssignments = new List<ProductAssignment>();
                foreach (var product in selectedProducts)
                {
                    var productToAdd = new ProductAssignment { ProductGuideID = productGuide.ID, ProductID = int.Parse(product) };
                    productGuide.ProductAssignments.Add(productToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(productGuide);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedProductData(productGuide);

            return View(productGuide);
        }

        // GET: ProductGuides/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGuide = await _context.ProductGuides
                .Include(pg => pg.CountryAssignment)
                .Include(pg => pg.ProductAssignments).ThenInclude(pg => pg.Product)

                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productGuide == null)
            {
                return NotFound();
            }
            PopulateAssignedProductData(productGuide);
            return View(productGuide);
        }
        private void PopulateAssignedProductData(ProductGuide productGuide)
        {
            var allProducts = _context.Products;
            var productGuideProducts = new HashSet<int>(productGuide.ProductAssignments.Select(c => c.ProductID));
            var viewModel = new List<AssignedProductData>();
            foreach (var product in allProducts)
                {
                    viewModel.Add(new AssignedProductData
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        Assigned = productGuideProducts.Contains(product.ProductID)
                    });
                }
                ViewData["Products"] = viewModel;
        }
        

        // POST: ProductGuides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedProducts)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGuideToUpdate = await _context.ProductGuides
                .Include(pg => pg.CountryAssignment)
                .Include(pg => pg.ProductAssignments)
                    .ThenInclude(pg => pg.Product)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (await TryUpdateModelAsync<ProductGuide>(
                productGuideToUpdate,
                "",
                pg => pg.FirstMidName, pg => pg.LastName, pg => pg.HireDate, pg => pg.CountryAssignment))
            {
                if (String.IsNullOrWhiteSpace(productGuideToUpdate.CountryAssignment?.Location))
                {
                    productGuideToUpdate.CountryAssignment = null;
                }
                UpdateProductGuideProducts(selectedProducts, productGuideToUpdate);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateProductGuideProducts(selectedProducts, productGuideToUpdate);
            PopulateAssignedProductData(productGuideToUpdate);
            return View(productGuideToUpdate);
        }
        private void UpdateProductGuideProducts(string[] selectedProducts, ProductGuide productGuideToUpdate)
        {
            if (selectedProducts == null)
            {
                productGuideToUpdate.ProductAssignments = new List<ProductAssignment>();
                return;
            }

            var selectedProductsHS = new HashSet<string>(selectedProducts);
            var productGuideProducts = new HashSet<int>
                (productGuideToUpdate.ProductAssignments.Select(c => c.Product.ProductID));
            foreach (var product in _context.Products)
            {
                if (selectedProductsHS.Contains(product.ProductID.ToString()))
                {
                    if (!productGuideProducts.Contains(product.ProductID))
                    {
                        productGuideToUpdate.ProductAssignments.Add(new ProductAssignment { ProductGuideID = productGuideToUpdate.ID, ProductID = product.ProductID });
                    }
                }
                else
                {

                    if (productGuideProducts.Contains(product.ProductID))
                    {
                        ProductAssignment productToRemove = productGuideToUpdate.ProductAssignments.FirstOrDefault(i => i.ProductID == product.ProductID);
                        _context.Remove(productToRemove);
                    }
                }
            }
        }

        // GET: ProductGuides/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productGuide = await _context.ProductGuides
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productGuide == null)
            {
                return NotFound();
            }

            return View(productGuide);
        }

        // POST: ProductGuides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ProductGuide productGuide = await _context.ProductGuides
            .Include(pg => pg.ProductAssignments)
            .SingleAsync(pg => pg.ID == id);

            var markets = await _context.Markets
                .Where(m => m.ProductGuideID == id)
                .ToListAsync();
            markets.ForEach(m => m.ProductGuideID = null);

            _context.ProductGuides.Remove(productGuide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductGuideExists(int id)
        {
            return _context.ProductGuides.Any(e => e.ID == id);
        }
    }
}
