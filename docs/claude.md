# Using Claude for User Story Generation

## Overview
Claude can assist with generating comprehensive user stories from product briefs. This document outlines best practices for using Claude in the development process.

## Prerequisites

### Required Context
Before generating user stories, provide Claude with:
1. **Product Brief** - Feature requirements and scope
2. **Data Dictionary** - Domain terminology and definitions
3. **Technical Stack** - Technology choices and constraints
4. **Existing Patterns** - Any established user story formats or conventions

## Effective Prompts

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
```

## Best Practices

### 1. Iterative Refinement
- Generate initial draft of stories
- Review for completeness and accuracy
- Request specific refinements
- Validate against brief requirements

### 2. Maintain Consistency
- Reference data dictionary for terminology
- Follow established story format
- Use consistent acceptance criteria structure
- Maintain uniform priority/sizing approach

### 3. Technical Alignment
- Specify technical stack constraints
- Request implementation notes
- Identify integration points
- Flag technical dependencies

### 4. Quality Checks
- Verify all acceptance criteria are testable
- Ensure coverage of edge cases and errors
- Check accessibility considerations
- Validate security requirements for sensitive features

## Example Workflow

### Step 1: Prepare Context
```
I need to generate user stories for the User Management epic.
Here's the brief: [paste or reference]
Here's our data dictionary: [paste or reference]
We're using React, .NET, Azure CosmosDB, and following WCAG 2.1 AA standards.
```

### Step 2: Generate Stories
```
Generate user stories covering:
- User registration and authentication
- Profile management
- Role-based access control
- Account security features

Include release planning with MVP and enhancement phases.
```

### Step 3: Review and Refine
```
Review the generated stories for:
1. Any story >8 points that should be broken down
2. Missing MFA/security features
3. Admin capabilities for user management
4. Bulk operations for administrators
```

### Step 4: Validate Output
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

1. **Too Generic** - Request specific acceptance criteria, not vague statements
2. **Missing Context** - Always provide brief and technical stack
3. **Inconsistent Format** - Specify format upfront for consistency
4. **No Dependencies** - Ask Claude to identify story dependencies
5. **Oversized Stories** - Request breakdown of large stories (>8 points)

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

1. **Be Specific** - Detailed prompts yield better results
2. **Provide Examples** - Show Claude existing stories as templates
3. **Iterate** - Generate, review, refine in multiple passes
4. **Validate** - Always review AI-generated content for accuracy
5. **Maintain Control** - Claude assists; humans decide

## Limitations

While Claude is powerful, remember:
- Always review technical feasibility with engineering team
- Validate story points through team estimation
- Ensure acceptance criteria match actual requirements
- Confirm technical notes align with architecture decisions
- Have product owner approve business requirements

## Resources

- Main process documentation: `docs/process.md`
- Example user stories: `docs/Design/UserStories/USER_MANAGEMENT.md`
- Data dictionary: `DATA_DICTIONARY.md`, `docs/Ideas/TERMS_DATA_DICTIONARY.md`
- Product brief: `docs/Ideas/todo-app-brief.md`
