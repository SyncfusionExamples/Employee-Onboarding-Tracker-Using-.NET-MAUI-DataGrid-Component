# Employee-Onboarding-Tracker-Using-.NET-MAUI-DataGrid-Component

This sample demonstrates an Employee Onboarding Tracker built with .NET MAUI using the Syncfusion SfDataGrid control. It shows how to wire up the grid with a view model, customize columns, enable frozen columns, selection, grouping, sorting and include UI for hiding/filtering/grouping columns. 

For the official documentation and additional details about .NET MAUI DataGrid (SfDataGrid), please refer: [Getting Started with .NET MAUI DataGrid](https://help.syncfusion.com/maui/datagrid/getting-started)

## Overview

The project binds the `SfDataGrid` to `EmployeeOnboardingViewModel` and uses a `Columns` collection from the view model to control which columns are shown. The sample contains UI to hide/show columns, apply filtering and grouping, and control sorting. It also freezes the first column so key identifiers stay visible while you scroll horizontally.

Key features demonstrated:

- Binding `SfDataGrid.ItemsSource` to an `Employees` collection in the view model.
- Disabling `AutoGenerateColumnsMode` and providing a `Columns` collection for dynamic control.
- `FrozenColumnCount="1"` to pin the first column while scrolling.
- Multiple selection and `SelectedRows` binding to the view model.
- Ribbon-like toolbar implemented with Syncfusion chips and popups for hide/filter/group/sort operations.

## XAML 

This shows the page's binding context, the toolbar chips and the grid configuration used in the sample:

```xml
<ContentPage.BindingContext>
    <local:EmployeeOnboardingViewModel />
</ContentPage.BindingContext>

<Grid Padding="10" RowDefinitions="Auto, *">

  <syncfusion:SfDataGrid x:Name="dataGrid"
                       SortColumnDescriptions="{Binding SortColumnDescriptions}"
                       GroupColumnDescriptions="{Binding GroupColumnDescriptions}"
                       AutoGenerateColumnsMode="None"
                       Columns="{Binding Columns}"
                       FrozenColumnCount="1"
                       Grid.Row="1"
                       AllowAsyncScrolling="True"
                       SelectionMode="Multiple"
                       SelectedRows="{Binding SelectedItems}"
                       ItemsSource="{Binding Employees}">
      <syncfusion:SfDataGrid.Behaviors>
          <local:DataGridBehavior/>
      </syncfusion:SfDataGrid.Behaviors>
  </syncfusion:SfDataGrid>

</Grid>
```

##### Conclusion
 
I hope you enjoyed learning how to create an Employee Onboarding Tracker using .NET MAUI DataGrid (SfDataGrid).
 
You can refer to our [.NET MAUI DataGrid’s feature tour](https://www.syncfusion.com/maui-controls/maui-datagrid) page to learn about its other groundbreaking feature representations. You can also explore our [.NET MAUI DataGrid Documentation](https://help.syncfusion.com/maui/datagrid/getting-started) to understand how to present and manipulate data. 
For current customers, you can check out our .NET MAUI components on the [License and Downloads](https://www.syncfusion.com/sales/teamlicense) page. If you are new to Syncfusion, you can try our 30-day [free trial](https://www.syncfusion.com/downloads/maui) to explore our .NET MAUI DataGrid and other .NET MAUI components.
 
If you have any queries or require clarifications, please let us know in the comments below. You can also contact us through our [support forums](https://www.syncfusion.com/forums), [Direct-Trac](https://support.syncfusion.com/create) or [feedback portal](https://www.syncfusion.com/feedback/maui?control=sfdatagrid), or the feedback portal. We are always happy to assist you!
