using CMCS.Web.Models;
using CMCS.Web.Services;
using Xunit;

namespace CMCS.Tests.Services;

public class ClaimsServiceTests
{
	[Fact]
	public void AddClaim_ShouldAssignIdAndAddToList()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = new Claim
		{
			LecturerId = 1,
			Month = 10,
			Year = 2025,
			HourlyRate = 500,
			TotalHours = 10,
			Amount = 5000,
			Status = ClaimStatus.Submitted
		};

		// Act
		var result = service.AddClaim(claim);

		// Assert
		Assert.NotEqual(0, result.Id);
		Assert.Equal(claim.LecturerId, result.LecturerId);
		Assert.Contains(result, service.GetAllClaims());
	}

	[Fact]
	public void GetClaimById_ExistingClaim_ShouldReturnClaim()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = new Claim
		{
			LecturerId = 1,
			Month = 10,
			Year = 2025,
			HourlyRate = 500,
			TotalHours = 10,
			Amount = 5000,
			Status = ClaimStatus.Submitted
		};
		var addedClaim = service.AddClaim(claim);

		// Act
		var result = service.GetClaimById(addedClaim.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(addedClaim.Id, result.Id);
	}

	[Fact]
	public void GetClaimById_NonExistingClaim_ShouldReturnNull()
	{
		// Arrange
		var service = new ClaimsService();

		// Act
		var result = service.GetClaimById(9999);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void VerifyClaim_ExistingClaim_ShouldUpdateStatus()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = new Claim
		{
			LecturerId = 1,
			Month = 10,
			Year = 2025,
			HourlyRate = 500,
			TotalHours = 10,
			Amount = 5000,
			Status = ClaimStatus.Submitted
		};
		var addedClaim = service.AddClaim(claim);

		// Act
		var result = service.VerifyClaim(addedClaim.Id, "Verified by coordinator");

		// Assert
		Assert.True(result);
		var verifiedClaim = service.GetClaimById(addedClaim.Id);
		Assert.Equal(ClaimStatus.Verified, verifiedClaim?.Status);
	}

	[Fact]
	public void ApproveClaim_ExistingClaim_ShouldUpdateStatus()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = new Claim
		{
			LecturerId = 1,
			Month = 10,
			Year = 2025,
			HourlyRate = 500,
			TotalHours = 10,
			Amount = 5000,
			Status = ClaimStatus.Verified
		};
		var addedClaim = service.AddClaim(claim);

		// Act
		var result = service.ApproveClaim(addedClaim.Id, "Approved by manager");

		// Assert
		Assert.True(result);
		var approvedClaim = service.GetClaimById(addedClaim.Id);
		Assert.Equal(ClaimStatus.Approved, approvedClaim?.Status);
	}

	[Fact]
	public void RejectClaim_ExistingClaim_ShouldUpdateStatus()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = new Claim
		{
			LecturerId = 1,
			Month = 10,
			Year = 2025,
			HourlyRate = 500,
			TotalHours = 10,
			Amount = 5000,
			Status = ClaimStatus.Submitted
		};
		var addedClaim = service.AddClaim(claim);

		// Act
		var result = service.RejectClaim(addedClaim.Id, "Missing documents");

		// Assert
		Assert.True(result);
		var rejectedClaim = service.GetClaimById(addedClaim.Id);
		Assert.Equal(ClaimStatus.Rejected, rejectedClaim?.Status);
	}

	[Fact]
	public void GetPendingClaimsForCoordinator_ShouldReturnSubmittedClaims()
	{
		// Arrange
		var service = new ClaimsService();
		var claim1 = service.AddClaim(new Claim { Status = ClaimStatus.Submitted, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var claim2 = service.AddClaim(new Claim { Status = ClaimStatus.Verified, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var claim3 = service.AddClaim(new Claim { Status = ClaimStatus.Submitted, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });

		// Act
		var result = service.GetPendingClaimsForCoordinator();

		// Assert
		// Result should contain at least our 2 submitted claims
		Assert.True(result.Count() >= 2);
		Assert.Contains(result, c => c.Id == claim1.Id && c.Status == ClaimStatus.Submitted);
		Assert.Contains(result, c => c.Id == claim3.Id && c.Status == ClaimStatus.Submitted);
		Assert.All(result, c => Assert.Equal(ClaimStatus.Submitted, c.Status));
	}

	[Fact]
	public void GetPendingClaimsForManager_ShouldReturnVerifiedClaims()
	{
		// Arrange
		var service = new ClaimsService();
		var claim1 = service.AddClaim(new Claim { Status = ClaimStatus.Submitted, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var claim2 = service.AddClaim(new Claim { Status = ClaimStatus.Verified, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var claim3 = service.AddClaim(new Claim { Status = ClaimStatus.Verified, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });

		// Act
		var result = service.GetPendingClaimsForManager();

		// Assert
		// Result should contain at least our 2 verified claims
		Assert.True(result.Count() >= 2);
		Assert.Contains(result, c => c.Id == claim2.Id && c.Status == ClaimStatus.Verified);
		Assert.Contains(result, c => c.Id == claim3.Id && c.Status == ClaimStatus.Verified);
		Assert.All(result, c => Assert.Equal(ClaimStatus.Verified, c.Status));
	}

	[Fact]
	public void AddDocumentToClaim_ExistingClaim_ShouldAddDocument()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = service.AddClaim(new Claim { Status = ClaimStatus.Submitted, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var document = new Document
		{
			FileName = "timesheet.pdf",
			FilePath = "/uploads/1/timesheet.pdf"
		};

		// Act
		var result = service.AddDocumentToClaim(claim.Id, document);

		// Assert
		Assert.True(result);
		var updatedClaim = service.GetClaimById(claim.Id);
		Assert.Single(updatedClaim?.Documents ?? new List<Document>());
		Assert.Equal("timesheet.pdf", updatedClaim?.Documents.First().FileName);
	}

	[Fact]
	public void AddDocumentToClaim_NonExistingClaim_ShouldReturnFalse()
	{
		// Arrange
		var service = new ClaimsService();
		var document = new Document
		{
			FileName = "timesheet.pdf",
			FilePath = "/uploads/1/timesheet.pdf"
		};

		// Act
		var result = service.AddDocumentToClaim(9999, document);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void RemoveDocumentFromClaim_ExistingDocument_ShouldRemove()
	{
		// Arrange
		var service = new ClaimsService();
		var claim = service.AddClaim(new Claim { Status = ClaimStatus.Submitted, Month = 10, Year = 2025, HourlyRate = 500, TotalHours = 10, Amount = 5000 });
		var document = new Document { FileName = "timesheet.pdf", FilePath = "/uploads/1/timesheet.pdf" };
		service.AddDocumentToClaim(claim.Id, document);
		var addedDoc = service.GetClaimById(claim.Id)?.Documents.First();

		// Act
		var result = service.RemoveDocumentFromClaim(claim.Id, addedDoc!.Id);

		// Assert
		Assert.True(result);
		var updatedClaim = service.GetClaimById(claim.Id);
		Assert.Empty(updatedClaim?.Documents ?? new List<Document>());
	}
}

