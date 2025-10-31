# Azure Communication Services Email Relay (Phase 1)

This backend folder contains the foundational assets for an Azure Function that relays email messages through Azure Communication Services (ACS). Phase 1 focuses on establishing structure, configuration primitives, and message schemas.

## Solution Layout

- `Azure.Comms.EmailRelay.sln` - Solution entry point.
- `Azure.Comms.EmailRelay.Functions/` - Azure Functions isolated worker project (net8.0).
  - `Program.cs` - Host builder, dependency setup, and service registrations.
  - `Configuration/` - Strongly typed configuration objects (`EmailQueueOptions`, `BlobStorageOptions`, `CommunicationServiceOptions`).
  - `Models/` - Canonical queue payload definitions for email dispatch (message, content, recipients, attachments, metadata, failures).
  - `Services/` - Email dispatch pipeline (validators, attachment loader, ACS client wrapper, queue publishers, metrics, failure archival).
  - `Functions/EmailDispatchFunction.cs` - Queue trigger that validates payloads, streams attachments, sends via ACS, and pushes poison messages to the failed queue.
  - `Functions/FailedEmailMonitorFunction.cs` - Processes failed queue entries, archives payloads, and optionally resubmits for retry.
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

## Email Dispatch Flow (Phase 2)

- Queue-triggered `EmailDispatchFunction` receives `EmailMessage` payloads and enriches metadata with dequeue counts.
- `EmailMessageValidator` enforces schema constraints and ensures a usable content envelope before attempted delivery.
- `AttachmentLoader` streams referenced blobs (default container `email-attachments`) and maps them to ACS `EmailAttachment` instances.
- `EmailDispatcher` converts the payload to ACS SDK types, applies a transient retry policy (Polly, 3 attempts with exponential backoff), and invokes `EmailClient.SendAsync`.
- Success path records structured metrics (`EmailMetricsRecorder`) and logs subject, recipients, duration, and ACS operation identifiers for App Insights ingestion.
- Failures are classified as transient (bubble back to queue for retry) or fatal (`ValidationException`, `InvalidOperationException`). Fatal cases, or messages that exceed `EmailQueues:MaxDequeueCount`, are promoted to the failed queue with rich diagnostics and stack traces via `FailureQueuePublisher`.

## Failure Queue Handling & Archival (Phase 3)

- `FailedEmailMonitorFunction` listens on `email-failed`, storing serialized payloads in the `email-logs` container using `FailureArchiveWriter` (`failures/yyyy/MM/dd/...json`).
- Diagnostics are logged with dequeue counts to enable alerting (e.g., Azure Monitor on function logs or queue depth).
- Optional auto-resubmission: when `Diagnostics["AutoRetry"] == "true"`, the original `EmailMessage` is re-enqueued to the outbox with a visibility delay derived from `EmailQueues:VisibilityTimeoutSeconds` using `OutboxQueuePublisher`.
- Failure telemetry remains available through structured logs, allowing dashboards to visualize counts, root causes, and retry decisions.

## Next Steps (Phase 2+)

- Parameterize retry policies, finalize template support, and integrate with ACS suppression lists.
- Wire structured logging/metrics into Application Insights (custom events, metrics, availability tests).
- Introduce infrastructure-as-code and CI/CD automation.
- Create manual tooling (CLI/HTTP) on top of failed queue archival for support workflows.
