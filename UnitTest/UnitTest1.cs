using Xunit;
using Prog2Bpartone.Services;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;
using System.Text;

namespace UnitTest
{
    public class UnitTest1
    // 5 unit tests
    {
        [Fact]
        public void SubmitClaim_ShouldAddNewClaim()
        {

            var initialCount = ClaimSection.GetAllClaims().Count;

            string claimName = "New Monthly Claim";
            string claimType = "Monthly Contract";
            string claimMonth = "October";


            bool result = ClaimSection.SubmitClaim(claimName, claimType, claimMonth);


            Assert.True(result);

            var allClaims = ClaimSection.GetAllClaims();
            Assert.Equal(initialCount + 1, allClaims.Count);

            var newClaim = allClaims[^1];
            Assert.Equal(claimName, newClaim.ClaimName);
            Assert.Equal(claimType, newClaim.ClaimType);
            Assert.Equal(claimMonth, newClaim.ClaimMonth);
            Assert.Equal(ClaimStatus.Pending, newClaim.Status);
            Assert.True((DateTime.Now - newClaim.SubmittedDate).TotalSeconds < 5);
        }

        [Fact]
        public void SubmitClaim_ShouldReturnFalse()
        {

            bool resultEmptyName = ClaimSection.SubmitClaim("", "Hourly", "October");
            bool resultEmptyType = ClaimSection.SubmitClaim("Claim 12", "", "October");
            bool resultEmptyMonth = ClaimSection.SubmitClaim("Claim 13", "Hourly", "");


            Assert.False(resultEmptyName);
            Assert.False(resultEmptyType);
            Assert.False(resultEmptyMonth);
        }


        [Fact]

        public async Task EncryptionFile_Successful()
        {
            var orginalContent = "This is a secret content that should be encrypted";
            var orginalBytes = Encoding.UTF8.GetBytes(orginalContent);
            var inputStream = new MemoryStream(orginalBytes);
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                Assert.True(File.Exists(tempFile), "Encrypted file should exist");

                var encryptedBytes = await File.ReadAllBytesAsync(tempFile);


                Assert.NotEqual(orginalBytes, encryptedBytes);


                Assert.True(encryptedBytes.Length > 0, "Encrypted file should have content");


                var encrpytedTest = Encoding.UTF8.GetString(encryptedBytes);
                Assert.DoesNotContain("This is a secret content that should be encrypted", encrpytedTest);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);

            }
        }
        [Fact]
        public void UpdateStatus_SetReviewedDate()
        {

            var claim = ClaimSection.GetClaimById(2);


            bool result = ClaimSection.UpdateStatus(claim.ClaimId, ClaimStatus.Approved);


            Assert.True(result);
            Assert.NotNull(claim.ReviewedDate);
            Assert.True((DateTime.Now - claim.ReviewedDate.Value).TotalSeconds < 5);
        }
        [Fact]
        public void ShouldHandleDocumentsCorrectly()
        {

            var documents = new List<UploadedDocument>
            {
                new UploadedDocument { FileName = "proof.pdf", FilePath = "/uploads/proof.pdf" }
            };

            bool result = ClaimSection.SubmitClaim("Documented Claim", "Expense Claim", "November", documents);


            Assert.True(result);
            var newClaim = ClaimSection.GetAllClaims()[^1];
            Assert.Single(newClaim.Documents);
            Assert.Equal("proof.pdf", newClaim.Documents[0].FileName);
        }
    }
}



