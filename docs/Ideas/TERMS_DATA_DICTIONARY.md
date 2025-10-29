# ToDo App - Terms Data Dictionary

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** Initial Release
**Source:** Based on terms from ToDo App Product Brief (docs/Ideas/todo-app-brief.md)

## Purpose

This data dictionary defines all business and technical terms specific to the ToDo application mentioned in the Ideas/Product Brief. It complements the main DATA_DICTIONARY.md by focusing exclusively on ToDo app domain concepts, ensuring consistent terminology across development, documentation, and team communication.

---

## Table of Contents

1. [Task Management Domain](#task-management-domain)
2. [User & Access Control Domain](#user--access-control-domain)
3. [Task Organization Domain](#task-organization-domain)
4. [Collaboration Domain](#collaboration-domain)
5. [Task Status & Progress Domain](#task-status--progress-domain)
6. [Advanced Task Features Domain](#advanced-task-features-domain)
7. [Reporting & Analytics Domain](#reporting--analytics-domain)
8. [Accessibility Domain](#accessibility-domain)
9. [Integration Domain](#integration-domain)
10. [Mobile & Offline Domain](#mobile--offline-domain)

---

## Task Management Domain

Core concepts for managing individual tasks and their properties.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Task** | A discrete unit of work that needs to be completed, containing information about what needs to be done, when, and by whom | To-Do Item, Action Item | Task List, Subtask | "Create a task to review the quarterly report" |
| **Task Priority** | The level of importance or urgency assigned to a task to help with scheduling and resource allocation | Priority Level | Low, Medium, High, Critical | "Set task priority to High for urgent deadline" |
| **Low Priority** | Tasks that are not time-sensitive and can be completed when higher priority work is done | Low, P3 | Task Priority | "Low priority tasks deferred to next week" |
| **Medium Priority** | Tasks with moderate importance that should be completed in reasonable timeframe | Medium, P2 | Task Priority | "Medium priority for routine maintenance tasks" |
| **High Priority** | Tasks requiring prompt attention and should be completed soon | High, P1 | Task Priority | "High priority bug fix for production issue" |
| **Critical Priority** | Urgent tasks requiring immediate attention with highest importance | Critical, P0, Urgent | Task Priority | "Critical priority for security vulnerability" |
| **Due Date** | The date and time by which a task must be completed | Deadline, Target Date | Reminder, Task | "Task due date is Friday at 5 PM" |
| **Reminder** | An automated notification sent before a task's due date to alert the assignee | Alert, Notification | Due Date, Notification | "Reminder sent 1 hour before meeting task" |
| **Task Description** | Detailed text explaining what the task involves, objectives, and any relevant context | Description, Details | Task, Task Notes | "Task description outlines review requirements" |
| **Task Notes** | Additional information, comments, or context added to a task for reference | Notes, Details, Comments | Task Description, Task | "Task notes include links to reference documents" |
| **Task Attachment** | Files, documents, or media attached to a task for reference or completion | File Attachment, Attachment | Azure Blob Storage, Task | "PDF specification added as task attachment" |
| **Task Creation** | The action of creating a new task in the system | Create Task, New Task | Task, CRUD | "Task creation requires title and assignee" |
| **Task Update** | Modifying existing task properties such as description, status, or priority | Edit Task, Modify Task | Task, CRUD | "Task update changed priority to High" |
| **Task Deletion** | Removing a task permanently from the system | Delete Task, Remove Task | Task, CRUD | "Task deletion requires confirmation" |
| **Task List** | A collection of related tasks grouped together for organization purposes | Project, Task Collection | Task, Task Organization | "Marketing campaign task list contains 15 tasks" |
| **Project** | A larger scope collection of tasks and task lists representing a significant initiative | Project Container | Task List, Epic | "Website redesign project spans multiple teams" |

---

## User & Access Control Domain

Terms related to users, authentication, authorization, and role-based permissions.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **User** | An individual who accesses and uses the ToDo application | Account, Person | User Profile, Authentication | "User can create and manage personal tasks" |
| **User Profile** | Personal information and settings associated with a user account | Profile, Account Profile | User, User Management | "User profile includes name, email, and preferences" |
| **User Registration** | The process of creating a new user account in the system | Sign Up, Account Creation | User, Authentication | "User registration requires email verification" |
| **User Authentication** | The process of verifying a user's identity when logging into the system | Login, Sign In, AuthN | User, Security | "User authentication via email and password" |
| **Role** | A defined set of permissions assigned to users determining what actions they can perform | User Role, Permission Level | Admin, Team Lead, Member | "Admin role has full system access" |
| **Admin** | A user role with full permissions to manage all aspects of the application | Administrator, Super User | Role, User | "Admin can manage all users and tasks" |
| **Team Lead** | A user role with elevated permissions to manage team members and their tasks | Manager, Supervisor | Role, Team, User | "Team Lead assigns tasks to team members" |
| **Member** | A standard user role with permissions to manage their own tasks and collaborate with others | Team Member, Standard User | Role, Team, User | "Member can create and complete assigned tasks" |
| **Role-Based Access Control** | Security approach restricting system access based on user roles | RBAC, Permission System | Role, Authorization, Security | "RBAC ensures only admins can delete users" |
| **User Management** | Administrative functions for creating, updating, and managing user accounts | Account Management | User, Admin, Role | "User management includes role assignment" |

---

## Task Organization Domain

Terms related to organizing, categorizing, filtering, and finding tasks.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Tag** | A label or keyword assigned to tasks for categorization and filtering | Label, Category, Keyword | Task, Filter | "Tasks tagged with 'urgent' for quick access" |
| **Custom Tag** | User-defined tags created to match specific organizational needs | User Tag, Personal Tag | Tag, Task Organization | "Custom tags like 'client-xyz' or 'quarter-1'" |
| **Task Filter** | Criteria used to show only tasks matching specific conditions | Filter, Query | Task, Search, Tag | "Filter tasks by status 'In Progress'" |
| **Status Filter** | Filtering tasks based on their current status | - | Task Filter, Task Status | "Status filter shows only completed tasks" |
| **Priority Filter** | Filtering tasks based on their priority level | - | Task Filter, Task Priority | "Priority filter displays only high priority items" |
| **Date Filter** | Filtering tasks based on due dates or date ranges | - | Task Filter, Due Date | "Date filter shows tasks due this week" |
| **Tag Filter** | Filtering tasks that have specific tags applied | - | Task Filter, Tag | "Tag filter shows all 'bug' tagged items" |
| **Task Search** | Functionality to find tasks by searching text in titles, descriptions, and notes | Search, Find | Task, Filter | "Search finds tasks containing 'report'" |
| **Task Sort** | Arranging tasks in a specific order based on criteria like date, priority, or title | Sort, Order | Task, Task Organization | "Sort tasks by due date ascending" |
| **Sort Criteria** | The attribute used to determine task ordering | Sort Order, Ordering | Task Sort | "Sort criteria: priority then due date" |
| **Task Categorization** | The practice of organizing tasks into logical groups using tags and lists | Organization, Grouping | Tag, Task List | "Task categorization improves findability" |

---

## Collaboration Domain

Terms related to team collaboration, task sharing, and communication.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Team** | A group of users who collaborate on shared tasks and projects | Work Group, Project Team | Team Member, Collaboration | "Marketing team collaborates on campaign tasks" |
| **Team Member** | A user who belongs to a team and can access shared team resources | Collaborator, Participant | Team, Member, User | "Team members can view all shared tasks" |
| **Task Sharing** | Making a task visible and accessible to other users or teams | Share Task, Task Access | Task, Collaboration, Team | "Task sharing allows team collaboration" |
| **List Sharing** | Making an entire task list visible and accessible to other users or teams | Share List, Project Sharing | Task List, Collaboration, Team | "List sharing enables team project management" |
| **Task Assignment** | The action of designating a specific user responsible for completing a task | Assign Task, Task Owner | Task, User, Team Member | "Task assigned to John for completion" |
| **Assignee** | The user who is assigned to and responsible for completing a task | Task Owner, Responsible Party | Task Assignment, User | "Assignee receives notification of new task" |
| **Task Comment** | Text-based feedback, updates, or discussion added to a task | Comment, Note, Discussion | Task, Collaboration | "Team members add comments on task progress" |
| **Task Update Notification** | Real-time alerts sent to users when tasks they're involved with are modified | Notification, Alert, Update | Task, Collaboration, Reminder | "Notification sent when task status changes" |
| **Real-time Notification** | Immediate alerts delivered as events occur in the system | Live Notification, Push Notification | Notification, Collaboration | "Real-time notifications for @mentions" |
| **Activity History** | A chronological record of all actions taken on a task or list | Audit Log, Change History | Task, Audit Trail | "Activity history shows who changed priority" |
| **Audit Trail** | A comprehensive log of all system activities for compliance and tracking | Audit Log, Activity Log | Activity History, Compliance | "Audit trail required for compliance features" |
| **Mention** | Using @username to notify and draw attention of a specific user in comments | @mention, User Tag | Comment, Notification, User | "@mention John in comment for his input" |

---

## Task Status & Progress Domain

Terms related to task lifecycle, status tracking, and progress measurement.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Task Status** | The current state of a task in its lifecycle | Status, State | Not Started, In Progress, Completed | "Task status updated to In Progress" |
| **Not Started** | Status indicating a task has been created but work hasn't begun | To Do, Pending, Backlog | Task Status | "New tasks default to Not Started status" |
| **In Progress** | Status indicating work on the task is actively underway | Active, Working On, In Development | Task Status | "In Progress tasks appear in daily standup" |
| **Completed** | Status indicating the task has been finished and objectives met | Done, Finished, Closed | Task Status, Task Completion | "Completed tasks archived after 30 days" |
| **Blocked** | Status indicating the task cannot proceed due to dependencies or obstacles | Impediment, Stuck | Task Status, Task Dependency | "Blocked status with reason: awaiting approval" |
| **Cancelled** | Status indicating the task is no longer needed and won't be completed | Abandoned, Rejected | Task Status | "Cancelled tasks removed from active views" |
| **Task Completion** | The act of marking a task as finished | Complete Task, Finish Task | Task, Task Status, Completed | "Task completion triggers notification to assignee" |
| **Progress Indicator** | Visual representation showing how much of a task or project is complete | Progress Bar, Completion Percentage | Task Progress, Subtask | "Progress indicator shows 75% complete" |
| **Task Progress** | The degree to which a task has been completed | Completion Progress | Progress Indicator, Subtask | "Task progress calculated from completed subtasks" |
| **Subtask** | A smaller, component task that is part of a larger parent task | Child Task, Sub-item | Task, Task Progress | "Break down design task into 5 subtasks" |
| **Subtask Tracking** | Monitoring completion of individual subtasks to measure overall progress | - | Subtask, Task Progress | "Subtask tracking shows 3 of 5 complete" |
| **Time Tracking** | Recording time spent working on tasks for productivity analysis | Time Log, Hours Logged | Task, Reporting | "Time tracking captures hours per task" |
| **Time Entry** | A record of time spent on a specific task | Log Entry, Time Record | Time Tracking, Task | "Time entry: 2.5 hours on documentation task" |

---

## Advanced Task Features Domain

Terms related to sophisticated task management capabilities.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Recurring Task** | A task that repeats automatically on a defined schedule | Repeating Task, Scheduled Task | Task, Recurrence Pattern | "Recurring task for weekly team meeting prep" |
| **Recurrence Pattern** | The schedule defining when and how often a recurring task repeats | Recurrence Schedule, Repeat Pattern | Recurring Task | "Recurrence pattern: every Monday at 9 AM" |
| **Daily Recurrence** | A recurrence pattern where tasks repeat every day or on weekdays | Daily Repeat | Recurrence Pattern, Recurring Task | "Daily recurrence for end-of-day status check" |
| **Weekly Recurrence** | A recurrence pattern where tasks repeat on specific days each week | Weekly Repeat | Recurrence Pattern, Recurring Task | "Weekly recurrence every Wednesday" |
| **Monthly Recurrence** | A recurrence pattern where tasks repeat on specific dates or days each month | Monthly Repeat | Recurrence Pattern, Recurring Task | "Monthly recurrence on first Friday" |
| **Custom Recurrence** | A user-defined recurrence pattern with flexible scheduling options | Custom Repeat, Advanced Recurrence | Recurrence Pattern, Recurring Task | "Custom recurrence: every 2 weeks on Tuesday" |
| **Task Dependency** | A relationship where one task cannot start or complete until another task reaches a certain state | Dependency, Predecessor/Successor | Task, Blocked | "Task B depends on Task A completion" |
| **Predecessor Task** | A task that must be completed before a dependent task can begin | Blocking Task, Parent Task | Task Dependency, Task | "Design is predecessor to development task" |
| **Successor Task** | A task that depends on another task's completion before it can start | Dependent Task, Child Task | Task Dependency, Task | "Testing is successor to development task" |
| **Task Template** | A predefined task structure that can be reused for common workflows | Template, Task Pattern | Task, Workflow | "Onboarding template creates 10 standard tasks" |
| **Template Library** | A collection of saved task templates available for reuse | Template Repository | Task Template | "Template library includes common project types" |
| **Bulk Operations** | Actions performed simultaneously on multiple selected tasks | Batch Operations, Multi-select Actions | Task | "Bulk operation assigns 10 tasks to Sarah" |
| **Bulk Assignment** | Assigning multiple tasks to one or more users at once | Batch Assignment, Multi-assign | Bulk Operations, Task Assignment | "Bulk assignment of urgent tasks to team leads" |
| **Bulk Update** | Modifying properties of multiple tasks simultaneously | Batch Update, Mass Update | Bulk Operations, Task Update | "Bulk update changes priority on 15 tasks" |
| **Bulk Delete** | Removing multiple tasks at once | Batch Delete, Mass Delete | Bulk Operations, Task Deletion | "Bulk delete of completed tasks from last quarter" |
| **Task Export** | Converting and downloading tasks in various file formats | Export, Data Export | Task, CSV, PDF | "Export project tasks to Excel for reporting" |
| **CSV Export** | Exporting tasks as comma-separated values file | Spreadsheet Export | Task Export, CSV | "CSV export includes all task fields" |
| **PDF Export** | Exporting tasks as PDF document for sharing or printing | Document Export | Task Export, PDF | "PDF export generates formatted task report" |

---

## Reporting & Analytics Domain

Terms related to productivity metrics, reports, and performance dashboards.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Dashboard** | A visual interface displaying key metrics, tasks, and productivity information | Overview, Control Panel | Reporting, Metrics | "Personal dashboard shows today's tasks" |
| **Personal Dashboard** | A user-specific dashboard showing individual productivity and tasks | My Dashboard, User Dashboard | Dashboard, User | "Personal dashboard displays my active tasks" |
| **Team Dashboard** | A shared dashboard showing team-level metrics and collaboration data | Team Overview, Team Metrics | Dashboard, Team | "Team dashboard shows sprint progress" |
| **Productivity Metrics** | Measurements indicating efficiency and output in task completion | Performance Metrics, KPIs | Reporting, Analytics | "Productivity metrics track tasks per day" |
| **Team Performance** | Collective metrics showing how well a team is achieving goals | Team Metrics, Team Productivity | Team, Productivity Metrics | "Team performance improved by 20% this quarter" |
| **Task Completion Statistics** | Data and metrics about how many tasks are completed over time | Completion Metrics, Done Statistics | Reporting, Task Completion | "Statistics show 85% task completion rate" |
| **Completion Rate** | The percentage of tasks completed within a given timeframe | Completion Percentage | Task Completion Statistics | "Monthly completion rate is 90%" |
| **Overdue Task Report** | A report listing all tasks past their due date | Late Task Report, Overdue Report | Report, Due Date | "Overdue report shows 12 tasks need attention" |
| **Time Tracking Report** | A report summarizing time spent on tasks and projects | Hours Report, Time Summary | Report, Time Tracking | "Time tracking report for client billing" |
| **Report Generation** | The process of creating customized reports based on task data | Custom Report, Report Creation | Reporting | "Generate weekly progress reports automatically" |
| **Custom Report** | A user-defined report with specific filters, groupings, and metrics | Ad-hoc Report, Custom Analytics | Report Generation, Reporting | "Custom report shows tasks by tag and user" |
| **Analytics** | Advanced data analysis of task patterns, trends, and insights | Data Analytics, Insights | Reporting, Metrics | "Analytics reveal peak productivity hours" |

---

## Accessibility Domain

Terms related to making the application usable by people with disabilities.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Accessibility** | The practice of making the application usable by people with disabilities | A11y, Web Accessibility | WCAG, ARIA, Inclusive Design | "Accessibility testing ensures all can use app" |
| **WCAG Compliance** | Adherence to Web Content Accessibility Guidelines standards | Web Accessibility Standards | WCAG, Accessibility | "WCAG 2.1 AA compliance required" |
| **WCAG 2.1** | Version 2.1 of Web Content Accessibility Guidelines | Web Accessibility Guidelines | WCAG Compliance, Accessibility | "Following WCAG 2.1 success criteria" |
| **WCAG AA** | Level AA conformance of WCAG, the standard target for most applications | Level AA, Double-A | WCAG 2.1, WCAG Compliance | "WCAG AA compliance for government contracts" |
| **Keyboard Navigation** | The ability to operate the application using only keyboard input | Keyboard Access, Keyboard Support | Accessibility, Focus Management | "Tab key provides keyboard navigation" |
| **Full Keyboard Support** | Complete functionality accessible via keyboard without requiring mouse | Keyboard Accessibility | Keyboard Navigation, Accessibility | "Full keyboard support for all task operations" |
| **Screen Reader** | Assistive technology that reads screen content aloud for visually impaired users | Assistive Technology, TTS | Accessibility, ARIA | "Screen reader announces task completion" |
| **Screen Reader Optimized** | Designed and tested to work effectively with screen reader software | Screen Reader Compatible | Screen Reader, Accessibility | "Interface optimized for JAWS and NVDA" |
| **ARIA Labels** | Accessible Rich Internet Applications attributes providing additional context for assistive technologies | Accessibility Labels, WAI-ARIA | ARIA, Screen Reader, Accessibility | "ARIA labels describe icon button purposes" |
| **ARIA Roles** | Semantic information defining the purpose and type of UI elements | Accessibility Roles | ARIA, Accessibility | "ARIA role='button' identifies clickable element" |
| **High Contrast Mode** | Visual display mode with enhanced color contrast for users with visual impairments | High Contrast Theme, Accessibility Mode | Accessibility, Theme | "High contrast mode improves text visibility" |
| **Color Contrast Ratio** | The measurement of difference between text and background colors | Contrast Ratio, WCAG Contrast | High Contrast Mode, WCAG | "4.5:1 color contrast ratio for normal text" |
| **Focus Indicator** | Visual highlight showing which element currently has keyboard focus | Focus Ring, Focus Outline | Keyboard Navigation, Accessibility | "Blue focus indicator shows active element" |
| **Focus Management** | Controlling where keyboard focus moves as users navigate | Focus Control | Keyboard Navigation, Focus Indicator | "Focus management moves to modal when opened" |
| **Semantic HTML** | Using HTML elements for their intended purpose to convey meaning | Meaningful Markup, Accessible HTML | Accessibility, HTML | "Semantic HTML uses <button> not <div> for buttons" |

---

## Integration Domain

Terms related to connecting the ToDo app with external services and systems.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Integration** | Connection between the ToDo app and external services or applications | External Connection, System Integration | API, Webhook | "Calendar integration syncs tasks with events" |
| **Calendar Integration** | Synchronization between tasks and external calendar applications | Calendar Sync, Calendar Connection | Integration, Due Date | "Google Calendar integration shows task deadlines" |
| **Calendar Sync** | Automatic bidirectional updating between tasks and calendar events | Two-way Sync | Calendar Integration, Sync | "Calendar sync creates events from tasks" |
| **Email Notification** | Automated emails sent to users about task updates and reminders | Email Alert, Email Reminder | Notification, Email | "Email notification sent for overdue tasks" |
| **Notification Email** | An email containing information about task changes or events | Alert Email | Email Notification | "Notification email lists today's due tasks" |
| **Webhook** | A mechanism for sending real-time task data to external applications via HTTP callbacks | HTTP Callback, Web Callback | Integration, API | "Webhook triggers Slack message on task completion" |
| **Webhook Support** | The capability to configure and send webhooks to external URLs | Webhook Integration | Webhook, Integration | "Webhook support for CI/CD pipeline triggers" |
| **External Integration** | Connection to third-party services and applications | Third-party Integration | Integration, API | "External integration with Jira for issue tracking" |
| **REST API** | Application Programming Interface for external applications to interact with the ToDo system | RESTful API, Web API | API, Integration, Endpoint | "REST API enables mobile app development" |
| **API Endpoint** | A specific URL path in the REST API for performing operations | API Route, API Path | REST API, Integration | "API endpoint POST /api/tasks creates new task" |
| **Third-party Application** | External software that integrates with the ToDo system | External App, Integration Partner | Integration, REST API | "Third-party applications access via API" |
| **API Key** | Authentication credential for external applications to access the API | Access Token, API Token | REST API, Security | "API key required for external integrations" |
| **API Authentication** | Verification of identity for external applications using the API | API Auth, API Security | API Key, REST API, Security | "API authentication via Bearer tokens" |

---

## Mobile & Offline Domain

Terms related to mobile device support and offline functionality.

| Term | Definition | Synonyms | Related Terms | Usage Example |
|------|------------|----------|---------------|---------------|
| **Responsive Design** | Design approach ensuring the interface adapts to different screen sizes and devices | Mobile-First Design, Adaptive Design | Mobile, UI Design | "Responsive design works on phone and desktop" |
| **Mobile Experience** | The user interface and functionality optimized for mobile devices | Mobile UI, Mobile App | Responsive Design, Mobile | "Mobile experience prioritizes touch interactions" |
| **Touch Interface** | User interface designed for touchscreen interactions | Touch UI, Mobile Interface | Mobile Experience, Responsive Design | "Touch interface has larger tap targets" |
| **Progressive Web App** | A web application using modern capabilities to deliver app-like experiences | PWA, Web App | Mobile, Offline Mode | "PWA can be installed on home screen" |
| **PWA** | Abbreviation for Progressive Web App | Progressive Web App | Mobile, Offline Mode | "PWA features enable offline access" |
| **Installable** | The ability for users to add the PWA to their device home screen | Add to Home Screen, Install | PWA, Mobile | "Installable app launches like native application" |
| **Offline Mode** | Functionality allowing the app to work without internet connection | Offline Access, Offline Support | PWA, Sync | "Offline mode caches tasks for viewing" |
| **Offline Access** | The ability to view and interact with cached data without connectivity | Offline Availability | Offline Mode, Cache | "Offline access to recently viewed tasks" |
| **Offline Capability** | Features and operations available when not connected to internet | Offline Functionality | Offline Mode, Sync | "Offline capability includes task creation" |
| **Data Synchronization** | The process of updating local and server data when connectivity is restored | Sync, Data Sync | Offline Mode, Online Mode | "Data synchronization occurs when back online" |
| **Sync** | Short for synchronization, updating data between device and server | Synchronization, Data Sync | Offline Mode, Online Mode | "Sync completes in background after reconnection" |
| **Online Mode** | Standard operation when the app has internet connectivity | Connected Mode, Online | Offline Mode, Sync | "Online mode provides real-time collaboration" |
| **Sync Conflict** | Situation where local and server data differ and need resolution | Data Conflict, Merge Conflict | Sync, Offline Mode | "Sync conflict: task modified on two devices" |
| **Conflict Resolution** | The process of resolving differences between local and server data | Merge Resolution, Conflict Handling | Sync Conflict, Sync | "Conflict resolution: server version takes precedence" |
| **Cache** | Local storage of data for quick access and offline availability | Local Cache, Data Cache | Offline Mode, Performance | "Cache stores last 100 viewed tasks" |
| **Service Worker** | Background script enabling offline functionality and caching in PWA | SW, Background Worker | PWA, Offline Mode, Cache | "Service worker caches app assets for offline use" |

---

## Cross-Reference to Main Data Dictionary

This document focuses on ToDo app-specific terms. For general software development, architecture, and infrastructure terms, refer to the main DATA_DICTIONARY.md which covers:

- Common Development Terms (Entity, Service, Repository, DTO, etc.)
- Architecture Patterns (DDD, CQRS, SOLID, etc.)
- Frontend Technologies (React, Components, Hooks, State Management, etc.)
- Backend Technologies (.NET, Controllers, Commands, Queries, etc.)
- Data Storage (CosmosDB, SQL Server, Blob Storage, Queues, etc.)
- Testing & Quality (Unit Tests, Integration Tests, Accessibility Testing, etc.)
- Infrastructure (Azure, Bicep, Functions, Deployment, etc.)
- Project Management (User Story, Epic, Sprint, etc.)

---

## Versioning and Maintenance

This data dictionary should be updated when:
- New ToDo app features are designed or implemented
- Business terminology changes or evolves
- User feedback identifies unclear or ambiguous terms
- Integration with new external services introduces terminology
- Accessibility requirements expand or change

**Revision History:**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | Claude Code | Initial release based on ToDo App Product Brief terms |

---

## Usage Guidelines

1. **Consistency**: Use these exact terms in code, API endpoints, UI labels, and documentation
2. **Development**: Reference these terms when writing user stories and technical specifications
3. **Code Naming**: Use terms as basis for class names, method names, and variables
   - Example: `TaskPriority` enum, `calculateTaskProgress()` method, `recurringTask` variable
4. **API Design**: Use terms in endpoint paths and request/response models
   - Example: `/api/tasks/{id}/subtasks`, `TaskAssignment` DTO
5. **UI Labels**: Use terms (or their user-friendly variants) in interface labels
   - Example: "Priority" dropdown, "Due Date" field, "Recurring Task" checkbox
6. **Documentation**: Reference this dictionary in feature specifications and user guides
7. **Team Communication**: Use these terms in meetings, tickets, and code reviews

---

## Contributing

To propose changes or additions:

1. Create a feature branch from main
2. Update relevant domain section in this file
3. Add entry to Revision History
4. Submit pull request with justification
5. Obtain approval from product owner and technical lead

---

**End of ToDo App Terms Data Dictionary**
