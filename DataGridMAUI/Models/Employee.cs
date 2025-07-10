using System.ComponentModel;

namespace DataGridMAUI
{
    public class Employee : INotifyPropertyChanged
    {
        private bool isSelected;
        private string name;
        private string email;
        private DateTime dateOfJoining;
        private int pendingTasks;
        private int onboardingProgressPercentage;
        private List<ChecklistItem> checkedlists;
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value; 
                OnPropertyChanged(nameof(IsSelected));
            }
        }       
        public string Name
        {
            get { return name; }
            set
            {
               name = value; 
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                email = value; 
                OnPropertyChanged(nameof(Email));
            }
        }
        public DateTime DateOfJoining
        {
            get { return dateOfJoining; }
            set
            {
                dateOfJoining = value;
                OnPropertyChanged(nameof(DateOfJoining));
            }
        }
        public int OnboardingProgressPercentage
        {
            get { return onboardingProgressPercentage; }
            set
            {
                onboardingProgressPercentage = value;
                OnPropertyChanged(nameof(OnboardingProgressPercentage));
            }
        }
        public int PendingTasks
        {
            get { return pendingTasks; }
            set
            {
                pendingTasks = value;
                OnPropertyChanged(nameof(PendingTasks));
            }
        }
        public List<ChecklistItem> Checklists
        { 
            get { return checkedlists; }
            set
            {
                checkedlists = value;
                OnPropertyChanged(nameof(Checklists));
            }
        }

        public Employee()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GridColumn : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}