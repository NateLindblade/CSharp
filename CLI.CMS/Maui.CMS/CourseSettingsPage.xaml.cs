using Library.CMS;
using System.Linq;
 
namespace Maui.CMS;
 
[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseSettingsPage : ContentPage
{
    private Course course;
 
    public string CourseId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                course = CmsRepository.Courses.FirstOrDefault(c => c.Id == id);
                LoadCurrentValues();
            }
        }
    }
 
    public CourseSettingsPage()
    {
        InitializeComponent();
    }
 
    private void LoadCurrentValues()
    {
        if (course == null)
        {
            return;
        }
 
        AMinEntry.Text = course.AMinimum.ToString();
        BMinEntry.Text = course.BMinimum.ToString();
        CMinEntry.Text = course.CMinimum.ToString();
        DMinEntry.Text = course.DMinimum.ToString();
    }
 
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!double.TryParse(AMinEntry.Text, out double aMin) ||
            !double.TryParse(BMinEntry.Text, out double bMin) ||
            !double.TryParse(CMinEntry.Text, out double cMin) ||
            !double.TryParse(DMinEntry.Text, out double dMin))
        {
            await DisplayAlert("Invalid Input", "Every cutoff needs to be a number.", "OK");
            return;
        }
 
        // Basic sanity check - A should require more than B, and so on.
        // Not strictly required by the issue, but nonsensical cutoffs
        // (like a D needing a higher percentage than a B) would be confusing.
        if (!(aMin > bMin && bMin > cMin && cMin > dMin))
        {
            await DisplayAlert("Invalid Cutoffs", "Cutoffs must be in descending order: A > B > C > D.", "OK");
            return;
        }
 
        CourseService.UpdateGradeRanges(course, aMin, bMin, cMin, dMin);
        await DisplayAlert("Saved", "Grade cutoffs updated.", "OK");
    }
 
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}