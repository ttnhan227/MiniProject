using Server.Models;

namespace Server.Repository;

public interface IAccountRepository
{
    // auth controller
    // account tang Total Amount vaf lay User
    Task<AccountModel> CheckLogin(AccountLogin accountLogin);
    string GenerateToken(AccountModel account);
    // account tang Total Amount
}