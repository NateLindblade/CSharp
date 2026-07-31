using System.Collections.Generic;
 
namespace Library.CMS
{
    // This holds the actual data (and the counters that generate stable Ids).
    // Both CLI.CMS and Maui.CMS will point at this same repository, so they
    // share one copy of the data instead of each keeping their own.
    public static class CmsRepository
    {
        public static List<Course> Courses = new List<Course>();
        public static int NextCourseId = 1;
 
        public static List<Student> AllStudents = new List<Student>();
        public static int NextStudentId = 1;
 
        public static int NextModuleId = 1;
        public static int NextModuleContentId = 1;
 
        public static int NextAssignmentId = 1;
        public static int NextSubmissionId = 1;
        public static int NextAssignmentGroupId = 1;
    }
}
 
  
