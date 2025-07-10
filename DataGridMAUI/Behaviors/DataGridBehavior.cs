using Syncfusion.Maui.DataGrid;
using System.Reflection;

namespace DataGridMAUI
{
    public partial class DataGridBehavior : Behavior<SfDataGrid>
    {
        private SfDataGrid? dataGrid;
        private EmployeeOnboardingViewModel? employeeOnboardingViewModel;

        protected override void OnAttachedTo(SfDataGrid bindable)
        {
            dataGrid = bindable;
            
            if (dataGrid != null)
            {
                dataGrid.CellValueChanged += DataGrid_CellValueChanged;
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                dataGrid.QueryRowHeight += DataGrid_QueryRowHeight;
                dataGrid.Loaded += DataGrid_Loaded;
            }
            
            base.OnAttachedTo(bindable);
        }

        private void DataGrid_QueryRowHeight(object? sender, DataGridQueryRowHeightEventArgs e)
        {
            if (e.RowIndex != 0)
            {
                //Calculates and sets the height of the row based on its content.
                e.Height = e.GetIntrinsicRowHeight(e.RowIndex);
                e.Handled = true;
            }
        }

        private void DataGrid_Loaded(object? sender, EventArgs e)
        {
            if (dataGrid == null) return;
            
            employeeOnboardingViewModel = dataGrid.BindingContext as EmployeeOnboardingViewModel;
            
            if (employeeOnboardingViewModel != null)
            {
                employeeOnboardingViewModel.PropertyChanged += EmployeeOnboardingViewModel_PropertyChanged;
                employeeOnboardingViewModel.Filtertextchanged += OnFilterChanged;
            }
        }

        private void EmployeeOnboardingViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "NeedToRefresh")
            {
                if (dataGrid != null)
                {
                    var gridModelProperty = dataGrid.GetType()
                     .GetProperty("DataGridModel", BindingFlags.NonPublic | BindingFlags.Instance);

                    var gridModel = gridModelProperty?.GetValue(dataGrid);

                    if (gridModel is DataGridModel model)
                    {
                        model.RefreshView();
                    }
                }
            }
        }

        public void OnFilterChanged()
        {
            if (this.dataGrid!.View != null)
            {
                this.dataGrid.View.Filter = this.employeeOnboardingViewModel!.FilerRecords;
                this.dataGrid.View.RefreshFilter();
            }
        }

        private void DataGrid_CellValueChanged(object? sender, DataGridCellValueChangedEventArgs e)
        {
            try
            {
                if (dataGrid == null || employeeOnboardingViewModel == null)
                    return;

                var rowData = e.RowData as Employee;
                if (e.Column.MappingName == "IsSelected")
                {
                    if (rowData.IsSelected && !employeeOnboardingViewModel.SelectedItems.Contains(rowData))
                        employeeOnboardingViewModel.SelectedItems.Add(rowData);

                }
                if (e.Column.MappingName == "IsSelected" && !(rowData.IsSelected))
                {
                    employeeOnboardingViewModel.SelectedItems.Remove(rowData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataGrid_CellValueChanged: {ex.Message}");
            }
        }

        private void DataGrid_SelectionChanged(object? sender, DataGridSelectionChangedEventArgs e)
        {
            if (e.AddedRows != null && e.RemovedRows != null)
            {
                UpdateIsSelected(e.AddedRows, e.RemovedRows);
            }
        }
       
        private static void UpdateIsSelected(IList<object> addedRows, IList<object> removedRows)
        {
            UpdateEmployeeSelection(addedRows, true);
            UpdateEmployeeSelection(removedRows, false);
        }

        private static void UpdateEmployeeSelection(IList<object>? rows, bool isSelected)
        {
            if (rows == null || rows.Count == 0) return;
            
            foreach (var item in rows)
            {
                if (item is Employee employee)
                {
                    employee.IsSelected = isSelected;
                }
            }
        }       
        protected override void OnDetachingFrom(SfDataGrid bindable)
        {
            if (dataGrid != null)
            {
                dataGrid.CellValueChanged -= DataGrid_CellValueChanged;
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                dataGrid.Loaded -= DataGrid_Loaded;
                dataGrid.QueryRowHeight -= DataGrid_QueryRowHeight;
                
                if (employeeOnboardingViewModel != null)
                {
                    employeeOnboardingViewModel.Filtertextchanged -= OnFilterChanged;
                    employeeOnboardingViewModel.PropertyChanged -= EmployeeOnboardingViewModel_PropertyChanged;
                }
            }

            dataGrid = null;
            employeeOnboardingViewModel = null;
            base.OnDetachingFrom(bindable);
        }
    }
}