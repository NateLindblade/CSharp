using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
public partial class ModuleManagementPage : ContentPage
{
    private Course course;
 
    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                course = CmsRepository.Courses.FirstOrDefault(c => c.Id == id);
                RefreshList();
            }
        }
    }
 
    public ModuleManagementPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (course == null)
        {
            return;
        }
 
        Title = $"{course.Name} - Modules";
        ModulesList.ItemsSource = null;
        ModulesList.ItemsSource = course.Modules;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var module = ModuleService.AddModule(course);
        RefreshList();
 
        // Jump straight into the new module's content, since an empty module isn't useful on its own.
        await Shell.Current.GoToAsync($"{nameof(ModuleContentPage)}?courseId={course.Id}&moduleId={module.Id}");
    }
 
    private async void OnManageContentClicked(object sender, EventArgs e)
    {
        var module = (sender as Button)?.BindingContext as Module;
        if (module == null)
        {
            return;
        }
 
        await Shell.Current.GoToAsync($"{nameof(ModuleContentPage)}?courseId={course.Id}&moduleId={module.Id}");
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var module = (sender as Button)?.BindingContext as Module;
        if (module == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert(
            "Delete Module",
            $"Delete Module {module.Id} and all of its content?",
            "Delete",
            "Cancel");
 
        if (confirmed)
        {
            ModuleService.DeleteModule(course, module.Id);
            RefreshList();
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}