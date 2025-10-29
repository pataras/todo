# Backend Implementation Guide - Release 1

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** Design Phase

## Overview

This document provides detailed implementation guidance for the .NET 10 backend of Release 1, including project setup, domain-driven design implementation, CQRS pattern, repository pattern, and Azure service integration.

---

## Table of Contents

1. [Project Setup](#project-setup)
2. [Project Structure](#project-structure)
3. [Domain Models](#domain-models)
4. [Repository Pattern](#repository-pattern)
5. [CQRS Implementation](#cqrs-implementation)
6. [Controllers](#controllers)
7. [Services](#services)
8. [Middleware](#middleware)
9. [Azure Integration](#azure-integration)
10. [Security](#security)
11. [Testing](#testing)
12. [Best Practices](#best-practices)

---

## Project Setup

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or Rider or VS Code with C# extensions
- Azure Cosmos DB Emulator (for local development)
- SQL Server / Azure SQL Database
- Azure Storage Emulator or Azurite

### Create Project

```bash
# Create solution
dotnet new sln -n TodoApp

# Create Web API project
dotnet new webapi -n TodoApp.Api -o src/TodoApp.Api
dotnet sln add src/TodoApp.Api

# Create test project
dotnet new xunit -n TodoApp.Tests -o tests/TodoApp.Tests
dotnet sln add tests/TodoApp.Tests

cd src/TodoApp.Api
```

### Install NuGet Packages

```bash
# Azure SDKs
dotnet add package Microsoft.Azure.Cosmos
dotnet add package Azure.Storage.Queues
dotnet add package Azure.Storage.Blobs
dotnet add package Azure.Security.KeyVault.Secrets
dotnet add package Azure.Identity

# MediatR for CQRS
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection

# FluentValidation
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

# Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

# Password hashing
dotnet add package BCrypt.Net-Next

# Logging
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.ApplicationInsights

# Swagger
dotnet add package Swashbuckle.AspNetCore

# Rate limiting
dotnet add package AspNetCoreRateLimit

# Entity Framework (for SQL Server reporting)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

---

## Project Structure

```
backend/
├── src/
│   └── TodoApp.Api/
│       ├── Domain/
│       │   └── UserManagement/
│       │       ├── Controllers/
│       │       │   ├── AuthController.cs
│       │       │   └── UserController.cs
│       │       ├── Commands/
│       │       │   ├── RegisterUser/
│       │       │   │   ├── RegisterUserCommand.cs
│       │       │   │   ├── RegisterUserCommandHandler.cs
│       │       │   │   └── RegisterUserCommandValidator.cs
│       │       │   ├── LoginUser/
│       │       │   ├── VerifyEmail/
│       │       │   ├── ForgotPassword/
│       │       │   └── ResetPassword/
│       │       ├── Queries/
│       │       │   └── GetUserProfile/
│       │       │       ├── GetUserProfileQuery.cs
│       │       │       └── GetUserProfileQueryHandler.cs
│       │       ├── Models/
│       │       │   ├── User.cs
│       │       │   ├── Token.cs
│       │       │   └── SecurityEvent.cs
│       │       ├── DTOs/
│       │       │   ├── RegisterRequestDto.cs
│       │       │   ├── LoginRequestDto.cs
│       │       │   ├── AuthResponseDto.cs
│       │       │   └── UserProfileDto.cs
│       │       ├── Services/
│       │       │   ├── IAuthService.cs
│       │       │   ├── AuthService.cs
│       │       │   ├── IUserService.cs
│       │       │   ├── UserService.cs
│       │       │   ├── ITokenService.cs
│       │       │   ├── TokenService.cs
│       │       │   ├── IPasswordHasher.cs
│       │       │   └── PasswordHasher.cs
│       │       └── Interfaces/
│       │           ├── IUserRepository.cs
│       │           ├── ITokenRepository.cs
│       │           └── ISecurityEventRepository.cs
│       ├── Infrastructure/
│       │   ├── Data/
│       │   │   ├── CosmosDB/
│       │   │   │   ├── CosmosDbContext.cs
│       │   │   │   ├── UserRepository.cs
│       │   │   │   ├── TokenRepository.cs
│       │   │   │   └── SecurityEventRepository.cs
│       │   │   └── SqlServer/
│       │   │       └── ReportingDbContext.cs
│       │   ├── Azure/
│       │   │   ├── Queues/
│       │   │   │   ├── IQueueService.cs
│       │   │   │   └── QueueService.cs
│       │   │   ├── Blob/
│       │   │   │   ├── IBlobService.cs
│       │   │   │   └── BlobService.cs
│       │   │   └── KeyVault/
│       │   │       ├── IKeyVaultService.cs
│       │   │       └── KeyVaultService.cs
│       │   └── Middleware/
│       │       ├── JwtAuthenticationMiddleware.cs
│       │       ├── ExceptionHandlingMiddleware.cs
│       │       └── RateLimitingMiddleware.cs
│       ├── Shared/
│       │   ├── Constants/
│       │   │   └── AppConstants.cs
│       │   ├── Exceptions/
│       │   │   ├── ValidationException.cs
│       │   │   ├── NotFoundException.cs
│       │   │   ├── UnauthorizedException.cs
│       │   │   └── BusinessException.cs
│       │   ├── Extensions/
│       │   │   ├── ServiceCollectionExtensions.cs
│       │   │   └── StringExtensions.cs
│       │   └── Models/
│       │       └── ApiResponse.cs
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── TodoApp.Api.csproj
├── tests/
│   └── TodoApp.Tests/
│       ├── UnitTests/
│       │   ├── CommandHandlers/
│       │   ├── Services/
│       │   └── Validators/
│       └── IntegrationTests/
└── TodoApp.sln
```

---

## Domain Models

### User Model

`Domain/UserManagement/Models/User.cs`:
```csharp
using Newtonsoft.Json;

namespace TodoApp.Api.Domain.UserManagement.Models;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; } = $"usr_{Guid.NewGuid():N}";

    [JsonProperty("type")]
    public string Type { get; set; } = "user";

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("emailLowercase")]
    public string EmailLowercase { get; set; } = string.Empty;

    [JsonProperty("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [JsonProperty("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("role")]
    public string Role { get; set; } = "Member";

    [JsonProperty("isEmailVerified")]
    public bool IsEmailVerified { get; set; } = false;

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; } = false;

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }

    [JsonProperty("deletedAt")]
    public DateTime? DeletedAt { get; set; }

    [JsonProperty("metadata")]
    public UserMetadata Metadata { get; set; } = new();

    [JsonProperty("failedLoginAttempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    [JsonProperty("accountLockedUntil")]
    public DateTime? AccountLockedUntil { get; set; }
}

public class UserMetadata
{
    [JsonProperty("registrationIp")]
    public string RegistrationIp { get; set; } = string.Empty;

    [JsonProperty("registrationUserAgent")]
    public string RegistrationUserAgent { get; set; } = string.Empty;

    [JsonProperty("passwordChangedAt")]
    public DateTime? PasswordChangedAt { get; set; }

    [JsonProperty("passwordHistory")]
    public List<string> PasswordHistory { get; set; } = new();
}
```

### Token Model

`Domain/UserManagement/Models/Token.cs`:
```csharp
using Newtonsoft.Json;

namespace TodoApp.Api.Domain.UserManagement.Models;

public class Token
{
    [JsonProperty("id")]
    public string Id { get; set; } = $"tok_{Guid.NewGuid():N}";

    [JsonProperty("type")]
    public string Type { get; set; } = "token";

    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("tokenType")]
    public TokenType TokenType { get; set; }

    [JsonProperty("token")]
    public string TokenValue { get; set; } = string.Empty;

    [JsonProperty("tokenHash")]
    public string TokenHash { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("sessionId")]
    public string? SessionId { get; set; }

    [JsonProperty("isUsed")]
    public bool IsUsed { get; set; } = false;

    [JsonProperty("usedAt")]
    public DateTime? UsedAt { get; set; }

    [JsonProperty("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("metadata")]
    public TokenMetadata Metadata { get; set; } = new();

    [JsonProperty("_ttl")]
    public int Ttl { get; set; }
}

public class TokenMetadata
{
    [JsonProperty("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonProperty("userAgent")]
    public string UserAgent { get; set; } = string.Empty;

    [JsonProperty("location")]
    public Location? Location { get; set; }
}

public class Location
{
    [JsonProperty("country")]
    public string Country { get; set; } = string.Empty;

    [JsonProperty("region")]
    public string Region { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;
}

public enum TokenType
{
    EmailVerification,
    PasswordReset,
    RefreshToken
}
```

### Security Event Model

`Domain/UserManagement/Models/SecurityEvent.cs`:
```csharp
using Newtonsoft.Json;

namespace TodoApp.Api.Domain.UserManagement.Models;

public class SecurityEvent
{
    [JsonProperty("id")]
    public string Id { get; set; } = $"evt_{Guid.NewGuid():N}";

    [JsonProperty("type")]
    public string Type { get; set; } = "securityEvent";

    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("eventType")]
    public SecurityEventType EventType { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonProperty("userAgent")]
    public string UserAgent { get; set; } = string.Empty;

    [JsonProperty("location")]
    public Location? Location { get; set; }

    [JsonProperty("sessionId")]
    public string? SessionId { get; set; }

    [JsonProperty("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    [JsonProperty("_ttl")]
    public int Ttl { get; set; } = 7776000; // 90 days
}

public enum SecurityEventType
{
    LoginSuccess,
    LoginFailed,
    LogoutSuccess,
    PasswordChanged,
    PasswordResetRequested,
    PasswordResetCompleted,
    EmailVerified,
    EmailVerificationRequested,
    AccountLocked,
    AccountUnlocked
}
```

---

## Repository Pattern

### IUserRepository Interface

`Domain/UserManagement/Interfaces/IUserRepository.cs`:
```csharp
using TodoApp.Api.Domain.UserManagement.Models;

namespace TodoApp.Api.Domain.UserManagement.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string userId);
    Task<bool> ExistsAsync(string email);
}
```

### UserRepository Implementation

`Infrastructure/Data/CosmosDB/UserRepository.cs`:
```csharp
using Microsoft.Azure.Cosmos;
using TodoApp.Api.Domain.UserManagement.Models;
using TodoApp.Api.Domain.UserManagement.Interfaces;

namespace TodoApp.Api.Infrastructure.Data.CosmosDB;

public class UserRepository : IUserRepository
{
    private readonly Container _container;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        CosmosClient cosmosClient,
        IConfiguration configuration,
        ILogger<UserRepository> logger)
    {
        var databaseName = configuration["CosmosDb:DatabaseName"] ?? "TodoAppDb";
        _container = cosmosClient.GetContainer(databaseName, "Users");
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        try
        {
            var response = await _container.ReadItemAsync<User>(
                userId,
                new PartitionKey(userId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.type = @type AND c.emailLowercase = @email AND c.isDeleted = false")
                .WithParameter("@type", "user")
                .WithParameter("@email", email.ToLowerInvariant());

            var iterator = _container.GetItemQueryIterator<User>(query);
            var results = await iterator.ReadNextAsync();

            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            user.EmailLowercase = user.Email.ToLowerInvariant();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            var response = await _container.CreateItemAsync(
                user,
                new PartitionKey(user.Id));

            _logger.LogInformation("User created: {UserId}", user.Id);
            return response.Resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;

            var response = await _container.ReplaceItemAsync(
                user,
                user.Id,
                new PartitionKey(user.Id));

            _logger.LogInformation("User updated: {UserId}", user.Id);
            return response.Resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteAsync(string userId)
    {
        try
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"User not found: {userId}");
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(user);

            _logger.LogInformation("User deleted: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        return user != null;
    }
}
```

---

## CQRS Implementation

### Register User Command

`Domain/UserManagement/Commands/RegisterUser/RegisterUserCommand.cs`:
```csharp
using MediatR;
using TodoApp.Api.Domain.UserManagement.Models;

namespace TodoApp.Api.Domain.UserManagement.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<User>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}
```

### Register User Command Validator

`Domain/UserManagement/Commands/RegisterUser/RegisterUserCommandValidator.cs`:
```csharp
using FluentValidation;

namespace TodoApp.Api.Domain.UserManagement.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[@$!%*?&#]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");
    }
}
```

### Register User Command Handler

`Domain/UserManagement/Commands/RegisterUser/RegisterUserCommandHandler.cs`:
```csharp
using MediatR;
using TodoApp.Api.Domain.UserManagement.Models;
using TodoApp.Api.Domain.UserManagement.Interfaces;
using TodoApp.Api.Domain.UserManagement.Services;
using TodoApp.Api.Shared.Exceptions;

namespace TodoApp.Api.Domain.UserManagement.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IQueueService _queueService;
    private readonly ISecurityEventRepository _securityEventRepository;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IQueueService queueService,
        ISecurityEventRepository securityEventRepository,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _queueService = queueService;
        _securityEventRepository = securityEventRepository;
        _logger = logger;
    }

    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering user: {Email}", request.Email);

        // Check if email already exists
        if (await _userRepository.ExistsAsync(request.Email))
        {
            throw new BusinessException("EMAIL_ALREADY_EXISTS", "Email already registered");
        }

        // Create user
        var user = new User
        {
            Email = request.Email,
            EmailLowercase = request.Email.ToLowerInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            Role = "Member",
            IsEmailVerified = false,
            Metadata = new UserMetadata
            {
                RegistrationIp = request.IpAddress,
                RegistrationUserAgent = request.UserAgent,
                PasswordChangedAt = DateTime.UtcNow,
                PasswordHistory = new List<string>
                {
                    _passwordHasher.HashPassword(request.Password)
                }
            }
        };

        // Save user
        await _userRepository.CreateAsync(user);

        // Generate verification token
        var token = await _tokenService.GenerateEmailVerificationTokenAsync(
            user.Id,
            user.Email);

        // Queue verification email
        await _queueService.QueueEmailAsync(new EmailQueueMessage
        {
            To = user.Email,
            Subject = "Verify Your Email Address",
            BodyHtml = $"<p>Hello {user.FullName},</p><p>Please verify your email by clicking: <a href=\"{GetVerificationUrl(token)}\">Verify Email</a></p>",
            BodyText = $"Hello {user.FullName}, Please verify your email by visiting: {GetVerificationUrl(token)}"
        });

        // Log security event
        await _securityEventRepository.CreateAsync(new SecurityEvent
        {
            UserId = user.Id,
            EventType = SecurityEventType.EmailVerificationRequested,
            Success = true,
            Ip = request.IpAddress,
            UserAgent = request.UserAgent
        });

        _logger.LogInformation("User registered successfully: {UserId}", user.Id);

        return user;
    }

    private string GetVerificationUrl(string token)
    {
        // TODO: Get from configuration
        return $"https://todoapp.com/verify-email?token={token}";
    }
}
```

---

## Controllers

### AuthController

`Domain/UserManagement/Controllers/AuthController.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TodoApp.Api.Domain.UserManagement.Commands.RegisterUser;
using TodoApp.Api.Domain.UserManagement.Commands.LoginUser;
using TodoApp.Api.Domain.UserManagement.DTOs;
using TodoApp.Api.Shared.Models;

namespace TodoApp.Api.Domain.UserManagement.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        _logger.LogInformation("Register request received for: {Email}", request.Email);

        var command = new RegisterUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            FullName = request.FullName,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            UserAgent = Request.Headers["User-Agent"].ToString()
        };

        var user = await _mediator.Send(command);

        var response = ApiResponse<object>.Success(
            new
            {
                userId = user.Id,
                email = user.Email,
                fullName = user.FullName,
                isEmailVerified = user.IsEmailVerified
            },
            "Registration successful. Please check your email to verify your account.");

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation("Login request received for: {Email}", request.Email);

        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            RememberMe = request.RememberMe,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            UserAgent = Request.Headers["User-Agent"].ToString()
        };

        var authResult = await _mediator.Send(command);

        // Set HTTP-only cookies
        SetAuthCookies(
            authResult.AccessToken,
            authResult.RefreshToken,
            request.RememberMe);

        var response = ApiResponse<AuthResponseDto>.Success(
            authResult,
            "Login successful");

        return Ok(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Logout request received");

        // Clear cookies
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        var response = ApiResponse<object>.Success(null, "Logout successful");
        return Ok(response);
    }

    private void SetAuthCookies(string accessToken, string refreshToken, bool rememberMe)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = cookieOptions.HttpOnly,
            Secure = cookieOptions.Secure,
            SameSite = cookieOptions.SameSite,
            Path = cookieOptions.Path,
            MaxAge = TimeSpan.FromMinutes(15)
        });

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = cookieOptions.HttpOnly,
            Secure = cookieOptions.Secure,
            SameSite = cookieOptions.SameSite,
            Path = cookieOptions.Path,
            MaxAge = rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromDays(7)
        });
    }
}
```

---

## Services

### IPasswordHasher Interface

`Domain/UserManagement/Services/IPasswordHasher.cs`:
```csharp
namespace TodoApp.Api.Domain.UserManagement.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
```

### PasswordHasher Implementation

`Domain/UserManagement/Services/PasswordHasher.cs`:
```csharp
namespace TodoApp.Api.Domain.UserManagement.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 10;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### ITokenService Interface

`Domain/UserManagement/Services/ITokenService.cs`:
```csharp
namespace TodoApp.Api.Domain.UserManagement.Services;

public interface ITokenService
{
    Task<string> GenerateEmailVerificationTokenAsync(string userId, string email);
    Task<string> GeneratePasswordResetTokenAsync(string userId, string email);
    Task<(string AccessToken, string RefreshToken)> GenerateAuthTokensAsync(User user, bool rememberMe);
    Task<bool> ValidateTokenAsync(string token, TokenType tokenType);
    Task<Token?> GetTokenAsync(string token, TokenType tokenType);
    Task MarkTokenAsUsedAsync(string tokenId);
}
```

---

## Middleware

### Exception Handling Middleware

`Infrastructure/Middleware/ExceptionHandlingMiddleware.cs`:
```csharp
using System.Net;
using System.Text.Json;
using TodoApp.Api.Shared.Exceptions;
using TodoApp.Api.Shared.Models;

namespace TodoApp.Api.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = exception switch
        {
            ValidationException validationEx => ApiResponse<object>.Error(
                "VALIDATION_ERROR",
                validationEx.Message,
                validationEx.Errors?.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList()),

            BusinessException businessEx => ApiResponse<object>.Error(
                businessEx.Code,
                businessEx.Message),

            NotFoundException notFoundEx => ApiResponse<object>.Error(
                "NOT_FOUND",
                notFoundEx.Message),

            UnauthorizedException unauthorizedEx => ApiResponse<object>.Error(
                "UNAUTHORIZED",
                unauthorizedEx.Message),

            _ => ApiResponse<object>.Error(
                "SERVER_ERROR",
                "An internal server error occurred")
        };

        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            BusinessException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
```

---

## Program.cs Configuration

`Program.cs`:
```csharp
using Microsoft.Azure.Cosmos;
using MediatR;
using FluentValidation;
using Serilog;
using TodoApp.Api.Infrastructure.Data.CosmosDB;
using TodoApp.Api.Domain.UserManagement.Interfaces;
using TodoApp.Api.Domain.UserManagement.Services;
using TodoApp.Api.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register CosmosDB
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["CosmosDb:ConnectionString"];
    return new CosmosClient(connectionString);
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ISecurityEventRepository, SecurityEventRepository>();

// Register Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? Array.Empty<string>())
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## Best Practices

### 1. Domain-Driven Design
- Organize code by domain (UserManagement, TaskManagement, etc.)
- Keep domain logic in domain layer
- Use ubiquitous language from data dictionary

### 2. CQRS Pattern
- Separate read (queries) and write (commands) operations
- Use MediatR for command/query handling
- Validate commands with FluentValidation

### 3. Repository Pattern
- Abstract data access behind interfaces
- Implement repositories in Infrastructure layer
- Use dependency injection

### 4. Error Handling
- Use custom exceptions for business errors
- Implement global exception handling middleware
- Return consistent error responses

### 5. Security
- Hash passwords with BCrypt
- Use JWT for authentication
- Store tokens in HTTP-only cookies
- Implement rate limiting
- Log security events

### 6. Logging
- Use structured logging with Serilog
- Log at appropriate levels
- Include correlation IDs
- Log security events

### 7. Testing
- Write unit tests for business logic
- Write integration tests for repositories
- Mock external dependencies
- Use test fixtures

---

## Next Steps

1. Set up project structure
2. Implement domain models
3. Create repositories
4. Implement CQRS commands/queries
5. Create controllers
6. Add middleware
7. Configure Program.cs
8. Write unit tests
9. Write integration tests
10. Deploy to Azure

---

## References

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Azure Cosmos DB .NET SDK](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/sql-api-sdk-dotnet-standard)
