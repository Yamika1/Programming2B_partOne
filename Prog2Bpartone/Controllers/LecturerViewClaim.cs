using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;
using System.Security.Claims;

namespace Prog2Bpartone.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerViewClaim : Controller
    {
        public IActionResult Index(string filter = "all")
        {
            try
            {
                var claims = ClaimSection.GetAllClaims();
                ViewBag.Filter = filter;

                claims = filter.ToLower() switch
                {
                    "pending" => ClaimSection.GetClaimsByStatus(ClaimStatus.Pending),
                    "approved" => ClaimSection.GetClaimsByStatus(ClaimStatus.Approved),
                    "declined" => ClaimSection.GetClaimsByStatus(ClaimStatus.Declined),
                    "verified" => ClaimSection.GetClaimsByStatus(ClaimStatus.Verified),
                    _ => claims
                };

                foreach (var c in claims)
                {
                    c.TotalAmount = c.HoursWorked * c.HourlyRate;
                }

                ViewBag.PendingCount = ClaimSection.GetPendingCount();
                ViewBag.ApprovedCount = ClaimSection.GetApprovedCount();
                ViewBag.DeclinedCount = ClaimSection.GetDeclinedCount();
                ViewBag.VerifiedCount = ClaimSection.GetVerifyCount();
                ViewBag.RejectedCount = ClaimSection.GetRejectedCount();

                return View(claims);
            }
            catch
            {
                ViewBag.Error = "Unable to load claims.";
                return View(new List<Claims>());
            }
        }

        public IActionResult Review(int id)
        {
            try
            {
                var claim = ClaimSection.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;

                return View(claim);
            }
            catch
            {
                TempData["Error"] = "Error loading claim.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? comments)
        {
            try
            {
                var success = ClaimSection.UpdateStatus(id, ClaimStatus.Approved);
                if (success)
                {
                    TempData["Success"] = "Claim approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error approving claim.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decline(int id, string? comments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for declining.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                var success = ClaimSection.UpdateStatus(id, ClaimStatus.Declined);

                if (success)
                {
                    TempData["Success"] = "Claim declined.";
                }
                else
                {
                    TempData["Error"] = "Claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error declining claim.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}