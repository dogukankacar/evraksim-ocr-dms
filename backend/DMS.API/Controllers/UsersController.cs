using DMS.Application.DTOs;
using DMS.Core.Common;
using DMS.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMS.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public UsersController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }

        return Ok(ApiResponse<List<UserDto>>.SuccessResponse(userDtos));
    }

    [HttpPost("{id}/toggle-admin")]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(ApiResponse<bool>.ErrorResponse("Kullanıcı bulunamadı."));

        if (user.Email == "admin@dms.com") 
            return BadRequest(ApiResponse<bool>.ErrorResponse("Ana admin hesabının yetkisi değiştirilemez."));

        var isCurrentlyAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        
        if (isCurrentlyAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Yetki durumu güncellendi."));
    }
}
