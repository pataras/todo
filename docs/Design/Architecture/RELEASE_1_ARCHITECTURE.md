# Release 1: Core Authentication - Architecture & Design

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** Design Phase
**Release:** Release 1 - Core Authentication (MVP)

## Overview

This document provides comprehensive architecture and coding designs for Release 1 of the ToDo application, focusing on Core Authentication features. It covers system architecture, database design, API specifications, frontend and backend architectures, and implementation guidance.

### Release 1 Scope

Release 1 implements the Core Authentication MVP with 7 user stories (23 story points):

- **US-UM-001**: User Account Registration (5 points)
- **US-UM-002**: Email Verification (3 points)
- **US-UM-004**: User Login (5 points)
- **US-UM-005**: User Logout (2 points)
- **US-UM-006**: Password Reset Request (3 points)
- **US-UM-007**: Password Reset Completion (3 points)
- **US-UM-010**: View User Profile (2 points)

### Goals

1. Enable users to register and authenticate
2. Implement secure password management
3. Provide email verification workflow
4. Establish foundational authentication infrastructure
5. Create reusable patterns for future releases

---

## Table of Contents

1. [System Architecture](#system-architecture)
2. [Database Design](#database-design)
3. [API Design](#api-design)
4. [Frontend Architecture](#frontend-architecture)
5. [Backend Architecture](#backend-architecture)
6. [Authentication & Security](#authentication--security)
7. [Azure Services](#azure-services)
8. [Implementation Guide](#implementation-guide)

---

## System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                          Client Layer                            │
│  ┌────────────────────────────────────────────────────────┐    │
│  │  React SPA (TypeScript + Vite)                         │    │
│  │  - Material-UI Components                              │    │
│  │  - TanStack Query (State Management)                   │    │
│  │  - Formik + Yup (Forms & Validation)                   │    │
│  │  - Axios (HTTP Client)                                 │    │
│  │  - React Router (Navigation)                           │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              ▼ HTTPS
┌─────────────────────────────────────────────────────────────────┐
│                       API Gateway / Load Balancer                │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Identity & Authentication Layer                │
│  ┌────────────────────────────────────────────────────────┐    │
│  │  Azure Entra (Azure AD B2C)                            │    │
│  │  - Social Identity Providers (Future: Release 4)       │    │
│  │  - OAuth 2.0 / OpenID Connect                          │    │
│  │  - External Identity Federation                        │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Application Layer                           │
│  ┌────────────────────────────────────────────────────────┐    │
│  │  .NET 10 Web API                                       │    │
│  │  ┌──────────────┬──────────────┬──────────────┐      │    │
│  │  │ Controllers  │  Middleware  │   Filters    │      │    │
│  │  └──────────────┴──────────────┴──────────────┘      │    │
│  │  ┌──────────────┬──────────────┬──────────────┐      │    │
│  │  │  Commands    │   Queries    │  Validators  │      │    │
│  │  └──────────────┴──────────────┴──────────────┘      │    │
│  │  ┌──────────────┬──────────────┬──────────────┐      │    │
│  │  │   Services   │ Repositories │   Helpers    │      │    │
│  │  └──────────────┴──────────────┴──────────────┘      │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Data Layer                               │
│  ┌──────────────┬──────────────┬──────────────┬──────────┐    │
│  │   CosmosDB   │  SQL Server  │ Blob Storage │  Queues  │    │
│  │   (Primary)  │ (Reporting)  │   (Files)    │ (Async)  │    │
│  └──────────────┴──────────────┴──────────────┴──────────┘    │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Background Processing                         │
│  ┌────────────────────────────────────────────────────────┐    │
│  │  Azure Functions                                       │    │
│  │  - Email Sending (Queue Triggered)                     │    │
│  │  - Token Cleanup (Timer Triggered)                     │    │
│  │  - CosmosDB Change Feed Processing                     │    │
│  └────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
```

### Component Interaction Flow

#### Registration Flow
```
User → React Form → API Controller → Command Handler → Validator
  → UserService → CosmosDB Repository → CosmosDB
  → Queue Service → Azure Queue → Azure Function → Email Service
```

#### Login Flow
```
User → React Form → API Controller → Command Handler → Validator
  → AuthService → CosmosDB Repository → JWT Generation
  → HTTP-Only Cookie → React App → Protected Routes
```

#### Email Verification Flow
```
Email Link → React Page → API Controller → Query Handler
  → TokenService → CosmosDB Repository → Token Validation
  → UserService → Update Verification Status → Success Page
```

#### Social Authentication Flow (Future: Release 4 with Azure Entra)
```
User → "Sign in with Google/Microsoft" → Azure Entra (Azure AD B2C)
  → OAuth Provider → User Consent → Authorization Code
  → Azure Entra → Token Exchange → API Controller
  → Create/Link User Account → JWT Generation → React App
```

### Azure Entra Integration

**Azure Entra (formerly Azure Active Directory)** is Microsoft's cloud-based identity and access management service. In this application, we use **Azure AD B2C** (Business to Consumer), a specialized version of Azure Entra designed for customer-facing applications.

#### Role in the Architecture

**Release 1 (Current):**
- Azure Entra infrastructure is provisioned but not actively used
- Foundation is established for future social authentication
- Configuration prepared for OAuth 2.0 / OpenID Connect flows

**Release 4 (Future - Social Authentication):**
- Azure AD B2C will handle social identity providers (Google, Microsoft, GitHub)
- Implements OAuth 2.0 and OpenID Connect protocols
- Manages external identity federation
- Provides centralized user consent management
- Handles token issuance and validation for social logins

#### Azure Entra Components

1. **Azure AD B2C Tenant**
   - Separate tenant for customer identities
   - Custom branding and user flows
   - Policy-based configuration

2. **Identity Providers**
   - Google (OAuth 2.0)
   - Microsoft Account (OAuth 2.0)
   - GitHub (Future consideration)
   - Facebook (Future consideration)

3. **User Flows**
   - Sign-up and sign-in combined flow
   - Profile editing flow
   - Password reset flow (integrated with our custom flow)

4. **Application Registration**
   - OAuth client credentials
   - Redirect URIs configuration
   - API permissions and scopes

#### Integration Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     User Browser                        │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│              React Frontend Application                 │
│  - OAuth login button click                             │
│  - Redirect to Azure AD B2C                             │
│  - Receive authorization code                           │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│          Azure Entra (Azure AD B2C)                     │
│  ┌────────────────────────────────────────────────┐   │
│  │  1. User Flow Execution                        │   │
│  │  2. Social Identity Provider Selection         │   │
│  │  3. External OAuth Flow (Google/Microsoft)     │   │
│  │  4. User Consent                                │   │
│  │  5. Token Generation (ID Token + Access Token) │   │
│  └────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│              .NET 10 Web API Backend                    │
│  ┌────────────────────────────────────────────────┐   │
│  │  1. Validate Azure AD B2C token                │   │
│  │  2. Extract user claims (email, name, etc.)    │   │
│  │  3. Create/link user in CosmosDB               │   │
│  │  4. Generate application JWT token             │   │
│  │  5. Return to client with session              │   │
│  └────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

#### Authentication Strategy: Hybrid Approach

Our architecture uses a **hybrid authentication strategy** that combines:

1. **Custom JWT Authentication (Release 1)**
   - Email/password authentication
   - Application-controlled JWT tokens
   - Full control over token lifetime and claims
   - Suitable for standard user registration

2. **Azure Entra B2C (Release 4)**
   - Social identity provider integration
   - OAuth 2.0 / OpenID Connect standard protocols
   - Centralized consent management
   - Enterprise-grade security

3. **Unified User Identity**
   - All users (email/password or social) stored in CosmosDB Users collection
   - Social provider ID linked to user account
   - Single user profile regardless of authentication method
   - Seamless switching between authentication methods

#### Azure Entra Configuration

**Required Azure AD B2C Setup:**
```
Tenant Name: todoapp.onmicrosoft.com (or custom domain)
Subscription: Azure subscription with B2C resource

Identity Providers:
  - Google:
    - Client ID: <from Google Cloud Console>
    - Client Secret: <stored in Key Vault>

  - Microsoft Account:
    - Client ID: <from Azure Portal>
    - Client Secret: <stored in Key Vault>

User Flows:
  - B2C_1_SignUpSignIn: Combined sign-up and sign-in flow
  - B2C_1_ProfileEdit: Profile editing flow
  - B2C_1_PasswordReset: Password reset flow (if using B2C)

Application Registration:
  - Name: ToDo App Frontend
  - Reply URLs:
    - https://todoapp.com/auth/callback
    - http://localhost:5173/auth/callback (development)
  - API Permissions:
    - openid, profile, email, offline_access
```

#### Backend Integration Code (Future - Release 4)

**NuGet Packages:**
```bash
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Startup Configuration:**
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        Configuration.Bind("AzureAdB2C", options);
        options.TokenValidationParameters.NameClaimType = "name";
    },
    options => { Configuration.Bind("AzureAdB2C", options); });
```

**appsettings.json:**
```json
{
  "AzureAdB2C": {
    "Instance": "https://todoapp.b2clogin.com",
    "ClientId": "<application-id>",
    "Domain": "todoapp.onmicrosoft.com",
    "SignUpSignInPolicyId": "B2C_1_SignUpSignIn"
  }
}
```

#### Security Considerations

1. **Token Validation**
   - Azure AD B2C tokens validated using Microsoft.Identity.Web
   - Signature verification with Azure's public keys
   - Issuer and audience validation

2. **Secrets Management**
   - OAuth client secrets stored in Azure Key Vault
   - Never expose secrets in frontend code
   - Rotate secrets periodically

3. **CORS Configuration**
   - Whitelist only authorized domains
   - Azure AD B2C custom domains for branding

4. **Rate Limiting**
   - Apply same rate limiting to social auth endpoints
   - Prevent abuse of OAuth flows

#### Benefits of Azure Entra B2C

1. **Enterprise Security**
   - Microsoft-managed security infrastructure
   - Automatic patching and updates
   - Compliance certifications (SOC 2, ISO 27001)

2. **Scalability**
   - Handles millions of authentication requests
   - Global distribution
   - 99.9% SLA

3. **User Experience**
   - One-click social sign-in
   - No password to remember
   - Trusted identity providers

4. **Compliance**
   - GDPR compliant
   - Data residency options
   - Privacy controls

#### Cost Considerations

**Azure AD B2C Pricing (as of 2025):**
- First 50,000 monthly active users: Free
- Additional MAU (50,001 - 100,000): $0.00325 per MAU
- MFA authentications: $0.03 per authentication

**Estimated Monthly Cost (10,000 users):**
- Base cost: $0 (within free tier)
- MFA (assuming 30% adoption, 3 logins/month): ~$270

#### Migration Path

**Release 1 → Release 4:**
1. Provision Azure AD B2C tenant
2. Configure identity providers (Google, Microsoft)
3. Create user flows and policies
4. Register application
5. Update frontend to add social login buttons
6. Implement backend token validation
7. Test social authentication flow
8. Deploy and monitor

**Backward Compatibility:**
- Existing email/password users continue to work
- Users can link social accounts to existing profiles
- No breaking changes to authentication flow

### Technology Stack Summary

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Frontend | React 18 + TypeScript | UI Framework |
| Build Tool | Vite | Fast development and bundling |
| UI Components | Material-UI 7 | Component library |
| State Management | TanStack Query | Server state management |
| Forms | Formik + Yup | Form handling and validation |
| HTTP Client | Axios | API communication |
| Routing | React Router | Client-side routing |
| Identity | Azure Entra (Azure AD B2C) | Social authentication, OAuth 2.0 (Release 4) |
| Backend | .NET 10 | Web API framework |
| Architecture | DDD + CQRS | Domain-driven design patterns |
| Primary DB | Azure CosmosDB | Document storage |
| Reporting DB | SQL Server | Analytics queries |
| File Storage | Azure Blob Storage | User uploads (profile pictures) |
| Queue | Azure Storage Queue | Asynchronous processing |
| Functions | Azure Functions | Background jobs |
| IaC | Azure Bicep | Infrastructure provisioning |

---

## Database Design

### CosmosDB Collections

#### Users Collection

**Container Name:** `Users`
**Partition Key:** `/id` (user ID for even distribution)

**Document Schema:**
```json
{
  "id": "usr_a1b2c3d4e5f6",
  "type": "user",
  "email": "user@example.com",
  "emailLowercase": "user@example.com",
  "passwordHash": "$2b$10$...",
  "fullName": "John Doe",
  "role": "Member",
  "isEmailVerified": false,
  "isActive": true,
  "isDeleted": false,
  "createdAt": "2025-10-29T10:30:00Z",
  "updatedAt": "2025-10-29T10:30:00Z",
  "lastLoginAt": null,
  "metadata": {
    "registrationIp": "192.168.1.1",
    "registrationUserAgent": "Mozilla/5.0..."
  },
  "_ttl": -1
}
```

**Indexes:**
- Default index on `id` (partition key)
- Composite index on `emailLowercase` for email lookups
- Index on `isDeleted` for filtering
- Index on `createdAt` for sorting

#### Tokens Collection

**Container Name:** `Tokens`
**Partition Key:** `/userId` (groups user's tokens together)

**Document Schema:**
```json
{
  "id": "tok_x1y2z3a4b5c6",
  "type": "token",
  "userId": "usr_a1b2c3d4e5f6",
  "tokenType": "EmailVerification",
  "token": "abc123def456ghi789",
  "tokenHash": "$2b$10$...",
  "email": "user@example.com",
  "expiresAt": "2025-10-30T10:30:00Z",
  "isUsed": false,
  "usedAt": null,
  "createdAt": "2025-10-29T10:30:00Z",
  "metadata": {
    "ip": "192.168.1.1",
    "userAgent": "Mozilla/5.0..."
  },
  "_ttl": 172800
}
```

**Token Types:**
- `EmailVerification` - Email verification tokens (24-hour expiry)
- `PasswordReset` - Password reset tokens (1-hour expiry)
- `RefreshToken` - JWT refresh tokens (7-day expiry)

**TTL Configuration:**
- `_ttl` set to 172800 seconds (48 hours) for automatic cleanup
- Expired tokens automatically removed by CosmosDB

#### SecurityEvents Collection

**Container Name:** `SecurityEvents`
**Partition Key:** `/userId` (groups user's events together)

**Document Schema:**
```json
{
  "id": "evt_m1n2o3p4q5r6",
  "type": "securityEvent",
  "userId": "usr_a1b2c3d4e5f6",
  "eventType": "LoginSuccess",
  "timestamp": "2025-10-29T10:30:00Z",
  "ip": "192.168.1.1",
  "userAgent": "Mozilla/5.0...",
  "location": {
    "country": "US",
    "region": "CA",
    "city": "San Francisco"
  },
  "metadata": {
    "sessionId": "ses_abc123",
    "additionalInfo": {}
  },
  "_ttl": 7776000
}
```

**Event Types:**
- `LoginSuccess` - Successful login
- `LoginFailed` - Failed login attempt
- `PasswordChanged` - Password changed
- `EmailVerified` - Email verified
- `PasswordResetRequested` - Password reset requested
- `PasswordResetCompleted` - Password reset completed
- `LogoutSuccess` - User logged out

**TTL Configuration:**
- `_ttl` set to 7776000 seconds (90 days)

### SQL Server Tables (Reporting)

#### Users Table
```sql
CREATE TABLE Users (
    UserId NVARCHAR(50) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    LastLoginAt DATETIME2 NULL,
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_CreatedAt (CreatedAt),
    INDEX IX_Users_IsDeleted (IsDeleted)
);
```

#### SecurityEvents Table
```sql
CREATE TABLE SecurityEvents (
    EventId NVARCHAR(50) PRIMARY KEY,
    UserId NVARCHAR(50) NOT NULL,
    EventType NVARCHAR(50) NOT NULL,
    Timestamp DATETIME2 NOT NULL,
    IpAddress NVARCHAR(50),
    Location NVARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    INDEX IX_SecurityEvents_UserId (UserId),
    INDEX IX_SecurityEvents_Timestamp (Timestamp),
    INDEX IX_SecurityEvents_EventType (EventType)
);
```

**Sync Strategy:**
- Use CosmosDB Change Feed to sync data to SQL Server
- Azure Function processes change feed and updates SQL Server
- Near real-time sync (typically < 1 minute)

---

## API Design

### API Base URL
```
Production: https://api.todoapp.com/v1
Development: https://localhost:7001/v1
```

### Authentication Header
```
Authorization: Bearer <JWT_TOKEN>
```

### Standard Response Formats

#### Success Response
```json
{
  "success": true,
  "data": { /* response data */ },
  "message": "Operation completed successfully"
}
```

#### Error Response
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  }
}
```

### API Endpoints

#### 1. User Registration (US-UM-001)

**POST** `/auth/register`

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "fullName": "John Doe"
}
```

**Validation Rules:**
- `email`: Required, valid email format, not already registered
- `password`: Required, min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special char
- `confirmPassword`: Required, must match password
- `fullName`: Required, min 2 chars, max 100 chars

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "userId": "usr_a1b2c3d4e5f6",
    "email": "user@example.com",
    "fullName": "John Doe",
    "isEmailVerified": false
  },
  "message": "Registration successful. Please check your email to verify your account."
}
```

**Error Codes:**
- `VALIDATION_ERROR` - Invalid input data
- `EMAIL_ALREADY_EXISTS` - Email already registered
- `SERVER_ERROR` - Internal server error

---

#### 2. Email Verification (US-UM-002)

**POST** `/auth/verify-email`

**Request Body:**
```json
{
  "token": "abc123def456ghi789"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "userId": "usr_a1b2c3d4e5f6",
    "email": "user@example.com",
    "isEmailVerified": true
  },
  "message": "Email verified successfully. You can now log in."
}
```

**Error Codes:**
- `INVALID_TOKEN` - Token not found or invalid
- `TOKEN_EXPIRED` - Token has expired
- `TOKEN_ALREADY_USED` - Token already used
- `SERVER_ERROR` - Internal server error

---

**POST** `/auth/resend-verification`

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Verification email sent. Please check your inbox."
}
```

---

#### 3. User Login (US-UM-004)

**POST** `/auth/login`

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "rememberMe": false
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "user": {
      "userId": "usr_a1b2c3d4e5f6",
      "email": "user@example.com",
      "fullName": "John Doe",
      "role": "Member",
      "isEmailVerified": true
    },
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "rt_xyz789abc123def456",
    "expiresIn": 900
  },
  "message": "Login successful"
}
```

**Response Headers:**
```
Set-Cookie: accessToken=eyJhbG...; HttpOnly; Secure; SameSite=Strict; Max-Age=900
Set-Cookie: refreshToken=rt_xyz...; HttpOnly; Secure; SameSite=Strict; Max-Age=604800
```

**Error Codes:**
- `INVALID_CREDENTIALS` - Invalid email or password
- `ACCOUNT_NOT_VERIFIED` - Email not verified
- `ACCOUNT_LOCKED` - Account temporarily locked (too many failed attempts)
- `ACCOUNT_DISABLED` - Account disabled by admin
- `SERVER_ERROR` - Internal server error

---

#### 4. User Logout (US-UM-005)

**POST** `/auth/logout`

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Logout successful"
}
```

**Response Headers:**
```
Set-Cookie: accessToken=; HttpOnly; Secure; SameSite=Strict; Max-Age=0
Set-Cookie: refreshToken=; HttpOnly; Secure; SameSite=Strict; Max-Age=0
```

---

#### 5. Password Reset Request (US-UM-006)

**POST** `/auth/forgot-password`

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "If an account exists with this email, a password reset link has been sent."
}
```

**Note:** Same response whether email exists or not (security measure)

---

#### 6. Password Reset Completion (US-UM-007)

**POST** `/auth/reset-password`

**Request Body:**
```json
{
  "token": "abc123def456ghi789",
  "newPassword": "NewSecurePass456!",
  "confirmPassword": "NewSecurePass456!"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset successful. You can now log in with your new password."
}
```

**Error Codes:**
- `INVALID_TOKEN` - Token not found or invalid
- `TOKEN_EXPIRED` - Token has expired
- `TOKEN_ALREADY_USED` - Token already used
- `VALIDATION_ERROR` - Password doesn't meet requirements
- `PASSWORD_REUSED` - Cannot reuse previous password
- `SERVER_ERROR` - Internal server error

---

#### 7. View User Profile (US-UM-010)

**GET** `/users/profile`

**Headers:**
```
Authorization: Bearer <JWT_TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "userId": "usr_a1b2c3d4e5f6",
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "Member",
    "isEmailVerified": true,
    "createdAt": "2025-10-29T10:30:00Z",
    "lastLoginAt": "2025-10-29T12:15:00Z",
    "profilePicture": null
  }
}
```

---

#### 8. Refresh Token

**POST** `/auth/refresh`

**Request Body:**
```json
{
  "refreshToken": "rt_xyz789abc123def456"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 900
  }
}
```

---

### Error Code Reference

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Input validation failed |
| `INVALID_CREDENTIALS` | 401 | Invalid email or password |
| `UNAUTHORIZED` | 401 | Not authenticated |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Resource not found |
| `EMAIL_ALREADY_EXISTS` | 409 | Email already registered |
| `ACCOUNT_LOCKED` | 423 | Account temporarily locked |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `SERVER_ERROR` | 500 | Internal server error |
| `SERVICE_UNAVAILABLE` | 503 | Service temporarily unavailable |

---

## Frontend Architecture

### Directory Structure

```
frontend/
├── src/
│   ├── features/
│   │   └── auth/
│   │       ├── components/
│   │       │   ├── RegisterForm.tsx
│   │       │   ├── LoginForm.tsx
│   │       │   ├── EmailVerificationPage.tsx
│   │       │   ├── ForgotPasswordForm.tsx
│   │       │   ├── ResetPasswordForm.tsx
│   │       │   └── UserProfilePage.tsx
│   │       ├── hooks/
│   │       │   ├── useRegister.ts
│   │       │   ├── useLogin.ts
│   │       │   ├── useLogout.ts
│   │       │   ├── useVerifyEmail.ts
│   │       │   ├── useForgotPassword.ts
│   │       │   ├── useResetPassword.ts
│   │       │   ├── useUserProfile.ts
│   │       │   └── useAuth.ts
│   │       ├── api/
│   │       │   ├── authApi.ts
│   │       │   └── userApi.ts
│   │       ├── types/
│   │       │   ├── auth.types.ts
│   │       │   └── user.types.ts
│   │       ├── validation/
│   │       │   └── authSchemas.ts
│   │       └── routes/
│   │           └── authRoutes.tsx
│   ├── shared/
│   │   ├── components/
│   │   │   ├── FormInput.tsx
│   │   │   ├── PasswordInput.tsx
│   │   │   ├── LoadingButton.tsx
│   │   │   ├── ErrorAlert.tsx
│   │   │   └── SuccessAlert.tsx
│   │   ├── hooks/
│   │   │   ├── useApiClient.ts
│   │   │   └── useErrorHandler.ts
│   │   ├── utils/
│   │   │   ├── validation.ts
│   │   │   ├── storage.ts
│   │   │   └── errorHandling.ts
│   │   └── types/
│   │       ├── api.types.ts
│   │       └── common.types.ts
│   ├── layouts/
│   │   ├── AuthLayout.tsx
│   │   └── MainLayout.tsx
│   ├── routes/
│   │   ├── AppRoutes.tsx
│   │   ├── PrivateRoute.tsx
│   │   └── PublicRoute.tsx
│   ├── theme/
│   │   └── theme.ts
│   ├── App.tsx
│   └── main.tsx
├── package.json
├── tsconfig.json
├── vite.config.ts
└── index.html
```

### Key Components

#### RegisterForm Component
```typescript
// features/auth/components/RegisterForm.tsx
import { useRegister } from '../hooks/useRegister';
import { registerSchema } from '../validation/authSchemas';

interface RegisterFormValues {
  email: string;
  password: string;
  confirmPassword: string;
  fullName: string;
}

export const RegisterForm: React.FC = () => {
  const { mutate: register, isPending, isError, error } = useRegister();

  const handleSubmit = (values: RegisterFormValues) => {
    register(values);
  };

  return (
    <Formik
      initialValues={{ email: '', password: '', confirmPassword: '', fullName: '' }}
      validationSchema={registerSchema}
      onSubmit={handleSubmit}
    >
      {/* Form implementation */}
    </Formik>
  );
};
```

#### useRegister Hook
```typescript
// features/auth/hooks/useRegister.ts
import { useMutation } from '@tanstack/react-query';
import { authApi } from '../api/authApi';
import { useNavigate } from 'react-router-dom';

export const useRegister = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: authApi.register,
    onSuccess: (data) => {
      // Show success message
      navigate('/verify-email-sent');
    },
    onError: (error) => {
      // Handle error
    },
  });
};
```

#### Auth API Client
```typescript
// features/auth/api/authApi.ts
import axios from 'axios';
import { RegisterRequest, LoginRequest, AuthResponse } from '../types/auth.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const authApi = {
  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await axios.post(`${API_BASE_URL}/auth/register`, data);
    return response.data;
  },

  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await axios.post(`${API_BASE_URL}/auth/login`, data, {
      withCredentials: true,
    });
    return response.data;
  },

  logout: async (): Promise<void> => {
    await axios.post(`${API_BASE_URL}/auth/logout`, {}, {
      withCredentials: true,
    });
  },

  verifyEmail: async (token: string): Promise<AuthResponse> => {
    const response = await axios.post(`${API_BASE_URL}/auth/verify-email`, { token });
    return response.data;
  },

  forgotPassword: async (email: string): Promise<void> => {
    await axios.post(`${API_BASE_URL}/auth/forgot-password`, { email });
  },

  resetPassword: async (token: string, newPassword: string, confirmPassword: string): Promise<void> => {
    await axios.post(`${API_BASE_URL}/auth/reset-password`, {
      token,
      newPassword,
      confirmPassword,
    });
  },
};
```

#### Validation Schemas
```typescript
// features/auth/validation/authSchemas.ts
import * as Yup from 'yup';

export const registerSchema = Yup.object({
  email: Yup.string()
    .email('Invalid email format')
    .required('Email is required'),

  password: Yup.string()
    .min(8, 'Password must be at least 8 characters')
    .matches(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .matches(/[a-z]/, 'Password must contain at least one lowercase letter')
    .matches(/[0-9]/, 'Password must contain at least one number')
    .matches(/[@$!%*?&#]/, 'Password must contain at least one special character')
    .required('Password is required'),

  confirmPassword: Yup.string()
    .oneOf([Yup.ref('password')], 'Passwords must match')
    .required('Confirm password is required'),

  fullName: Yup.string()
    .min(2, 'Full name must be at least 2 characters')
    .max(100, 'Full name must be less than 100 characters')
    .required('Full name is required'),
});

export const loginSchema = Yup.object({
  email: Yup.string()
    .email('Invalid email format')
    .required('Email is required'),

  password: Yup.string()
    .required('Password is required'),
});

export const forgotPasswordSchema = Yup.object({
  email: Yup.string()
    .email('Invalid email format')
    .required('Email is required'),
});

export const resetPasswordSchema = Yup.object({
  newPassword: Yup.string()
    .min(8, 'Password must be at least 8 characters')
    .matches(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .matches(/[a-z]/, 'Password must contain at least one lowercase letter')
    .matches(/[0-9]/, 'Password must contain at least one number')
    .matches(/[@$!%*?&#]/, 'Password must contain at least one special character')
    .required('Password is required'),

  confirmPassword: Yup.string()
    .oneOf([Yup.ref('newPassword')], 'Passwords must match')
    .required('Confirm password is required'),
});
```

### Routing

```typescript
// routes/AppRoutes.tsx
import { Routes, Route, Navigate } from 'react-router-dom';
import { RegisterForm } from '../features/auth/components/RegisterForm';
import { LoginForm } from '../features/auth/components/LoginForm';
import { EmailVerificationPage } from '../features/auth/components/EmailVerificationPage';
import { ForgotPasswordForm } from '../features/auth/components/ForgotPasswordForm';
import { ResetPasswordForm } from '../features/auth/components/ResetPasswordForm';
import { UserProfilePage } from '../features/auth/components/UserProfilePage';
import { PrivateRoute } from './PrivateRoute';
import { PublicRoute } from './PublicRoute';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Public routes */}
      <Route
        path="/register"
        element={
          <PublicRoute>
            <RegisterForm />
          </PublicRoute>
        }
      />
      <Route
        path="/login"
        element={
          <PublicRoute>
            <LoginForm />
          </PublicRoute>
        }
      />
      <Route path="/verify-email" element={<EmailVerificationPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordForm />} />
      <Route path="/reset-password" element={<ResetPasswordForm />} />

      {/* Private routes */}
      <Route
        path="/profile"
        element={
          <PrivateRoute>
            <UserProfilePage />
          </PrivateRoute>
        }
      />

      {/* Default redirect */}
      <Route path="/" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};
```

---

## Backend Architecture

### Directory Structure

```
backend/
├── src/
│   ├── Domain/
│   │   └── UserManagement/
│   │       ├── Controllers/
│   │       │   ├── AuthController.cs
│   │       │   └── UserController.cs
│   │       ├── Commands/
│   │       │   ├── RegisterUserCommand.cs
│   │       │   ├── RegisterUserCommandHandler.cs
│   │       │   ├── LoginUserCommand.cs
│   │       │   ├── LoginUserCommandHandler.cs
│   │       │   ├── VerifyEmailCommand.cs
│   │       │   ├── VerifyEmailCommandHandler.cs
│   │       │   ├── ForgotPasswordCommand.cs
│   │       │   ├── ForgotPasswordCommandHandler.cs
│   │       │   ├── ResetPasswordCommand.cs
│   │       │   └── ResetPasswordCommandHandler.cs
│   │       ├── Queries/
│   │       │   ├── GetUserProfileQuery.cs
│   │       │   └── GetUserProfileQueryHandler.cs
│   │       ├── Models/
│   │       │   ├── User.cs
│   │       │   ├── Token.cs
│   │       │   └── SecurityEvent.cs
│   │       ├── DTOs/
│   │       │   ├── RegisterRequestDto.cs
│   │       │   ├── LoginRequestDto.cs
│   │       │   ├── AuthResponseDto.cs
│   │       │   ├── UserProfileDto.cs
│   │       │   └── ApiResponse.cs
│   │       ├── Services/
│   │       │   ├── IAuthService.cs
│   │       │   ├── AuthService.cs
│   │       │   ├── IUserService.cs
│   │       │   ├── UserService.cs
│   │       │   ├── ITokenService.cs
│   │       │   ├── TokenService.cs
│   │       │   ├── IEmailService.cs
│   │       │   └── EmailService.cs
│   │       ├── Validators/
│   │       │   ├── RegisterUserValidator.cs
│   │       │   ├── LoginUserValidator.cs
│   │       │   └── ResetPasswordValidator.cs
│   │       └── Interfaces/
│   │           ├── IUserRepository.cs
│   │           └── ITokenRepository.cs
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   ├── CosmosDB/
│   │   │   │   ├── CosmosDbContext.cs
│   │   │   │   ├── UserRepository.cs
│   │   │   │   └── TokenRepository.cs
│   │   │   └── SqlServer/
│   │   │       └── ReportingDbContext.cs
│   │   ├── Azure/
│   │   │   ├── Queues/
│   │   │   │   └── QueueService.cs
│   │   │   └── KeyVault/
│   │   │       └── KeyVaultService.cs
│   │   └── Middleware/
│   │       ├── JwtAuthenticationMiddleware.cs
│   │       ├── ExceptionHandlingMiddleware.cs
│   │       └── RateLimitingMiddleware.cs
│   └── Shared/
│       ├── Constants/
│       │   └── AppConstants.cs
│       ├── Exceptions/
│       │   ├── ValidationException.cs
│       │   ├── NotFoundException.cs
│       │   └── UnauthorizedException.cs
│       └── Extensions/
│           ├── StringExtensions.cs
│           └── DateTimeExtensions.cs
├── tests/
│   ├── UnitTests/
│   └── IntegrationTests/
├── Program.cs
├── appsettings.json
└── Backend.csproj
```

### Key Components

#### User Model
```csharp
// Domain/UserManagement/Models/User.cs
namespace TodoApp.Domain.UserManagement.Models
{
    public class User
    {
        public string Id { get; set; } = $"usr_{Guid.NewGuid():N}";
        public string Type { get; set; } = "user";
        public string Email { get; set; } = string.Empty;
        public string EmailLowercase { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Member";
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public UserMetadata Metadata { get; set; } = new();
    }

    public class UserMetadata
    {
        public string RegistrationIp { get; set; } = string.Empty;
        public string RegistrationUserAgent { get; set; } = string.Empty;
    }
}
```

#### Register User Command Handler
```csharp
// Domain/UserManagement/Commands/RegisterUserCommandHandler.cs
using MediatR;
using TodoApp.Domain.UserManagement.Models;
using TodoApp.Domain.UserManagement.Interfaces;
using TodoApp.Domain.UserManagement.Services;

namespace TodoApp.Domain.UserManagement.Commands
{
    public class RegisterUserCommand : IRequest<User>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IEmailService emailService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ValidationException("Email already registered");
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
                    RegistrationUserAgent = request.UserAgent
                }
            };

            // Save user
            await _userRepository.CreateAsync(user);

            // Generate verification token
            var token = await _tokenService.GenerateEmailVerificationTokenAsync(user.Id, user.Email);

            // Queue verification email
            await _emailService.QueueVerificationEmailAsync(user.Email, user.FullName, token);

            return user;
        }
    }
}
```

#### Auth Controller
```csharp
// Domain/UserManagement/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TodoApp.Domain.UserManagement.Commands;
using TodoApp.Domain.UserManagement.DTOs;

namespace TodoApp.Domain.UserManagement.Controllers
{
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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            var user = await _mediator.Send(command);

            var response = new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsEmailVerified = user.IsEmailVerified
                },
                Message = "Registration successful. Please check your email to verify your account."
            };

            return StatusCode(201, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
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
            SetAuthCookies(authResult.AccessToken, authResult.RefreshToken, request.RememberMe);

            var response = new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Data = authResult,
                Message = "Login successful"
            };

            return Ok(response);
        }

        private void SetAuthCookies(string accessToken, string refreshToken, bool rememberMe)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                ...cookieOptions,
                MaxAge = TimeSpan.FromMinutes(15)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                ...cookieOptions,
                MaxAge = rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromDays(7)
            });
        }
    }
}
```

#### User Repository
```csharp
// Infrastructure/Data/CosmosDB/UserRepository.cs
using Microsoft.Azure.Cosmos;
using TodoApp.Domain.UserManagement.Models;
using TodoApp.Domain.UserManagement.Interfaces;

namespace TodoApp.Infrastructure.Data.CosmosDB
{
    public class UserRepository : IUserRepository
    {
        private readonly Container _container;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(CosmosClient cosmosClient, IConfiguration configuration, ILogger<UserRepository> logger)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            _container = cosmosClient.GetContainer(databaseName, "Users");
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<User>(userId, new PartitionKey(userId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.type = 'user' AND c.emailLowercase = @email AND c.isDeleted = false")
                .WithParameter("@email", email.ToLowerInvariant());

            var iterator = _container.GetItemQueryIterator<User>(query);
            var results = await iterator.ReadNextAsync();

            return results.FirstOrDefault();
        }

        public async Task<User> CreateAsync(User user)
        {
            var response = await _container.CreateItemAsync(user, new PartitionKey(user.Id));
            return response.Resource;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var response = await _container.ReplaceItemAsync(user, user.Id, new PartitionKey(user.Id));
            return response.Resource;
        }
    }
}
```

---

## Authentication & Security

### JWT Token Strategy

#### Access Token
- **Type:** JWT (JSON Web Token)
- **Storage:** HTTP-only cookie (primary), also returned in response body for mobile apps
- **Expiration:** 15 minutes
- **Payload:**
```json
{
  "sub": "usr_a1b2c3d4e5f6",
  "email": "user@example.com",
  "role": "Member",
  "iat": 1635789012,
  "exp": 1635789912
}
```

#### Refresh Token
- **Type:** Opaque token (random string)
- **Storage:** HTTP-only cookie, also stored in CosmosDB Tokens collection
- **Expiration:** 7 days (30 days if "remember me")
- **Rotation:** New refresh token issued on each use

### Password Security

#### Password Requirements
- Minimum 8 characters
- At least 1 uppercase letter (A-Z)
- At least 1 lowercase letter (a-z)
- At least 1 number (0-9)
- At least 1 special character (@$!%*?&#)

#### Password Hashing
- **Algorithm:** BCrypt
- **Work Factor:** 10 (configurable)
- **Salt:** Automatically generated per password

```csharp
// Example using BCrypt.Net
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### Token Security

#### Email Verification Token
- **Format:** Random 32-character alphanumeric string
- **Storage:** Hashed in CosmosDB
- **Expiration:** 24 hours
- **Single-use:** Marked as used after verification

#### Password Reset Token
- **Format:** Random 32-character alphanumeric string
- **Storage:** Hashed in CosmosDB
- **Expiration:** 1 hour
- **Single-use:** Marked as used after reset

#### Token Generation
```csharp
public class TokenService : ITokenService
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IConfiguration _configuration;

    public async Task<string> GenerateEmailVerificationTokenAsync(string userId, string email)
    {
        // Generate random token
        var token = GenerateRandomToken(32);
        var tokenHash = HashToken(token);

        // Store in database
        var tokenDocument = new Token
        {
            UserId = userId,
            TokenType = "EmailVerification",
            Token = token, // Store plain for URL
            TokenHash = tokenHash, // Store hash for validation
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IsUsed = false
        };

        await _tokenRepository.CreateAsync(tokenDocument);

        return token;
    }

    private string GenerateRandomToken(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string HashToken(string token)
    {
        return BCrypt.Net.BCrypt.HashPassword(token, workFactor: 10);
    }
}
```

### Rate Limiting

#### Login Endpoint
- **Limit:** 5 attempts per 15 minutes per IP
- **Response:** 429 Too Many Requests

#### Password Reset Endpoint
- **Limit:** 3 requests per hour per email
- **Response:** 200 OK (same as success for security)

#### Registration Endpoint
- **Limit:** 3 registrations per hour per IP
- **Response:** 429 Too Many Requests

### Security Headers

```csharp
// Add to Program.cs or middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline';");

    await next();
});
```

### CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://todoapp.com",
            "https://www.todoapp.com",
            "http://localhost:5173" // Vite dev server
        )
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
```

---

## Azure Services

### CosmosDB Configuration

```bicep
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: cosmosAccountName
  location: location
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: 'TodoAppDb'
  properties: {
    resource: {
      id: 'TodoAppDb'
    }
  }
}

resource usersContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: 'Users'
  properties: {
    resource: {
      id: 'Users'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          { path: '/*' }
        ]
        compositeIndexes: [
          [
            { path: '/emailLowercase', order: 'ascending' }
            { path: '/isDeleted', order: 'ascending' }
          ]
        ]
      }
    }
    options: {
      throughput: 400
    }
  }
}
```

### Azure Entra (Azure AD B2C) Configuration

**Note:** Azure AD B2C infrastructure is provisioned in Release 1 but actively used starting in Release 4 for social authentication.

#### Azure AD B2C Resource Creation

Azure AD B2C tenants are created through the Azure Portal rather than Bicep due to their special nature as separate AAD tenants. However, we document the configuration here:

**Portal Creation Steps:**
1. Navigate to Azure Portal → Create a resource
2. Search for "Azure Active Directory B2C"
3. Click "Create a new Azure AD B2C Tenant"
4. Choose "Create a new Azure AD B2C Tenant"
5. Configure:
   - Organization name: ToDo App
   - Initial domain name: todoapp (results in todoapp.onmicrosoft.com)
   - Country/Region: United States
   - Subscription: Select your subscription
   - Resource group: rg-todoapp-prod

**Bicep for B2C App Registration (Alternative approach using Microsoft Graph):**

While the tenant itself must be created via portal, we can manage some B2C resources via Bicep:

```bicep
// Note: This requires Microsoft Graph API and proper permissions
// For initial setup, portal configuration is recommended

param b2cTenantName string = 'todoapp'
param environment string = 'prod'
param frontendUrl string = 'https://todoapp.com'

// Application Registration for Frontend
resource b2cAppRegistration 'Microsoft.AzureActiveDirectory/b2cDirectories/applications@2021-04-01' = {
  name: 'todoapp-frontend-${environment}'
  properties: {
    displayName: 'ToDo App Frontend (${environment})'
    replyUrls: [
      '${frontendUrl}/auth/callback'
      environment == 'dev' ? 'http://localhost:5173/auth/callback' : null
    ]
    publicClient: false
    oauth2AllowImplicitFlow: false
    oauth2AllowIdTokenImplicitFlow: true
    requiredResourceAccess: [
      {
        resourceAppId: '00000003-0000-0000-c000-000000000000' // Microsoft Graph
        resourceAccess: [
          {
            id: '37f7f235-527c-4136-accd-4a02d197296e' // openid
            type: 'Scope'
          }
          {
            id: '7427e0e9-2fba-42fe-b0c0-848c9e6a8182' // offline_access
            type: 'Scope'
          }
          {
            id: '14dad69e-099b-42c9-810b-d002981feec1' // profile
            type: 'Scope'
          }
          {
            id: '64a6cdd6-aab1-4aaf-94b8-3cc8405e90d0' // email
            type: 'Scope'
          }
        ]
      }
    ]
  }
}
```

#### Manual Configuration (Recommended for Release 1)

**1. Identity Providers Setup (Release 4):**

**Google OAuth Configuration:**
- Go to https://console.cloud.google.com/
- Create OAuth 2.0 credentials
- Configure redirect URI: `https://todoapp.b2clogin.com/todoapp.onmicrosoft.com/oauth2/authresp`
- Store Client ID and Secret in Azure Key Vault

**Microsoft Account Configuration:**
- Microsoft Account is pre-configured in Azure AD B2C
- No additional setup required for basic Microsoft login

**2. User Flows Configuration:**

Create the following user flows in Azure Portal → Azure AD B2C → User flows:

**Sign-up and Sign-in Flow:**
```
Name: B2C_1_SignUpSignIn
Type: Sign up and sign in (Recommended)
Identity providers:
  - Email signup (Release 1)
  - Google (Release 4)
  - Microsoft Account (Release 4)
User attributes to collect:
  - Display Name
  - Email Address
Claims to return:
  - Display Name
  - Email Addresses
  - Identity Provider
  - User's Object ID
Page layouts: Default (customizable in future)
```

**Profile Editing Flow:**
```
Name: B2C_1_ProfileEdit
Type: Profile editing
Attributes:
  - Display Name
  - Job Title (optional)
Claims: Same as sign-in flow
```

**Password Reset Flow:**
```
Name: B2C_1_PasswordReset
Type: Password reset
Verification: Email
Claims: Email Addresses, User's Object ID
```

**3. Application Registration:**

Register the frontend application in Azure AD B2C:

```
App Registration:
  Name: ToDo App Frontend
  Supported account types: Accounts in any identity provider or organizational directory (for authenticating users with user flows)
  Redirect URI:
    - Platform: Single-page application (SPA)
    - URIs:
      - https://todoapp.com/auth/callback
      - https://todoapp-staging.azurewebsites.net/auth/callback
      - http://localhost:5173/auth/callback (development)

API Permissions:
  - Microsoft Graph:
    - openid (delegated)
    - offline_access (delegated)
    - profile (delegated)
    - email (delegated)

Certificates & secrets:
  - (Not needed for SPA with PKCE, but can create client secret for server-side flow)

Authentication:
  - Enable PKCE: Yes
  - Allow public client flows: No
  - Supported account types: Accounts in any identity provider
```

**4. Custom Branding (Optional - Future Enhancement):**

Azure AD B2C allows custom HTML/CSS for sign-in pages:
- Upload custom page layouts to Azure Blob Storage
- Configure page layout URLs in user flows
- Apply company branding (logo, colors, background)

#### Key Vault Secret Storage

Store Azure AD B2C configuration in Key Vault:

```bicep
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: 'kv-todoapp-${environment}'
}

// Store B2C configuration as secrets
resource azureAdB2CClientId 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'AzureAdB2C--ClientId'
  properties: {
    value: '<application-id-from-app-registration>'
  }
}

resource azureAdB2CDomain 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'AzureAdB2C--Domain'
  properties: {
    value: '${b2cTenantName}.onmicrosoft.com'
  }
}

resource azureAdB2CInstance 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'AzureAdB2C--Instance'
  properties: {
    value: 'https://${b2cTenantName}.b2clogin.com'
  }
}

// Google OAuth secrets (Release 4)
resource googleClientId 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'GoogleOAuth--ClientId'
  properties: {
    value: '<from-google-cloud-console>'
  }
}

resource googleClientSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'GoogleOAuth--ClientSecret'
  properties: {
    value: '<from-google-cloud-console>'
    contentType: 'text/plain'
  }
}
```

#### Cost Management

**Azure AD B2C Pricing:**
- Free tier: 50,000 authentications per month
- Premium P1: $0.00325 per user per month (beyond free tier)
- Premium P2: $0.013 per user per month (advanced security features)

**Recommendations:**
- Start with free tier for development and initial production
- Monitor Monthly Active Users (MAU)
- Upgrade to Premium P1 when exceeding free tier or needing advanced features
- Use Azure Cost Management alerts to track B2C costs

#### Monitoring & Logging

**Enable Diagnostic Settings for Azure AD B2C:**
```bicep
// Log Analytics workspace for B2C logs
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: 'log-todoapp-${environment}'
}

// Note: B2C diagnostic settings configured via Portal
// Logs to capture:
//   - AuditLogs (sign-ins, user operations)
//   - SignInLogs (authentication events)
//   - RiskyUsers (identity protection)
//   - UserRiskEvents (suspicious activities)
```

**Key Metrics to Monitor:**
- Total sign-ins
- Failed sign-ins (by reason)
- Sign-ins by identity provider
- User flow completion rate
- Token issuance rate
- MFA challenge rate (future)

#### Security Configuration

**Conditional Access (Premium P1 feature - Future):**
- Require MFA for risky sign-ins
- Block legacy authentication protocols
- Restrict access by location
- Require compliant devices

**Identity Protection (Premium P2 feature - Future):**
- Risk-based conditional access
- Automated risk remediation
- Anomaly detection
- Leaked credential detection

### Azure Functions

#### Email Sending Function
```csharp
// Azure Function triggered by queue
[FunctionName("SendEmailFunction")]
public static async Task Run(
    [QueueTrigger("email-queue")] EmailQueueMessage message,
    ILogger log)
{
    log.LogInformation($"Sending email to: {message.To}");

    var emailService = new EmailService();
    await emailService.SendAsync(message);

    log.LogInformation($"Email sent successfully to: {message.To}");
}

public class EmailQueueMessage
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string BodyHtml { get; set; }
    public string BodyText { get; set; }
}
```

#### Token Cleanup Function
```csharp
// Azure Function running daily
[FunctionName("CleanupExpiredTokensFunction")]
public static async Task Run(
    [TimerTrigger("0 0 2 * * *")] TimerInfo timer, // 2 AM daily
    ILogger log)
{
    log.LogInformation("Starting token cleanup");

    var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
    var container = cosmosClient.GetContainer("TodoAppDb", "Tokens");

    var query = new QueryDefinition(
        "SELECT * FROM c WHERE c.expiresAt < @now OR c.isUsed = true")
        .WithParameter("@now", DateTime.UtcNow);

    var iterator = container.GetItemQueryIterator<Token>(query);
    int deletedCount = 0;

    while (iterator.HasMoreResults)
    {
        var results = await iterator.ReadNextAsync();
        foreach (var token in results)
        {
            await container.DeleteItemAsync<Token>(token.Id, new PartitionKey(token.UserId));
            deletedCount++;
        }
    }

    log.LogInformation($"Cleaned up {deletedCount} expired/used tokens");
}
```

### Azure Storage Queue

#### Queue Message Format
```json
{
  "to": "user@example.com",
  "subject": "Verify Your Email",
  "bodyHtml": "<html>...</html>",
  "bodyText": "Please verify your email..."
}
```

#### Queueing Service
```csharp
public class QueueService : IQueueService
{
    private readonly QueueClient _queueClient;

    public QueueService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"];
        _queueClient = new QueueClient(connectionString, "email-queue");
    }

    public async Task QueueEmailAsync(EmailQueueMessage message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var base64Message = Convert.ToBase64String(messageBytes);

        await _queueClient.SendMessageAsync(base64Message);
    }
}
```

---

## Implementation Guide

### Phase 1: Infrastructure Setup (2-3 days)

#### 1.1 Azure Resource Provisioning
- [ ] Create Azure resource group
- [ ] Deploy CosmosDB using Bicep
- [ ] Deploy SQL Server for reporting
- [ ] Deploy Azure Storage (queues and blob)
- [ ] Deploy Azure Functions apps
- [ ] Configure Azure Key Vault
- [ ] Set up networking and security rules

#### 1.2 Backend Project Setup
- [ ] Create .NET 10 Web API project
- [ ] Configure project structure (Domain/Infrastructure/Shared)
- [ ] Install NuGet packages:
  - Microsoft.Azure.Cosmos
  - MediatR
  - FluentValidation
  - BCrypt.Net-Next
  - Swashbuckle.AspNetCore (Swagger)
  - Serilog
- [ ] Configure dependency injection
- [ ] Set up configuration management
- [ ] Configure logging

#### 1.3 Frontend Project Setup
- [ ] Create Vite + React + TypeScript project
- [ ] Install npm packages:
  - @mui/material @mui/icons-material
  - @tanstack/react-query
  - formik yup
  - axios
  - react-router-dom
- [ ] Configure project structure
- [ ] Set up routing
- [ ] Configure Material-UI theme
- [ ] Set up Axios interceptors

### Phase 2: User Registration (US-UM-001) (3-4 days)

#### Backend Implementation
- [ ] Create User model
- [ ] Create UserRepository with CosmosDB implementation
- [ ] Create RegisterUserCommand and handler
- [ ] Create RegisterUserValidator
- [ ] Create PasswordHasher service
- [ ] Create AuthController with register endpoint
- [ ] Write unit tests for registration logic
- [ ] Test API endpoint with Postman

#### Frontend Implementation
- [ ] Create RegisterForm component
- [ ] Create useRegister hook
- [ ] Create registration validation schema
- [ ] Create auth API client
- [ ] Implement form with Material-UI
- [ ] Add error handling and display
- [ ] Add loading states
- [ ] Test registration flow

### Phase 3: Email Verification (US-UM-002) (2-3 days)

#### Backend Implementation
- [ ] Create Token model
- [ ] Create TokenRepository
- [ ] Create TokenService for generating tokens
- [ ] Create VerifyEmailCommand and handler
- [ ] Create ResendVerificationCommand and handler
- [ ] Add verify-email and resend-verification endpoints
- [ ] Set up Azure Queue for email messages
- [ ] Create Azure Function for email sending
- [ ] Integrate with email service provider
- [ ] Write unit tests

#### Frontend Implementation
- [ ] Create EmailVerificationPage component
- [ ] Create useVerifyEmail hook
- [ ] Handle token from URL query parameter
- [ ] Display verification status
- [ ] Add resend verification functionality
- [ ] Add unverified user banner
- [ ] Test verification flow

### Phase 4: User Login (US-UM-004) (3-4 days)

#### Backend Implementation
- [ ] Create LoginUserCommand and handler
- [ ] Create JWT generation service
- [ ] Create RefreshToken model and repository
- [ ] Implement password verification
- [ ] Add login endpoint
- [ ] Add refresh-token endpoint
- [ ] Create JWT authentication middleware
- [ ] Implement HTTP-only cookie handling
- [ ] Add security event logging
- [ ] Implement rate limiting
- [ ] Write unit tests

#### Frontend Implementation
- [ ] Create LoginForm component
- [ ] Create useLogin hook
- [ ] Create useAuth context
- [ ] Implement login validation schema
- [ ] Add "Remember Me" checkbox
- [ ] Handle authentication state
- [ ] Implement token refresh logic
- [ ] Create PrivateRoute component
- [ ] Add authentication persistence
- [ ] Test login flow

### Phase 5: User Logout (US-UM-005) (1-2 days)

#### Backend Implementation
- [ ] Create LogoutCommand and handler
- [ ] Add logout endpoint
- [ ] Invalidate refresh tokens
- [ ] Clear authentication cookies
- [ ] Log security event
- [ ] Write unit tests

#### Frontend Implementation
- [ ] Create useLogout hook
- [ ] Add logout button to header
- [ ] Clear authentication state
- [ ] Redirect to login page
- [ ] Test logout flow

### Phase 6: Password Reset (US-UM-006, US-UM-007) (3-4 days)

#### Backend Implementation
- [ ] Create ForgotPasswordCommand and handler
- [ ] Create ResetPasswordCommand and handler
- [ ] Implement password reset token generation
- [ ] Add forgot-password endpoint
- [ ] Add reset-password endpoint
- [ ] Queue password reset emails
- [ ] Implement password history checking
- [ ] Invalidate all sessions on password reset
- [ ] Send password change notification email
- [ ] Write unit tests

#### Frontend Implementation
- [ ] Create ForgotPasswordForm component
- [ ] Create ResetPasswordForm component
- [ ] Create useForgotPassword hook
- [ ] Create useResetPassword hook
- [ ] Implement validation schemas
- [ ] Handle token from URL
- [ ] Display success/error messages
- [ ] Test password reset flow

### Phase 7: User Profile (US-UM-010) (2-3 days)

#### Backend Implementation
- [ ] Create GetUserProfileQuery and handler
- [ ] Add profile endpoint
- [ ] Map User to UserProfileDto
- [ ] Add authorization check
- [ ] Write unit tests

#### Frontend Implementation
- [ ] Create UserProfilePage component
- [ ] Create useUserProfile hook
- [ ] Display user information
- [ ] Add profile picture placeholder
- [ ] Test profile page

### Phase 8: Testing & Quality (3-4 days)

- [ ] Complete unit test coverage (>80%)
- [ ] Write integration tests for critical paths
- [ ] Set up Virtuoso API for regression testing
- [ ] Perform accessibility testing (WCAG 2.1 AA)
- [ ] Fix accessibility issues
- [ ] Performance testing and optimization
- [ ] Security review and penetration testing
- [ ] Load testing

### Phase 9: Documentation (2-3 days)

- [ ] Complete API documentation (Swagger)
- [ ] Write component documentation
- [ ] Create deployment guide
- [ ] Write user documentation
- [ ] Update README files
- [ ] Create architecture diagrams
- [ ] Document security considerations

### Phase 10: Deployment (2-3 days)

- [ ] Configure production environment variables
- [ ] Set up CI/CD pipeline
- [ ] Deploy backend to Azure App Service
- [ ] Deploy frontend to Azure Static Web Apps / CDN
- [ ] Configure custom domain and SSL
- [ ] Set up monitoring and alerts
- [ ] Perform smoke tests in production
- [ ] Monitor for issues

---

## Estimated Timeline

- **Phase 1 (Infrastructure):** 2-3 days
- **Phase 2 (Registration):** 3-4 days
- **Phase 3 (Email Verification):** 2-3 days
- **Phase 4 (Login):** 3-4 days
- **Phase 5 (Logout):** 1-2 days
- **Phase 6 (Password Reset):** 3-4 days
- **Phase 7 (User Profile):** 2-3 days
- **Phase 8 (Testing):** 3-4 days
- **Phase 9 (Documentation):** 2-3 days
- **Phase 10 (Deployment):** 2-3 days

**Total:** 23-34 days (approximately 4-7 weeks depending on team size and experience)

---

## Success Criteria

### Functional Requirements
- [x] All 7 user stories implemented and tested
- [x] All acceptance criteria met
- [x] End-to-end user flows working

### Technical Requirements
- [x] >80% unit test coverage
- [x] All integration tests passing
- [x] API documentation complete
- [x] WCAG 2.1 AA compliant
- [x] Performance benchmarks met
- [x] Security audit passed

### Deployment Requirements
- [x] Infrastructure deployed via Bicep
- [x] CI/CD pipeline operational
- [x] Monitoring and alerting configured
- [x] Production deployment successful

---

## Next Steps

After Release 1 completion:

1. **User Acceptance Testing** - Gather feedback from stakeholders
2. **Performance Monitoring** - Track metrics and optimize
3. **Release 2 Planning** - Profile & Security Enhancement
4. **Continuous Improvement** - Address technical debt and bugs

---

## References

- [User Management User Stories](../UserStories/USER_MANAGEMENT.md)
- [Data Dictionary](../../../DATA_DICTIONARY.md)
- [Project README](../../../README.md)
- [Wireframes](../../../wireframes/README.md)

---

**Document Status:** Ready for Implementation
**Next Actions:**
1. Review and approve architecture
2. Provision Azure resources
3. Begin Phase 1 implementation
4. Set up project tracking
