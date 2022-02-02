using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebAPI.Authentication.Domain.Entities;
using WebAPI.Authentication.Infrastructure.Options;
using WebAPI.Authentication.Infrastructure.Providers;
using WebAPI.Authentication.Infrastructure.Setup;
using WebAPI.Authentication.UseCases.Models;
using WebAPI.Authentication.UseCases.Models.Input;
using WebAPI.Authentication.UseCases.Models.Output;
using WebAPI.Authentication.UseCases.Types;

namespace WebAPI.Authentication.UseCases.Requests.Commands;

public class RegisterCommand : IRequest<ServerResponse>
{
  public RegisterDto RegisterDto { get; set; } = null!;
}


public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ServerResponse>
{
  readonly IOptions<JwtOptions> _jwtOptions;
  readonly IHttpContextAccessor _httpContextAccessor;
  readonly IMapper _mapper;
  readonly UserManager<User> _userManager;

  public RegisterCommandHandler
  (
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    UserManager<User> userManager
  )
  {
    _jwtOptions = jwtOptions;
    _httpContextAccessor = httpContextAccessor;
    _mapper = mapper;
    _userManager = userManager;
  }

  /// <summary>
  /// Handles a request.
  /// </summary>
  /// <param name="request">The request.</param>
  /// <param name="token">Cancellation token.</param>
  /// <returns>Server Response.</returns>
  public async Task<ServerResponse> Handle(RegisterCommand request, CancellationToken token)
  {
    var httpContext = _httpContextAccessor.HttpContext;

    try
    {
      var user = _mapper.Map<User>(request.RegisterDto);
      var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);

      if (result.Succeeded)
      {
        var tempUser = await _userManager.FindByEmailAsync(request.RegisterDto.Email);
        await _userManager.AddToRoleAsync(tempUser, RoleNames.AllRoles.ElementAt(0));

        // Get a user roles
        var rolesList = await _userManager.GetRolesAsync(user);

        // Creating an object with a token and user data.
        var profile = new ProfileDto(user.Id, user.UserName, user.CreatedAt, user.Email, rolesList.ElementAt(0));

        var jwtProvider = new JwtProvider(_jwtOptions, _userManager);
        var jwt = await jwtProvider.GenerateToken(user);

        // Adding data to browser cookies
        httpContext!.Response.Cookies.Append(Messages.JwtCookiesKey, jwt, new CookieOptions
        {
          HttpOnly = true,
          Secure = true,
          SameSite = SameSiteMode.None
        });

        return await Task.FromResult(new ServerResponse(200, true, Messages.RegistrationSuccess, profile));
      }

      return await Task.FromResult(new ServerResponse(200, Messages.RegistrationFailed,
          result.Errors.Select(e => e.Description).ToArray()
        )
      );
    }
    catch (Exception ex)
    {
      return await Task.FromResult(new ServerResponse(500, ex.Message, ""));
    }
  }
}
