using CMCS.Web.Models;
using CMCS.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers;

public class LecturerController : Controller
{
	// In-memory storage for demo purposes (Part 2 prototype)
	private static List<Claim> _claims = new();
	private static int _nextClaimId = 1;

	public IActionResult Dashboard()
	{
		return View();
	}

	[HttpGet]
	public IActionResult Submit()
	{
		var vm = new ClaimSubmissionViewModel
		{
			Month = DateTime.UtcNow.Month,
			Year = DateTime.UtcNow.Year,
			HourlyRate = 0,
			Items = new List<ClaimLineItem>
			{
				new ClaimLineItem() // Pre-populate one empty row
			}
		};
		return View(vm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Submit(ClaimSubmissionViewModel model)
	{
		try
		{
			// Remove empty items before validation
			model.Items = model.Items?.Where(x => !string.IsNullOrWhiteSpace(x.ActivityDescription) || x.Hours > 0).ToList() ?? new List<ClaimLineItem>();

			// Validate at least one activity is provided
			if (!model.Items.Any())
			{
				ModelState.AddModelError("Items", "At least one activity is required");
			}

			// Validate individual items
			for (int i = 0; i < model.Items.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(model.Items[i].ActivityDescription))
				{
					ModelState.AddModelError($"Items[{i}].ActivityDescription", "Activity description is required");
				}
				if (model.Items[i].Hours <= 0)
				{
					ModelState.AddModelError($"Items[{i}].Hours", "Hours must be greater than 0");
				}
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Create and save the claim
			var claim = new Claim
			{
				Id = _nextClaimId++,
				LecturerId = 1, // Hardcoded for prototype
				Month = model.Month,
				Year = model.Year,
				HourlyRate = model.HourlyRate,
				TotalHours = model.TotalHours,
				Amount = model.TotalAmount,
				Status = ClaimStatus.Submitted,
				Lines = model.Items.Select(item => new ClaimLine
				{
					ActivityDescription = item.ActivityDescription,
					Hours = item.Hours
				}).ToList()
			};

			_claims.Add(claim);

			TempData["SuccessMessage"] = $"Claim submitted successfully! Claim ID: {claim.Id}. Total Amount: R{claim.Amount:N2}";
			return RedirectToAction(nameof(MyClaims));
		}
		catch (Exception ex)
		{
			ModelState.AddModelError(string.Empty, $"An error occurred while submitting your claim: {ex.Message}");
			return View(model);
		}
	}

	public IActionResult MyClaims()
	{
		// Combine in-memory claims with sample data
		var allClaims = new List<Claim>(_claims);
		
		if (!allClaims.Any())
		{
			// Show sample data if no claims submitted yet
			allClaims.Add(new Claim { Id = 101, Month = DateTime.UtcNow.Month, Year = DateTime.UtcNow.Year, Status = ClaimStatus.Submitted, TotalHours = 10, HourlyRate = 500, Amount = 5000 });
			allClaims.Add(new Claim { Id = 102, Month = DateTime.UtcNow.AddMonths(-1).Month, Year = DateTime.UtcNow.Year, Status = ClaimStatus.Verified, TotalHours = 8, HourlyRate = 500, Amount = 4000 });
		}

		return View(allClaims.OrderByDescending(c => c.Id).ToList());
	}

	public IActionResult UploadDocuments(int id)
	{
		ViewBag.ClaimId = id;
		return View();
	}
}


