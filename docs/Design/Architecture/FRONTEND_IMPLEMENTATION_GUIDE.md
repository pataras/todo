# Frontend Implementation Guide - Release 1

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** Design Phase

## Overview

This document provides detailed implementation guidance for the frontend of Release 1, including project setup, component development, state management, routing, and best practices.

---

## Table of Contents

1. [Project Setup](#project-setup)
2. [Project Structure](#project-structure)
3. [Configuration](#configuration)
4. [Shared Components](#shared-components)
5. [Authentication Feature](#authentication-feature)
6. [Routing & Navigation](#routing--navigation)
7. [State Management](#state-management)
8. [Error Handling](#error-handling)
9. [Testing](#testing)
10. [Best Practices](#best-practices)

---

## Project Setup

### Prerequisites

- Node.js 18+ and npm 9+
- Git
- VS Code (recommended) with extensions:
  - ESLint
  - Prettier
  - TypeScript and JavaScript Language Features

### Create Project

```bash
# Create Vite project with React + TypeScript
npm create vite@latest frontend -- --template react-ts

cd frontend

# Install dependencies
npm install

# Install UI and styling
npm install @mui/material @mui/icons-material @emotion/react @emotion/styled

# Install routing
npm install react-router-dom

# Install form management
npm install formik yup

# Install data fetching
npm install @tanstack/react-query axios

# Install dev dependencies
npm install -D @types/node
```

### Configure TypeScript

Update `tsconfig.json`:
```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,

    /* Bundler mode */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",

    /* Linting */
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,

    /* Path aliases */
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"],
      "@/features/*": ["src/features/*"],
      "@/shared/*": ["src/shared/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

### Configure Vite

Update `vite.config.ts`:
```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@/features': path.resolve(__dirname, './src/features'),
      '@/shared': path.resolve(__dirname, './src/shared'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:7001',
        changeOrigin: true,
      },
    },
  },
});
```

---

## Project Structure

```
frontend/
├── public/
│   └── vite.svg
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
│   │       │   ├── useAuth.ts
│   │       │   ├── useRegister.ts
│   │       │   ├── useLogin.ts
│   │       │   ├── useLogout.ts
│   │       │   ├── useVerifyEmail.ts
│   │       │   ├── useForgotPassword.ts
│   │       │   ├── useResetPassword.ts
│   │       │   └── useUserProfile.ts
│   │       ├── api/
│   │       │   ├── authApi.ts
│   │       │   └── userApi.ts
│   │       ├── types/
│   │       │   ├── auth.types.ts
│   │       │   └── user.types.ts
│   │       └── validation/
│   │           └── authSchemas.ts
│   ├── shared/
│   │   ├── components/
│   │   │   ├── FormInput.tsx
│   │   │   ├── PasswordInput.tsx
│   │   │   ├── LoadingButton.tsx
│   │   │   ├── ErrorAlert.tsx
│   │   │   ├── SuccessAlert.tsx
│   │   │   └── PageHeader.tsx
│   │   ├── hooks/
│   │   │   ├── useApiClient.ts
│   │   │   └── useErrorHandler.ts
│   │   ├── utils/
│   │   │   ├── validation.ts
│   │   │   ├── storage.ts
│   │   │   └── errorHandling.ts
│   │   ├── contexts/
│   │   │   └── AuthContext.tsx
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
│   ├── main.tsx
│   └── vite-env.d.ts
├── .env.development
├── .env.production
├── .eslintrc.json
├── .prettierrc
├── index.html
├── package.json
├── tsconfig.json
├── tsconfig.node.json
└── vite.config.ts
```

---

## Configuration

### Environment Variables

`.env.development`:
```env
VITE_API_BASE_URL=http://localhost:7001/v1
VITE_APP_NAME=ToDo App
VITE_ENABLE_LOGGING=true
```

`.env.production`:
```env
VITE_API_BASE_URL=https://api.todoapp.com/v1
VITE_APP_NAME=ToDo App
VITE_ENABLE_LOGGING=false
```

### Material-UI Theme

`src/theme/theme.ts`:
```typescript
import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
      light: '#42a5f5',
      dark: '#1565c0',
      contrastText: '#fff',
    },
    secondary: {
      main: '#9c27b0',
      light: '#ba68c8',
      dark: '#7b1fa2',
      contrastText: '#fff',
    },
    error: {
      main: '#d32f2f',
    },
    warning: {
      main: '#ed6c02',
    },
    info: {
      main: '#0288d1',
    },
    success: {
      main: '#2e7d32',
    },
    background: {
      default: '#f5f5f5',
      paper: '#ffffff',
    },
  },
  typography: {
    fontFamily: [
      '-apple-system',
      'BlinkMacSystemFont',
      '"Segoe UI"',
      'Roboto',
      '"Helvetica Neue"',
      'Arial',
      'sans-serif',
    ].join(','),
    h1: {
      fontSize: '2.5rem',
      fontWeight: 600,
    },
    h2: {
      fontSize: '2rem',
      fontWeight: 600,
    },
    h3: {
      fontSize: '1.75rem',
      fontWeight: 600,
    },
    h4: {
      fontSize: '1.5rem',
      fontWeight: 600,
    },
    h5: {
      fontSize: '1.25rem',
      fontWeight: 600,
    },
    h6: {
      fontSize: '1rem',
      fontWeight: 600,
    },
  },
  shape: {
    borderRadius: 8,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontWeight: 600,
        },
      },
    },
    MuiTextField: {
      defaultProps: {
        variant: 'outlined',
        fullWidth: true,
      },
    },
  },
});
```

---

## Shared Components

### FormInput Component

`src/shared/components/FormInput.tsx`:
```typescript
import React from 'react';
import { TextField, TextFieldProps } from '@mui/material';
import { useField } from 'formik';

