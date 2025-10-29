import { useState } from 'react';
import {
  Box,
  Container,
  AppBar,
  Toolbar,
  Typography,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Chip,
  IconButton,
  Paper,
  Stepper,
  Step,
  StepLabel,
  Button,
  Divider,
  Card,
  CardContent,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import NavigateBeforeIcon from '@mui/icons-material/NavigateBefore';
import NavigateNextIcon from '@mui/icons-material/NavigateNext';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import { userStories } from './mockData';
import RegistrationWireframe from './components/RegistrationWireframe';
import EmailVerificationWireframe from './components/EmailVerificationWireframe';
import LoginWireframe from './components/LoginWireframe';
import PasswordResetRequestWireframe from './components/PasswordResetRequestWireframe';
import PasswordResetCompletionWireframe from './components/PasswordResetCompletionWireframe';
import UserProfileWireframe from './components/UserProfileWireframe';

const wireframeComponents: Record<string, React.ComponentType<any>> = {
  'US-UM-001': RegistrationWireframe,
  'US-UM-002': EmailVerificationWireframe,
  'US-UM-004': LoginWireframe,
  'US-UM-006': PasswordResetRequestWireframe,
  'US-UM-007': PasswordResetCompletionWireframe,
  'US-UM-010': UserProfileWireframe,
};

function App() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedStoryId, setSelectedStoryId] = useState<string>(userStories[0].id);
  const [currentStep, setCurrentStep] = useState(0);

  const selectedStory = userStories.find((story) => story.id === selectedStoryId);
  const WireframeComponent = selectedStoryId ? wireframeComponents[selectedStoryId] : null;

  const handleStorySelect = (storyId: string) => {
    setSelectedStoryId(storyId);
    setCurrentStep(0);
    setDrawerOpen(false);
  };

  const handleStepChange = (step: number) => {
    setCurrentStep(step);
  };

  const handlePreviousStep = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleNextStep = () => {
    if (selectedStory?.steps && currentStep < selectedStory.steps.length - 1) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handleReset = () => {
    setCurrentStep(0);
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'High':
        return 'error';
      case 'Medium':
        return 'warning';
      case 'Low':
        return 'success';
      default:
        return 'default';
    }
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      {/* App Bar */}
      <AppBar position="static">
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => setDrawerOpen(true)}
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            User Story Wireframes - Release 1: Core Authentication
          </Typography>
          <Chip
            label={`Story Points: ${selectedStory?.storyPoints || 0}`}
            color="secondary"
            sx={{ mr: 1 }}
          />
          {selectedStory && (
            <Chip
              label={selectedStory.priority}
              color={getPriorityColor(selectedStory.priority)}
            />
          )}
        </Toolbar>
      </AppBar>

      {/* Navigation Drawer */}
      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <Box sx={{ width: 350, p: 2 }}>
          <Typography variant="h6" gutterBottom>
            Select User Story
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <List>
            {userStories.map((story) => (
              <ListItem key={story.id} disablePadding>
                <ListItemButton
                  selected={selectedStoryId === story.id}
                  onClick={() => handleStorySelect(story.id)}
                >
                  <ListItemText
                    primary={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="subtitle2">{story.id}</Typography>
                        <Chip
                          label={story.storyPoints}
                          size="small"
                          color="primary"
                          sx={{ fontSize: '0.7rem' }}
                        />
                      </Box>
                    }
                    secondary={story.title}
                  />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>

      {/* Main Content */}
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4, flex: 1 }}>
        {selectedStory && (
          <>
            {/* Story Information Card */}
            <Card sx={{ mb: 3 }}>
              <CardContent>
                <Typography variant="h5" gutterBottom>
                  {selectedStory.id}: {selectedStory.title}
                </Typography>
                <Box sx={{ my: 2 }}>
                  <Typography variant="body1" color="text.secondary">
                    <strong>As a</strong> {selectedStory.asA}
                  </Typography>
                  <Typography variant="body1" color="text.secondary">
                    <strong>I want to</strong> {selectedStory.iWantTo}
                  </Typography>
                  <Typography variant="body1" color="text.secondary">
                    <strong>So that</strong> {selectedStory.soThat}
                  </Typography>
                </Box>
              </CardContent>
            </Card>

            {/* Step Navigation */}
            {selectedStory.steps && selectedStory.steps.length > 0 && (
              <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="h6" gutterBottom>
                  Story Flow Steps
                </Typography>
                <Stepper activeStep={currentStep} sx={{ mb: 3 }}>
                  {selectedStory.steps.map((step) => (
                    <Step key={step.id}>
                      <StepLabel>{step.title}</StepLabel>
                    </Step>
                  ))}
                </Stepper>
                <Box sx={{ display: 'flex', gap: 1, justifyContent: 'center' }}>
                  <Button
                    variant="outlined"
                    startIcon={<NavigateBeforeIcon />}
                    onClick={handlePreviousStep}
                    disabled={currentStep === 0}
                  >
                    Previous
                  </Button>
                  <Button
                    variant="outlined"
                    startIcon={<RestartAltIcon />}
                    onClick={handleReset}
                  >
                    Reset
                  </Button>
                  <Button
                    variant="outlined"
                    endIcon={<NavigateNextIcon />}
                    onClick={handleNextStep}
                    disabled={!selectedStory.steps || currentStep >= selectedStory.steps.length - 1}
                  >
                    Next
                  </Button>
                </Box>
                {selectedStory.steps[currentStep] && (
                  <Box sx={{ mt: 2, p: 2, bgcolor: 'action.hover', borderRadius: 1 }}>
                    <Typography variant="subtitle2" color="primary" gutterBottom>
                      Current Step: {selectedStory.steps[currentStep].title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {selectedStory.steps[currentStep].description}
                    </Typography>
                  </Box>
                )}
              </Paper>
            )}

            {/* Wireframe Component */}
            {WireframeComponent && (
              <WireframeComponent
                currentStep={currentStep}
                onStepChange={handleStepChange}
              />
            )}

            {/* Acceptance Criteria */}
            <Paper sx={{ p: 3, mt: 3 }}>
              <Typography variant="h6" gutterBottom>
                Acceptance Criteria
              </Typography>
              <List dense>
                {selectedStory.acceptanceCriteria.map((criteria) => (
                  <ListItem key={criteria.id}>
                    <ListItemText primary={`• ${criteria.text}`} />
                  </ListItem>
                ))}
              </List>
            </Paper>

            {/* Technical Notes */}
            {selectedStory.technicalNotes && selectedStory.technicalNotes.length > 0 && (
              <Paper sx={{ p: 3, mt: 3 }}>
                <Typography variant="h6" gutterBottom>
                  Technical Notes
                </Typography>
                <List dense>
                  {selectedStory.technicalNotes.map((note, index) => (
                    <ListItem key={index}>
                      <ListItemText primary={`• ${note}`} />
                    </ListItem>
                  ))}
                </List>
              </Paper>
            )}
          </>
        )}
      </Container>

      {/* Footer */}
      <Box
        component="footer"
        sx={{
          py: 2,
          px: 2,
          mt: 'auto',
          backgroundColor: 'background.paper',
          borderTop: 1,
          borderColor: 'divider',
        }}
      >
        <Typography variant="body2" color="text.secondary" align="center">
          ToDo App - User Story Wireframes | MUI 7 | Release 1: Core Authentication (MVP)
        </Typography>
      </Box>
    </Box>
  );
}

export default App;
