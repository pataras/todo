# Database Design - Release 1

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** Design Phase

## Overview

This document provides detailed database design specifications for Release 1 of the ToDo application's authentication domain. It covers CosmosDB collections, SQL Server tables, indexing strategies, data access patterns, and synchronization approaches.

---

## Table of Contents

1. [Database Strategy](#database-strategy)
2. [CosmosDB Design](#cosmosdb-design)
3. [SQL Server Design](#sql-server-design)
4. [Data Synchronization](#data-synchronization)
5. [Backup & Recovery](#backup--recovery)
6. [Performance Considerations](#performance-considerations)

---

## Database Strategy

### Dual Database Approach

The application uses a dual-database strategy:

| Database | Purpose | Data Type | Usage Pattern |
|----------|---------|-----------|---------------|
| **CosmosDB** | Primary operational database | Transactional, real-time | High-throughput writes and reads |
| **SQL Server** | Reporting and analytics | Aggregated, historical | Complex queries and reports |

### Why This Approach?

1. **CosmosDB Strengths:**
   - Global distribution and low latency
   - Flexible schema for evolving domain models
   - Excellent for high-throughput transactional workloads
   - Built-in change feed for event sourcing

2. **SQL Server Strengths:**
   - Complex JOIN operations for reporting
   - Familiar SQL query language
   - Mature tooling for BI and analytics
   - Better for ad-hoc queries

---

## CosmosDB Design

### Database Configuration

**Database Name:** `TodoAppDb`
**API:** SQL API (Core)
**Consistency Level:** Session (default)
**Provisioned Throughput:** Autoscale (400-4000 RU/s per container)

### Container Design Philosophy

**Design Principles:**
1. Each container represents an aggregate root or closely related data
2. Partition keys chosen for even distribution and access patterns
3. Documents include denormalized data to minimize cross-partition queries
4. TTL used for automatic cleanup of temporary data

---

### 1. Users Container

#### Container Configuration

```json
{
  "id": "Users",
  "partitionKey": {
    "paths": ["/id"],
    "kind": "Hash"
  },
  "indexingPolicy": {
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
      { "path": "/*" }
    ],
    "excludedPaths": [
      { "path": "/metadata/*" }
    ],
    "compositeIndexes": [
      [
        { "path": "/emailLowercase", "order": "ascending" },
        { "path": "/isDeleted", "order": "ascending" }
      ],
      [
        { "path": "/createdAt", "order": "descending" },
        { "path": "/isDeleted", "order": "ascending" }
      ]
    ]
  },
  "uniqueKeyPolicy": {
    "uniqueKeys": [
      { "paths": ["/emailLowercase"] }
    ]
  },
  "defaultTtl": -1
}
```

#### Document Schema

```typescript
interface UserDocument {
  // Core identity
  id: string;                    // Partition key, format: "usr_{guid}"
  type: "user";                  // Document type discriminator

  // Authentication
  email: string;                 // Original case email
  emailLowercase: string;        // Lowercase for case-insensitive lookup
  passwordHash: string;          // BCrypt hash

  // Profile
  fullName: string;              // User's full name
  profilePictureUrl?: string;    // Blob storage URL (future)

  // Authorization
  role: "Member" | "TeamLead" | "Admin";
  permissions?: string[];        // Future: granular permissions

  // Status
  isEmailVerified: boolean;
  isActive: boolean;             // Can user log in?
  isDeleted: boolean;            // Soft delete flag
  deletedAt?: string;            // ISO 8601 timestamp

  // Audit
  createdAt: string;             // ISO 8601 timestamp
  updatedAt: string;             // ISO 8601 timestamp
  lastLoginAt?: string;          // ISO 8601 timestamp

  // Metadata
  metadata: {
    registrationIp: string;
    registrationUserAgent: string;
    passwordChangedAt?: string;
    passwordHistory?: string[];  // Last 5 password hashes
  };

  // Security
  failedLoginAttempts?: number;
  accountLockedUntil?: string;   // ISO 8601 timestamp

  // CosmosDB
  _rid?: string;                 // Resource ID (auto-generated)
  _self?: string;                // Self link (auto-generated)
  _etag?: string;                // ETag for optimistic concurrency
  _ts?: number;                  // Timestamp (auto-generated)
  _ttl?: number;                 // Time to live (-1 = never expire)
}
```

#### Example Document

```json
{
  "id": "usr_a1b2c3d4e5f6g7h8i9j0",
  "type": "user",
  "email": "john.doe@example.com",
  "emailLowercase": "john.doe@example.com",
  "passwordHash": "$2b$10$N9qo8uLOickgx2ZMRZoMye/3J.w8YfZv9K3A6sGJN9WrZ1O7xB8mG",
  "fullName": "John Doe",
  "role": "Member",
  "isEmailVerified": true,
  "isActive": true,
  "isDeleted": false,
  "createdAt": "2025-10-29T10:30:00.000Z",
  "updatedAt": "2025-10-29T10:30:00.000Z",
  "lastLoginAt": "2025-10-29T12:15:30.000Z",
  "metadata": {
    "registrationIp": "192.168.1.100",
    "registrationUserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "passwordChangedAt": "2025-10-29T10:30:00.000Z",
    "passwordHistory": [
      "$2b$10$N9qo8uLOickgx2ZMRZoMye/3J.w8YfZv9K3A6sGJN9WrZ1O7xB8mG"
    ]
  },
  "failedLoginAttempts": 0,
  "_ttl": -1
}
```

#### Indexing Strategy

| Index Type | Paths | Purpose | Performance Impact |
|------------|-------|---------|-------------------|
| Primary | `/id` | Partition key lookups | Included by default |
| Composite | `/emailLowercase`, `/isDeleted` | Email lookup excluding deleted users | +2 RU per query |
| Composite | `/createdAt`, `/isDeleted` | Chronological listing | +2 RU per query |
| Excluded | `/metadata/*` | Reduce index size | -1 RU per write |

#### Query Patterns

**1. Get User by ID**
```sql
-- Cost: ~1 RU (point read)
SELECT * FROM c WHERE c.id = @userId
```

**2. Get User by Email**
```sql
-- Cost: ~3 RU (indexed query)
SELECT * FROM c
WHERE c.type = 'user'
  AND c.emailLowercase = @email
  AND c.isDeleted = false
```

**3. List Active Users**
```sql
-- Cost: ~5 RU per page (paginated)
SELECT * FROM c
WHERE c.type = 'user'
  AND c.isActive = true
  AND c.isDeleted = false
ORDER BY c.createdAt DESC
OFFSET @offset LIMIT @limit
```

---

### 2. Tokens Container

#### Container Configuration

```json
{
  "id": "Tokens",
  "partitionKey": {
    "paths": ["/userId"],
    "kind": "Hash"
  },
  "indexingPolicy": {
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
      { "path": "/*" }
    ],
    "excludedPaths": [
      { "path": "/metadata/*" }
    ],
    "compositeIndexes": [
      [
        { "path": "/tokenType", "order": "ascending" },
        { "path": "/isUsed", "order": "ascending" },
        { "path": "/expiresAt", "order": "ascending" }
      ]
    ]
  },
  "defaultTtl": 172800
}
```

#### Document Schema

```typescript
interface TokenDocument {
  // Core identity
  id: string;                    // Format: "tok_{guid}"
  type: "token";                 // Document type discriminator
  userId: string;                // Partition key

  // Token data
  tokenType: "EmailVerification" | "PasswordReset" | "RefreshToken";
  token: string;                 // Plain token (for URL/cookie)
  tokenHash: string;             // BCrypt hash for validation

  // Associated data
  email?: string;                // For email tokens
  sessionId?: string;            // For refresh tokens

  // Status
  isUsed: boolean;
  usedAt?: string;               // ISO 8601 timestamp

  // Expiration
  expiresAt: string;             // ISO 8601 timestamp
  createdAt: string;             // ISO 8601 timestamp

  // Metadata
  metadata: {
    ip: string;
    userAgent: string;
    location?: {
      country: string;
      region: string;
      city: string;
    };
  };

  // CosmosDB
  _ttl: number;                  // Auto-delete after expiration
}
```

#### Example Documents

**Email Verification Token:**
```json
{
  "id": "tok_x1y2z3a4b5c6d7e8f9g0",
  "type": "token",
  "userId": "usr_a1b2c3d4e5f6g7h8i9j0",
  "tokenType": "EmailVerification",
  "token": "abc123def456ghi789jkl012mno345pq",
  "tokenHash": "$2b$10$N9qo8uLOickgx2ZMRZoMye/3J.w8YfZv9K3A6sGJN9WrZ1O7xB8mG",
  "email": "john.doe@example.com",
  "isUsed": false,
  "expiresAt": "2025-10-30T10:30:00.000Z",
  "createdAt": "2025-10-29T10:30:00.000Z",
  "metadata": {
    "ip": "192.168.1.100",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
  },
  "_ttl": 172800
}
```

**Password Reset Token:**
```json
{
  "id": "tok_m5n6o7p8q9r0s1t2u3v4",
  "type": "token",
  "userId": "usr_a1b2c3d4e5f6g7h8i9j0",
  "tokenType": "PasswordReset",
  "token": "rst789uvw012xyz345abc678def901gh",
  "tokenHash": "$2b$10$KLdfjw8YfZv9K3A6sGJN9WrZ1O7xB8mGN9qo8uLOickgx2ZMRZoMye",
  "email": "john.doe@example.com",
  "isUsed": false,
  "expiresAt": "2025-10-29T11:30:00.000Z",
  "createdAt": "2025-10-29T10:30:00.000Z",
  "metadata": {
    "ip": "192.168.1.100",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
  },
  "_ttl": 7200
}
```

**Refresh Token:**
```json
{
  "id": "tok_w7x8y9z0a1b2c3d4e5f6",
  "type": "token",
  "userId": "usr_a1b2c3d4e5f6g7h8i9j0",
  "tokenType": "RefreshToken",
  "token": "rt_hij234klm567nop890qrs123tuv456",
  "tokenHash": "$2b$10$YfZv9K3A6sGJN9WrZ1O7xB8mGN9qo8uLOickgx2ZMRZoMyeKLdfjw8",
  "sessionId": "ses_abc123def456",
  "isUsed": false,
  "expiresAt": "2025-11-05T10:30:00.000Z",
  "createdAt": "2025-10-29T10:30:00.000Z",
  "metadata": {
    "ip": "192.168.1.100",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
    "location": {
      "country": "US",
      "region": "CA",
      "city": "San Francisco"
    }
  },
  "_ttl": 604800
}
```

#### TTL Strategy

| Token Type | Expiration | TTL (seconds) | Reason |
|------------|-----------|---------------|---------|
| EmailVerification | 24 hours | 172800 (48 hours) | Grace period after expiration |
| PasswordReset | 1 hour | 7200 (2 hours) | Grace period after expiration |
| RefreshToken | 7-30 days | Matches expiration | Auto-cleanup |

#### Query Patterns

**1. Validate Token**
```sql
-- Cost: ~3 RU
SELECT * FROM c
WHERE c.userId = @userId
  AND c.tokenType = @tokenType
  AND c.isUsed = false
  AND c.expiresAt > @now
```

**2. Get User's Active Refresh Tokens**
```sql
-- Cost: ~3 RU
SELECT * FROM c
WHERE c.userId = @userId
  AND c.tokenType = 'RefreshToken'
  AND c.isUsed = false
  AND c.expiresAt > @now
```

**3. Invalidate All User Tokens**
```sql
-- Update query, cost varies by number of tokens
UPDATE c
SET c.isUsed = true, c.usedAt = @now
WHERE c.userId = @userId
```

---

### 3. SecurityEvents Container

#### Container Configuration

```json
{
  "id": "SecurityEvents",
  "partitionKey": {
    "paths": ["/userId"],
    "kind": "Hash"
  },
  "indexingPolicy": {
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
      { "path": "/*" }
    ],
    "excludedPaths": [
      { "path": "/metadata/*" }
    ],
    "compositeIndexes": [
      [
        { "path": "/timestamp", "order": "descending" },
        { "path": "/eventType", "order": "ascending" }
      ]
    ]
  },
  "defaultTtl": 7776000
}
```

#### Document Schema

```typescript
interface SecurityEventDocument {
  // Core identity
  id: string;                    // Format: "evt_{guid}"
  type: "securityEvent";         // Document type discriminator
  userId: string;                // Partition key

  // Event details
  eventType: SecurityEventType;
  timestamp: string;             // ISO 8601 timestamp
  success: boolean;              // Was the action successful?

  // Context
  ip: string;
  userAgent: string;
  location?: {
    country: string;
    region: string;
    city: string;
    latitude?: number;
    longitude?: number;
  };

  // Session
  sessionId?: string;

  // Additional metadata
  metadata: {
    reason?: string;             // For failures
    additionalInfo?: Record<string, any>;
  };

  // CosmosDB
  _ttl: number;                  // 90 days
}

type SecurityEventType =
  | "LoginSuccess"
  | "LoginFailed"
  | "LogoutSuccess"
  | "PasswordChanged"
  | "PasswordResetRequested"
  | "PasswordResetCompleted"
  | "EmailVerified"
  | "EmailVerificationRequested"
  | "AccountLocked"
  | "AccountUnlocked"
  | "MfaEnabled"
  | "MfaDisabled"
  | "SuspiciousActivity";
```

#### Example Document

```json
{
  "id": "evt_m1n2o3p4q5r6s7t8u9v0",
  "type": "securityEvent",
  "userId": "usr_a1b2c3d4e5f6g7h8i9j0",
  "eventType": "LoginSuccess",
  "timestamp": "2025-10-29T12:15:30.000Z",
  "success": true,
  "ip": "192.168.1.100",
  "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
  "location": {
    "country": "US",
    "region": "CA",
    "city": "San Francisco",
    "latitude": 37.7749,
    "longitude": -122.4194
  },
  "sessionId": "ses_abc123def456",
  "metadata": {
    "additionalInfo": {
      "rememberMe": false
    }
  },
  "_ttl": 7776000
}
```

#### Query Patterns

**1. Get User's Recent Activity**
```sql
-- Cost: ~5 RU per page
SELECT * FROM c
WHERE c.userId = @userId
ORDER BY c.timestamp DESC
OFFSET @offset LIMIT @limit
```

**2. Get Failed Login Attempts**
```sql
-- Cost: ~5 RU
SELECT * FROM c
WHERE c.userId = @userId
  AND c.eventType = 'LoginFailed'
  AND c.timestamp > @since
ORDER BY c.timestamp DESC
```

**3. Detect Suspicious Activity**
```sql
-- Cost: varies
SELECT c.location.city, COUNT(1) as loginCount
FROM c
WHERE c.userId = @userId
  AND c.eventType = 'LoginSuccess'
  AND c.timestamp > @last24Hours
GROUP BY c.location.city
```

---

## SQL Server Design

### Database Schema

**Database Name:** `TodoAppReporting`

### 1. Users Table

```sql
CREATE TABLE [dbo].[Users] (
    -- Primary Key
    [UserId] NVARCHAR(50) NOT NULL PRIMARY KEY,

    -- Core Data
    [Email] NVARCHAR(255) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Member',

    -- Status
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Timestamps
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NOT NULL,
    [LastLoginAt] DATETIME2(7) NULL,
    [DeletedAt] DATETIME2(7) NULL,

    -- Metadata
    [RegistrationIp] NVARCHAR(50) NULL,

    -- Audit
    [SyncedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

-- Indexes
CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email
    ON [dbo].[Users] ([Email])
    WHERE [IsDeleted] = 0;

CREATE NONCLUSTERED INDEX IX_Users_CreatedAt
    ON [dbo].[Users] ([CreatedAt] DESC);

CREATE NONCLUSTERED INDEX IX_Users_IsDeleted_IsActive
    ON [dbo].[Users] ([IsDeleted], [IsActive]);

CREATE NONCLUSTERED INDEX IX_Users_Role
    ON [dbo].[Users] ([Role])
    WHERE [IsDeleted] = 0;
```

### 2. SecurityEvents Table

```sql
CREATE TABLE [dbo].[SecurityEvents] (
    -- Primary Key
    [EventId] NVARCHAR(50) NOT NULL PRIMARY KEY,

    -- Foreign Key
    [UserId] NVARCHAR(50) NOT NULL,

    -- Event Data
    [EventType] NVARCHAR(50) NOT NULL,
    [Timestamp] DATETIME2(7) NOT NULL,
    [Success] BIT NOT NULL,

    -- Context
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Country] NVARCHAR(100) NULL,
    [Region] NVARCHAR(100) NULL,
    [City] NVARCHAR(100) NULL,

    -- Session
    [SessionId] NVARCHAR(50) NULL,

    -- Metadata
    [Reason] NVARCHAR(500) NULL,

    -- Audit
    [SyncedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),

    -- Foreign Key Constraint
    CONSTRAINT FK_SecurityEvents_Users FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([UserId])
);

-- Indexes
CREATE NONCLUSTERED INDEX IX_SecurityEvents_UserId_Timestamp
    ON [dbo].[SecurityEvents] ([UserId], [Timestamp] DESC);

CREATE NONCLUSTERED INDEX IX_SecurityEvents_EventType
    ON [dbo].[SecurityEvents] ([EventType]);

CREATE NONCLUSTERED INDEX IX_SecurityEvents_Timestamp
    ON [dbo].[SecurityEvents] ([Timestamp] DESC);

CREATE NONCLUSTERED INDEX IX_SecurityEvents_IpAddress
    ON [dbo].[SecurityEvents] ([IpAddress]);
```

### 3. Reporting Views

#### Active Users View
```sql
CREATE VIEW [dbo].[vw_ActiveUsers] AS
SELECT
    UserId,
    Email,
    FullName,
    Role,
    IsEmailVerified,
    CreatedAt,
    LastLoginAt,
    DATEDIFF(day, LastLoginAt, GETUTCDATE()) AS DaysSinceLastLogin
FROM [dbo].[Users]
WHERE IsActive = 1
  AND IsDeleted = 0;
```

#### User Activity Summary View
```sql
CREATE VIEW [dbo].[vw_UserActivitySummary] AS
SELECT
    u.UserId,
    u.Email,
    u.FullName,
    COUNT(CASE WHEN se.EventType = 'LoginSuccess' THEN 1 END) AS TotalLogins,
    COUNT(CASE WHEN se.EventType = 'LoginFailed' THEN 1 END) AS FailedLogins,
    MAX(se.Timestamp) AS LastActivity,
    COUNT(DISTINCT se.SessionId) AS TotalSessions
FROM [dbo].[Users] u
LEFT JOIN [dbo].[SecurityEvents] se ON u.UserId = se.UserId
WHERE u.IsDeleted = 0
GROUP BY u.UserId, u.Email, u.FullName;
```

### 4. Stored Procedures

#### Get User Statistics
```sql
CREATE PROCEDURE [dbo].[sp_GetUserStatistics]
    @StartDate DATETIME2,
    @EndDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(CASE WHEN IsEmailVerified = 1 THEN 1 END) AS VerifiedUsers,
        COUNT(CASE WHEN IsEmailVerified = 0 THEN 1 END) AS UnverifiedUsers,
        COUNT(CASE WHEN IsActive = 1 THEN 1 END) AS ActiveUsers,
        COUNT(CASE WHEN IsActive = 0 THEN 1 END) AS InactiveUsers,
        COUNT(CASE WHEN Role = 'Admin' THEN 1 END) AS AdminUsers,
        COUNT(CASE WHEN Role = 'TeamLead' THEN 1 END) AS TeamLeadUsers,
        COUNT(CASE WHEN Role = 'Member' THEN 1 END) AS MemberUsers
    FROM [dbo].[Users]
    WHERE IsDeleted = 0
      AND CreatedAt BETWEEN @StartDate AND @EndDate;
END;
```

---

## Data Synchronization

### CosmosDB to SQL Server Sync

#### Strategy: Change Feed Processing

**Architecture:**
```
CosmosDB Container → Change Feed → Azure Function → SQL Server
```

#### Implementation

**Azure Function (C#):**
```csharp
[FunctionName("SyncUsersToSqlServer")]
public static async Task Run(
    [CosmosDBTrigger(
        databaseName: "TodoAppDb",
        containerName: "Users",
        Connection = "CosmosDbConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)]
    IReadOnlyList<Document> documents,
    ILogger log)
{
    if (documents == null || documents.Count == 0)
    {
        return;
    }

    log.LogInformation($"Processing {documents.Count} user documents");

    using var connection = new SqlConnection(
        Environment.GetEnvironmentVariable("SqlConnectionString"));
    await connection.OpenAsync();

    foreach (var document in documents)
    {
        var user = JsonConvert.DeserializeObject<User>(document.ToString());

        await UpsertUserAsync(connection, user);
    }

    log.LogInformation("Sync completed successfully");
}

private static async Task UpsertUserAsync(SqlConnection connection, User user)
{
    const string sql = @"
        MERGE [dbo].[Users] AS target
        USING (SELECT @UserId AS UserId) AS source
        ON target.UserId = source.UserId
        WHEN MATCHED THEN
            UPDATE SET
                Email = @Email,
                FullName = @FullName,
                Role = @Role,
                IsEmailVerified = @IsEmailVerified,
                IsActive = @IsActive,
                IsDeleted = @IsDeleted,
                UpdatedAt = @UpdatedAt,
                LastLoginAt = @LastLoginAt,
                DeletedAt = @DeletedAt,
                SyncedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT (UserId, Email, FullName, Role, IsEmailVerified, IsActive,
                    IsDeleted, CreatedAt, UpdatedAt, LastLoginAt, DeletedAt,
                    RegistrationIp, SyncedAt)
            VALUES (@UserId, @Email, @FullName, @Role, @IsEmailVerified, @IsActive,
                    @IsDeleted, @CreatedAt, @UpdatedAt, @LastLoginAt, @DeletedAt,
                    @RegistrationIp, GETUTCDATE());
    ";

    using var command = new SqlCommand(sql, connection);
    command.Parameters.AddWithValue("@UserId", user.Id);
    command.Parameters.AddWithValue("@Email", user.Email);
    command.Parameters.AddWithValue("@FullName", user.FullName);
    command.Parameters.AddWithValue("@Role", user.Role);
    command.Parameters.AddWithValue("@IsEmailVerified", user.IsEmailVerified);
    command.Parameters.AddWithValue("@IsActive", user.IsActive);
    command.Parameters.AddWithValue("@IsDeleted", user.IsDeleted);
    command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
    command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);
    command.Parameters.AddWithValue("@LastLoginAt", (object)user.LastLoginAt ?? DBNull.Value);
    command.Parameters.AddWithValue("@DeletedAt", (object)user.DeletedAt ?? DBNull.Value);
    command.Parameters.AddWithValue("@RegistrationIp",
        (object)user.Metadata?.RegistrationIp ?? DBNull.Value);

    await command.ExecuteNonQueryAsync();
}
```

#### Sync Characteristics

- **Latency:** Near real-time (typically < 1 minute)
- **Reliability:** Automatic retries on failure
- **Idempotency:** MERGE ensures safe re-processing
- **Monitoring:** Azure Functions logging and Application Insights

---

## Backup & Recovery

### CosmosDB Backup

**Strategy:** Azure built-in continuous backup

**Configuration:**
- **Mode:** Continuous (point-in-time restore)
- **Retention:** 30 days
- **RPO (Recovery Point Objective):** Any point within last 30 days
- **RTO (Recovery Time Objective):** 1-2 hours

**Restore Process:**
1. Open Azure Portal
2. Navigate to CosmosDB account
3. Select "Point In Time Restore"
4. Choose timestamp
5. Create new account from backup

### SQL Server Backup

**Strategy:** Automated backups via Azure SQL Database

**Configuration:**
- **Full Backup:** Weekly
- **Differential Backup:** Daily
- **Transaction Log Backup:** Every 10 minutes
- **Retention:** 35 days
- **Geo-Redundant:** Yes

**Restore Process:**
```sql
-- Restore to point in time
RESTORE DATABASE TodoAppReporting_Restored
FROM DATABASE_SNAPSHOT = 'TodoAppReporting_2025_10_29_10_00';

-- Or use Azure Portal
-- SQL Databases → TodoAppReporting → Restore
```

---

## Performance Considerations

### CosmosDB Optimization

#### Request Units (RU) Budget

| Operation | Expected RU Cost | Frequency | Daily RU |
|-----------|-----------------|-----------|----------|
| User registration | 10 RU | 100/day | 1,000 |
| User login | 15 RU | 1,000/day | 15,000 |
| Profile read | 3 RU | 2,000/day | 6,000 |
| Token validation | 5 RU | 1,000/day | 5,000 |
| Security event write | 5 RU | 2,000/day | 10,000 |
| **Total Daily** | | | **37,000 RU** |

**Average RU/s:** 37,000 / 86,400 = **0.43 RU/s**
**Recommended Provisioning:** 400 RU/s (with autoscale to 4,000 RU/s)

#### Optimization Techniques

1. **Use Point Reads:** Query by ID + partition key = 1 RU
2. **Minimize Cross-Partition Queries:** Design partition keys carefully
3. **Use Appropriate Consistency:** Session consistency is usually sufficient
4. **Batch Operations:** Use bulk operations for multiple writes
5. **Cache Frequently Accessed Data:** Reduce database reads

### SQL Server Optimization

1. **Indexes:** Create indexes on frequently queried columns
2. **Statistics:** Keep statistics up-to-date for query optimization
3. **Partitioning:** Consider table partitioning for large datasets
4. **Columnstore:** Use for analytical queries on large tables
5. **Query Store:** Enable for query performance insights

---

## Monitoring & Alerts

### CosmosDB Metrics

Monitor:
- Request Units consumed
- Throttled requests (429 errors)
- Latency (P99)
- Availability
- Storage usage

### SQL Server Metrics

Monitor:
- DTU/vCore usage
- Query performance
- Deadlocks
- Connection count
- Storage usage

### Alerts

Set up alerts for:
- RU consumption > 80%
- 429 throttling errors
- Query latency > 1 second
- Failed sync operations
- Storage > 80% capacity

---

## Next Steps

1. **Provision Resources:**
   - Create CosmosDB account and containers
   - Create SQL Server database and tables
   - Set up change feed functions

2. **Implement Repositories:**
   - UserRepository
   - TokenRepository
   - SecurityEventRepository

3. **Test Sync:**
   - Verify change feed processing
   - Validate data consistency
   - Test recovery procedures

4. **Monitor Performance:**
   - Set up dashboards
   - Configure alerts
   - Perform load testing

---

## References

- [CosmosDB Best Practices](https://docs.microsoft.com/en-us/azure/cosmos-db/best-practice)
- [Change Feed Processor](https://docs.microsoft.com/en-us/azure/cosmos-db/change-feed-processor)
- [Azure SQL Database](https://docs.microsoft.com/en-us/azure/azure-sql/)
