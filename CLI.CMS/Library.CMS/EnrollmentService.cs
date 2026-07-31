using System.Collections.Generic;
using System.Linq;
 
namespace Library.CMS
{
    public static class EnrollmentService
    {
        public static Student FindStudentByCode(string code)
        {
            return CmsRepository.AllStudents.FirstOrDefault(s => s.Code == code);
        }
 
        public static Student CreateStudent(string code, string name, string classification)
        {
            var student = new Student
            {
                Id = CmsRepository.NextStudentId,
                Name = name,
                Code = code,
                Classification = classification
            };
 
            CmsRepository.AllStudents.Add(student);
            CmsRepository.NextStudentId++;
 
            return student;
        }
 
        public static bool EnrollStudentInCourse(Course course, Student student)
        {
            if (course.Roster.Any(s => s.Id == student.Id))
            {
                return false;
            }
 
            course.Roster.Add(student);
            return true;
        }
 
        public static bool RemoveFromRoster(Course course, int studentId)
        {
            var match = course.Roster.FirstOrDefault(s => s.Id == studentId);
 
            if (match == null)
            {
                return false;
            }
 
            course.Roster.Remove(match);
            return true;
        }
 
        public static List<Course> GetEnrolledCourses(int studentId)
        {
            return CmsRepository.Courses
                .Where(c => c.Roster.Any(s => s.Id == studentId))
                .ToList();
        }
 
        public static void EditStudent(Student student, string name, string classification)
        {
            student.Name = name;
            student.Classification = classification;
        }
 
        public static bool DeleteStudentFromSystem(int studentId)
        {
            var student = CmsRepository.AllStudents.FirstOrDefault(s => s.Id == studentId);
 
            if (student == null)
            {
                return false;
            }
 
            foreach (var course in CmsRepository.Courses)
            {
                RemoveFromRoster(course, studentId);
 
                foreach (var assignment in course.Assignments)
                {
                    assignment.Submissions.RemoveAll(sub => sub.StudentId == studentId);
                }
            }
 
            CmsRepository.AllStudents.Remove(student);
            return true;
        }
    }
}
