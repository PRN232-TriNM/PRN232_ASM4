using EVCS.SOAP.WebMVC.TriNM.SOAPModels;
using EVCS.SOAP.WebMVC.TriNM.SOAPServices;
using Microsoft.AspNetCore.Mvc;

namespace EVCS.SOAP.WebMVC.TriNM.Controllers;

public class StationController : Controller
{
    private readonly IStationSoapClient _soapClient;

    public StationController(IStationSoapClient soapClient)
    {
        _soapClient = soapClient;
    }

    public async Task<IActionResult> Index(string? name, string? location, bool? isActive, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            PagedStationResult result;
            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(location) || isActive.HasValue)
            {
                result = await _soapClient.SearchStationsPagedAsync(name, location, isActive, pageNumber, pageSize);
            }
            else
            {
                result = await _soapClient.GetStationsPagedAsync(pageNumber, pageSize);
            }
            
            ViewBag.Name = name;
            ViewBag.Location = location;
            ViewBag.IsActive = isActive;
            ViewBag.PageNumber = result.PageNumber;
            ViewBag.PageSize = result.PageSize;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.HasPreviousPage = result.HasPreviousPage;
            ViewBag.HasNextPage = result.HasNextPage;
            ViewBag.TotalCount = result.TotalCount;
            
            return View(result.Items);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading stations: {ex.Message}";
            return View(new List<Station>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? name, string? location, bool? isActive, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _soapClient.SearchStationsPagedAsync(name, location, isActive, pageNumber, pageSize);
            return Json(new
            {
                items = result.Items,
                totalCount = result.TotalCount,
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalPages = result.TotalPages,
                hasPreviousPage = result.HasPreviousPage,
                hasNextPage = result.HasNextPage
            });
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    // GET: Station/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var station = await _soapClient.GetByIdAsync(id.Value);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading station: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Station/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Station/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StationCode,StationName,Address,City,Province,Latitude,Longitude,Capacity,CurrentAvailable,Owner,ContactPhone,ContactEmail,Description,IsActive")] Station station)
    {
        if (ModelState.IsValid)
        {
            try
            {
                station.CreatedDate = DateTime.Now;
                var id = await _soapClient.CreateAsync(station);
                TempData["SuccessMessage"] = "Station created successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating station: {ex.Message}");
            }
        }
        return View(station);
    }

    // GET: Station/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var station = await _soapClient.GetByIdAsync(id.Value);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading station: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Station/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StationId,StationCode,StationName,Address,City,Province,Latitude,Longitude,Capacity,CurrentAvailable,Owner,ContactPhone,ContactEmail,Description,IsActive,CreatedDate")] Station station)
    {
        if (id != station.StationId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                station.ModifiedDate = DateTime.Now;
                await _soapClient.UpdateAsync(id, station);
                TempData["SuccessMessage"] = "Station updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating station: {ex.Message}");
            }
        }
        return View(station);
    }

    // GET: Station/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var station = await _soapClient.GetByIdAsync(id.Value);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading station: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Station/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _soapClient.DeleteAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Station deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete station.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting station: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}

