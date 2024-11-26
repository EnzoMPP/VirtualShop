using VirtualShopMinimalAPI.Models;

public interface IUserService
{
    Task<IResult> RegisterUser(User user);
    Task<IResult> LoginUser(LoginRequest loginRequest);
    Task<IResult> GetUserProfile(string email);
    Task<IResult> UpdateUserProfile(string email, User updatedUser);
    Task<IResult> AddAdmin(User admin);
            Task<IResult> GetPurchasedProducts(string email);

}