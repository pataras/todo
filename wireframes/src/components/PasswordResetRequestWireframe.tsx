import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Alert,
  Link,
} from '@mui/material';
import EmailIcon from '@mui/icons-material/Email';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function PasswordResetRequestWireframe({ currentStep, onStepChange }: Props) {
  const [email, setEmail] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onStepChange(currentStep + 1);
  };

  return (
    <Box sx={{ maxWidth: 500, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Reset Password
        </Typography>

        <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
          Enter your email address and we'll send you a link to reset your password.
        </Typography>

        {/* Step 1: Reset Request Form */}
        {currentStep === 0 && (
          <Box component="form" onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Email Address"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              margin="normal"
              required
              helperText="We'll send a password reset link to this email"
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              size="large"
              sx={{ mt: 3 }}
            >
              Send Reset Link
            </Button>
            <Button
              fullWidth
              variant="text"
              startIcon={<ArrowBackIcon />}
              sx={{ mt: 1 }}
            >
              Back to Sign In
            </Button>
          </Box>
        )}

        {/* Step 2: Confirmation Message */}
        {currentStep === 1 && (
          <Box textAlign="center">
            <EmailIcon sx={{ fontSize: 80, color: 'primary.main', mb: 2 }} />
            <Typography variant="h6" gutterBottom>
              Check Your Email
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              If an account exists for <strong>{email}</strong>, we've sent a password reset link.
            </Typography>
            <Alert severity="info" sx={{ mb: 2 }}>
              For security reasons, we don't reveal whether an account exists with this email.
            </Alert>
            <Alert severity="warning" sx={{ mb: 2 }}>
              The reset link will expire in 1 hour.
            </Alert>
            <Button
              fullWidth
              variant="contained"
              onClick={() => onStepChange(2)}
            >
              Continue
            </Button>
          </Box>
        )}

        {/* Step 3: Email Sent */}
        {currentStep === 2 && (
          <Box textAlign="center">
            <Typography variant="h6" gutterBottom>
              Email Sent Successfully
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Please check your inbox and spam folder.
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              Didn't receive the email?
            </Typography>
            <Button
              fullWidth
              variant="outlined"
              onClick={() => onStepChange(0)}
              sx={{ mb: 1 }}
            >
              Try Again
            </Button>
            <Link href="#" underline="hover" variant="body2">
              Contact Support
            </Link>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
