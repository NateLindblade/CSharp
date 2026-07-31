using System.Collections.Generic;
using System.Linq;
 
namespace Library.CMS
{
    public static class EnrollmentService
    {
        // Looks for an existing student by FSUID. Returns null if none exists yet -
        // the caller (UI) uses that to decide whether it needs to ask for a name.
        public static Student FindStudentByCode(string code)
        {
            return CmsRepository.AllStudents.FirstOrDefault(s => s.Code == code);
        }
 
        // Only one list of students should ever exist (CmsRepository.AllStudents).
        // This creates a brand new one and adds them to that shared list.
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
 
        // Adds an existing student to a course's roster (a shallow reference,
        // not a copy). Returns false if they were already enrolled.
        public static bool EnrollStudentInCourse(Course course, Student student)
        {
            if (course.Roster.Any(s => s.Id == student.Id))
            {
                return false;
            }
 
            course.Roster.Add(student);
            return true;
        }
 
        // Removes a student from one course's roster only - they still exist in
        // CmsRepository.AllStudents and stay enrolled in any other courses.
        // Used for both a teacher unenrolling someone and a student leaving themselves.
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
 
        // All courses (from the shared repository) that this student is on the roster for.
        public static List<Course> GetEnrolledCourses(int studentId)
        {
            return CmsRepository.Courses
                .Where(c => c.Roster.Any(s => s.Id == studentId))
                .ToList();
        }
 
        // FSUID (Code) is treated as a fixed identifier, so only name and
        // classification are editable here.
        public static void EditStudent(Student student, string name, string classification)
        {
            student.Name = name;
            student.Classification = classification;
        }
 
        // Removes a student from the system entirely: their enrollment in every
        // course, and every submission (and its grade, since Grade lives on the
        // Submission) they ever made in any course. Returns false if no such student exists.
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