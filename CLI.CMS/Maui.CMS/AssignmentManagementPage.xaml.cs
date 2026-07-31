using Library.CMS;
using System;
using System.IO;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
public partial class AssignmentManagementPage : ContentPage
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
 
    public AssignmentManagementPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (course == null)
        {
            return;
        }
 
        Title = $"{course.Name} - Assignments";
        AssignmentsList.ItemsSource = null;
        AssignmentsList.ItemsSource = course.Assignments;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Assignment", "Enter the assignment name:");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var description = await DisplayPromptAsync("Add Assignment", "Enter the description:");
 
        var pointsInput = await DisplayPromptAsync("Add Assignment", "Enter the available points:", keyboard: Keyboard.Numeric);
        if (!int.TryParse(pointsInput, out int points))
        {
            await DisplayAlert("Invalid Points", "That's not a valid number of points.", "OK");
            return;
        }
 
        var dueDateInput = await DisplayPromptAsync("Add Assignment", "Enter the due date (MM/DD/YYYY):");
        if (!DateTime.TryParse(dueDateInput, out DateTime dueDate))
        {
            await DisplayAlert("Invalid Date", "That's not a valid date.", "OK");
            return;
        }
 
        AssignmentService.AddAssignment(course, name, description, points, dueDate);
        RefreshList();
    }
 
    private async void OnAddQuizClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Add Quiz", "Enter the quiz name:");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var question = await DisplayPromptAsync("Add Quiz", "Enter the question:");
        if (string.IsNullOrWhiteSpace(question))
        {
            await DisplayAlert("Question Required", "A quiz needs a question.", "OK");
            return;
        }
 
        var description = await DisplayPromptAsync("Add Quiz", "Enter the description:");
 
        var pointsInput = await DisplayPromptAsync("Add Quiz", "Enter the available points:", keyboard: Keyboard.Numeric);
        if (!int.TryParse(pointsInput, out int points))
        {
            await DisplayAlert("Invalid Points", "That's not a valid number of points.", "OK");
            return;
        }
 
        var dueDateInput = await DisplayPromptAsync("Add Quiz", "Enter the due date (MM/DD/YYYY):");
        if (!DateTime.TryParse(dueDateInput, out DateTime dueDate))
        {
            await DisplayAlert("Invalid Date", "That's not a valid date.", "OK");
            return;
        }
 
        AssignmentService.AddQuiz(course, name, description, points, dueDate, question);
        RefreshList();
    }
 
    private async void OnGradeClicked(object sender, EventArgs e)
    {
        var assignment = (sender as Button)?.BindingContext as Assignment;
        if (assignment == null)
        {
            return;
        }
 
        await Shell.Current.GoToAsync($"{nameof(GradeSubmissionsPage)}?courseId={course.Id}&assignmentId={assignment.Id}");
    }
 
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var assignment = (sender as Button)?.BindingContext as Assignment;
        if (assignment == null)
        {
            return;
        }
 
        var name = await DisplayPromptAsync("Edit Assignment", "Enter the new name:", initialValue: assignment.Name);
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
 
        var description = await DisplayPromptAsync("Edit Assignment", "Enter the new description:", initialValue: assignment.Description);
 
        var pointsInput = await DisplayPromptAsync("Edit Assignment", "Enter the new available points:",
            initialValue: assignment.AvailablePoints.ToString(), keyboard: Keyboard.Numeric);
        int? newPoints = int.TryParse(pointsInput, out int parsedPoints) ? parsedPoints : (int?)null;
 
        var dueDateInput = await DisplayPromptAsync("Edit Assignment", "Enter the new due date (MM/DD/YYYY):",
            initialValue: assignment.DueDate.ToShortDateString());
        DateTime? newDueDate = DateTime.TryParse(dueDateInput, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
 
        AssignmentService.UpdateAssignment(assignment, name, description, newPoints, newDueDate);
        RefreshList();
    }
 
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        var assignment = (sender as Button)?.BindingContext as Assignment;
        if (assignment == null)
        {
            return;
        }
 
        // Every other course - can't copy an assignment into the same course it's already in.
        var otherCourses = CmsRepository.Courses.Where(c => c.Id != course.Id).ToList();
 
        if (otherCourses.Count == 0)
        {
            await DisplayAlert("No Other Courses", "There are no other courses to copy this assignment to.", "OK");
            return;
        }
 
        var courseLabels = otherCourses.Select(c => $"{c.Name} ({c.Code})").ToArray();
        var choice = await DisplayActionSheet("Copy to which course?", "Cancel", null, courseLabels);
 
        var targetIndex = Array.IndexOf(courseLabels, choice);
        if (targetIndex < 0)
        {
            return;
        }
 
        var targetCourse = otherCourses[targetIndex];
 
        // AddAssignment never copies Submissions - it always starts a new
        // assignment with an empty submissions list, which is exactly what
        // this issue asks for.
        AssignmentService.AddAssignment(targetCourse, assignment.Name, assignment.Description, assignment.AvailablePoints, assignment.DueDate);
 
        await DisplayAlert("Copied", $"'{assignment.Name}' was copied to {targetCourse.Name}.", "OK");
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var assignment = (sender as Button)?.BindingContext as Assignment;
        if (assignment == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert(
            "Delete Assignment",
            $"Delete '{assignment.Name}'? This also deletes every student's submission for it.",
            "Delete",
            "Cancel");
 
        if (confirmed)
        {
            AssignmentService.DeleteAssignment(course, assignment.Id);
            RefreshList();
        }
    }
 
    private async void OnExportClicked(object sender, EventArgs e)
    {
        var defaultPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{course.Code}_assignments.csv");
 
        var path = await DisplayPromptAsync("Export Assignments", "Save to which file path?", initialValue: defaultPath);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
 
        var content = AssignmentService.BuildExportContent(course);
        File.WriteAllText(path, content);
 
        await DisplayAlert("Exported", $"Assignments saved to:\n{path}", "OK");
    }
 
    private async void OnImportClicked(object sender, EventArgs e)
    {
        var path = await DisplayPromptAsync("Import Assignments", "Enter the file path to import from:");
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
        int added = AssignmentService.ImportAssignments(course, content);
 
        RefreshList();
        await DisplayAlert("Imported", $"{added} assignment(s) added.", "OK");
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}