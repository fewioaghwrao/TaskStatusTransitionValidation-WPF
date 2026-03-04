using System.Windows.Controls;
using System.Windows.Input;
using TaskStatusWpf.ViewModels;

namespace TaskStatusWpf.Views;

public partial class ProjectsView : UserControl
{
    public ProjectsView()
    {
        InitializeComponent();

        ProjectsGrid.MouseDoubleClick += (_, __) =>
        {
            if (DataContext is ProjectsViewModel vm && vm.OpenTasksCommand.CanExecute(null))
                vm.OpenTasksCommand.Execute(null);
        };
    }
}