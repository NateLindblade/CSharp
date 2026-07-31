using System;
using System.Collections.Generic;
using Library.CMS;
using System.IO;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(IsTeacher), "isTeacher")]
[QueryProperty(nameof(StudentId), "studentId")]
public partial class CourseDetailPage : ContentPage
{
    private Course course;
    private Student currentStudent;
 
    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                course = CmsRepository.Courses.FirstOrDefault(c => c.Id == id);
                LoadCourse();
            }
        }
    }
 
    // Set via ?isTeacher=true when navigated to from TeacherCoursesPage.
    // Students navigating here never set this, so it stays hidden for them.
    public string IsTeacher
    {
        set
        {
            ManageRosterButton.IsVisible = value == "true";
            ManageAssignmentsButton.IsVisible = value == "true";
            ManageModulesButton.IsVisible = value == "true";
            ManageAnnouncementsButton.IsVisible = value == "true";
            CourseSettingsButton.IsVisible = value == "true";
            ExportGradeBookButton.IsVisible = value == "true";
        }
    }
 
    // Set via ?studentId=... when navigated to from StudentCoursesPage.
    // Only present when a student is viewing, which is what unlocks the grades section.
    public string StudentId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                currentStudent = CmsRepository.AllStudents.FirstOrDefault(s => s.Id == id);
                LoadGrades();
            }
        }
    }
 
    public CourseDetailPage()
    {
        InitializeComponent();
    }
 
    private void LoadCourse()
    {
        if (course == null)
        {
            return;
        }
 
        Title = course.Name;
        CourseNameLabel.Text = $"{course.Name} ({course.Code})";
 
        var moduleLines = new List<string>();
        foreach (var module in course.Modules)
        {
            foreach (var content in module.Content)
            {
                moduleLines.Add($"Module {module.Id} - {ModuleService.DescribeContent(content)}");
            }
        }
        ModulesList.ItemsSource = moduleLines;
 
        var assignmentLines = course.Assignments
            .Select(a =>
            {
                var line = $"{a.Name} - {a.AvailablePoints} points - due {a.DueDate.ToShortDateString()}";
                if (a is Quiz quiz)
                {
                    line += $"\nQuestion: {quiz.Question}";
                }
                return line;
            })
            .ToList();
        AssignmentsList.ItemsSource = assignmentLines;
    }
 
    private void LoadGrades()
    {
        if (course == null || currentStudent == null)
        {
            return;
        }
 
        GradesHeaderLabel.IsVisible = true;
        GradesList.IsVisible = true;
        FinalGradeLabel.IsVisible = true;
 
        SubmitHeaderLabel.IsVisible = true;
        AssignmentPicker.IsVisible = true;
        SubmissionEditor.IsVisible = true;
        ChooseFileButton.IsVisible = true;
        SubmitButton.IsVisible = true;
        AssignmentPicker.ItemsSource = course.Assignments;
 
        // Announcements only show up in the student view, per issue #42.
        if (course.Announcements.Count > 0)
        {
            AnnouncementsList.IsVisible = true;
            AnnouncementsList.ItemsSource = course.Announcements;
        }
 
        RefreshGradeLines();
    }
 
    private void RefreshGradeLines()
    {
        var gradeLines = new List<string>();
        foreach (var assignment in course.Assignments)
        {
            var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == currentStudent.Id);
 
            if (submission == null)
            {
                gradeLines.Add($"{assignment.Name}: Not submitted");
            }
            else if (submission.Grade.HasValue)
            {
                gradeLines.Add($"{assignment.Name}: {submission.Grade.Value}/{assignment.AvailablePoints}");
            }
            else
            {
                var fileNote = string.IsNullOrEmpty(submission.FileName) ? "" : $" (file: {submission.FileName})";
                gradeLines.Add($"{assignment.Name}: Submitted{fileNote}, not yet graded");
            }
        }
        GradesList.ItemsSource = gradeLines;
 
        var finalGrade = AssignmentGroupService.CalculateFinalGrade(course, currentStudent);
 
        CurrentGradeLabel.IsVisible = true;
        CurrentGradeLabel.Text = finalGrade.HasValue
            ? $"Current Grade: {AssignmentGroupService.GetLetterGrade(course, finalGrade.Value)}"
            : "Current Grade: N/A";
 
        FinalGradeLabel.Text = finalGrade.HasValue
            ? $"Weighted course average: {finalGrade.Value:0.##}%"
            : "Weighted course average: not available yet";
    }
 
    private string selectedFileName;
    private string selectedFilePath;
 
    private async void OnChooseFileClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
 
            if (result != null)
            {
                selectedFileName = result.FileName;
                selectedFilePath = result.FullPath;
                SelectedFileLabel.IsVisible = true;
                SelectedFileLabel.Text = $"Attached: {selectedFileName}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Could Not Pick File", ex.Message, "OK");
        }
    }
 
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (AssignmentPicker.SelectedItem is not Assignment assignment)
        {
            await DisplayAlert("Pick an Assignment", "Choose which assignment you're submitting for.", "OK");
            return;
        }
 
        if (string.IsNullOrWhiteSpace(SubmissionEditor.Text))
        {
            await DisplayAlert("Empty Response", "Enter a response before submitting.", "OK");
            return;
        }
 
        AssignmentService.SubmitAssignment(assignment, currentStudent, SubmissionEditor.Text, selectedFileName, selectedFilePath);
 
        SubmissionEditor.Text = string.Empty;
        AssignmentPicker.SelectedItem = null;
        selectedFileName = null;
        selectedFilePath = null;
        SelectedFileLabel.IsVisible = false;
        SelectedFileLabel.Text = string.Empty;
 
        // Refresh so "Not submitted" immediately updates to "Submitted, not yet graded".
        RefreshGradeLines();
 
        await DisplayAlert("Submitted", "Your response has been submitted.", "OK");
    }
 
    private async void OnManageRosterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(RosterManagementPage)}?courseId={course.Id}");
    }
 
    private async void OnManageAssignmentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AssignmentManagementPage)}?courseId={course.Id}");
    }
 
    private async void OnManageModulesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ModuleManagementPage)}?courseId={course.Id}");
    }
 
    private async void OnManageAnnouncementsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AnnouncementManagementPage)}?courseId={course.Id}");
    }
 
    private async void OnCourseSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CourseSettingsPage)}?courseId={course.Id}");
    }
 
    private async void OnExportGradeBookClicked(object sender, EventArgs e)
    {
        var defaultPath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{course.Code}_gradebook.csv");
 
        var path = await DisplayPromptAsync("Export Grade Book", "Save to which file path?", initialValue: defaultPath);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }
 
        var content = GradeBookService.BuildGradeBookCsv(course);
        File.WriteAllText(path, content);
 
        await DisplayAlert("Exported", $"Grade book saved to:\n{path}", "OK");
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}