using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BankingApp.Model {
    public enum Role {
        Client = 1,
        Manager = 2,
        Admin = 3
    }
    public abstract class User : EntityBase<BankingContext> {
        public User() { }
        public User(string lastName, string firstName, string email, string password) {
            LastName = lastName;
            FirstName = firstName;
            Email = email;
            Password = password;
        }
        public int UserId { get; set; }
        [Required]
        public string LastName { get; set; }
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public Role Role { get; set; }

        public static User GetById(int UserID) {
            return Context.Users.SingleOrDefault(u => u.UserId == UserID);
        }
    }
}
