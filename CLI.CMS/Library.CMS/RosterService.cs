using System;
using System.Text;
 
namespace Library.CMS
{
    public static class RosterService
    {
        public static string BuildExportContent(Course course)
        {
            var sb = new StringBuilder();
 
            foreach (var student in course.Roster)
            {
                sb.AppendLine($"{student.Code},{student.Name},{student.Classification}");
            }
 
            return sb.ToString();
        }
 
        public static int ImportRoster(Course course, string fileContent)
        {
            int newlyEnrolled = 0;
 
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
 
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
 
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
 
                var parts = line.Split(',');
 
                if (parts.Length < 2)
                {
                    continue;
                }
 
                var code = parts[0].Trim();
                var name = parts[1].Trim();
                var classification = parts.Length > 2 ? parts[2].Trim() : string.Empty;
 
                var student = EnrollmentService.FindStudentByCode(code);
 
                if (student == null)
                {
                    student = EnrollmentService.CreateStudent(code, name, classification);
                }
 
                bool enrolled = EnrollmentService.EnrollStudentInCourse(course, student);
 
                if (enrolled)
                {
                    newlyEnrolled++;
                }
            }
 
            return newlyEnrolled;
        }
    }
}
