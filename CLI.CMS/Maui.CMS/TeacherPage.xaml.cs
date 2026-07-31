namespace Maui.CMS;
 
public partial class TeacherPage : ContentPage
{
    public TeacherPage()
    {
        InitializeComponent();
    }
 
    private async void OnManageStudentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StudentManagementPage));
    }
 
    private async void OnViewCoursesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TeacherCoursesPage));
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}