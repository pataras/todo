import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Alert,
  Link,
  Checkbox,
  FormControlLabel,
  LinearProgress,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function LoginWireframe({ currentStep, onStepChange }: Props) {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    rememberMe: false,
  });
  const [showError, setShowError] = useState(false);

  const handleChange = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [field]: e.target.value });
    setShowError(false);
  };

  const handleCheckbox = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, rememberMe: e.target.checked });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (currentStep === 0) {
      onStepChange(1);
    } else if (currentStep === 1) {
      // Simulate validation
      if (formData.email && formData.password) {
        onStepChange(2);
      } else {
        setShowError(true);
      }
    }
  };

  return (
    <Box sx={{ maxWidth: 500, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Sign In
        </Typography>

        <Typography variant="body2" color="text.secondary" align="center" sx={{ mb: 3 }}>
          Welcome back! Please sign in to continue.
        </Typography>

        {/* Step 1: Login Form */}
        {currentStep === 0 && (
          <Box component="form" onSubmit={handleSubmit}>
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
            />
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 1 }}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.rememberMe}
                    onChange={handleCheckbox}
                    color="primary"
                  />
                }
                label="Remember me"
              />
              <Link href="#" underline="hover" variant="body2">
                Forgot password?
              </Link>
            </Box>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              size="large"
              sx={{ mt: 3 }}
            >
              Sign In
            </Button>
            <Typography variant="body2" align="center" sx={{ mt: 2 }}>
              Don't have an account?{' '}
              <Link href="#" underline="hover">
                Sign up
              </Link>
            </Typography>
          </Box>
        )}

        {/* Step 2: Credential Validation */}
        {currentStep === 1 && (
          <Box>
            <Alert severity="info" sx={{ mb: 2 }}>
              Validating credentials...
            </Alert>
            <LinearProgress sx={{ mb: 2 }} />
            {showError ? (
              <Box sx={{ mt: 2 }}>
                <Alert severity="error" sx={{ mb: 2 }}>
                  Invalid email or password. Please try again.
                </Alert>
                <Button
                  fullWidth
                  variant="outlined"
                  onClick={() => onStepChange(0)}
                >
                  Try Again
                </Button>
              </Box>
            ) : (
              <Button
                fullWidth
                variant="contained"
                onClick={() => onStepChange(2)}
                sx={{ mt: 2 }}
              >
                Complete Login
              </Button>
            )}
          </Box>
        )}

        {/* Step 3: Successful Login */}
        {currentStep === 2 && (
          <Box textAlign="center">
            <CheckCircleIcon sx={{ fontSize: 80, color: 'success.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Login Successful!
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              Welcome back, {formData.email || 'user'}!
            </Typography>
            <Alert severity="success" sx={{ mb: 2 }}>
              Authentication token generated successfully.
            </Alert>
            <LinearProgress sx={{ mb: 2 }} />
            <Typography variant="body2" color="text.secondary">
              Redirecting to dashboard...
            </Typography>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
