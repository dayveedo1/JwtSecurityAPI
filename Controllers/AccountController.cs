using AutoMapper;
using JwtSecurityApi.Data.Model;
using JwtSecurityApi.Data.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtSecurityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<ApiUser> userManager;
        //private readonly SignInManager<ApiUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IMapper mapper;

        public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, IMapper mapper)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            logger.LogInformation($"Registration Attempt for {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await userManager.CreateAsync(user, userDto.Password);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState);
                }
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in {nameof(Register)}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
                //another return type for an exception
                //return Problem($"Something went wrong in {nameof(Register)}", statusCode: 500);
            }
        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        //{

        //    logger.LogInformation($"Login Attempt for {userDto.Email}");
        //    //check if the values parsed are validated, if not correct, throw an error message
        //    if (!ModelState.IsValid)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, ModelState);
        //    }

        //    try
        //    {

        //        var result = await signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, false, false);
        //        if (!result.Succeeded)
        //        {
        //            return StatusCode(StatusCodes.Status401Unauthorized, userDto);
        //        }
        //        return StatusCode(StatusCodes.Status200OK);

        //        //check if the user is valid, if not valid, return Unauthorized
        //        //if (!await authManager.ValidateUser(userDto)) //either ways would work perfectly
        //        //{
        //        //    return StatusCode(StatusCodes.Status401Unauthorized);
        //        //}
        //        ////if validated, create a token for the user
        //        //return StatusCode(StatusCodes.Status200OK,
        //        //    new { Token = await authManager.CreateToken() });
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Something went wrong in the {nameof(Login)}");
        //    }
        //}

    }
}
