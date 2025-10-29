import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Alert,
  LinearProgress,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import LockResetIcon from '@mui/icons-material/LockReset';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function PasswordResetCompletionWireframe({ currentStep, onStepChange }: Props) {
  const [formData, setFormData] = useState({
    password: '',
    confirmPassword: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [field]: e.target.value });
    if (errors[field]) {
      setErrors({ ...errors, [field]: '' });
    }
  };

  const validatePassword = () => {
    const newErrors: Record<string, string> = {};

    if (formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (currentStep === 1 && validatePassword()) {
      onStepChange(2);
    }
  };

  return (
    <Box sx={{ maxWidth: 500, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Box textAlign="center" sx={{ mb: 3 }}>
          <LockResetIcon sx={{ fontSize: 60, color: 'primary.main', mb: 1 }} />
          <Typography variant="h4" component="h1" gutterBottom>
            Reset Your Password
          </Typography>
        </Box>

        {/* Step 1: Token Validation */}
        {currentStep === 0 && (
          <Box textAlign="center">
            <Alert severity="info" sx={{ mb: 2 }}>
              Validating reset token...
            </Alert>
            <LinearProgress sx={{ mb: 3 }} />
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Please wait while we verify your reset link.
            </Typography>
            <Button
              fullWidth
              variant="contained"
              onClick={() => onStepChange(1)}
            >
              Token Valid - Continue
            </Button>
          </Box>
        )}

        {/* Step 2: New Password Form */}
        {currentStep === 1 && (
          <Box component="form" onSubmit={handleSubmit}>
            <Alert severity="success" sx={{ mb: 2 }}>
              Reset token validated successfully!
            </Alert>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Please enter your new password.
            </Typography>
            <TextField
              fullWidth
              label="New Password"
              type="password"
              value={formData.password}
              onChange={handleChange('password')}
              error={!!errors.password}
              helperText={errors.password || 'Min 8 characters, uppercase, lowercase, number, special character'}
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="Confirm New Password"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange('confirmPassword')}
              error={!!errors.confirmPassword}
              helperText={errors.confirmPassword}
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
              Reset Password
            </Button>
          </Box>
        )}

        {/* Step 3: Success & Redirect */}
        {currentStep === 2 && (
          <Box textAlign="center">
            <CheckCircleIcon sx={{ fontSize: 80, color: 'success.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Password Reset Successful!
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              Your password has been updated successfully.
            </Typography>
            <Alert severity="success" sx={{ mb: 2 }}>
              All existing sessions have been terminated for security.
            </Alert>
            <Alert severity="info" sx={{ mb: 3 }}>
              You will receive an email confirmation of this password change.
            </Alert>
            <LinearProgress sx={{ mb: 2 }} />
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Redirecting to login page...
            </Typography>
            <Button fullWidth variant="contained">
              Go to Login
            </Button>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
