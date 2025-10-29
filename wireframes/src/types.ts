export interface AcceptanceCriteria {
  id: string;
  text: string;
  completed: boolean;
}

export interface UserStory {
  id: string;
  title: string;
  asA: string;
  iWantTo: string;
  soThat: string;
  priority: 'High' | 'Medium' | 'Low';
  storyPoints: number;
  acceptanceCriteria: AcceptanceCriteria[];
  technicalNotes?: string[];
  steps?: StoryStep[];
}

export interface StoryStep {
  id: string;
  title: string;
  description: string;
  component?: React.ComponentType<any>;
}

export interface MockUser {
  id: string;
  email: string;
  fullName: string;
  role: 'Member' | 'Team Lead' | 'Admin';
  registrationDate: string;
  lastLogin: string;
  verified: boolean;
  mfaEnabled: boolean;
}
