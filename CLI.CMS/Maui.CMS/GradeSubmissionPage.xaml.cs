using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class GradeSubmissionsPage : ContentPage
{
    private Course course;
    private Assignment assignment;
 
    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                course = CmsRepository.Courses.FirstOrDefault(c => c.Id == id);
            }
        }
    }
 
    public string AssignmentId
    {
        set
        {
            if (int.TryParse(value, out int id) && course != null)
            {
                assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
                RefreshList();
            }
        }
    }
 
    // Pairs a real Submission with display-only text, since a Submission only
    // stores a StudentId - the readable name and summary get built here.
    private class SubmissionRow
    {
        public Submission Submission { get; set; }
        public string StudentName { get; set; }
        public string Summary { get; set; }
        public bool HasFile { get; set; }
    }
 
    public GradeSubmissionsPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (assignment == null)
        {
            return;
        }
 
        Title = $"Grade: {assignment.Name}";
 
        SubmissionsList.ItemsSource = assignment.Submissions.Select(s =>
        {
            var student = CmsRepository.AllStudents.FirstOrDefault(st => st.Id == s.StudentId);
            var studentName = student != null ? student.Name : $"Student {s.StudentId}";
 
            var gradeText = s.Grade.HasValue ? $"{s.Grade.Value}/{assignment.AvailablePoints}" : "ungraded";
            var fileText = string.IsNullOrEmpty(s.FileName) ? "" : $" | File: {s.FileName}";
            var commentText = string.IsNullOrEmpty(s.Comment) ? "" : $" | Comment: {s.Comment}";
 
            return new SubmissionRow
            {
                Submission = s,
                StudentName = studentName,
                Summary = $"{s.Content} | Grade: {gradeText}{fileText}{commentText}",
                HasFile = !string.IsNullOrEmpty(s.FilePath)
            };
        }).ToList();
    }
 
    private async void OnGradeClicked(object sender, EventArgs e)
    {
        var row = (sender as Button)?.BindingContext as SubmissionRow;
        if (row == null)
        {
            return;
        }
 
        var mode = await DisplayActionSheet("Grade using:", "Cancel", null, "Points", "Percentage");
 
        if (mode == "Points")
        {
            var pointsInput = await DisplayPromptAsync("Grade", $"Enter points earned (out of {assignment.AvailablePoints}):", keyboard: Keyboard.Numeric);
            if (!int.TryParse(pointsInput, out int points))
            {
                await DisplayAlert("Invalid", "That's not a valid number of points.", "OK");
                return;
            }
 
            AssignmentService.GradeByPoints(row.Submission, points);
        }
        else if (mode == "Percentage")
        {
            var percentInput = await DisplayPromptAsync("Grade", "Enter percentage (e.g. 90 for 90%):", keyboard: Keyboard.Numeric);
            if (!double.TryParse(percentInput, out double percent))
            {
                await DisplayAlert("Invalid", "That's not a valid percentage.", "OK");
                return;
            }
 
            AssignmentService.GradeByPercentage(row.Submission, assignment.AvailablePoints, percent);
        }
        else
        {
            return;
        }
 
        var comment = await DisplayPromptAsync("Feedback", "Enter a comment for the student (or leave blank):", initialValue: row.Submission.Comment);
        AssignmentService.SetComment(row.Submission, comment);
 
        RefreshList();
    }
 
    private async void OnOpenFileClicked(object sender, EventArgs e)
    {
        var row = (sender as Button)?.BindingContext as SubmissionRow;
        if (row == null || string.IsNullOrEmpty(row.Submission.FilePath))
        {
            return;
        }
 
        try
        {
            // Launcher is MAUI's cross-platform equivalent of the console
            // app's Process.Start - the right way to open a file from a
            // shared UI project, since Process.Start is Windows/desktop-specific.
            await Launcher.Default.OpenAsync(new OpenFileRequest(row.Submission.FileName, new ReadOnlyFile(row.Submission.FilePath)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Could Not Open File", ex.Message, "OK");
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}