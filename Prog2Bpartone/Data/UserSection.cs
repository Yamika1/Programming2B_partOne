using Prog2Bpartone.Models;

namespace Prog2Bpartone.Data
{
    public class UserSection
    {
        private static List<HR> _users = new List<HR>
        {
            new HR
            {
                Id = 1,
              FullName = "Yamika Govender",
                Email = "yamik@gmail.com",
                ContactNumber = 0823456789,
                hourlyRate = 250,
                Status = UserStatus.Approved,
                   Documents = new List<UploadedDocument>()
            },
              new HR
            {
               Id = 2,
            FullName = "Shaolin Govender",
                Email = "shao@gmail.com",
                ContactNumber = 0796856432,
                hourlyRate = 150,
                  Status = UserStatus.Declined,
                   Documents = new List<UploadedDocument>()
            },
                new HR
            {
                 Id = 3,
            FullName = "Joe Miller",
                Email = "jMiller@gmail.com",
                ContactNumber = 0832456758,
                hourlyRate = 689,
                  Status = UserStatus.Approved,
                   Documents = new List<UploadedDocument>()
            }
        };
        private static int _Id = 20;
        public static List<HR> GetAllUsers() => _users.ToList();

        public static HR? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public static List<HR> GetUsersByStatus(UserStatus status)
            => _users.Where(u => u.Status == status).ToList();

        public static void AddUser(HR users)
        {
            users.Id = _Id;
            _Id++;

            users.Status = UserStatus.Approved;
            _users.Add(users);
        }
        public static bool UpdateStatus(int id, UserStatus newStatus)
        {
            var users = _users.FirstOrDefault(u => u.Id == id);
            if (users == null) return false;



            users.Status = newStatus;


            return true;
        }
        public static bool NewUser(string fullName, string email, int HourlyRate, int contactNumber, List<UploadedDocument>? documents = null)
        {

            if (string.IsNullOrWhiteSpace(fullName) ||

                string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var newUser = new HR
            {
                Id = _Id++,
                FullName = fullName,
                hourlyRate = HourlyRate,
                ContactNumber = contactNumber,
                Status = UserStatus.Approved,
                Email = email,
                Documents = documents ?? new List<UploadedDocument>()
            };

            _users.Add(newUser);
            return true;
        }
    }
}
