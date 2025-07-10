using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Syncfusion.Maui.DataGrid;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using Syncfusion.Maui.Data;

namespace DataGridMAUI
{
    public partial class EmployeeOnboardingViewModel : INotifyPropertyChanged
    {
        // IsOpen property for open/close popup
        private bool isOpenForHideColumns;
        private bool isOpenForFilterColumns;
        private bool isOpenForGroupColumns;
        private bool isOpenForSortColumns;
        private bool isOpenForSearch;
        private bool isOnState;
        private bool isChecked;

        // Columns and Data manipulation property definition
        private ColumnCollection columns = new ColumnCollection();
        private GroupColumnDescriptionCollection groupColumns = new GroupColumnDescriptionCollection();
        private SortColumnDescriptionCollection sortColumns = new SortColumnDescriptionCollection();
        private string? filtertext = string.Empty;
        private GridColumn? selectedcolumn;
        private GridColumn? selectedgroupColumn;
        private GridColumn? selectedsortcolumn;
        private string? selectedcondition = "Contains";
        internal FilterChanged? Filtertextchanged;
        internal bool NeedToRefresh = false;
        private ObservableCollection<Employee> employees;
        private ObservableCollection<object> selectedItems;

        public ObservableCollection<Employee> Employees
        {
            get
            {
                return employees;
            }
            set
            {
                employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }
        
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        /// <summary>
        /// Gets or sets the value of FilterText and notifies user when value gets changed 
        /// </summary>
        public string? FilterText
        {
            get
            {
                return this.filtertext;
            }

            set
            {
                this.filtertext = value;
                this.OnFilterTextChanged();
                this.OnPropertyChanged("FilterText");
            }
        }

        public List<GridColumn> GridColumns { get; set; } = new List<GridColumn>();
        public List<GridColumn> GridColumnsForGroup { get; set; } = new List<GridColumn>();
        public List<GridColumn> GridColumnsForFilter { get; set; } = new List<GridColumn>();
        public List<GridColumn> GridColumnsForSort { get; set; } = new List<GridColumn>();

        // Commands for open popupto handle columns and data manipulation.
        public ICommand HideColumns { get; set; } = null!;
        public ICommand HideAllColumns { get; set; } = null!;
        public ICommand ShowAllColumns { get; set; } = null!;

        public ICommand FilterColumns { get; set; } = null!;
        public ICommand ClearFilter { get; set; } = null!;

        public ICommand GroupColumns { get; set; } = null!;
        public ICommand ClearGroup { get; set; } = null!;

        public ICommand SortColumns { get; set; } = null!;
        public ICommand ClearSort { get; set; } = null!;

        public List<string> SearchConditions { get; set; } = new List<string>();
        public string? SelectedCondition
        {
            get
            {
                return selectedcondition;
            }
            set
            {
                selectedcondition = value;
                OnPropertyChanged(nameof(SelectedCondition));
            }
        }

        public GridColumn? SelectedColumn
        {
            get
            {
                return selectedcolumn;
            }
            set
            {
                selectedcolumn = value;
                OnPropertyChanged(nameof(SelectedColumn));
            }
        }

        public GridColumn? SelectedGroupColumn
        {
            get
            {
                return selectedgroupColumn;
            }
            set
            {
                selectedgroupColumn = value;
                this.ExecuteAddGrouping();
                OnPropertyChanged(nameof(SelectedGroupColumn));
            }
        }

        public GridColumn? SelectedSortColumn
        {
            get
            {
                return selectedsortcolumn;
            }
            set
            {
                selectedsortcolumn = value;
                ExecuteAddSorting();
                OnPropertyChanged(nameof(SelectedSortColumn));
            }
        }


        public bool IsOpenForHideColumns
        {
            get
            {
                return isOpenForHideColumns;
            }
            set
            {
                isOpenForHideColumns = value;
                OnPropertyChanged(nameof(IsOpenForHideColumns));
            }
        }

        public bool IsOpenForFilterColumns
        {
            get
            {
                return isOpenForFilterColumns;
            }
            set
            {
                isOpenForFilterColumns = value;
                OnPropertyChanged(nameof(IsOpenForFilterColumns));
            }
        }

        public bool IsOpenForGroupColumns
        {
            get
            {
                return isOpenForGroupColumns;
            }
            set
            {
                isOpenForGroupColumns = value;
                OnPropertyChanged(nameof(IsOpenForGroupColumns));
            }

        }

        public bool IsOpenForSortColumns
        {
            get
            {
                return isOpenForSortColumns;
            }
            set
            {
                isOpenForSortColumns = value;
                OnPropertyChanged(nameof(IsOpenForSortColumns));
            }
        }

        public bool IsOpenForSearch
        {
            get
            {
                return isOpenForSearch;
            }
            set
            {
                isOpenForSearch = value;
                OnPropertyChanged(nameof(IsOpenForSearch));
            }
        }

        public bool IsOnState
        {
            get
            {
                return isOnState;
            }
            set
            {
                isOnState = value;
                ExecuteAddSorting();
                OnPropertyChanged(nameof(IsOnState));
            }
        }

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
                UpdateAllCheckBoxes();
            }
        }

