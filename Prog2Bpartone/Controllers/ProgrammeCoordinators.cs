using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;
using System;
using System.Security.Claims;

namespace Prog2Bpartone.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProgrammeCoordinators : Controller
    {
        private readonly AppDBContext _context;

        private readonly ILogger<ProgrammeCoordinators> _logger;

        public ProgrammeCoordinators(AppDBContext context, ILogger<ProgrammeCoordinators> logger)
        {
            _context = context;

            _logger = logger;
        }

        public IActionResult Index(string filter = "all")
        {
            try
            {

                var claims = ClaimSection.GetAllClaims();

                filter = filter.ToLower();
                claims = filter switch
                {
                    "pending" => ClaimSection.GetClaimsByStatus(ClaimStatus.Pending),


                    "verified" => ClaimSection.GetClaimsByStatus(ClaimStatus.Verified),
                    "rejected" => ClaimSection.GetClaimsByStatus(ClaimStatus.Rejected),
                    _ => claims
                };

                ViewBag.Filter = filter;
                ViewBag.PendingCount = ClaimSection.GetPendingCount();
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
                var claims = ClaimSection.GetClaimById(id);
                if (claims == null)
                {
                    TempData["Error"] = "claims not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(claims);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading claims.";
                return RedirectToAction(nameof(Index));
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(int id)
        {
            try
            {
                var success = ClaimSection.UpdateStatus(id, ClaimStatus.Verified);

                if (success)
                {
                    TempData["Success"] = "Claim verified successfully!";

                    return RedirectToAction(nameof(Index), new { filter = "verified" });
                }
                else
                {
                    TempData["Error"] = "Claim not verified.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error verifying claim.";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string? comments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for rejecting.";
                    return RedirectToAction(nameof(Review), new { id });
                }


                var success = ClaimSection.UpdateStatus(id, ClaimStatus.Rejected);

                if (success)
                {
                    TempData["Success"] = "claim rejected.";
                }
                else
                {
                    TempData["Error"] = "claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error rejecting claim.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}