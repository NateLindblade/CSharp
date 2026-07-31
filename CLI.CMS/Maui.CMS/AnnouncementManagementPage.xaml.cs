using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
public partial class AnnouncementManagementPage : ContentPage
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
 
    public AnnouncementManagementPage()
    {
        InitializeComponent();
    }
 
    private void RefreshList()
    {
        if (course == null)
        {
            return;
        }
 
        Title = $"{course.Name} - Announcements";
        AnnouncementsList.ItemsSource = null;
        AnnouncementsList.ItemsSource = course.Announcements;
    }
 
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var text = await DisplayPromptAsync("Add Announcement", "Enter the announcement text:");
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
 
        CourseService.AddAnnouncement(course, text);
        RefreshList();
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}