        public ColumnCollection Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }

        public GroupColumnDescriptionCollection GroupColumnDescriptions
        {
            get
            {
                return groupColumns;
            }
            set
            {
                groupColumns = value;
                OnPropertyChanged(nameof(GroupColumnDescription));
            }
        }

        public SortColumnDescriptionCollection SortColumnDescriptions
        {
            get
            {
                return sortColumns;
            }
            set
            {
                sortColumns = value;
                OnPropertyChanged(nameof(SortColumnDescriptions));
            }
        }
        public EmployeeOnboardingViewModel()
        {
            InitializeCollections();
            GenerateEmployees();
            InitializeColumns();
            InitializeCommands();
            PopulateSearchCriteria();
            PopuplateColumnNames();
        }

        private void InitializeCollections()
        {
            Employees = new ObservableCollection<Employee>();
            SelectedItems = new ObservableCollection<object>();
        }

        string[] checkLists =
        {
            "Signed employment contract",
            "Submit your bank account details",
            "Get introduced to the team",
            "Office tour",
            "Pick your equipments from Alice",
            "Check in with your manager",
            "Setup benefits",
            "Go over key resources",
            "Weekly 1-1 with manager"
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private void PopuplateColumnNames()
        {
            GridColumns = new List<GridColumn>();
            GridColumnsForFilter = new List<GridColumn>();
            GridColumnsForGroup = new List<GridColumn>();
            GridColumnsForSort = new List<GridColumn>();

            GridColumnsForFilter.Add(new GridColumn() { Name = "All Columns", DisplayName = "All Columns" });
            if (columns != null)
            {
                columns.ForEach(o =>
                {
                    if (o != null && o.MappingName != "IsSelected")
                    {
                        var column = new GridColumn() { Name = o.MappingName ?? string.Empty, DisplayName = o.HeaderText ?? string.Empty };
                        GridColumns.Add(column);
                        column.PropertyChanged += OnIsCheckedChanged;
                    }
                });
            }
            GridColumnsForFilter.AddRange(GridColumns);
            GridColumnsForGroup.AddRange(GridColumns);
            GridColumnsForSort.AddRange(GridColumns);

            GridColumns.RemoveAt(0);

            SelectedColumn = GridColumnsForFilter.FirstOrDefault();
            SelectedGroupColumn = GridColumnsForGroup.FirstOrDefault();
            SelectedSortColumn = GridColumnsForSort.FirstOrDefault();
        }
        private void PopulateSearchCriteria()
        {
            SearchConditions = new List<string>
            {
                "Contains",
                "Equals",
                "Does Not Equal",
            };
        }
        private void OnIsCheckedChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                var column = sender as GridColumn;
                columns.First(o => o.MappingName == column.Name).Visible = !column.IsChecked;
            }
        }

        private void InitializeColumns()
        {
            columns = new ColumnCollection();

            // CheckBox Column
            var checkBoxColumn = new DataGridCheckBoxColumn()
            {
                MappingName = "IsSelected",
                HeaderText = "",
                Width = 50,
                HeaderTemplate = new DataTemplate(() =>
                {
                    var headerCheckBox = new Syncfusion.Maui.Buttons.SfCheckBox()
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    headerCheckBox.SetBinding(Syncfusion.Maui.Buttons.SfCheckBox.IsCheckedProperty, new Binding() { Path = "IsChecked", Mode = BindingMode.TwoWay, Source = this });
                    return headerCheckBox;
                })
            };

            // Text Columns
            var nameColumn = new DataGridTextColumn()
            {
                MappingName = "Name",
                HeaderText = "Name",
                Width = 150
            };

            var emailColumn = new DataGridTextColumn()
            {
                MappingName = "Email",
                HeaderText = "Email",
                Width = 150
            };

            // Date Column
            var dateColumn = new DataGridDateColumn()
            {
                MappingName = "DateOfJoining",
                HeaderText = "Date of Joining",
                Format = "dd MMMM, yyyy",
                Width = 180
            };

            // Progress Bar Template Column
            var progressColumn = new DataGridTemplateColumn()
            {
                MappingName = "OnboardingProgressPercentage",
                HeaderText = "Onboarding Progress",
                Width = 180,
                CellTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid
                    {
                        ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                    };

                    var progressBar = new Syncfusion.Maui.ProgressBar.SfLinearProgressBar
                    {
                        TrackHeight = 10,
                        TrackCornerRadius = 5,
                        ProgressHeight = 10,
                        ProgressCornerRadius = 5,
                        VerticalOptions = LayoutOptions.Center,
                        Background = Colors.Transparent,
                        HeightRequest = 10
                    };
                    progressBar.SetBinding(Syncfusion.Maui.ProgressBar.SfLinearProgressBar.ProgressProperty, "OnboardingProgressPercentage");

                    var label = new Label
                    {
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(5)
                    };
                    label.SetBinding(Label.TextProperty, new Binding("OnboardingProgressPercentage", stringFormat: "{0}%"));

                    grid.Add(progressBar, 0, 0);
                    grid.Add(label, 1, 0);

                    return grid;
                })
            };

            // Pending Tasks Column
            var tasksColumn = new DataGridTextColumn()
            {
                MappingName = "PendingTasks",
                HeaderText = "Pending Tasks",
                Width = 180,
                DisplayBinding = new Binding("PendingTasks", stringFormat: "Pending Tasks : {0}")
            };

            // Checklists Template Column
            var checklistsColumn = new DataGridTemplateColumn()
            {
                MappingName = "Checklists",
                HeaderText = "Checklists",
                Width = 300,
                CellTemplate = new DataTemplate(() =>
                {
                    var collectionView = new CollectionView();
                    collectionView.SetBinding(CollectionView.ItemsSourceProperty, "Checklists");

                    collectionView.ItemTemplate = new DataTemplate(() =>
                    {
                        var grid = new Grid
                        {
                            ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                            Margin = new Thickness(2)
                        };

                        var taskLabel = new Label
                        {
                            Margin = new Thickness(5, 2),       
                            HorizontalOptions = LayoutOptions.Start,
                        };
                        taskLabel.SetBinding(Label.TextProperty, "Task");
                      //s  taskLabel.SetBinding(Label.BackgroundColorProperty, new Binding("IsCompleted", converter: new BoolToColorConverter()));

                        var checkLabel = new Label
                        {
                            Text = "✓",
                            TextColor = Colors.Green,
                            FontAttributes = FontAttributes.Bold,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        checkLabel.SetBinding(Label.IsVisibleProperty, "IsCompleted");

                        grid.Add(taskLabel, 1, 0);
                        grid.Add(checkLabel, 0, 0);

                        return grid;
                    });

                    return collectionView;
                })
            };

            // Add columns to the DataGrid
            columns.Add(checkBoxColumn);
            columns.Add(nameColumn);
            columns.Add(emailColumn);
            columns.Add(dateColumn);
            columns.Add(progressColumn);
            columns.Add(tasksColumn);
            columns.Add(checklistsColumn);
        }
        private void InitializeCommands()
        {
            HideColumns = new Command(ExecuteHideColumns);
            ShowAllColumns = new Command(ExecuteShowAllColumns);
            HideAllColumns = new Command(ExecuteHideAllColumns);

            FilterColumns = new Command(ExecuteFilterColumns);
            ClearFilter = new Command(ExecuteClearFilter);
            GroupColumns = new Command(ExecuteGroupColumns);
            ClearGroup = new Command(ExecuteClearGroups);
            SortColumns = new Command(ExecuteSortColumns);
            ClearSort = new Command(ExecuteClearSorts);
        }

        // sorting
        private void ExecuteSortColumns()
        {
            IsOpenForSortColumns = true;
        }
        private void ExecuteClearSorts()
        {
            if (SortColumnDescriptions == null)
                return;
            SortColumnDescriptions.Clear();

            SelectedSortColumn = null;            
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");

            Application.Current?.Dispatcher.Dispatch(() =>
            {
                IsOpenForSortColumns = false;
            });
        }

        private void ExecuteAddSorting()
        {            
            if (SelectedSortColumn != null)
            {
                SortColumnDescriptions.Clear();
                var sortColumnDescription = new SortColumnDescription() 
                { 
                    ColumnName = this.SelectedSortColumn.Name ?? string.Empty, 
                    SortDirection = IsOnState ? ListSortDirection.Ascending : ListSortDirection.Descending 
                };
                SortColumnDescriptions.Add(sortColumnDescription);
            }
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");
        }

        // Grouping
        private void ExecuteGroupColumns()
        {
            IsOpenForGroupColumns = true;
        }
        private void ExecuteClearGroups()
        {
            if (GroupColumnDescriptions == null)
                return;
            GroupColumnDescriptions.Clear();

            SelectedGroupColumn = null;
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");

            Application.Current?.Dispatcher.Dispatch(() =>
            {
                IsOpenForGroupColumns = false;
            });
        }

        private void ExecuteAddGrouping()
        {            
            if (SelectedGroupColumn != null)
            {
                GroupColumnDescriptions.Clear();
                var groupColumnDescription = new GroupColumnDescription() { ColumnName = this.SelectedGroupColumn.Name ?? string.Empty };
                GroupColumnDescriptions.Add(groupColumnDescription);
            }
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");
        }

        // Show/Hide columns
        private void ExecuteHideColumns()
        {
            IsOpenForHideColumns = true;
        }

        private void ExecuteHideAllColumns()
        {
            foreach (var item in columns)
            {
                if (item.MappingName == "IsSelected")
                    continue;
                item.Visible = false;
            }
            foreach (var item in GridColumns)
            {
                item.IsChecked = true;
                item.OnPropertyChanged("IsChecked");
            }
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");
        }

        private void ExecuteShowAllColumns()
        {
            foreach (var item in columns)
            {
                if (item.MappingName == "IsSelected")
                    continue;
                item.Visible = true;
            }
            foreach (var item in GridColumns)
            {
                item.IsChecked = false;
                item.OnPropertyChanged("IsChecked");
            }
            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");
        }

        private void UpdateAllCheckBoxes()
        {
            if (Employees != null)
            {                            
                foreach (var employee in Employees)
                {
                    employee.IsSelected = IsChecked;
                    
                    // If checked, add to SelectedItems
                    if (IsChecked)
                    {
                        if(!SelectedItems.Contains(employee))
                            SelectedItems.Add(employee);
                    }
                    else
                    {
                        SelectedItems.Remove(employee);
                    }
                }
            }
        }

        // Filtering
        private void ExecuteClearFilter()
        {
            SelectedCondition = string.Empty;
            SelectedColumn = null;

            FilterText = string.Empty;

            NeedToRefresh = true;
            OnPropertyChanged("NeedToRefresh");

            Application.Current ?.Dispatcher.Dispatch(() =>
            {
                IsOpenForFilterColumns = false;
            });
        }

        private void ExecuteFilterColumns()
        {
            IsOpenForFilterColumns = true;
            NeedToRefresh = true;
           OnPropertyChanged("NeedToRefresh");
        }

        private void GenerateEmployees()
        {
            // Creating checklist items with random completion status for demo purposes
            var tyrionChecklist = CreateChecklistItems(3); // 3 items completed
            var sansaChecklist = CreateChecklistItems(4);  // 4 items completed
            var nedChecklist = CreateChecklistItems(5);    // 5 items completed
            var jonChecklist = CreateChecklistItems(7);    // 7 items completed
            var aryaChecklist = CreateChecklistItems(6);   // 6 items completed
            var jaimieChecklist = CreateChecklistItems(8); // 8 items completed
            var cerseiChecklist = CreateChecklistItems(2); // 2 items completed
            var daenerysChecklist = CreateChecklistItems(9); // 9 items completed
            var robbChecklist = CreateChecklistItems(5);   // 5 items completed
            var branChecklist = CreateChecklistItems(4);   // 4 items completed
            var joffreyChecklist = CreateChecklistItems(1); // 1 item completed
            var margaeryChecklist = CreateChecklistItems(6); // 6 items completed
            
            // Additional checklists for 20 more employees
            var brienneChecklist = CreateChecklistItems(7);  // 7 items completed
            var samwellChecklist = CreateChecklistItems(6);  // 6 items completed
            var theonChecklist = CreateChecklistItems(3);    // 3 items completed
            var drogoChecklist = CreateChecklistItems(5);    // 5 items completed
            var missandeiChecklist = CreateChecklistItems(8);// 8 items completed
            var greywormChecklist = CreateChecklistItems(9); // 9 items completed
            var melisandreChecklist = CreateChecklistItems(4); // 4 items completed
            var stannisChecklist = CreateChecklistItems(2);  // 2 items completed
            var davosChecklist = CreateChecklistItems(5);    // 5 items completed
            var gendryChecklist = CreateChecklistItems(6);   // 6 items completed
            var tormundChecklist = CreateChecklistItems(4);  // 4 items completed
            var ygrittChecklist = CreateChecklistItems(7);   // 7 items completed
            var olenaChecklist = CreateChecklistItems(8);    // 8 items completed
            var ellaria_sandChecklist = CreateChecklistItems(4); // 4 items completed
            var oberynChecklist = CreateChecklistItems(6);   // 6 items completed
            var varyChecklist = CreateChecklistItems(7);     // 7 items completed
            var littlefingerChecklist = CreateChecklistItems(5); // 5 items completed
            var hodorChecklist = CreateChecklistItems(2);    // 2 items completed
            var jorahChecklist = CreateChecklistItems(8);    // 8 items completed
            var myrcellaChecklist = CreateChecklistItems(3); // 3 items completed

            Employees.Add(new Employee
            {
                Name = "Tyrion Lannister",
                Email = "tyrion@xyz.com",
                DateOfJoining = new DateTime(2021, 11, 18),
                OnboardingProgressPercentage = 33,
                PendingTasks = 6,
                Checklists = tyrionChecklist
            });

            Employees.Add(new Employee
            {
                Name = "Sansa Stark",
                Email = "sansa@xyz.com",
                DateOfJoining = new DateTime(2021, 11, 4),
                OnboardingProgressPercentage = 44,
                PendingTasks = 5,
                Checklists = sansaChecklist
            });

            Employees.Add(new Employee
            {
                Name = "Ned Stark",
                Email = "ned@xyz.com",
                DateOfJoining = new DateTime(2021, 11, 16),
                OnboardingProgressPercentage = 56,
                PendingTasks = 4,
                Checklists = nedChecklist
            });

            Employees.Add(new Employee
            {
                Name = "Jon Snow",
                Email = "jon@xyz.com",
                DateOfJoining = new DateTime(2021, 11, 23),
                OnboardingProgressPercentage = 78,
                PendingTasks = 2,
                Checklists = jonChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Arya Stark",
                Email = "arya@xyz.com",
                DateOfJoining = new DateTime(2022, 1, 15),
                OnboardingProgressPercentage = 67,
                PendingTasks = 3,
                Checklists = aryaChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Jaimie Lannister",
                Email = "jaimie@xyz.com",
                DateOfJoining = new DateTime(2022, 2, 10),
                OnboardingProgressPercentage = 89,
                PendingTasks = 1,
                Checklists = jaimieChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Cersei Lannister",
                Email = "cersei@xyz.com",
                DateOfJoining = new DateTime(2022, 3, 5),
                OnboardingProgressPercentage = 22,
                PendingTasks = 7,
                Checklists = cerseiChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Daenerys Targaryen",
                Email = "daenerys@xyz.com",
                DateOfJoining = new DateTime(2022, 3, 25),
                OnboardingProgressPercentage = 100,
                PendingTasks = 0,
                Checklists = daenerysChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Robb Stark",
                Email = "robb@xyz.com",
                DateOfJoining = new DateTime(2022, 4, 12),
                OnboardingProgressPercentage = 56,
                PendingTasks = 4,
                Checklists = robbChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Bran Stark",
                Email = "bran@xyz.com",
                DateOfJoining = new DateTime(2022, 5, 8),
                OnboardingProgressPercentage = 44,
                PendingTasks = 5,
                Checklists = branChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Joffrey Baratheon",
                Email = "joffrey@xyz.com",
                DateOfJoining = new DateTime(2022, 6, 20),
                OnboardingProgressPercentage = 11,
                PendingTasks = 8,
                Checklists = joffreyChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Margaery Tyrell",
                Email = "margaery@xyz.com",
                DateOfJoining = new DateTime(2022, 7, 15),
                OnboardingProgressPercentage = 67,
                PendingTasks = 3,
                Checklists = margaeryChecklist
            });
            
            // Adding 20 more employees
            Employees.Add(new Employee
            {
                Name = "Brienne of Tarth",
                Email = "brienne@xyz.com",
                DateOfJoining = new DateTime(2022, 8, 5),
                OnboardingProgressPercentage = 78,
                PendingTasks = 2,
                Checklists = brienneChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Samwell Tarly",
                Email = "samwell@xyz.com",
                DateOfJoining = new DateTime(2022, 8, 12),
                OnboardingProgressPercentage = 67,
                PendingTasks = 3,
                Checklists = samwellChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Theon Greyjoy",
                Email = "theon@xyz.com",
                DateOfJoining = new DateTime(2022, 8, 20),
                OnboardingProgressPercentage = 33,
                PendingTasks = 6,
                Checklists = theonChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Khal Drogo",
                Email = "drogo@xyz.com",
                DateOfJoining = new DateTime(2022, 9, 3),
                OnboardingProgressPercentage = 56,
                PendingTasks = 4,
                Checklists = drogoChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Missandei",
                Email = "missandei@xyz.com",
                DateOfJoining = new DateTime(2022, 9, 10),
                OnboardingProgressPercentage = 89,
                PendingTasks = 1,
                Checklists = missandeiChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Grey Worm",
                Email = "greyworm@xyz.com",
                DateOfJoining = new DateTime(2022, 9, 17),
                OnboardingProgressPercentage = 100,
                PendingTasks = 0,
                Checklists = greywormChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Melisandre",
                Email = "melisandre@xyz.com",
                DateOfJoining = new DateTime(2022, 9, 25),
                OnboardingProgressPercentage = 44,
                PendingTasks = 5,
                Checklists = melisandreChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Stannis Baratheon",
                Email = "stannis@xyz.com",
                DateOfJoining = new DateTime(2022, 10, 2),
                OnboardingProgressPercentage = 22,
                PendingTasks = 7,
                Checklists = stannisChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Davos Seaworth",
                Email = "davos@xyz.com",
                DateOfJoining = new DateTime(2022, 10, 9),
                OnboardingProgressPercentage = 56,
                PendingTasks = 4,
                Checklists = davosChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Gendry",
                Email = "gendry@xyz.com",
                DateOfJoining = new DateTime(2022, 10, 16),
                OnboardingProgressPercentage = 67,
                PendingTasks = 3,
                Checklists = gendryChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Tormund Giantsbane",
                Email = "tormund@xyz.com",
                DateOfJoining = new DateTime(2022, 10, 23),
                OnboardingProgressPercentage = 44,
                PendingTasks = 5,
                Checklists = tormundChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Ygritte",
                Email = "ygritte@xyz.com",
                DateOfJoining = new DateTime(2022, 11, 1),
                OnboardingProgressPercentage = 78,
                PendingTasks = 2,
                Checklists = ygrittChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Olena Tyrell",
                Email = "olena@xyz.com",
                DateOfJoining = new DateTime(2022, 11, 8),
                OnboardingProgressPercentage = 89,
                PendingTasks = 1,
                Checklists = olenaChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Ellaria Sand",
                Email = "ellaria@xyz.com",
                DateOfJoining = new DateTime(2022, 11, 15),
                OnboardingProgressPercentage = 44,
                PendingTasks = 5,
                Checklists = ellaria_sandChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Oberyn Martell",
                Email = "oberyn@xyz.com",
                DateOfJoining = new DateTime(2022, 11, 22),
                OnboardingProgressPercentage = 67,
                PendingTasks = 3,
                Checklists = oberynChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Varys",
                Email = "varys@xyz.com",
                DateOfJoining = new DateTime(2022, 12, 1),
                OnboardingProgressPercentage = 78,
                PendingTasks = 2,
                Checklists = varyChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Petyr Baelish",
                Email = "littlefinger@xyz.com",
                DateOfJoining = new DateTime(2022, 12, 8),
                OnboardingProgressPercentage = 56,
                PendingTasks = 4,
                Checklists = littlefingerChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Hodor",
                Email = "hodor@xyz.com",
                DateOfJoining = new DateTime(2022, 12, 15),
                OnboardingProgressPercentage = 22,
                PendingTasks = 7,
                Checklists = hodorChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Jorah Mormont",
                Email = "jorah@xyz.com",
                DateOfJoining = new DateTime(2022, 12, 22),
                OnboardingProgressPercentage = 89,
                PendingTasks = 1,
                Checklists = jorahChecklist
            });
            
            Employees.Add(new Employee
            {
                Name = "Myrcella Baratheon",
                Email = "myrcella@xyz.com",
                DateOfJoining = new DateTime(2022, 12, 29),
                OnboardingProgressPercentage = 33,
                PendingTasks = 6,
                Checklists = myrcellaChecklist
            });
        }

        private List<ChecklistItem> CreateChecklistItems(int completedCount)
        {
            var items = new List<ChecklistItem>();
            
            for (int i = 0; i < checkLists.Length; i++)
            {
                // Mark items as completed based on the completedCount
                bool isCompleted = i < completedCount;
                items.Add(new ChecklistItem(checkLists[i], isCompleted));
            }
            
            return items;
        }

        private void OnFilterTextChanged()
        {
            if (this.Filtertextchanged != null)
            {
                this.Filtertextchanged();
            }
        }

        public bool FilerRecords(object o)
        {
            if (SelectedColumn == null && string.IsNullOrEmpty(this.SelectedCondition))
                return true;

            double res;
            bool checkNumeric = double.TryParse(this.FilterText, out res);
            var item = o as Employee;
            if (item != null && !string.IsNullOrEmpty(this.FilterText) && this.FilterText.Equals(string.Empty))
            {
                return true;
            }
            else
            {
                if (item != null && SelectedColumn != null)
                {
                    if (checkNumeric && !(this.SelectedColumn.Name ?? string.Empty).Equals("All Columns") && !(this.SelectedCondition ?? string.Empty).Equals("Contains"))
                    {
                        bool result = this.MakeNumericFilter(item, this.SelectedColumn.Name, this.SelectedCondition);
                        return result;
                    }
                    else if ((this.SelectedColumn.Name ?? string.Empty).Equals("All Columns"))
                    {
                        if ((item.Name ?? string.Empty).ToString().ToLower().Contains(this.FilterText!.ToLower()) ||
                            (item.Email ?? string.Empty).ToString().ToLower().Contains(this.FilterText.ToLower()) ||
                            item.DateOfJoining.ToString().ToLower().Contains(this.FilterText.ToLower()) ||
                            item.OnboardingProgressPercentage.ToString().ToLower().Contains(this.FilterText.ToLower()) ||
                            item.PendingTasks.ToString().ToLower().Contains(this.FilterText.ToLower()) ||
                            (item.Checklists != null && item.Checklists.ToString().ToLower().Contains(this.FilterText.ToLower())))                           
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        bool result = this.MakeStringFilter(item, this.SelectedColumn.Name, this.SelectedCondition!);
                        return result;
                    }
                }
            }
            return false;
        }
        private bool MakeStringFilter(Employee o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            if (value == null) return false;
            
            var exactValue = value.GetValue(o, null);
            if (exactValue == null) return false;
            
            exactValue = exactValue.ToString()?.ToLower() ?? string.Empty;
            string text = this.FilterText!.ToLower();
            var methods = typeof(string).GetMethods();

            if (methods.Length != 0)
            {
                if (condition == "Contains")
                {
                    var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
                    if (methodInfo != null && exactValue != null)
                    {
                        bool result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text })!;
                        return result1;
                    }
                    return false;
                }
                else if (exactValue != null && exactValue.ToString() == text.ToString())
                {
                    bool result1 = string.Equals(exactValue.ToString(), text.ToString());
                    if (condition == "Equals")
                    {
                        return result1;
                    }
                    else if (condition == "NotEquals")
                    {
                        return false;
                    }
                }
                else if (condition == "NotEquals")
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Used decide to make the numeric filter
        /// </summary>
        /// <param name="o">o</param>
        /// <param name="option">option</param>
        /// <param name="condition">condition</param>
        /// <returns>true or false value</returns>
        private bool MakeNumericFilter(Employee o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value!.GetValue(o, null);
            double res;
            bool checkNumeric = exactValue != null && double.TryParse(exactValue.ToString(), out res);
            if (checkNumeric)
            {
                switch (condition)
                {
                    case "Equals":
                        try
                        {
                            if (exactValue.ToString() == this.FilterText)
                            {
                                if (Convert.ToDouble(exactValue) == Convert.ToDouble(this.FilterText))
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }

                        break;
                    case "NotEquals":
                        try
                        {
                            if (Convert.ToDouble(this.FilterText) != Convert.ToDouble(exactValue))
                            {
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            return true;
                        }

                        break;
                }
            }

            return false;
        }


        /// <summary>
        /// Used to send a Notification while Filter Changed
        /// </summary>
        internal delegate void FilterChanged();

    }
}