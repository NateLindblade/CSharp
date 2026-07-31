using Library.CMS;
using System.IO;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
public partial class RosterManagementPage : ContentPage
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
 
    public RosterManagementPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (course == null)
        {
            return;
        }
 
        Title = $"{course.Name} - Roster";
        RosterList.ItemsSource = null;
        RosterList.ItemsSource = course.Roster;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var code = await DisplayPromptAsync("Add to Roster", "Enter the student's FSUID:");
        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }
 
        // Reuse the same student if this FSUID already exists anywhere in the
        // system, instead of creating a duplicate - same rule as the console app.
        var student = EnrollmentService.FindStudentByCode(code);
 
        if (student == null)
        {
            var name = await DisplayPromptAsync("Add to Roster", "This student doesn't exist yet. Enter their name:");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
 
            var classification = await DisplayPromptAsync("Add to Roster", "Enter their classification:");
            student = EnrollmentService.CreateStudent(code, name, classification);
        }
 
        bool enrolled = EnrollmentService.EnrollStudentInCourse(course, student);
 
        if (!enrolled)
        {
            await DisplayAlert("Already Enrolled", $"{student.Name} is already on this course's roster.", "OK");
        }
 
        RefreshList();
    }
 
    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        var student = (sender as Button)?.BindingContext as Student;
        if (student == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert(
            "Remove Student",
            $"Remove {student.Name} from this course's roster?",
            "Remove",
            "Cancel");
 
        if (confirmed)
        {
            EnrollmentService.RemoveFromRoster(course, student.Id);
            RefreshList();
        }
    }
 
    private async void OnExportClicked(object sender, EventArgs e)
    {
        var defaultPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{course.Code}_roster.csv");
 
        var path = await DisplayPromptAsync("Export Roster", "Save to which file path?", initialValue: defaultPath);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
 
        var content = RosterService.BuildExportContent(course);
        File.WriteAllText(path, content);
 
        await DisplayAlert("Exported", $"Roster saved to:\n{path}", "OK");
    }
 
    private async void OnImportClicked(object sender, EventArgs e)
    {
        var path = await DisplayPromptAsync("Import Roster", "Enter the file path to import from:");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
 
        if (!File.Exists(path))
        {
            await DisplayAlert("File Not Found", $"No file found at:\n{path}", "OK");
            return;
        }
 
        var content = File.ReadAllText(path);
        int added = RosterService.ImportRoster(course, content);
 
        RefreshList();
        await DisplayAlert("Imported", $"{added} new student(s) added to the roster.", "OK");
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}