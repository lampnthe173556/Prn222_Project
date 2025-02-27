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
    }
}
