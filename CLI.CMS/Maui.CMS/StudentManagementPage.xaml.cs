using Library.CMS;
 
namespace Maui.CMS;
 
public partial class StudentManagementPage : ContentPage
{
    public StudentManagementPage()
    {
        InitializeComponent();
    }
 
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshList();
    }
 
    private void RefreshList()
    {
        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = CmsRepository.AllStudents;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var code = await DisplayPromptAsync("Add Student", "Enter the student's FSUID:");
        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }
 
        var name = await DisplayPromptAsync("Add Student", "Enter the student's name:");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var classification = await DisplayPromptAsync("Add Student", "Enter the student's classification:");
 
        EnrollmentService.CreateStudent(code, name, classification);
        RefreshList();
    }
 
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var student = (sender as Button)?.BindingContext as Student;
        if (student == null)
        {
            return;
        }
 
        var name = await DisplayPromptAsync("Edit Student", "Enter the new name:", initialValue: student.Name);
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var classification = await DisplayPromptAsync("Edit Student", "Enter the new classification:", initialValue: student.Classification);
 
        EnrollmentService.EditStudent(student, name, classification);
        RefreshList();
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var student = (sender as Button)?.BindingContext as Student;
        if (student == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert(
            "Delete Student",
            $"Delete {student.Name}? This removes them from every course they're enrolled in and deletes their submissions and grades.",
            "Delete",
            "Cancel");
 
        if (confirmed)
        {
            EnrollmentService.DeleteStudentFromSystem(student.Id);
            RefreshList();
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}