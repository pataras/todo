# Using Claude for User Story Generation

## Overview
Claude can assist with generating comprehensive user stories from product briefs. This document outlines best practices for using Claude in the development process.

## General Approach

### Ask Questions, Don't Assume
When requirements are unclear or ambiguous, Claude should:
- **Ask clarifying questions** rather than making assumptions
- **Identify gaps** in provided information upfront
- **Confirm understanding** before proceeding with implementation
- **Surface unknowns** explicitly for decision-making

### Create Concise Plans
Before executing tasks, Claude should:
- **Present a brief, clear plan** outlining the approach
- **Break down complex tasks** into logical steps
- **Identify dependencies** and potential blockers
- **Get approval** before proceeding with significant changes

### Iterative Collaboration
- Start with questions to clarify requirements
- Present a concise plan for feedback
- Execute incrementally with validation points
- Adjust based on feedback and new information

## Prerequisites

### Required Context
Before generating user stories, provide Claude with:
1. **Product Brief** - Feature requirements and scope
2. **Data Dictionary** - Domain terminology and definitions
3. **Technical Stack** - Technology choices and constraints
4. **Existing Patterns** - Any established user story formats or conventions

## Effective Prompts

### Before Starting: Ask Questions
```
I need help generating user stories for [Epic Name].

Before I begin, I have questions:
1. What priority framework should I use? (MoSCoW, High/Medium/Low, etc.)
2. Should I include story points? If so, what scale? (Fibonacci, T-shirt sizes, etc.)
3. What acceptance criteria format do you prefer?
4. Are there any specific compliance requirements? (WCAG level, security standards, etc.)
5. What level of technical detail is needed in implementation notes?
```

### Present a Plan First
```
Based on the brief, I'll generate user stories covering:
1. User authentication (registration, login, MFA) - ~5-7 stories
2. Profile management (view, edit, preferences) - ~3-4 stories
3. Role-based access control - ~4-5 stories
4. Account security features - ~3-4 stories

This will result in approximately 15-20 user stories organized into 3 releases.

Does this scope align with your expectations? Any areas to add, remove, or emphasize?
```

### Initial User Story Generation
```
Generate comprehensive user stories for [Epic Name] based on the provided brief.

Context:
- Brief: [path to brief or content]
- Data Dictionary: [path or reference]
- Technical Stack: React, .NET, Azure CosmosDB

Requirements:
- Use standard format: As a [role], I want to [action], so that [benefit]
- Include priority, story points, dependencies
- Write detailed acceptance criteria
- Add technical implementation notes
- Group into logical releases
```

### Refinement Prompts
```
Review these user stories and:
- Break down stories >8 points
- Add accessibility requirements
- Identify missing edge cases
- Check consistency with data dictionary

Questions before refining:
- Any specific areas of concern to focus on?
- Should I maintain current story IDs or renumber after splitting?
```

## Best Practices

### 1. Start with Questions and Planning
- **Ask first** - Clarify unknowns before generating stories
- **Create a brief plan** - Outline scope and approach for approval
- **Identify assumptions** - Surface any assumptions explicitly
- **Get alignment** - Confirm the plan matches expectations

### 2. Iterative Refinement
- Generate initial draft of stories based on confirmed plan
- Review for completeness and accuracy
- Request specific refinements
- Validate against brief requirements

### 3. Maintain Consistency
- Reference data dictionary for terminology
- Follow established story format
- Use consistent acceptance criteria structure
- Maintain uniform priority/sizing approach
- **Ask when uncertain** about terminology or conventions

### 4. Technical Alignment
- Specify technical stack constraints
- Request implementation notes
- Identify integration points
- Flag technical dependencies
- **Question technical feasibility** when uncertain

### 5. Quality Checks
- Verify all acceptance criteria are testable
- Ensure coverage of edge cases and errors
- Check accessibility considerations
- Validate security requirements for sensitive features
- **Raise concerns** rather than making assumptions

## Example Workflow

### Step 1: Prepare Context
```
I need to generate user stories for the User Management epic.
Here's the brief: [paste or reference]
Here's our data dictionary: [paste or reference]
We're using React, .NET, Azure CosmosDB, and following WCAG 2.1 AA standards.
```

### Step 2: Claude Asks Questions
```
Before I start generating stories, I have some questions:
1. Should I include passwordless authentication options, or focus on traditional password-based auth?
2. What priority framework should I use for these stories?
3. Are there any specific regulatory requirements (GDPR, HIPAA, etc.)?
4. Should admin features be in a separate epic or included here?
5. What's the expected timeline - is this a 2-week sprint or 3-month project?
```

