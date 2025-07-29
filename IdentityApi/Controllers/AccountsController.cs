using IdentityApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        //user işlemleri için userManager
        private readonly UserManager<IdentityUser> _userManager;

        //login işlemleri için gerekli signInManager
        private readonly SignInManager<IdentityUser> _signInManager;

        //role işlemleri için gerekli roleManager
        private readonly RoleManager<IdentityRole> _roleManager;

        //dependency injection ile başka sınıflardan nesneler kullanma imkanı sağlıyorum
        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //kayıt işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //model valid mi kontrolü
            if (ModelState.IsValid)
            {
                //valid ise IdentityUser sınıfından yeni bir user oluşturuyorum
                var user = new IdentityUser
                {
                    //ilgili bilgileri modelimden alıyorum
                    UserName = model.Email,
                    Email = model.Email
                };
                //userManager ile kayıt oluşturup kayıt işleminin sonucunu tutuyorum 
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { message = "Kayıt başarılı" });
                }
                else
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }
            }
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
        //login işlemi
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //signInManager ile login işlemini sağlıyorum
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    return Ok(new { message = "Giriş başarılı" });
                }
                else
                {
                    return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı" });
                }
            }
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
        //role oluşturma
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            //role name verilmiş mi 
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                //roleManager ile role oluşturuyorum
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    return Ok(new { message = "Rol başarıyla oluşturuldu" });
                }

                else
                {
                    return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
                }
            }
            return BadRequest(new { message = "Rol adı boş olamaz" });
        }
        //kullanıcıya role ekleme
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(AddRoleViewModel model)
        {
            // kullanıcı var mı kontrol ediyorum
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null)
            {
                return BadRequest(new { message = "Kullanıcı bulunamadı" });
            }
            //role var mı diye kontrol ediyorum
            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                return NotFound(new { message = "Rol bulunamadı" });
            }
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                return Ok(new { message = "Rol başarıyla eklendi" });
            }
            else
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
        }
        //kullanıcının rolleri
        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı" });
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(roles);
            }
        }
        //tüm roller
        [HttpGet("roles")]
        public IActionResult GetRoles() {
            var roles=_roleManager.Roles.ToList();
            return Ok(roles);
        }
    }
}