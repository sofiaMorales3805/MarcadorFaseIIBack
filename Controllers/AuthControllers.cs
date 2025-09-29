using Microsoft.AspNetCore.Mvc;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Services.Interfaces;
using MarcadorFaseIIApi.Models.DTOs;

namespace MarcadorFaseIIApi.Constrollers;

[ApiController]
[Route("api/[controller]")]
public class AuthControllers(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.AuthenticateAsync(request);
        if (response == null)
            return Unauthorized("Invalid username or password");

        return Ok(response);
    }

}
