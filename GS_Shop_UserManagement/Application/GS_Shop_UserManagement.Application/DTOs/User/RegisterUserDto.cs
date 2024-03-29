﻿using GS_Shop_UserManagement.Application.DTOs.Common;

namespace GS_Shop_UserManagement.Application.DTOs.User;

public class RegisterUserDto : BaseDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}