using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
public partial class TeacherCoursesPage : ContentPage
{
    public TeacherCoursesPage()
    {
        InitializeComponent();
    }
 
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CoursesList.ItemsSource = CmsRepository.Courses;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var code = await DisplayPromptAsync("Add Course", "Enter the course code:");
        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }
 
        var name = await DisplayPromptAsync("Add Course", "Enter the course name:");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var description = await DisplayPromptAsync("Add Course", "Enter the course description:");
        var semester = await DisplayPromptAsync("Add Course", "Enter the semester (e.g. Fall 2026):");
        var section = await DisplayPromptAsync("Add Course", "Enter the section (e.g. 01):");
 
        CourseService.AddCourse(code, name, description, semester, section);
 
        CoursesList.ItemsSource = null;
        CoursesList.ItemsSource = CmsRepository.Courses;
    }
 
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        var course = (sender as Button)?.BindingContext as Course;
        if (course == null)
        {
            return;
        }
 
        // CourseService.CopyCourse already deep-copies modules, assignments,
        // and groups while excluding roster/submissions - same logic the
        // console app's Copy Course option uses.
        var newCourse = CourseService.CopyCourse(course.Id);
 
        CoursesList.ItemsSource = null;
        CoursesList.ItemsSource = CmsRepository.Courses;
 
        if (newCourse != null)
        {
            await DisplayAlert("Copied", $"'{course.Name}' was copied to a new course: '{newCourse.Name}'.", "OK");
        }
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var course = (sender as Button)?.BindingContext as Course;
        if (course == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert(
            "Delete Course",
            $"Delete '{course.Name}'? Students stay enrolled in their other courses; only this course is removed.",
            "Delete",
            "Cancel");
 
        if (confirmed)
        {
            CourseService.DeleteCourse(course.Id);
            CoursesList.ItemsSource = null;
            CoursesList.ItemsSource = CmsRepository.Courses;
        }
    }
 
    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Course course)
        {
            // isTeacher=true tells CourseDetailPage to show teacher-only actions
            // (like managing the roster) instead of the student-facing view.
            await Shell.Current.GoToAsync($"{nameof(CourseDetailPage)}?courseId={course.Id}&isTeacher=true");
            CoursesList.SelectedItem = null;
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
