using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(ModuleId), "moduleId")]
public partial class ModuleContentPage : ContentPage
{
    private Course course;
    private Module module;
 
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
 
    public string ModuleId
    {
        set
        {
            if (int.TryParse(value, out int id) && course != null)
            {
                module = course.Modules.FirstOrDefault(m => m.Id == id);
                RefreshList();
            }
        }
    }
 
    // A small local pairing of each content item with its display text, since
    // ModuleService.DescribeContent is a method and XAML can only bind to properties.
    private class ContentRow
    {
        public ModuleContent Content { get; set; }
        public string Description { get; set; }
    }
 
    public ModuleContentPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (module == null)
        {
            return;
        }
 
        Title = $"Module {module.Id} Content";
        ContentList.ItemsSource = module.Content
            .Select(c => new ContentRow { Content = c, Description = ModuleService.DescribeContent(c) })
            .ToList();
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var choice = await DisplayActionSheet("Add Content", "Cancel", null, "Page", "File", "Assignment");
 
        if (choice == "Page")
        {
            var text = await DisplayPromptAsync("Add Page", "Enter the page content:");
            if (!string.IsNullOrWhiteSpace(text))
            {
                ModuleService.AddPageContent(module, text);
            }
        }
        else if (choice == "File")
        {
            var fileName = await DisplayPromptAsync("Add File", "Enter the file name (e.g. syllabus.pdf):");
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
 
            var filePath = await DisplayPromptAsync("Add File", "Enter the file path:");
            ModuleService.AddFileContent(module, fileName, filePath);
        }
        else if (choice == "Assignment")
        {
            if (course.Assignments.Count == 0)
            {
                await DisplayAlert("No Assignments", "This course has no assignments to embed yet.", "OK");
                return;
            }
 
            var assignmentNames = course.Assignments.Select(a => a.Name).ToArray();
            var assignmentChoice = await DisplayActionSheet("Choose an assignment", "Cancel", null, assignmentNames);
 
            var assignment = course.Assignments.FirstOrDefault(a => a.Name == assignmentChoice);
            if (assignment != null)
            {
                ModuleService.AddAssignmentContent(module, assignment);
            }
        }
        else
        {
            return;
        }
 
        RefreshList();
    }
 
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var row = (sender as Button)?.BindingContext as ContentRow;
        if (row == null)
        {
            return;
        }
 
        if (row.Content is PageContent page)
        {
            var newText = await DisplayPromptAsync("Edit Page", "Enter the new page content:", initialValue: page.Content);
            if (newText != null)
            {
                page.Content = newText;
            }
        }
        else if (row.Content is FileContent file)
        {
            var newName = await DisplayPromptAsync("Edit File", "Enter the new file name:", initialValue: file.FileName);
            var newPath = await DisplayPromptAsync("Edit File", "Enter the new file path:", initialValue: file.FilePath);
            if (newName != null)
            {
                file.FileName = newName;
            }
            if (newPath != null)
            {
                file.FilePath = newPath;
            }
        }
        else if (row.Content is AssignmentContent assignmentContent)
        {
            var assignmentNames = course.Assignments.Select(a => a.Name).ToArray();
            var assignmentChoice = await DisplayActionSheet("Change embedded assignment to:", "Cancel", null, assignmentNames);
 
            var newAssignment = course.Assignments.FirstOrDefault(a => a.Name == assignmentChoice);
            if (newAssignment != null)
            {
                assignmentContent.Assignment = newAssignment;
            }
        }
 
        RefreshList();
    }
 
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var row = (sender as Button)?.BindingContext as ContentRow;
        if (row == null)
        {
            return;
        }
 
        bool confirmed = await DisplayAlert("Delete Content", $"Delete '{row.Description}'?", "Delete", "Cancel");
 
        if (confirmed)
        {
            ModuleService.RemoveContent(module, row.Content);
            RefreshList();
        }
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}