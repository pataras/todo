import { Box, Typography, Paper, Button, Alert, LinearProgress } from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import EmailIcon from '@mui/icons-material/Email';
import ErrorIcon from '@mui/icons-material/Error';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function EmailVerificationWireframe({ currentStep, onStepChange }: Props) {
  const handleNext = () => {
    onStepChange(currentStep + 1);
  };

  return (
    <Box sx={{ maxWidth: 500, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        {/* Step 1: Email Sent */}
        {currentStep === 0 && (
          <Box textAlign="center">
            <EmailIcon sx={{ fontSize: 80, color: 'primary.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Verification Email Sent
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              We've sent a verification link to your email address.
            </Typography>
            <Alert severity="info" sx={{ mb: 2 }}>
              The verification link will expire in 24 hours.
            </Alert>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Check your inbox and click the link to verify your account.
            </Typography>
            <Button fullWidth variant="contained" onClick={handleNext}>
              Simulate Click Verification Link
            </Button>
            <Button fullWidth variant="text" sx={{ mt: 1 }}>
              Resend Verification Email
            </Button>
          </Box>
        )}

        {/* Step 2: Click Verification Link */}
        {currentStep === 1 && (
          <Box textAlign="center">
            <LinearProgress sx={{ mb: 3 }} />
            <Typography variant="h6" gutterBottom>
              Verifying Your Email...
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Please wait while we verify your account.
            </Typography>
            <Button variant="contained" onClick={handleNext}>
              Complete Verification
            </Button>
          </Box>
        )}

        {/* Step 3: Verification Success */}
        {currentStep === 2 && (
          <Box textAlign="center">
            <CheckCircleIcon sx={{ fontSize: 80, color: 'success.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Email Verified Successfully!
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              Your account has been verified and is now fully activated.
            </Typography>
            <Alert severity="success" sx={{ mb: 2 }}>
              You now have full access to all features.
            </Alert>
            <Button fullWidth variant="contained" onClick={handleNext}>
              Go to Dashboard
            </Button>
          </Box>
        )}

        {/* Optional: Token Expired Error State */}
        {currentStep === 99 && (
          <Box textAlign="center">
            <ErrorIcon sx={{ fontSize: 80, color: 'error.main', mb: 2 }} />
            <Typography variant="h5" gutterBottom>
              Verification Link Expired
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              This verification link has expired. Please request a new one.
            </Typography>
            <Button fullWidth variant="contained" onClick={() => onStepChange(0)}>
              Request New Verification Email
            </Button>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
