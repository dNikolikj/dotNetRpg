namespace dotNetRpg.Data
{
    public interface IAuthRepository
    {

        Task<ServiceResponse<int>> Register(User user, string password);

        Task<ServiceResponse<string>> LogIn(string username, string password);
        Task<bool> UserExists(string userName);

    }
}
