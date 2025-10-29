import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Alert,
  Link,
  LinearProgress,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function RegistrationWireframe({ currentStep, onStepChange }: Props) {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    fullName: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [field]: e.target.value });
    if (errors[field]) {
      setErrors({ ...errors, [field]: '' });
    }
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.email.includes('@')) {
      newErrors.email = 'Please enter a valid email address';
    }
    if (formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }
    if (!formData.fullName) {
      newErrors.fullName = 'Full name is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (currentStep === 0) {
      onStepChange(1);
    } else if (currentStep === 1) {
      if (validateForm()) {
        onStepChange(2);
      }
    } else if (currentStep === 2) {
      onStepChange(3);
    }
  };

  return (
    <Box sx={{ maxWidth: 500, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Create Account
        </Typography>

        <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
          Join us to start managing your tasks
        </Typography>

        {/* Step 1: Registration Form */}
        {currentStep === 0 && (
          <Box component="form" onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Full Name"
              value={formData.fullName}
              onChange={handleChange('fullName')}
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Email"
              type="email"
              value={formData.email}
              onChange={handleChange('email')}
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Password"
              type="password"
              value={formData.password}
              onChange={handleChange('password')}
              margin="normal"
              required
              helperText="Min 8 characters, uppercase, lowercase, number, special character"
            />
            <TextField
              fullWidth
              label="Confirm Password"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange('confirmPassword')}
              margin="normal"
              required
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              size="large"
              sx={{ mt: 3 }}
            >
              Continue
            </Button>
            <Typography variant="body2" align="center" sx={{ mt: 2 }}>
              Already have an account?{' '}
              <Link href="#" underline="hover">
                Sign in
              </Link>
            </Typography>
          </Box>
        )}

        {/* Step 2: Form Validation */}
        {currentStep === 1 && (
          <Box>
            <Alert severity="info" sx={{ mb: 2 }}>
              Validating your information...
            </Alert>
            <LinearProgress sx={{ mb: 2 }} />
            {Object.keys(errors).length > 0 ? (
              <Box sx={{ mt: 2 }}>
                <Alert severity="error" icon={<ErrorIcon />} sx={{ mb: 2 }}>
                  Please correct the following errors:
                </Alert>
                {Object.entries(errors).map(([field, error]) => (
                  <Typography key={field} color="error" variant="body2" sx={{ mb: 1 }}>
                    • {error}
                  </Typography>
                ))}
                <Button
                  fullWidth
                  variant="outlined"
                  onClick={() => onStepChange(0)}
                  sx={{ mt: 2 }}
                >
                  Go Back
                </Button>
              </Box>
            ) : (
              <Button
                fullWidth
                variant="contained"
                onClick={handleSubmit}
                sx={{ mt: 2 }}
              >
                Complete Registration
              </Button>
            )}
          </Box>
        )}

        {/* Step 3: Success Message */}
        {currentStep === 2 && (
          <Box textAlign="center">
            <CheckCircleIcon sx={{ fontSize: 80, color: 'success.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Registration Successful!
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              We've sent a verification email to <strong>{formData.email}</strong>
            </Typography>
            <Alert severity="info" sx={{ mb: 2 }}>
              Please check your email and click the verification link to activate your account.
            </Alert>
            <Button
              fullWidth
              variant="contained"
              onClick={handleSubmit}
              sx={{ mt: 2 }}
            >
              Continue
            </Button>
          </Box>
        )}

        {/* Step 4: Redirect to Login */}
        {currentStep === 3 && (
          <Box textAlign="center">
            <Typography variant="h6" gutterBottom>
              Redirecting to Login...
            </Typography>
            <LinearProgress sx={{ mt: 2 }} />
            <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
              You can now sign in with your credentials
            </Typography>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
