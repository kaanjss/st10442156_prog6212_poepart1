using CMCS.Web.Models;

namespace CMCS.Web.Services;

/// <summary>
/// In-memory claims service for Part 2 prototype
/// This simulates a database for demonstration purposes
/// </summary>
public class ClaimsService
{
	private static List<Claim> _claims = new();
	private static int _nextClaimId = 1;

	public IEnumerable<Claim> GetAllClaims()
	{
		return _claims.OrderByDescending(c => c.Id).ToList();
	}

	public Claim? GetClaimById(int id)
	{
		return _claims.FirstOrDefault(c => c.Id == id);
	}

	public IEnumerable<Claim> GetClaimsByStatus(ClaimStatus status)
	{
		return _claims.Where(c => c.Status == status).OrderByDescending(c => c.Id).ToList();
	}

	public IEnumerable<Claim> GetPendingClaimsForCoordinator()
	{
		// Coordinators verify submitted claims
		return _claims.Where(c => c.Status == ClaimStatus.Submitted).OrderByDescending(c => c.Id).ToList();
	}

	public IEnumerable<Claim> GetPendingClaimsForManager()
	{
		// Managers approve verified claims
		return _claims.Where(c => c.Status == ClaimStatus.Verified).OrderByDescending(c => c.Id).ToList();
	}

	public Claim AddClaim(Claim claim)
	{
		claim.Id = _nextClaimId++;
		_claims.Add(claim);
		return claim;
	}

	public bool UpdateClaimStatus(int claimId, ClaimStatus newStatus, string? comment = null)
	{
		var claim = GetClaimById(claimId);
		if (claim == null)
			return false;

		claim.Status = newStatus;
		return true;
	}

	public bool VerifyClaim(int claimId, string comment)
	{
		return UpdateClaimStatus(claimId, ClaimStatus.Verified, comment);
	}

	public bool ApproveClaim(int claimId, string comment)
	{
		return UpdateClaimStatus(claimId, ClaimStatus.Approved, comment);
	}

	public bool RejectClaim(int claimId, string comment)
	{
		return UpdateClaimStatus(claimId, ClaimStatus.Rejected, comment);
	}

	// Seed with sample data for demonstration
	public void SeedSampleData()
	{
		if (_claims.Any())
			return;

		var sampleClaims = new List<Claim>
		{
			new Claim
			{
				Id = _nextClaimId++,
				LecturerId = 1,
				Month = DateTime.UtcNow.Month,
				Year = DateTime.UtcNow.Year,
				HourlyRate = 500,
				TotalHours = 12,
				Amount = 6000,
				Status = ClaimStatus.Submitted,
				Lines = new List<ClaimLine>
				{
					new ClaimLine { Id = 1, ActivityDescription = "Lecture: PROG6212 - Introduction to C#", Hours = 6 },
					new ClaimLine { Id = 2, ActivityDescription = "Tutorial: Arrays and Lists", Hours = 4 },
					new ClaimLine { Id = 3, ActivityDescription = "Consultation: Student queries", Hours = 2 }
				}
			},
			new Claim
			{
				Id = _nextClaimId++,
				LecturerId = 2,
				Month = DateTime.UtcNow.Month,
				Year = DateTime.UtcNow.Year,
				HourlyRate = 450,
				TotalHours = 8,
				Amount = 3600,
				Status = ClaimStatus.Submitted,
				Lines = new List<ClaimLine>
				{
					new ClaimLine { Id = 4, ActivityDescription = "Lecture: IPMA3221 - Project Management", Hours = 5 },
					new ClaimLine { Id = 5, ActivityDescription = "Marking: Assignment 1", Hours = 3 }
				}
			},
			new Claim
			{
				Id = _nextClaimId++,
				LecturerId = 3,
				Month = DateTime.UtcNow.AddMonths(-1).Month,
				Year = DateTime.UtcNow.AddMonths(-1).Year,
				HourlyRate = 550,
				TotalHours = 15,
				Amount = 8250,
				Status = ClaimStatus.Verified,
				Lines = new List<ClaimLine>
				{
					new ClaimLine { Id = 6, ActivityDescription = "Lecture: DATA3043 - Database Design", Hours = 8 },
					new ClaimLine { Id = 7, ActivityDescription = "Lab Session: SQL Queries", Hours = 5 },
					new ClaimLine { Id = 8, ActivityDescription = "Marking: ICE Tasks", Hours = 2 }
				}
			}
		};

		_claims.AddRange(sampleClaims);
	}
}