### Step 3: Claude Presents Plan
```
Based on your answers, here's my plan:

**Scope:**
- User registration and authentication (traditional + MFA) - ~6 stories
- Profile management (view, edit, delete account) - ~4 stories
- Role-based access control (user/admin roles) - ~5 stories
- Account security (password reset, session management) - ~3 stories

**Releases:**
- Release 1 (MVP): Basic registration, login, profile viewing
- Release 2: MFA, password reset, profile editing
- Release 3: RBAC, admin controls
- Release 4: Advanced security features

**Total:** ~18 stories, 3-4 releases

Does this align with your vision? Any adjustments needed?
```

### Step 4: Generate Stories
```
[After getting approval]
Generate user stories covering:
- User registration and authentication
- Profile management
- Role-based access control
- Account security features

Include release planning with MVP and enhancement phases.
```

### Step 5: Review and Refine
```
Review the generated stories for:
1. Any story >8 points that should be broken down
2. Missing MFA/security features
3. Admin capabilities for user management
4. Bulk operations for administrators
```

### Step 6: Validate Output
- Check against brief requirements
- Ensure technical feasibility
- Verify story points are reasonable
- Confirm dependencies are accurate

## Output Format

### Expected Structure
```markdown
# [Epic Name] - User Stories

## Overview
[Epic description]

## [Category Name]

### US-[ID]: [Story Title]
As a [role]
I want to [action]
So that [benefit]

Priority: [High/Medium/Low]
Story Points: [1-13]
Dependencies: [US-IDs]

#### Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2

#### Technical Notes
- Implementation guidance
- Integration points
```

## Common Pitfalls to Avoid

1. **Making Assumptions** - Claude should ask questions, not guess at requirements
2. **Skipping the Plan** - Always present a brief plan before executing
3. **Too Generic** - Request specific acceptance criteria, not vague statements
4. **Missing Context** - Always provide brief and technical stack
5. **Inconsistent Format** - Specify format upfront for consistency
6. **No Dependencies** - Ask Claude to identify story dependencies
7. **Oversized Stories** - Request breakdown of large stories (>8 points)
8. **Silent Uncertainties** - Surface unknowns explicitly instead of proceeding blindly

## Advanced Techniques

### Story Mapping
```
Create a story map organizing these user stories into releases:
- Release 1: MVP with core authentication
- Release 2: Profile and security enhancements
- Release 3: RBAC and admin features
- Release 4: Advanced features

Estimate effort per release.
```

### Technical Specifications
```
For story US-UM-008 (MFA Setup), generate:
- API endpoint specifications
- Database schema requirements
- Integration dependencies
- Security considerations
```

### Acceptance Criteria Expansion
```
Expand acceptance criteria for [story ID] to include:
- Error handling scenarios
- Loading states
- Accessibility requirements
- Mobile responsiveness
```

## Integration with Development Process

### After User Story Generation
1. **Review** - Team reviews generated stories
2. **Refine** - Use Claude to address feedback
3. **Estimate** - Validate story points with team
4. **Plan** - Organize into sprints
5. **Implement** - Follow Definition of Done

### Continuous Improvement
- Document effective prompts
- Refine story templates based on what works
- Build context library for future generations
- Update Claude guidelines as process evolves

## Tips for Success

1. **Start with Questions** - Expect Claude to ask clarifying questions before proceeding
2. **Review the Plan** - Claude should present a concise plan for your approval
3. **Be Specific** - Detailed prompts yield better results
4. **Provide Examples** - Show Claude existing stories as templates
5. **Iterate** - Generate, review, refine in multiple passes
6. **Validate** - Always review AI-generated content for accuracy
7. **Maintain Control** - Claude assists; humans decide
8. **Encourage Questions** - Reward Claude for asking rather than assuming
9. **Keep Plans Concise** - Plans should be brief summaries, not exhaustive details

## Limitations

While Claude is powerful, remember:
- Claude should ask questions when uncertain, not make assumptions
- Always review technical feasibility with engineering team
- Validate story points through team estimation
- Ensure acceptance criteria match actual requirements
- Confirm technical notes align with architecture decisions
- Have product owner approve business requirements
- Plans are starting points for discussion, not final decisions

## Resources

- Main process documentation: `docs/process.md`
- Example user stories: `docs/Design/UserStories/USER_MANAGEMENT.md`
- Data dictionary: `DATA_DICTIONARY.md`, `docs/Ideas/TERMS_DATA_DICTIONARY.md`
- Product brief: `docs/Ideas/todo-app-brief.md`
