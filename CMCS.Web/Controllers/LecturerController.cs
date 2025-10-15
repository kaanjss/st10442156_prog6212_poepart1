using CMCS.Web.Models;
using CMCS.Web.Models.ViewModels;
using CMCS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMCS.Web.Controllers;

public class LecturerController : Controller
{
	private readonly ClaimsService _claimsService;

	public LecturerController(ClaimsService claimsService)
	{
		_claimsService = claimsService;
	}

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

			claim = _claimsService.AddClaim(claim);

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
		var claims = _claimsService.GetAllClaims();
		return View(claims);
	}

	public IActionResult UploadDocuments(int id)
	{
		ViewBag.ClaimId = id;
		return View();
	}
}


