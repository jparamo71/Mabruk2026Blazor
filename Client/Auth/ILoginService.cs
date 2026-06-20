using MabrukBlazor2026.Shared.Dtos;

namespace MabrukBlazor2026.Client.Auth
{
    public interface ILoginService
    {
        Task Login(UserTokenDto tokenDTO);
        Task Logout();
        Task<string> GetClaimByType(string claimtype);
    }
}
