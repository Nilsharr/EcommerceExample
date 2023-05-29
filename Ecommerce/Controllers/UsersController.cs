using Ecommerce.Dtos;
using Ecommerce.Enums;
using Ecommerce.Filters;
using Ecommerce.Interfaces;
using Ecommerce.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = Ecommerce.Interfaces.IAuthorizationService;

namespace Ecommerce.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserLoginRequest> _loginValidator;
    private readonly IValidator<UserRegisterRequest> _registerValidator;
    private readonly IValidator<AddressDto> _addressValidator;
    private readonly IValidator<ProductIdItemDto> _productIdItemValidator;

    public UsersController(IAuthorizationService authorizationService, IRepositoryFactory repositoryFactory,
        IValidator<UserLoginRequest> loginValidator, IValidator<UserRegisterRequest> registerValidator,
        IValidator<AddressDto> addressValidator, IValidator<ProductIdItemDto> productIdItemValidator)
    {
        _authorizationService = authorizationService;
        _userRepository = repositoryFactory.CreateUserRepository();
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _addressValidator = addressValidator;
        _productIdItemValidator = productIdItemValidator;
    }

    [HttpGet]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<UserDto>> GetAll()
    {
        var users = await _userRepository.GetAll();
        return Ok(users);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Get()
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetById(userId);
        if (user is not null)
        {
            return Ok(user);
        }

        return NotFound();
    }

    [HttpGet("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<UserDto>> Get(string id)
    {
        var user = await _userRepository.GetById(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserAuthorizationResponse>> Register(UserRegisterRequest registerRequest)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var hashedPassword = _authorizationService.HashPassword(registerRequest.Password);
        var user = await _userRepository.Add(new UserDto
        {
            Name = registerRequest.Name, Surname = registerRequest.Surname, Email = registerRequest.Email,
            Password = hashedPassword.hashedPassword, Salt = hashedPassword.salt
        });

        var token = await _authorizationService.GenerateAuthToken(user.Id);
        return Ok(new UserAuthorizationResponse
            {AuthToken = token.authToken, TokenExpireDate = token.tokenExpireDate, Role = UserRole.User});
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserAuthorizationResponse>> Login(UserLoginRequest loginRequest)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var user = await _userRepository.GetByEmail(loginRequest.Email);
        if (user is null)
        {
            return Unauthorized();
        }

        var isValid = _authorizationService.VerifyHashedPassword(loginRequest.Password, user.Password, user.Salt);
        if (!isValid)
        {
            return Unauthorized();
        }

        var token = await _authorizationService.GenerateAuthToken(user.Id);
        return Ok(new UserAuthorizationResponse
            {AuthToken = token.authToken, TokenExpireDate = token.tokenExpireDate, Role = user.Role});
    }

    [HttpPost("address")]
    public async Task<ActionResult<AddressDto>> AddUserAddress(AddressDto address)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        var validationResult = await _addressValidator.ValidateAsync(address);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var inserted = await _userRepository.AddAddress(userId, address);
        return Ok(inserted);
    }

    [HttpPost("cart")]
    public async Task<ActionResult> AddProductToCart(ProductIdItemDto productIdItem)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        var validationResult = await _productIdItemValidator.ValidateAsync(productIdItem);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        await _userRepository.AddProductToCart(userId, productIdItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult> DeleteUser(string id)
    {
        await _userRepository.Delete(id);
        return NoContent();
    }

    [HttpDelete("address/{addressId}")]
    public async Task<ActionResult> DeleteUserAddress(string addressId)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        await _userRepository.DeleteAddress(userId, addressId);
        return NoContent();
    }

    [HttpDelete("cart/{productId}")]
    public async Task<ActionResult> DeleteProductFromCart(string productId)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        await _userRepository.DeleteProductFromCart(userId, productId);
        return NoContent();
    }
}