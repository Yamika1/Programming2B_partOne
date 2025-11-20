using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;
using Prog2Bpartone.Services;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace Prog2Bpartone.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly FileEncryptionService _encryptionService;
        private readonly ILogger<LecturerController> _logger;

        public LecturerController(
            AppDBContext context,
            IWebHostEnvironment environment,

            ILogger<LecturerController> logger)
        {
            _context = context;
            _environment = environment;
            _encryptionService = new FileEncryptionService();

            _logger = logger;
        }



        public IActionResult Index()
        {
            try
            {

                var dbClaims = _context.Claims
                    .Include(c => c.Documents)
                    .ToList();


                var staticClaims = ClaimSection.GetAllClaims();


                var allClaims = dbClaims.Concat(staticClaims).ToList();

                return View(allClaims);
            }
            catch (Exception)
            {
                ViewBag.Error = "Unable to load claims";
                return View(new List<Claims>());
            }
        }


        public IActionResult AddClaim()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClaim(List<IFormFile> documents, Claims claims)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(claims.ClaimName))
                {
                    ModelState.AddModelError("ClaimName", "Claim name is required.");
                }

                if (string.IsNullOrWhiteSpace(claims.ClaimType))
                {
                    ModelState.AddModelError("ClaimType", "Claim type is required.");
                }


                if (claims.HoursWorked > 180)
                {
                    ModelState.AddModelError("HoursWorked", "Hours worked cannot exceed 180 hours.");
                }

                if (!ModelState.IsValid)
                {
                    return View(claims);
                }


                claims.TotalAmount = claims.HoursWorked * claims.HourlyRate;


                claims.SubmittedDate = DateTime.Now;
                claims.Status = ClaimStatus.Pending;


                if (documents != null && documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".xlsx" };
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("Documents", $"File type {extension} not allowed.");
                                return View(claims);
                            }

                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                            var encryptedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = file.OpenReadStream())
                            {
                                await _encryptionService.EncryptFileAsync(fileStream, encryptedFilePath);
                            }

                            claims.Documents.Add(new UploadedDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                FileSize = file.Length,
                                IsEncrypted = true
                            });
                        }
                    }
                }


                _context.Claims.Add(claims);
                _context.SaveChanges();

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error submitting claim: " + ex.Message;
                return View(claims);
            }
        }


        public IActionResult Details(int id)
        {
            try
            {

                var dbClaim = _context.Claims
                    .Include(c => c.Documents)
                    .FirstOrDefault(c => c.ClaimId == id);

                if (dbClaim != null)
                    return View(dbClaim);


                var staticClaim = ClaimSection.GetClaimById(id);

                if (staticClaim != null)
                    return View(staticClaim);

                TempData["Error"] = "Claim not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error loading claim details.";
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> DownloadDocument(int claimId, int docId)
        {
            var claim = _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
                return NotFound("Claim not found.");

            var doc = claim.Documents.FirstOrDefault(d => d.Id == docId);
            if (doc == null)
                return NotFound("Document not found.");

            var filePath = Path.Combine(_environment.WebRootPath, doc.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            using var decryptedStream = await _encryptionService.DecryptFileStream(filePath);

            var contentType = Path.GetExtension(doc.FileName).ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(decryptedStream.ToArray(), contentType, doc.FileName);
        }
    }
}
