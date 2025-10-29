import {
  Box,
  Typography,
  Paper,
  Avatar,
  Chip,
  Divider,
  List,
  ListItem,
  ListItemText,
  Button,
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import SecurityIcon from '@mui/icons-material/Security';
import EditIcon from '@mui/icons-material/Edit';
import { mockUsers } from '../mockData';

interface Props {
  currentStep: number;
  onStepChange: (step: number) => void;
}

export default function UserProfileWireframe({ currentStep, onStepChange }: Props) {
  const user = mockUsers[0]; // Use first mock user

  return (
    <Box sx={{ maxWidth: 800, mx: 'auto', mt: 4 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        {/* Step 1: Navigate to Profile */}
        {currentStep === 0 && (
          <Box textAlign="center">
            <Typography variant="h5" gutterBottom>
              User Navigation
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              Click the profile button in the navigation menu to view your profile.
            </Typography>
            <Button
              variant="contained"
              size="large"
              onClick={() => onStepChange(1)}
            >
              View My Profile
            </Button>
          </Box>
        )}

        {/* Step 2: View Profile Information */}
        {currentStep === 1 && (
          <Box>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
              <Avatar
                sx={{
                  width: 100,
                  height: 100,
                  bgcolor: 'primary.main',
                  fontSize: '2.5rem',
                  mr: 3,
                }}
              >
                {user.fullName.charAt(0)}
              </Avatar>
              <Box sx={{ flex: 1 }}>
                <Typography variant="h4" gutterBottom>
                  {user.fullName}
                </Typography>
                <Chip
                  label={user.role}
                  color="primary"
                  size="small"
                  sx={{ mr: 1 }}
                />
                <Chip
                  icon={user.verified ? <CheckCircleIcon /> : <CancelIcon />}
                  label={user.verified ? 'Verified' : 'Not Verified'}
                  color={user.verified ? 'success' : 'warning'}
                  size="small"
                  sx={{ mr: 1 }}
                />
                <Chip
                  icon={<SecurityIcon />}
                  label={user.mfaEnabled ? 'MFA Enabled' : 'MFA Disabled'}
                  color={user.mfaEnabled ? 'success' : 'default'}
                  size="small"
                />
              </Box>
              <Button
                variant="outlined"
                startIcon={<EditIcon />}
              >
                Edit Profile
              </Button>
            </Box>

            <Divider sx={{ my: 3 }} />

            <Typography variant="h6" gutterBottom>
              Account Information
            </Typography>

            <List>
              <ListItem>
                <ListItemText
                  primary="Email Address"
                  secondary={user.email}
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Role"
                  secondary={user.role}
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Registration Date"
                  secondary={new Date(user.registrationDate).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                  })}
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Last Login"
                  secondary={new Date(user.lastLogin).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                  })}
                />
              </ListItem>
            </List>

            <Divider sx={{ my: 3 }} />

            <Typography variant="h6" gutterBottom>
              Security Settings
            </Typography>

            <List>
              <ListItem>
                <ListItemText
                  primary="Account Verification"
                  secondary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      {user.verified ? (
                        <>
                          <CheckCircleIcon color="success" fontSize="small" />
                          <span>Email verified</span>
                        </>
                      ) : (
                        <>
                          <CancelIcon color="warning" fontSize="small" />
                          <span>Email not verified</span>
                        </>
                      )}
                    </Box>
                  }
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Multi-Factor Authentication"
                  secondary={
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <SecurityIcon fontSize="small" color={user.mfaEnabled ? 'success' : 'disabled'} />
                      <span>{user.mfaEnabled ? 'Enabled' : 'Disabled'}</span>
                    </Box>
                  }
                />
                {!user.mfaEnabled && (
                  <Button variant="outlined" size="small">
                    Enable MFA
                  </Button>
                )}
              </ListItem>
            </List>

            <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
              <Button variant="contained" startIcon={<EditIcon />}>
                Edit Profile
              </Button>
              <Button variant="outlined">
                Change Password
              </Button>
              <Button variant="outlined">
                Security Settings
              </Button>
            </Box>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
