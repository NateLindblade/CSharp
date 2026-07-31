using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
public partial class StudentPage : ContentPage
{
    public StudentPage()
    {
        InitializeComponent();
    }
 
    protected override void OnAppearing()
    {
        base.OnAppearing();
 
        // Refresh the list every time this page appears, in case a student
        // was added or enrolled somewhere else since we last showed it.
        StudentsList.ItemsSource = CmsRepository.AllStudents;
    }
 
    private async void OnStudentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Student student)
        {
            await Shell.Current.GoToAsync($"{nameof(StudentCoursesPage)}?studentId={student.Id}");
 
            // Clear the selection so tapping the same student again still fires the event.
            StudentsList.SelectedItem = null;
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
 