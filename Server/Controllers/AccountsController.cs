using AutoMapper;
using MabrukBlazor2026.Shared.Dtos;
using MabrukBlazor2026.Shared.Models;
using MabrukBlazor2026.Shared.ModelsInventario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MabrukBlazor2026.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly MabrukContext context;
        private readonly MabrukInventarioContext inventarioContext;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;

        public AccountsController(
            MabrukContext context,
            MabrukInventarioContext inventarioContext,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            this.context = context;
            this.inventarioContext = inventarioContext;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }


        /// <summary>
        /// Validating if email account already exists in
        /// Mabruk database system. Then generate a jwt token.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        [HttpGet("exists_email/{email}")]
        public async Task<ActionResult<UserInfoAccountDto>> GetExistsEmail(string email)
        {
            var user = await context.Usuario
                .FirstOrDefaultAsync(x => x.CorreoElectronico.ToUpper().Equals(email.ToUpper()));
            if (user != null)
            {
                var userInfoAccount = new UserInfoAccountDto()
                {
                    UserId = user.UsuarioId,
                    Email = user.CorreoElectronico,
                    FullName = user.NombreUsuario,
                    AllowUploadImage = false
                };

                var permiso = await inventarioContext.Usuariopermiso
                    .FirstOrDefaultAsync(x => x.UsuarioId == user.UsuarioId);

                if (permiso != null)
                {
                    userInfoAccount.AllowUploadImage = permiso.PermitidoSubirImagen;
                }

                UserInfoDto infoUser = new UserInfoDto()
                {
                    UserName = userInfoAccount.FullName,
                    Email = userInfoAccount.Email,
                    UserId = userInfoAccount.UserId
                };
                UserTokenDto token = BuildToken(UserInfoDto userInfo, List<string> roles);

                return Ok(userInfoAccount);
            }
            return NotFound();
        }



        /// <summary>
        /// Autentication login using Mabruk system usuario entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns>a single token jwt</returns>
        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(UserInfoDto model)
        {
            MabrukBlazor2026.Shared.Models.OutputParameter<int?> errorNumber = new MabrukBlazor2026.Shared.Models.OutputParameter<int?>();
            var usersDB = await context.Procedures.sp_autenticar_2Async(model.UserName, model.Password, 0, errorNumber);
            if (usersDB != null && usersDB.Any()) 
            {
                sp_autenticar_2Result firstUser = usersDB.FirstOrDefault()!;

                List<string> roles = new List<string>() { firstUser.NombrePerfil };
                var token  = BuildToken(model, roles);
                return Ok(token);
            }
            return BadRequest();
        }



        /// <summary>
        /// Genera un token con solo la cuenta de correo electrónico.
        /// Es útil en la app donde no se requiere introducir password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("loginapp")]
        public async Task<ActionResult> PostLoginApp(string email)
        {
            var userInfo = await context.Usuario
                .Include(x => x.Perfil)
                .FirstOrDefaultAsync(x => x.CorreoElectronico.ToUpper().Equals(email.ToUpper()));
            if (userInfo != null) {
                UserInfoDto userInfoDto = new UserInfoDto()
                { 
                    UserId = userInfo.UsuarioId.ToString(),
                    UserName =userInfo.NombreUsuario,
                    Email  = userInfo.CorreoElectronico
                };

                List<string> roles = new List<string>() { userInfo.Perfil.NombrePerfil };
                var token = BuildToken(userInfoDto, roles);
                return Ok(new
                {
                    ok = true,
                    token
                });
            }

            return BadRequest();
        }




        #region private 


        /// <summary>
        /// Build a JWT with userinfo
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private UserTokenDto BuildToken(UserInfoDto userInfo, List<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Sid, userInfo.UserId)
            };


            httpContextAccessor?.HttpContext?.Session.SetString("UserId", userInfo.UserId!);


            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
                );

            return new UserTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

        }



        #endregion

    }
}
