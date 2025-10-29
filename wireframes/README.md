# User Story Wireframes - ToDo Application

Interactive wireframes for ToDo Application user stories built with React, TypeScript, and Material-UI 7.

## Overview

This application provides interactive wireframes for **Release 1: Core Authentication (MVP)** user stories, including:

- **US-UM-001**: User Account Registration
- **US-UM-002**: Email Verification
- **US-UM-004**: User Login
- **US-UM-006**: Password Reset Request
- **US-UM-007**: Password Reset Completion
- **US-UM-010**: View User Profile

## Features

- **Story Navigation**: Browse all user stories via side drawer
- **Step-by-Step Flow**: Progress through each user story step with visual indicators
- **Interactive Forms**: Working forms with validation and error states
- **Story Context**: View acceptance criteria, technical notes, and story metadata
- **State Management**: Navigate between steps using Previous/Next/Reset controls
- **Responsive Design**: Material-UI 7 components with professional styling

## Getting Started

### Prerequisites

- Node.js 18+ installed
- npm or yarn package manager

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

Open your browser to the URL shown in the terminal (typically `http://localhost:5173`)

### Build

```bash
npm run build
```

## Usage

1. **Select a Story**: Click the menu icon (☰) in the top-left to open the story navigation drawer
2. **View Story Details**: See the story description, priority, and story points in the header
3. **Navigate Steps**: Use the stepper controls to move through the user story flow
4. **Interact with Wireframes**: Fill out forms and click buttons to simulate the user journey
5. **Review Requirements**: Scroll down to see acceptance criteria and technical notes

## Project Structure

```
wireframes/
├── src/
│   ├── components/           # Wireframe components for each user story
│   │   ├── RegistrationWireframe.tsx
│   │   ├── EmailVerificationWireframe.tsx
│   │   ├── LoginWireframe.tsx
│   │   ├── PasswordResetRequestWireframe.tsx
│   │   ├── PasswordResetCompletionWireframe.tsx
│   │   └── UserProfileWireframe.tsx
│   ├── types.ts             # TypeScript type definitions
│   ├── mockData.ts          # Mock data for stories and users
│   ├── App.tsx              # Main application with navigation and state control
│   └── main.tsx             # Application entry point
├── WIREFRAME_PROCESS.md     # Documentation of the wireframing process
└── README.md                # This file
```

## Technology Stack

- **React 18+** - UI framework
- **TypeScript** - Type safety
- **Material-UI 7** - Component library
- **Vite** - Build tool and dev server
- **Emotion** - Styling solution

## Process Documentation

See [WIREFRAME_PROCESS.md](./WIREFRAME_PROCESS.md) for detailed documentation on the user story to wireframe conversion process.

## Contributing

To add new user story wireframes:

1. Add story data to `src/mockData.ts`
2. Create wireframe component in `src/components/`
3. Register component in `wireframeComponents` object in `src/App.tsx`
4. Follow the existing component pattern with `currentStep` and `onStepChange` props

## License

This is a wireframe prototype for the ToDo application project.
