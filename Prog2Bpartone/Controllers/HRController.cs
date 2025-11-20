using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;
using Prog2Bpartone.Services;

namespace Prog2Bpartone.Controllers
{
    [Authorize(Roles = "super user")]
    public class HRController : Controller
    {
        private readonly ILogger<HRController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly FileEncryptionService _encryptionService;

        public HRController(ILogger<HRController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            _encryptionService = new FileEncryptionService();
        }


        public IActionResult Index()
        {
            var users = UserSection.GetAllUsers();
            return View(users);
        }


        public IActionResult AddUser() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(List<IFormFile> documents, HR users)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(users.FullName))
                {
                    ViewBag.Error = "Full Name is required";
                    return View(users);
                }

                if (string.IsNullOrWhiteSpace(users.Email))
                {
                    ViewBag.Error = "Email is required";
                    return View(users);
                }

                if (users.Documents == null)
                    users.Documents = new List<UploadedDocument>();

                if (documents != null && documents.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".xlsx" };
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ViewBag.Error = $"File extension {extension} not allowed";
                                return View(users);
                            }

                            var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                            var encryptedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = file.OpenReadStream())
                            {
                                await _encryptionService.EncryptFileAsync(fileStream, encryptedFilePath);
                            }

                            users.Documents.Add(new UploadedDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                FileSize = file.Length,
                                IsEncrypted = true
                            });
                        }
                    }
                }

                UserSection.AddUser(users);
                TempData["Success"] = "User submitted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error adding user: " + ex.Message;
                return View(users);
            }
        }

        // Details
        public IActionResult Details(int id)
        {
            var user = UserSection.GetUserById(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> DownloadDocument(int userId, int docId)
        {
            try
            {
                var user = UserSection.GetUserById(userId);
                if (user == null)
                    return NotFound("User not found.");

                var doc = user.Documents.FirstOrDefault(d => d.Id == docId);
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
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }


        public IActionResult Review(int id)
        {
            var user = UserSection.GetUserById(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var success = UserSection.UpdateStatus(id, UserStatus.Approved);
            TempData[success ? "Success" : "Error"] = success ? "User approved." : "User not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decline(int id, string comments)
        {
            if (string.IsNullOrWhiteSpace(comments))
            {
                TempData["Error"] = "Please provide a reason for declining.";
                return RedirectToAction(nameof(Review), new { id });
            }

            var success = UserSection.UpdateStatus(id, UserStatus.Declined);
            TempData[success ? "Success" : "Error"] = success ? "User declined." : "User not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}