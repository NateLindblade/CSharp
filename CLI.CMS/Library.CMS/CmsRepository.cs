using System.Collections.Generic;
 
namespace Library.CMS
{
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
 
  
