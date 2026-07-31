using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCoursesPage : ContentPage
{
    private Student student;
 
    // Set automatically by Shell when navigated to with ?studentId=...
    // (see StudentPage.OnStudentSelected). Parsing the Id and loading the
    // courses happens right here rather than a separate method, since this
    // setter only ever needs to run once per navigation.
    public string StudentId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                student = CmsRepository.AllStudents.FirstOrDefault(s => s.Id == id);
                LoadCourses();
            }
        }
    }
 
    public StudentCoursesPage()
    {
        InitializeComponent();
    }
 
    private void LoadCourses()
    {
        if (student == null)
        {
            return;
        }
 
        Title = $"{student.Name}'s Courses";
        CoursesList.ItemsSource = EnrollmentService.GetEnrolledCourses(student.Id);
    }
 
    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Course course)
        {
            await Shell.Current.GoToAsync($"{nameof(CourseDetailPage)}?courseId={course.Id}&studentId={student.Id}");
            CoursesList.SelectedItem = null;
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
 