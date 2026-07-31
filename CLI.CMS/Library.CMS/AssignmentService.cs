using System;
using System.Linq;
using System.Text;
 
namespace Library.CMS
{
    public static class AssignmentService
    {
        public static Assignment AddAssignment(Course course, string name, string description, int availablePoints, DateTime dueDate)
        {
            var assignment = new Assignment
            {
                Id = CmsRepository.NextAssignmentId,
                Name = name,
                Description = description,
                AvailablePoints = availablePoints,
                DueDate = dueDate
            };
 
            course.Assignments.Add(assignment);
            CmsRepository.NextAssignmentId++;
 
            return assignment;
        }
 
        // Same idea as AddAssignment, but creates a Quiz (which just adds a
        // Question on top of everything a regular Assignment already has).
        public static Quiz AddQuiz(Course course, string name, string description, int availablePoints, DateTime dueDate, string question)
        {
            var quiz = new Quiz
            {
                Id = CmsRepository.NextAssignmentId,
                Name = name,
                Description = description,
                AvailablePoints = availablePoints,
                DueDate = dueDate,
                Question = question
            };
 
            course.Assignments.Add(quiz);
            CmsRepository.NextAssignmentId++;
 
            return quiz;
        }
 
        public static bool DeleteAssignment(Course course, int assignmentId)
        {
            var match = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
 
            if (match == null)
            {
                return false;
            }
 
            course.Assignments.Remove(match);
            return true;
        }
 
        public static void UpdateAssignment(Assignment assignment, string name, string description, int? newPoints, DateTime? newDueDate)
        {
            assignment.Name = name;
            assignment.Description = description;
 
            if (newPoints.HasValue)
            {
                assignment.AvailablePoints = newPoints.Value;
            }
 
            if (newDueDate.HasValue)
            {
                assignment.DueDate = newDueDate.Value;
            }
        }
 
        public static Submission SubmitAssignment(Assignment assignment, Student student, string content, string fileName = null, string filePath = null)
        {
            var submission = new Submission
            {
                Id = CmsRepository.NextSubmissionId,
                StudentId = student.Id,
                AssignmentId = assignment.Id,
                Content = content,
                SubmissionDate = DateTime.Now,
                FileName = fileName,
                FilePath = filePath
            };
 
            assignment.Submissions.Add(submission);
            CmsRepository.NextSubmissionId++;
 
            return submission;
        }
 
        public static void GradeByPoints(Submission submission, int points)
        {
            submission.Grade = points;
        }
 
        public static void GradeByPercentage(Submission submission, int availablePoints, double percent)
        {
            submission.Grade = (int)Math.Round(percent / 100 * availablePoints);
        }
 
        public static void SetComment(Submission submission, string comment)
        {
            submission.Comment = comment;
        }
 
        public static string BuildExportContent(Course course)
        {
            var sb = new StringBuilder();
 
            foreach (var assignment in course.Assignments)
            {
                sb.AppendLine($"{assignment.Name},{assignment.Description},{assignment.AvailablePoints},{assignment.DueDate:MM/dd/yyyy}");
            }
 
            return sb.ToString();
        }
 
        public static int ImportAssignments(Course course, string fileContent)
        {
            int added = 0;
 
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
 
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
 
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
 
                var parts = line.Split(',');
 
                if (parts.Length < 4)
                {
                    continue;
                }
 
                var name = parts[0].Trim();
                var description = parts[1].Trim();
 
                if (!int.TryParse(parts[2].Trim(), out int points))
                {
                    continue;
                }
 
                if (!DateTime.TryParse(parts[3].Trim(), out DateTime dueDate))
                {
                    continue;
                }
 
                AddAssignment(course, name, description, points, dueDate);
                added++;
            }
 
            return added;
        }
    }
}
