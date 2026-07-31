using System.Linq;
using System.Text;
 
namespace Library.CMS
{
    public static class GradeBookService
    {
        // Header row: Student,<assignment 1>,<assignment 2>,...
        // Then one row per student, with their grade (blank if ungraded or
        // not submitted) under each assignment's column.
        public static string BuildGradeBookCsv(Course course)
        {
            var sb = new StringBuilder();
 
            sb.Append("Student");
            foreach (var assignment in course.Assignments)
            {
                sb.Append($",{assignment.Name}");
            }
            sb.AppendLine();
 
            foreach (var student in course.Roster)
            {
                sb.Append(student.Name);
 
                foreach (var assignment in course.Assignments)
                {
                    var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                    var gradeText = submission != null && submission.Grade.HasValue ? submission.Grade.Value.ToString() : "";
                    sb.Append($",{gradeText}");
                }
 
                sb.AppendLine();
            }
 
            return sb.ToString();
        }
    }
}