type FormInputProps = {
  name: string;
  label: string;
} & Omit<TextFieldProps, 'name' | 'label' | 'error' | 'helperText'>;

export const FormInput: React.FC<FormInputProps> = ({ name, label, ...props }) => {
  const [field, meta] = useField(name);
  const showError = meta.touched && Boolean(meta.error);

  return (
    <TextField
      {...field}
      {...props}
      label={label}
      error={showError}
      helperText={showError ? meta.error : props.helperText}
      fullWidth
    />
  );
};
```

### PasswordInput Component

`src/shared/components/PasswordInput.tsx`:
```typescript
import React, { useState } from 'react';
import { IconButton, InputAdornment, TextField, TextFieldProps } from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useField } from 'formik';

type PasswordInputProps = {
  name: string;
  label: string;
} & Omit<TextFieldProps, 'name' | 'label' | 'error' | 'helperText' | 'type'>;

export const PasswordInput: React.FC<PasswordInputProps> = ({ name, label, ...props }) => {
  const [field, meta] = useField(name);
  const [showPassword, setShowPassword] = useState(false);
  const showError = meta.touched && Boolean(meta.error);

  const handleTogglePassword = () => {
    setShowPassword((prev) => !prev);
  };

  return (
    <TextField
      {...field}
      {...props}
      type={showPassword ? 'text' : 'password'}
      label={label}
      error={showError}
      helperText={showError ? meta.error : props.helperText}
      fullWidth
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <IconButton
              aria-label="toggle password visibility"
              onClick={handleTogglePassword}
              edge="end"
            >
              {showPassword ? <VisibilityOff /> : <Visibility />}
            </IconButton>
          </InputAdornment>
        ),
      }}
    />
  );
};
```

### LoadingButton Component

`src/shared/components/LoadingButton.tsx`:
```typescript
import React from 'react';
import { Button, ButtonProps, CircularProgress } from '@mui/material';

type LoadingButtonProps = {
  loading?: boolean;
  children: React.ReactNode;
} & ButtonProps;

export const LoadingButton: React.FC<LoadingButtonProps> = ({
  loading = false,
  disabled,
  children,
  ...props
}) => {
  return (
    <Button {...props} disabled={disabled || loading}>
      {loading && (
        <CircularProgress
          size={20}
          sx={{ position: 'absolute', left: '50%', marginLeft: '-10px' }}
        />
      )}
      <span style={{ visibility: loading ? 'hidden' : 'visible' }}>{children}</span>
    </Button>
  );
};
```

### ErrorAlert Component

`src/shared/components/ErrorAlert.tsx`:
```typescript
import React from 'react';
import { Alert, AlertProps } from '@mui/material';

type ErrorAlertProps = {
  message?: string;
  details?: string[];
} & Omit<AlertProps, 'severity'>;

