using EcormerProjectPRN222.Models;

namespace EcormerProjectPRN222.Dao.AccountDAO
{
    public class AccountDAO
    {
        private static readonly MyProjectClothingContext _context = new MyProjectClothingContext();
        
        public static Account GetAccount(string email, string password)
        {
            return _context.Accounts.SingleOrDefault(a => a.Email.Equals(email) && a.Password.Equals(password));
        }

        public static bool checkDuplicate(string email)
        {
            Account account = _context.Accounts.FirstOrDefault(s => s.Email.Equals(email));
            return account != null;
        }

        public static bool checkDuplicateUserName(string userName)
        {
            Account account = _context.Accounts.FirstOrDefault(s => s.Username.Equals(userName));
            return account != null;
        }

        public static void RegisterAccount(string userName, string password, string fullName, string email, string phone, string location)
        {
            Account account = new Account() { Username = userName, Password = password, FullName = fullName, Email = email, Phone = phone, Location = location, RoleId = 0 };
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public static Account GetAccountByEmail(string? email)
        {
            Account account = _context.Accounts.FirstOrDefault(s => s.Email.Equals(email));
            return account;
        }

        

        public static void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }
    }
}
