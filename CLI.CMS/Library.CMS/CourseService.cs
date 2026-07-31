using System;
using System.Collections.Generic;
using System.Linq;
 
namespace Library.CMS
{
    public static class CourseService
    {
        public static Course AddCourse(string code, string name, string description, string semester, string section)
        {
            var course = new Course
            {
                Id = CmsRepository.NextCourseId,
                Code = code,
                Name = name,
                Description = description,
                Semester = semester,
                Section = section
            };
 
            CmsRepository.Courses.Add(course);
            CmsRepository.NextCourseId++;
 
            return course;
        }
 
        // Returns true if a course with this Id was found and removed.
        public static bool DeleteCourse(int courseId)
        {
            var match = CmsRepository.Courses.FirstOrDefault(c => c.Id == courseId);
 
            if (match == null)
            {
                return false;
            }
 
            CmsRepository.Courses.Remove(match);
            return true;
        }
 
        // Returns true if the course was found and updated.
        public static bool UpdateDescription(int courseId, string newDescription)
        {
            var course = CmsRepository.Courses.FirstOrDefault(c => c.Id == courseId);
 
            if (course == null)
            {
                return false;
            }
 
            course.Description = newDescription;
            return true;
        }
 
        // Returns every course if semesterFilter is empty, otherwise only the
        // ones matching it (case-insensitive) - used by issue #26.
        public static List<Course> FilterBySemester(string semesterFilter)
        {
            if (string.IsNullOrEmpty(semesterFilter))
            {
                return CmsRepository.Courses;
            }
 
            return CmsRepository.Courses
                .Where(c => c.Semester != null && c.Semester.Equals(semesterFilter, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
 
        // Deep copies everything about a course except its roster and student
        // submissions (issue #24). Built by composing the other services, so
        // every copied piece gets its own real, unique Id the normal way.
        public static Course CopyCourse(int courseId)
        {
            var original = CmsRepository.Courses.FirstOrDefault(c => c.Id == courseId);
 
            if (original == null)
            {
                return null;
            }
 
            // AddCourse already gives us an empty Roster - copies shouldn't bring students along.
            var newCourse = AddCourse(original.Code, original.Name, original.Description, original.Semester, original.Section);
 
            // Copy assignments first, and remember which new Assignment matches
            // which original one - modules and groups both reference assignments,
            // and they need to point at the *new* copies, not the originals.
            var assignmentMap = new Dictionary<int, Assignment>();
 
            foreach (var assignment in original.Assignments)
            {
                // AssignmentService.AddAssignment doesn't copy Submissions - excluded on purpose.
                var newAssignment = AssignmentService.AddAssignment(
                    newCourse, assignment.Name, assignment.Description, assignment.AvailablePoints, assignment.DueDate);
 
                assignmentMap[assignment.Id] = newAssignment;
            }
 
            foreach (var module in original.Modules)
            {
                var newModule = ModuleService.AddModule(newCourse);
 
                foreach (var content in module.Content)
                {
                    if (content is PageContent page)
                    {
                        ModuleService.AddPageContent(newModule, page.Content);
                    }
                    else if (content is FileContent file)
                    {
                        ModuleService.AddFileContent(newModule, file.FileName, file.FilePath);
                    }
                    else if (content is AssignmentContent assignmentContent)
                    {
                        ModuleService.AddAssignmentContent(newModule, assignmentMap[assignmentContent.Assignment.Id]);
                    }
                }
            }
 
            foreach (var group in original.AssignmentGroups)
            {
                var newGroup = AssignmentGroupService.AddGroup(newCourse, group.Name);
                AssignmentGroupService.SetWeight(newGroup, group.Weight);
 
                foreach (var assignment in group.Assignments)
                {
                    AssignmentGroupService.AddAssignmentToGroup(newGroup, assignmentMap[assignment.Id]);
                }
            }
 
            return newCourse;
        }
 
        public static void AddAnnouncement(Course course, string text)
        {
            course.Announcements.Add(text);
        }
 
        public static void UpdateGradeRanges(Course course, double aMin, double bMin, double cMin, double dMin)
        {
            course.AMinimum = aMin;
            course.BMinimum = bMin;
            course.CMinimum = cMin;
            course.DMinimum = dMin;
        }
    }
}