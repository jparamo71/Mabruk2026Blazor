using MabrukBlazor2026.Client.Helper;
using MabrukBlazor2026.Client.Repository;
using MabrukBlazor2026.Shared.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MabrukBlazor2026.Client.Auth
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider, ILoginService
    {

        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        private readonly IRepository repository;

        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";

        private AuthenticationState Anonymous =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));



        public CustomAuthenticationProvider(
            IJSRuntime js,
            HttpClient httpClient,
            IRepository repository)
        {
            this.js = js;
            this.httpClient = httpClient;
            this.repository = repository;
        }


        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await js.GetFromSessionStorage(TOKENKEY);

            if (token is null)
            {
                return Anonymous;
            }

            var expirationTimeObject = await js.GetFromSessionStorage(EXPIRATIONTOKENKEY);
            DateTime expirationTime;

            if (expirationTimeObject is null)
            {
                await Clear();
                return Anonymous;
            }

            if (DateTime.TryParse(expirationTimeObject.ToString(), out expirationTime))
            {
                if (ExpiredToken(expirationTime))
                {
                    await Clear();
                    return Anonymous;
                }

                if (ShouldRenewToken(expirationTime))
                {
                    token = await RenewToken(token.ToString()!);
                }
            }

            return BuildAuthenticationState(token.ToString()!);
        }


        private bool ExpiredToken(DateTime expirationTime)
        {
            return expirationTime <= DateTime.UtcNow;
        }


        private bool ShouldRenewToken(DateTime expirationTime)
        {
            return expirationTime.Subtract(DateTime.UtcNow) < TimeSpan.FromMinutes(5);
        }


        public async Task RenewTokenManagement()
        {
            var expirationTimeObject = await js.GetFromSessionStorage(EXPIRATIONTOKENKEY);
            DateTime expirationTime;

            if (DateTime.TryParse(expirationTimeObject.ToString(), out expirationTime))
            {
                if (ExpiredToken(expirationTime))
                {
                    await Logout();
                }

                if (ShouldRenewToken(expirationTime))
                {
                    var token = await js.GetFromSessionStorage(TOKENKEY);
                    var newToken = await RenewToken(token.ToString()!);
                    var authState = BuildAuthenticationState(newToken);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }


        private async Task<string> RenewToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token);

            var newTokenResponse = await repository.Get<UserTokenDto>("api/cuentas/RenovarToken");
            var newToken = newTokenResponse.Response!;

           
            await js.SaveInSessionStorage(TOKENKEY, newToken.Token);
            await js.SaveInSessionStorage(EXPIRATIONTOKENKEY, newToken.Expiration.ToString());

            return newToken.Token;
        }


        public async Task<bool> UpdateToken(UserTokenDto token)
        {
            await js.SaveInSessionStorage(TOKENKEY, token.Token);
            await js.SaveInSessionStorage(EXPIRATIONTOKENKEY, token.Expiration.ToString());

            return true;
        }


        public AuthenticationState BuildAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var claims = ParseClaimsFromJwt(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "jwt")));
        }


        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var deserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            return deserializedToken.Claims;
        }


        public async Task Login(UserTokenDto tokenDTO)
        {
            await js.SaveInSessionStorage(TOKENKEY, tokenDTO.Token);
            await js.SaveInSessionStorage(EXPIRATIONTOKENKEY, tokenDTO.Expiration.ToString());
            var authState = BuildAuthenticationState(tokenDTO.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }


        public async Task Logout()
        {
            await Clear();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }


        public async Task<string> GetClaimByType(string claimtype)
        {
            var token = await js.GetFromSessionStorage(TOKENKEY);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.ToString());
            string claimValue = jwt.Claims.First(c => c.Type == claimtype).Value;
            return claimValue;
        }
        


        private async Task Clear()
        {
            await js.RemoveFromSessionStorage(TOKENKEY);
            await js.RemoveFromSessionStorage(EXPIRATIONTOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

    }
}
