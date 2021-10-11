using AutoMapper;
using JwtSecurityApi.Data.Model;
using JwtSecurityApi.Data.Security;
using JwtSecurityApi.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthManager authManager;
        private readonly ILogger<AccountController> logger;
        private readonly IMapper mapper;

        public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, IAuthManager authManager, IMapper mapper)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.authManager = authManager;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Endpoint to register a new user
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
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
                await userManager.AddToRolesAsync(user, userDto.Roles);
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

        /// <summary>
        /// Endpoint to login & generate token
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {

            logger.LogInformation($"Login Attempt for {userDto.Email}");
            //check if the values parsed are validated, if not correct, throw an error message
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            try
            {
                //check if the user is valid, if not valid, return Unauthorized
                //(await authManager.ValidateUser(userDto) == false)
                if (!await authManager.ValidateUser(userDto)) //either ways would work perfectly
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
                //if validated, create a token for the user
                return StatusCode(StatusCodes.Status200OK,
                    new { Token = await authManager.CreateToken() });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something went wrong in the {nameof(Login)}");
            }
        }

        /// <summary>
        /// Test endpoint
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="User")]
        [HttpGet("GetTest")]
        public string GetTest()
        {
            return "Success";
        }

    }
}
