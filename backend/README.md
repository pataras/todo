# Azure Communication Services Email Relay (Phase 1)

This backend folder contains the foundational assets for an Azure Function that relays email messages through Azure Communication Services (ACS). Phase 1 focuses on establishing structure, configuration primitives, and message schemas.

## Solution Layout

- `Azure.Comms.EmailRelay.sln` - Solution entry point.
- `Azure.Comms.EmailRelay.Functions/` - Azure Functions isolated worker project (net8.0).
  - `Program.cs` - Host builder and dependency setup with options validation.
  - `Configuration/` - Strongly typed configuration objects (`EmailQueueOptions`, `BlobStorageOptions`, `CommunicationServiceOptions`).
  - `Models/` - Canonical queue payload definitions for email dispatch (message, content, recipients, attachments, metadata).
  - `Functions/EmailDispatchFunction.cs` - Queue trigger placeholder awaiting Phase 2 logic.
  - `host.json` / `local.settings.json` - Runtime configuration with placeholder secrets for local development.

## Configuration Overview

The project currently uses placeholder secrets, intended to be replaced with secure storage in later phases.

| Setting Key | Description | Phase 1 Default |
| --- | --- | --- |
| `AzureWebJobsStorage` | Storage account used for function state and queue bindings. | `UseDevelopmentStorage=true` |
| `EmailQueues:OutboxQueueName` | Primary queue that triggers email dispatch. | `email-outbox` |
| `EmailQueues:FailedQueueName` | Queue for poison/failed messages. | `email-failed` |
| `EmailQueues:MaxDequeueCount` | Logical retry limit before failure handling. | `5` |
| `EmailQueues:VisibilityTimeoutSeconds` | Default visibility timeout (seconds) for rescheduling messages. | `300` |
| `AttachmentStorage:ConnectionString` | Blob storage connection for attachments. | `UseDevelopmentStorage=true` |
| `AttachmentStorage:ContainerName` | Blob container that stores attachments. | `email-attachments` |
| `AttachmentStorage:LogsContainerName` | Blob container for audit/log artifacts. | `email-logs` |
| `CommunicationService:ConnectionString` | ACS Email connection string. | `endpoint=...;accesskey=...` |
| `CommunicationService:DefaultSenderAddress` | Default sender (from) address. | `no-reply@contoso.com` |

## Message Schema (Outbox Queue)

```jsonc
{
  "MessageId": "string (guid)",
  "Subject": "string",
  "Content": {
    "PlainTextBody": "string?",
    "HtmlBody": "string?",
    "TemplateId": "string?",
    "TemplateParameters": { "key": "value" },
    "PreferHtml": true
  },
  "Recipients": [
    {
      "Address": "user@example.com",
      "DisplayName": "Optional name",
      "Type": "To|Cc|Bcc"
    }
  ],
  "SenderOverride": "string?",
  "ReplyTo": [ { "Address": "..." } ],
  "Attachments": [
    {
      "BlobContainerName": "email-attachments",
      "BlobName": "path/to/blob",
      "ContentType": "application/pdf",
      "ContentLength": 12345
    }
  ],
  "Metadata": {
    "CorrelationId": "string?",
    "ProducerMessageId": "string?",
    "EnqueuedAtUtc": "2025-10-31T12:00:00Z",
    "AttemptCount": 0,
    "Properties": { "tenantId": "abc" }
  }
}
```

Attachments reference blobs rather than embedding raw content, keeping queue messages lightweight and consistent with ACS requirements.

## Next Steps (Phase 2+)

- Implement ACS email send logic with resiliency policies (Polly) and blob streaming for attachments.
- Route failures to a dedicated queue with serialized error context.
- Add observability (App Insights) and alerting primitives.
- Introduce infrastructure-as-code and CI/CD automation.