export const ErrorAlert: React.FC<ErrorAlertProps> = ({ message, details, ...props }) => {
  if (!message && (!details || details.length === 0)) {
    return null;
  }

  return (
    <Alert severity="error" {...props}>
      {message && <div>{message}</div>}
      {details && details.length > 0 && (
        <ul style={{ margin: '8px 0 0 0', paddingLeft: '20px' }}>
          {details.map((detail, index) => (
            <li key={index}>{detail}</li>
          ))}
        </ul>
      )}
    </Alert>
  );
};
```

### SuccessAlert Component

`src/shared/components/SuccessAlert.tsx`:
```typescript
import React from 'react';
import { Alert, AlertProps } from '@mui/material';

type SuccessAlertProps = {
  message: string;
} & Omit<AlertProps, 'severity' | 'children'>;

export const SuccessAlert: React.FC<SuccessAlertProps> = ({ message, ...props }) => {
  return (
    <Alert severity="success" {...props}>
      {message}
    </Alert>
  );
};
```

---

## Authentication Feature

### Types

`src/features/auth/types/auth.types.ts`:
```typescript
export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  fullName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface VerifyEmailRequest {
  token: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AuthResponse {
  success: boolean;
  data: {
    user: User;
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
  };
  message: string;
}

export interface User {
  userId: string;
  email: string;
  fullName: string;
  role: string;
  isEmailVerified: boolean;
}

export interface ApiError {
  success: false;
  error: {
    code: string;
    message: string;
    details?: Array<{
      field: string;
      message: string;
    }>;
  };
}
```

### Validation Schemas

`src/features/auth/validation/authSchemas.ts`:
```typescript
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

### API Client

`src/features/auth/api/authApi.ts`:
```typescript
import axios from 'axios';
import type {
  RegisterRequest,
  LoginRequest,
  AuthResponse,
  VerifyEmailRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
} from '../types/auth.types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authApi = {
  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await apiClient.post('/auth/register', data);
    return response.data;
  },

  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await apiClient.post('/auth/login', data);
    return response.data;
  },

  logout: async (): Promise<void> => {
    await apiClient.post('/auth/logout');
  },

  verifyEmail: async (data: VerifyEmailRequest): Promise<AuthResponse> => {
    const response = await apiClient.post('/auth/verify-email', data);
    return response.data;
  },

  resendVerification: async (email: string): Promise<void> => {
    await apiClient.post('/auth/resend-verification', { email });
  },

  forgotPassword: async (data: ForgotPasswordRequest): Promise<void> => {
    await apiClient.post('/auth/forgot-password', data);
  },

  resetPassword: async (data: ResetPasswordRequest): Promise<void> => {
    await apiClient.post('/auth/reset-password', data);
  },

  refreshToken: async (): Promise<AuthResponse> => {
    const response = await apiClient.post('/auth/refresh');
    return response.data;
  },
};
```

### Custom Hooks

`src/features/auth/hooks/useRegister.ts`:
```typescript
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import type { RegisterRequest } from '../types/auth.types';

export const useRegister = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (data: RegisterRequest) => authApi.register(data),
    onSuccess: () => {
      navigate('/verify-email-sent');
    },
  });
};
```

`src/features/auth/hooks/useLogin.ts`:
```typescript
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './useAuth';
import { authApi } from '../api/authApi';
import type { LoginRequest } from '../types/auth.types';

export const useLogin = () => {
  const navigate = useNavigate();
  const { setUser } = useAuth();

  return useMutation({
    mutationFn: (data: LoginRequest) => authApi.login(data),
    onSuccess: (response) => {
      setUser(response.data.user);
      navigate('/dashboard');
    },
  });
};
```

`src/features/auth/hooks/useLogout.ts`:
```typescript
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './useAuth';
import { authApi } from '../api/authApi';

export const useLogout = () => {
  const navigate = useNavigate();
  const { setUser } = useAuth();

  return useMutation({
    mutationFn: () => authApi.logout(),
    onSuccess: () => {
      setUser(null);
      navigate('/login');
    },
  });
};
```

### Register Form Component

`src/features/auth/components/RegisterForm.tsx`:
```typescript
import React from 'react';
import { Formik, Form } from 'formik';
import { Box, Typography, Link as MuiLink, Paper } from '@mui/material';
import { Link } from 'react-router-dom';
import { FormInput } from '@/shared/components/FormInput';
import { PasswordInput } from '@/shared/components/PasswordInput';
import { LoadingButton } from '@/shared/components/LoadingButton';
import { ErrorAlert } from '@/shared/components/ErrorAlert';
import { useRegister } from '../hooks/useRegister';
import { registerSchema } from '../validation/authSchemas';
import type { RegisterRequest } from '../types/auth.types';

export const RegisterForm: React.FC = () => {
  const { mutate: register, isPending, isError, error } = useRegister();

  const handleSubmit = (values: RegisterRequest) => {
    register(values);
  };

  const initialValues: RegisterRequest = {
    email: '',
    password: '',
    confirmPassword: '',
    fullName: '',
  };

  return (
    <Paper elevation={3} sx={{ p: 4, maxWidth: 500, mx: 'auto', mt: 8 }}>
      <Typography variant="h4" component="h1" gutterBottom align="center">
        Create Account
      </Typography>
      <Typography variant="body2" color="text.secondary" align="center" mb={3}>
        Sign up to get started with ToDo App
      </Typography>

      {isError && (
        <ErrorAlert
          message={error?.response?.data?.error?.message || 'Registration failed'}
          details={error?.response?.data?.error?.details?.map((d: any) => d.message)}
          sx={{ mb: 2 }}
        />
      )}

      <Formik
        initialValues={initialValues}
        validationSchema={registerSchema}
        onSubmit={handleSubmit}
      >
        {() => (
          <Form>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <FormInput
                name="fullName"
                label="Full Name"
                autoComplete="name"
                autoFocus
              />

              <FormInput
                name="email"
                label="Email Address"
                type="email"
                autoComplete="email"
              />

              <PasswordInput
                name="password"
                label="Password"
                autoComplete="new-password"
              />

              <PasswordInput
                name="confirmPassword"
                label="Confirm Password"
                autoComplete="new-password"
              />

              <LoadingButton
                type="submit"
                variant="contained"
                size="large"
                loading={isPending}
                fullWidth
              >
                Create Account
              </LoadingButton>

              <Box sx={{ textAlign: 'center', mt: 2 }}>
                <Typography variant="body2">
                  Already have an account?{' '}
                  <MuiLink component={Link} to="/login">
                    Sign in
                  </MuiLink>
                </Typography>
              </Box>
            </Box>
          </Form>
        )}
      </Formik>
    </Paper>
  );
};
```

### Login Form Component

`src/features/auth/components/LoginForm.tsx`:
```typescript
import React from 'react';
import { Formik, Form, Field } from 'formik';
import {
  Box,
  Typography,
  Link as MuiLink,
  Paper,
  FormControlLabel,
  Checkbox,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { FormInput } from '@/shared/components/FormInput';
import { PasswordInput } from '@/shared/components/PasswordInput';
import { LoadingButton } from '@/shared/components/LoadingButton';
import { ErrorAlert } from '@/shared/components/ErrorAlert';
import { useLogin } from '../hooks/useLogin';
import { loginSchema } from '../validation/authSchemas';
import type { LoginRequest } from '../types/auth.types';

export const LoginForm: React.FC = () => {
  const { mutate: login, isPending, isError, error } = useLogin();

  const handleSubmit = (values: LoginRequest) => {
    login(values);
  };

  const initialValues: LoginRequest = {
    email: '',
    password: '',
    rememberMe: false,
  };

  return (
    <Paper elevation={3} sx={{ p: 4, maxWidth: 500, mx: 'auto', mt: 8 }}>
      <Typography variant="h4" component="h1" gutterBottom align="center">
        Sign In
      </Typography>
      <Typography variant="body2" color="text.secondary" align="center" mb={3}>
        Welcome back! Please sign in to continue.
      </Typography>

      {isError && (
        <ErrorAlert
          message={error?.response?.data?.error?.message || 'Login failed'}
          sx={{ mb: 2 }}
        />
      )}

      <Formik
        initialValues={initialValues}
        validationSchema={loginSchema}
        onSubmit={handleSubmit}
      >
        {({ values, setFieldValue }) => (
          <Form>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              <FormInput
                name="email"
                label="Email Address"
                type="email"
                autoComplete="email"
                autoFocus
              />

              <PasswordInput
                name="password"
                label="Password"
                autoComplete="current-password"
              />

              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={values.rememberMe}
                      onChange={(e) => setFieldValue('rememberMe', e.target.checked)}
                    />
                  }
                  label="Remember me"
                />
                <MuiLink component={Link} to="/forgot-password" variant="body2">
                  Forgot password?
                </MuiLink>
              </Box>

              <LoadingButton
                type="submit"
                variant="contained"
                size="large"
                loading={isPending}
                fullWidth
              >
                Sign In
              </LoadingButton>

              <Box sx={{ textAlign: 'center', mt: 2 }}>
                <Typography variant="body2">
                  Don't have an account?{' '}
                  <MuiLink component={Link} to="/register">
                    Sign up
                  </MuiLink>
                </Typography>
              </Box>
            </Box>
          </Form>
        )}
      </Formik>
    </Paper>
  );
};
```

---

## Routing & Navigation

### Auth Context

`src/shared/contexts/AuthContext.tsx`:
```typescript
import React, { createContext, useContext, useState, useEffect } from 'react';
import type { User } from '@/features/auth/types/auth.types';

interface AuthContextType {
  user: User | null;
  setUser: (user: User | null) => void;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check if user is authenticated on mount
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
    setIsLoading(false);
  }, []);

  useEffect(() => {
    // Persist user to localStorage
    if (user) {
      localStorage.setItem('user', JSON.stringify(user));
    } else {
      localStorage.removeItem('user');
    }
  }, [user]);

  const value = {
    user,
    setUser,
    isAuthenticated: !!user,
    isLoading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuthContext = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuthContext must be used within AuthProvider');
  }
  return context;
};
```

### Private Route

`src/routes/PrivateRoute.tsx`:
```typescript
import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuthContext } from '@/shared/contexts/AuthContext';
import { Box, CircularProgress } from '@mui/material';

interface PrivateRouteProps {
  children: React.ReactNode;
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuthContext();
  const location = useLocation();

  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '100vh',
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
};
```

### App Routes

`src/routes/AppRoutes.tsx`:
```typescript
import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { RegisterForm } from '@/features/auth/components/RegisterForm';
import { LoginForm } from '@/features/auth/components/LoginForm';
import { EmailVerificationPage } from '@/features/auth/components/EmailVerificationPage';
import { ForgotPasswordForm } from '@/features/auth/components/ForgotPasswordForm';
import { ResetPasswordForm } from '@/features/auth/components/ResetPasswordForm';
import { UserProfilePage } from '@/features/auth/components/UserProfilePage';
import { PrivateRoute } from './PrivateRoute';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/register" element={<RegisterForm />} />
      <Route path="/login" element={<LoginForm />} />
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
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};
```

---

## State Management

### TanStack Query Setup

`src/main.tsx`:
```typescript
import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AuthProvider } from '@/shared/contexts/AuthContext';
import { theme } from './theme/theme';
import App from './App';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <AuthProvider>
            <App />
          </AuthProvider>
        </ThemeProvider>
      </BrowserRouter>
    </QueryClientProvider>
  </React.StrictMode>
);
```

---

## Best Practices

### 1. Component Design
- Use functional components exclusively
- Keep components small and focused
- Extract reusable logic into custom hooks
- Use TypeScript for type safety

### 2. State Management
- Use TanStack Query for server state
- Use Context API for global client state
- Use local state (useState) for component-specific state

### 3. Form Handling
- Use Formik for form management
- Use Yup for validation schemas
- Extract validation schemas into separate files
- Display field-level and form-level errors

### 4. API Communication
- Centralize API calls in api files
- Use Axios interceptors for auth and error handling
- Handle loading and error states consistently
- Use proper TypeScript types for requests/responses

### 5. Error Handling
- Display user-friendly error messages
- Log errors for debugging
- Implement global error boundary
- Handle network errors gracefully

### 6. Accessibility
- Use semantic HTML elements
- Add ARIA labels where needed
- Ensure keyboard navigation works
- Test with screen readers
- Maintain proper focus management

### 7. Performance
- Use React.memo for expensive components
- Implement code splitting with lazy loading
- Optimize bundle size
- Use production builds for deployment

---

## Next Steps

1. Set up project with dependencies
2. Implement shared components
3. Create authentication feature components
4. Set up routing and navigation
5. Implement state management
6. Add error handling
7. Write unit tests
8. Perform accessibility testing

---

## References

- [React Documentation](https://react.dev/)
- [Material-UI Documentation](https://mui.com/)
- [TanStack Query Documentation](https://tanstack.com/query)
- [Formik Documentation](https://formik.org/)
- [TypeScript Documentation](https://www.typescriptlang.org/)
