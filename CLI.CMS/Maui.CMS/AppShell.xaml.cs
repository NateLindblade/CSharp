namespace Maui.CMS;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(TeacherPage), typeof(TeacherPage));
		Routing.RegisterRoute(nameof(StudentPage), typeof(StudentPage));
		Routing.RegisterRoute(nameof(StudentCoursesPage), typeof(StudentCoursesPage));
		Routing.RegisterRoute(nameof(CourseDetailPage), typeof(CourseDetailPage));
		Routing.RegisterRoute(nameof(StudentManagementPage), typeof(StudentManagementPage));
		Routing.RegisterRoute(nameof(TeacherCoursesPage), typeof(TeacherCoursesPage));
		Routing.RegisterRoute(nameof(RosterManagementPage), typeof(RosterManagementPage));
		Routing.RegisterRoute(nameof(AssignmentManagementPage), typeof(AssignmentManagementPage));
		Routing.RegisterRoute(nameof(ModuleManagementPage), typeof(ModuleManagementPage));
		Routing.RegisterRoute(nameof(ModuleContentPage), typeof(ModuleContentPage));
		Routing.RegisterRoute(nameof(AnnouncementManagementPage), typeof(AnnouncementManagementPage));
		Routing.RegisterRoute(nameof(CourseSettingsPage), typeof(CourseSettingsPage));
		Routing.RegisterRoute(nameof(GradeSubmissionsPage), typeof(GradeSubmissionsPage));
	}
}
