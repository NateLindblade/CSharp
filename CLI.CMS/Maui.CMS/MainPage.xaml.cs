namespace Maui.CMS;
 
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
 
    private async void OnTeacherClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TeacherPage));
    }
 
    private async void OnStudentClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StudentPage));
    }
}
 