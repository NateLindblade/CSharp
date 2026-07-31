using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Library.CMS;
 
namespace MyApp
{
    internal class Program
    {
        static List<Course> courses = CmsRepository.Courses;
 
        static List<Student> allStudents = CmsRepository.AllStudents;
  
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Content Management System");
 
            bool keepRunning = true;
 
            while (keepRunning)
            {
                Console.WriteLine();
                Console.WriteLine("Are you a student or a teacher?");
                Console.WriteLine("1. Student");
                Console.WriteLine("2. Teacher");
                Console.WriteLine("3. Exit");
 
                var selection = Console.ReadLine();
 
                switch (selection)
                {
                    case "1":
                        Console.WriteLine("Student login isn't set up yet. Ask a teacher to proxy as you for now.");
                        break;
                    case "2":
                        ShowTeacherMenu();
                        break;
                    case "3":
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
        }
 
        static void ShowStudentMenu(Student student)
        {
            var enrolledCourses = EnrollmentService.GetEnrolledCourses(student.Id);
 
            bool inStudentMenu = true;
 
            while (inStudentMenu)
            {
                Console.WriteLine();
                Console.WriteLine($"Student Menu - {student.Name}");
 
                if (enrolledCourses.Count == 0)
                {
                    Console.WriteLine("You are not enrolled in any courses.");
                    return;
                }
 
                Console.WriteLine("Your courses:");
                foreach (var c in enrolledCourses)
                {
                    Console.WriteLine($"Id {c.Id}: {c.Name} ({c.Code})");
                }
                Console.WriteLine("Enter a course Id to view it, or type 'back' to return:");
 
                var input = Console.ReadLine();
 
                if (input == "back")
                {
                    inStudentMenu = false;
                }
                else if (int.TryParse(input, out int selectedId))
                {
                    var match = enrolledCourses.FirstOrDefault(c => c.Id == selectedId);
 
                    if (match != null)
                    {
                        ShowCourseMenu(match, student);
                    }
                    else
                    {
                        Console.WriteLine("That course Id isn't in your enrolled courses.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }
 
        static void ShowTeacherMenu()
        {
            bool inTeacherMenu = true;
 
            while (inTeacherMenu)
            {
                Console.WriteLine();
                Console.WriteLine("Teacher Menu");
                Console.WriteLine("1. Add a new course");
                Console.WriteLine("2. Select an existing course");
                Console.WriteLine("3. Proxy as a student");
                Console.WriteLine("4. Delete a course");
                Console.WriteLine("5. Copy a course");
                Console.WriteLine("6. Back to main menu");
 
                var selection = Console.ReadLine();
 
                switch (selection)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        SelectCourse();
                        break;
                    case "3":
                        ProxyAsStudent();
                        break;
                    case "4":
                        DeleteCourse();
                        break;
                    case "5":
                        CopyCourse();
                        break;
                    case "6":
                        inTeacherMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
        }
 
        static void AddCourse()
        {
            Console.WriteLine("Enter the course code:");
            var code = Console.ReadLine();
 
            Console.WriteLine("Enter the course name:");
            var name = Console.ReadLine();
 
            Console.WriteLine("Enter the course description:");
            var description = Console.ReadLine();
 
            Console.WriteLine("Enter the semester (e.g. Fall 2026):");
            var semester = Console.ReadLine();
 
            Console.WriteLine("Enter the section (e.g. 01):");
            var section = Console.ReadLine();
 
            var course = CourseService.AddCourse(code, name, description, semester, section);
 
            Console.WriteLine($"Course '{course.Name}' added with Id {course.Id}");
        }
 
        static void SelectCourse()
        {
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses exist yet.");
                return;
            }
 
            Console.WriteLine("Enter a semester to filter by (or leave blank to see all):");
            var filter = Console.ReadLine();
 
            var filteredCourses = CourseService.FilterBySemester(filter);
 
            if (filteredCourses.Count == 0)
            {
                Console.WriteLine("No courses found for that semester.");
                return;
            }
 
            var groupedBySemester = filteredCourses
                .GroupBy(c => c.Semester)
                .OrderBy(g => g.Key);
 
            foreach (var group in groupedBySemester)
            {
                Console.WriteLine($"-- {group.Key} --");
                foreach (var c in group)
                {
                    Console.WriteLine($"Id {c.Id}: {c.Name} ({c.Code}) - Section {c.Section}");
                }
            }
 
            Console.WriteLine("Enter the Id of the course you want to select:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var match = courses.FirstOrDefault(c => c.Id == selectedId);
 
                if (match != null)
                {
                    ShowCourseMenu(match);
                }
                else
                {
                    Console.WriteLine("No course found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void ShowCourseMenu(Course course, Student currentStudent = null)
        {
            bool inCourseMenu = true;
 
            while (inCourseMenu)
            {
                Console.WriteLine();
                Console.WriteLine($"Course Menu - {course.Name} ({course.Code})");
                Console.WriteLine("1. See all modules and module content");
                Console.WriteLine("2. See all assignments");
                Console.WriteLine("3. See other students in the course");
                Console.WriteLine("4. See course schedule");
                Console.WriteLine("5. Enroll a student");
                Console.WriteLine("6. Update description");
                Console.WriteLine("7. Add a module");
                Console.WriteLine("8. Add content to a module");
                Console.WriteLine("9. Modify content in a module");
                Console.WriteLine("10. Remove content from a module");
                Console.WriteLine("11. Add an assignment");
                Console.WriteLine("12. Delete an assignment");
                Console.WriteLine("13. Edit an assignment");
                Console.WriteLine("14. Submit an assignment (students only)");
                Console.WriteLine("15. Grade a submission (teachers only)");
                Console.WriteLine("16. Unenroll a student");
                Console.WriteLine("17. Open a file from a module");
                Console.WriteLine("18. Manage assignment groups");
                Console.WriteLine("19. See my grades (students only)");
                Console.WriteLine("20. Back");
 
                var selection = Console.ReadLine();
 
                switch (selection)
                {
                    case "1":
                        ShowModules(course);
                        break;
                    case "2":
                        ShowAssignments(course);
                        break;
                    case "3":
                        ShowStudents(course);
                        break;
                    case "4":
                        ShowSchedule(course);
                        break;
                    case "5":
                        EnrollStudent(course);
                        break;
                    case "6":
                        UpdateDescription(course);
                        break;
                    case "7":
                        AddModule(course);
                        break;
                    case "8":
                        AddModuleContent(course);
                        break;
                    case "9":
                        ModifyModuleContent(course);
                        break;
                    case "10":
                        RemoveModuleContent(course);
                        break;
                    case "11":
                        AddAssignment(course);
                        break;
                    case "12":
                        DeleteAssignment(course);
                        break;
                    case "13":
                        EditAssignment(course);
                        break;
                    case "14":
                        SubmitAssignment(course, currentStudent);
                        break;
                    case "15":
                        GradeSubmission(course, currentStudent);
                        break;
                    case "16":
                        bool studentLeftCourse = UnenrollStudent(course, currentStudent);
                        if (studentLeftCourse)
                        {
                            inCourseMenu = false;
                        }
                        break;
                    case "17":
                        OpenFile(course);
                        break;
                    case "18":
                        ShowAssignmentGroupMenu(course);
                        break;
                    case "19":
                        ShowMyGrades(course, currentStudent);
                        break;
                    case "20":
                        inCourseMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
        }
 
        static void ShowModules(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules yet.");
                return;
            }
 
            foreach (var module in course.Modules)
            {
                Console.WriteLine($"Module {module.Id}:");
                foreach (var content in module.Content)
                {
                    Console.WriteLine($"  - {ModuleService.DescribeContent(content)}");
                }
            }
        }
 
        static void ShowAssignments(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments yet.");
                return;
            }
 
            foreach (var assignment in course.Assignments)
            {
                Console.WriteLine($"{assignment.Name} - {assignment.AvailablePoints} points");
            }
        }
 
        static void ShowStudents(Course course)
        {
            if (course.Roster.Count == 0)
            {
                Console.WriteLine("No students enrolled yet.");
                return;
            }
 
            foreach (var student in course.Roster)
            {
                Console.WriteLine($"{student.Name} ({student.Classification})");
            }
        }
 
        static void ShowSchedule(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments yet.");
                return;
            }
 
            foreach (var assignment in course.Assignments)
            {
                Console.WriteLine($"{assignment.Name} - due {assignment.DueDate.ToShortDateString()}");
            }
        }
 
        static void EnrollStudent(Course course)
        {
            Console.WriteLine("Enter the student's FSUID:");
            var code = Console.ReadLine();
 
            // Only one list of students should exist. If this FSUID is already
            // in the system (maybe from another course), reuse that same student
            // instead of creating a duplicate.
            var student = EnrollmentService.FindStudentByCode(code);
 
            if (student == null)
            {
                Console.WriteLine("This student doesn't exist yet. Enter their name:");
                var name = Console.ReadLine();
 
                Console.WriteLine("Enter their classification:");
                var classification = Console.ReadLine();
 
                student = EnrollmentService.CreateStudent(code, name, classification);
            }
 
            bool enrolled = EnrollmentService.EnrollStudentInCourse(course, student);
 
            if (enrolled)
            {
                Console.WriteLine($"{student.Name} has been enrolled in {course.Name}.");
            }
            else
            {
                Console.WriteLine($"{student.Name} is already enrolled in this course.");
            }
        }
 
        static void ProxyAsStudent()
        {
            if (allStudents.Count == 0)
            {
                Console.WriteLine("No students exist yet.");
                return;
            }
 
            Console.WriteLine("Existing students:");
            foreach (var s in allStudents)
            {
                Console.WriteLine($"Id {s.Id}: {s.Name}");
            }
 
            Console.WriteLine("Enter the Id of the student to proxy as:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var match = allStudents.FirstOrDefault(s => s.Id == selectedId);
 
                if (match != null)
                {
                    ShowStudentMenu(match);
                }
                else
                {
                    Console.WriteLine("No student found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void DeleteCourse()
        {
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses exist yet.");
                return;
            }
 
            Console.WriteLine("Existing courses:");
            foreach (var c in courses)
            {
                Console.WriteLine($"Id {c.Id}: {c.Name} ({c.Code})");
            }
 
            Console.WriteLine("Enter the Id of the course to delete:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                bool deleted = CourseService.DeleteCourse(selectedId);
 
                if (deleted)
                {
                    Console.WriteLine("Course has been deleted.");
                }
                else
                {
                    Console.WriteLine("No course found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void CopyCourse()
        {
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses exist yet.");
                return;
            }
 
            Console.WriteLine("Existing courses:");
            foreach (var c in courses)
            {
                Console.WriteLine($"Id {c.Id}: {c.Name} ({c.Code})");
            }
 
            Console.WriteLine("Enter the Id of the course to copy:");
            var input = Console.ReadLine();
 
            if (!int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var newCourse = CourseService.CopyCourse(selectedId);
 
            if (newCourse != null)
            {
                Console.WriteLine($"Course copied. New course '{newCourse.Name}' added with Id {newCourse.Id}");
            }
            else
            {
                Console.WriteLine("No course found with that Id.");
            }
        }
 
        static void UpdateDescription(Course course)
        {
            Console.WriteLine($"Current description: {course.Description}");
            Console.WriteLine("Enter the new description:");
            var newDescription = Console.ReadLine();
 
            CourseService.UpdateDescription(course.Id, newDescription);
            Console.WriteLine("Description updated.");
        }
 
        static void AddModule(Course course)
        {
            var module = ModuleService.AddModule(course);
 
            Console.WriteLine($"Module added with Id {module.Id}. Use 'See all modules' to add content to it later.");
        }
 
        static void AddModuleContent(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules exist yet. Add a module first.");
                return;
            }
 
            Console.WriteLine("Existing modules:");
            foreach (var m in course.Modules)
            {
                Console.WriteLine($"Id {m.Id}");
            }
 
            Console.WriteLine("Enter the Id of the module to add content to:");
            var input = Console.ReadLine();
 
            if (!int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var module = course.Modules.FirstOrDefault(m => m.Id == selectedId);
 
            if (module == null)
            {
                Console.WriteLine("No module found with that Id.");
                return;
            }
 
            Console.WriteLine("What type of content do you want to add?");
            Console.WriteLine("1. Page (plain text)");
            Console.WriteLine("2. File");
            Console.WriteLine("3. Assignment");
 
            var typeSelection = Console.ReadLine();
 
            switch (typeSelection)
            {
                case "1":
                    Console.WriteLine("Enter the page content:");
                    var pageText = Console.ReadLine();
 
                    ModuleService.AddPageContent(module, pageText);
                    Console.WriteLine("Page added.");
                    break;
 
                case "2":
                    Console.WriteLine("Enter the file name (e.g. syllabus.pdf):");
                    var fileName = Console.ReadLine();
 
                    Console.WriteLine("Enter the file path:");
                    var filePath = Console.ReadLine();
 
                    ModuleService.AddFileContent(module, fileName, filePath);
                    Console.WriteLine("File added.");
                    break;
 
                case "3":
                    if (course.Assignments.Count == 0)
                    {
                        Console.WriteLine("No assignments exist yet. Add one first.");
                        return;
                    }
 
                    Console.WriteLine("Existing assignments:");
                    foreach (var a in course.Assignments)
                    {
                        Console.WriteLine($"Id {a.Id}: {a.Name}");
                    }
 
                    Console.WriteLine("Enter the Id of the assignment to embed:");
                    var assignmentInput = Console.ReadLine();
 
                    if (int.TryParse(assignmentInput, out int assignmentId))
                    {
                        var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
 
                        if (assignment != null)
                        {
                            ModuleService.AddAssignmentContent(module, assignment);
                            Console.WriteLine("Assignment embedded in module.");
                        }
                        else
                        {
                            Console.WriteLine("No assignment found with that Id.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("That is not a valid Id.");
                    }
                    break;
 
                default:
                    Console.WriteLine("Invalid selection.");
                    break;
            }
        }
 
        static void ModifyModuleContent(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules exist yet. Add a module first.");
                return;
            }
 
            Console.WriteLine("Existing modules:");
            foreach (var m in course.Modules)
            {
                Console.WriteLine($"Id {m.Id}");
            }
 
            Console.WriteLine("Enter the Id of the module to modify:");
            var moduleInput = Console.ReadLine();
 
            if (!int.TryParse(moduleInput, out int moduleId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var module = course.Modules.FirstOrDefault(m => m.Id == moduleId);
 
            if (module == null)
            {
                Console.WriteLine("No module found with that Id.");
                return;
            }
 
            if (module.Content.Count == 0)
            {
                Console.WriteLine("This module has no content yet.");
                return;
            }
 
            // Content items don't have a stable, teacher-facing Id shown here,
            // so we select by position in the list instead - same as before.
            Console.WriteLine("Current content:");
            for (int i = 0; i < module.Content.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ModuleService.DescribeContent(module.Content[i])}");
            }
 
            Console.WriteLine("Enter the number of the content to update:");
            var indexInput = Console.ReadLine();
 
            if (!int.TryParse(indexInput, out int selectedNumber) || selectedNumber < 1 || selectedNumber > module.Content.Count)
            {
                Console.WriteLine("That is not a valid selection.");
                return;
            }
 
            var content = module.Content[selectedNumber - 1];
 
            if (content is PageContent page)
            {
                Console.WriteLine($"Enter the new page content (current: {page.Content}):");
                page.Content = Console.ReadLine();
                Console.WriteLine("Page updated.");
            }
            else if (content is FileContent file)
            {
                Console.WriteLine($"Enter the new file name (current: {file.FileName}):");
                file.FileName = Console.ReadLine();
 
                Console.WriteLine($"Enter the new file path (current: {file.FilePath}):");
                file.FilePath = Console.ReadLine();
 
                Console.WriteLine("File updated.");
            }
            else if (content is AssignmentContent assignmentContent)
            {
                if (course.Assignments.Count == 0)
                {
                    Console.WriteLine("No assignments exist to link to.");
                    return;
                }
 
                Console.WriteLine("Existing assignments:");
                foreach (var a in course.Assignments)
                {
                    Console.WriteLine($"Id {a.Id}: {a.Name}");
                }
 
                Console.WriteLine("Enter the Id of the assignment this should embed instead:");
                var assignmentInput = Console.ReadLine();
 
                if (int.TryParse(assignmentInput, out int assignmentId))
                {
                    var match = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
 
                    if (match != null)
                    {
                        assignmentContent.Assignment = match;
                        Console.WriteLine("Embedded assignment updated.");
                    }
                    else
                    {
                        Console.WriteLine("No assignment found with that Id.");
                    }
                }
                else
                {
                    Console.WriteLine("That is not a valid Id.");
                }
            }
        }
 
        static void RemoveModuleContent(Course course)
        {
            if (course.Modules.Count == 0)
            {
                Console.WriteLine("No modules exist yet. Add a module first.");
                return;
            }
 
            Console.WriteLine("Existing modules:");
            foreach (var m in course.Modules)
            {
                Console.WriteLine($"Id {m.Id}");
            }
 
            Console.WriteLine("Enter the Id of the module to remove content from:");
            var moduleInput = Console.ReadLine();
 
            if (!int.TryParse(moduleInput, out int moduleId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var module = course.Modules.FirstOrDefault(m => m.Id == moduleId);
 
            if (module == null)
            {
                Console.WriteLine("No module found with that Id.");
                return;
            }
 
            if (module.Content.Count == 0)
            {
                Console.WriteLine("This module has no content yet.");
                return;
            }
 
            Console.WriteLine("Current content:");
            for (int i = 0; i < module.Content.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ModuleService.DescribeContent(module.Content[i])}");
            }
 
            Console.WriteLine("Enter the number of the content to remove:");
            var indexInput = Console.ReadLine();
 
            if (int.TryParse(indexInput, out int selectedNumber) && selectedNumber >= 1 && selectedNumber <= module.Content.Count)
            {
                var removed = ModuleService.RemoveContentAt(module, selectedNumber - 1);
                Console.WriteLine($"Removed: {ModuleService.DescribeContent(removed)}");
            }
            else
            {
                Console.WriteLine("That is not a valid selection.");
            }
        }
 
        static void AddAssignment(Course course)
        {
            Console.WriteLine("Enter the assignment name:");
            var name = Console.ReadLine();
 
            Console.WriteLine("Enter the assignment description:");
            var description = Console.ReadLine();
 
            Console.WriteLine("Enter the available points:");
            var pointsInput = Console.ReadLine();
 
            if (!int.TryParse(pointsInput, out int availablePoints))
            {
                Console.WriteLine("That is not a valid number of points.");
                return;
            }
 
            Console.WriteLine("Enter the due date (MM/DD/YYYY):");
            var dueDateInput = Console.ReadLine();
 
            if (!DateTime.TryParse(dueDateInput, out DateTime dueDate))
            {
                Console.WriteLine("That is not a valid date.");
                return;
            }
 
            var assignment = AssignmentService.AddAssignment(course, name, description, availablePoints, dueDate);
 
            Console.WriteLine($"Assignment '{assignment.Name}' added with Id {assignment.Id}");
        }
 
        static void DeleteAssignment(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet.");
                return;
            }
 
            Console.WriteLine("Existing assignments:");
            foreach (var a in course.Assignments)
            {
                Console.WriteLine($"Id {a.Id}: {a.Name}");
            }
 
            Console.WriteLine("Enter the Id of the assignment to delete:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var assignmentName = course.Assignments.FirstOrDefault(a => a.Id == selectedId)?.Name;
 
                bool deleted = AssignmentService.DeleteAssignment(course, selectedId);
 
                if (deleted)
                {
                    Console.WriteLine($"Assignment '{assignmentName}' and its submissions have been deleted.");
                }
                else
                {
                    Console.WriteLine("No assignment found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void EditAssignment(Course course)
        {
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet.");
                return;
            }
 
            Console.WriteLine("Existing assignments:");
            foreach (var a in course.Assignments)
            {
                Console.WriteLine($"Id {a.Id}: {a.Name}");
            }
 
            Console.WriteLine("Enter the Id of the assignment to edit:");
            var input = Console.ReadLine();
 
            if (!int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == selectedId);
 
            if (assignment == null)
            {
                Console.WriteLine("No assignment found with that Id.");
                return;
            }
 
            Console.WriteLine($"Enter the new name (current: {assignment.Name}):");
            var name = Console.ReadLine();
 
            Console.WriteLine($"Enter the new description (current: {assignment.Description}):");
            var description = Console.ReadLine();
 
            Console.WriteLine($"Enter the new available points (current: {assignment.AvailablePoints}):");
            var pointsInput = Console.ReadLine();
            int? newPoints = int.TryParse(pointsInput, out int parsedPoints) ? parsedPoints : (int?)null;
 
            Console.WriteLine($"Enter the new due date (current: {assignment.DueDate.ToShortDateString()}):");
            var dueDateInput = Console.ReadLine();
            DateTime? newDueDate = DateTime.TryParse(dueDateInput, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
 
            AssignmentService.UpdateAssignment(assignment, name, description, newPoints, newDueDate);
 
            Console.WriteLine("Assignment updated.");
        }
 
        static void SubmitAssignment(Course course, Student currentStudent)
        {
            if (currentStudent == null)
            {
                Console.WriteLine("Only students can submit assignments. Proxy as a student to use this.");
                return;
            }
 
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet.");
                return;
            }
 
            Console.WriteLine("Existing assignments:");
            foreach (var a in course.Assignments)
            {
                Console.WriteLine($"Id {a.Id}: {a.Name}");
            }
 
            Console.WriteLine("Enter the Id of the assignment to submit for:");
            var input = Console.ReadLine();
 
            if (!int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == selectedId);
 
            if (assignment == null)
            {
                Console.WriteLine("No assignment found with that Id.");
                return;
            }
 
            Console.WriteLine("Enter your submission content:");
            var content = Console.ReadLine();
 
            AssignmentService.SubmitAssignment(assignment, currentStudent, content);
 
            Console.WriteLine("Submission added.");
        }
 
        static void GradeSubmission(Course course, Student currentStudent)
        {
            if (currentStudent != null)
            {
                Console.WriteLine("Only teachers can grade submissions.");
                return;
            }
 
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet.");
                return;
            }
 
            Console.WriteLine("Existing assignments:");
            foreach (var a in course.Assignments)
            {
                Console.WriteLine($"Id {a.Id}: {a.Name}");
            }
 
            Console.WriteLine("Enter the Id of the assignment to review:");
            var assignmentInput = Console.ReadLine();
 
            if (!int.TryParse(assignmentInput, out int assignmentId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
 
            if (assignment == null)
            {
                Console.WriteLine("No assignment found with that Id.");
                return;
            }
 
            if (assignment.Submissions.Count == 0)
            {
                Console.WriteLine("No submissions exist for this assignment yet.");
                return;
            }
 
            Console.WriteLine("Submissions:");
            foreach (var s in assignment.Submissions)
            {
                var student = course.Roster.FirstOrDefault(r => r.Id == s.StudentId);
                var studentName = student != null ? student.Name : $"Student {s.StudentId}";
                var gradeText = s.Grade.HasValue ? s.Grade.Value.ToString() : "ungraded";
                var commentText = string.IsNullOrEmpty(s.Comment) ? "no comment" : s.Comment;
                Console.WriteLine($"Id {s.Id} - {studentName} - Content: {s.Content} - Grade: {gradeText} - Comment: {commentText}");
            }
 
            Console.WriteLine("Enter the Id of the submission to grade:");
            var submissionInput = Console.ReadLine();
 
            if (!int.TryParse(submissionInput, out int submissionId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var submission = assignment.Submissions.FirstOrDefault(s => s.Id == submissionId);
 
            if (submission == null)
            {
                Console.WriteLine("No submission found with that Id.");
                return;
            }
 
            Console.WriteLine("Grade using:");
            Console.WriteLine("1. Points (out of available points)");
            Console.WriteLine("2. Percentage");
            var modeSelection = Console.ReadLine();
 
            if (modeSelection == "1")
            {
                Console.WriteLine($"Enter the points earned (out of {assignment.AvailablePoints}):");
                var pointsInput = Console.ReadLine();
 
                if (int.TryParse(pointsInput, out int points))
                {
                    AssignmentService.GradeByPoints(submission, points);
                }
                else
                {
                    Console.WriteLine("That is not a valid number of points.");
                    return;
                }
            }
            else if (modeSelection == "2")
            {
                Console.WriteLine("Enter the percentage (e.g. 90 for 90%):");
                var percentInput = Console.ReadLine();
 
                if (double.TryParse(percentInput, out double percent))
                {
                    AssignmentService.GradeByPercentage(submission, assignment.AvailablePoints, percent);
                }
                else
                {
                    Console.WriteLine("That is not a valid percentage.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid selection.");
                return;
            }
 
            Console.WriteLine("Enter a comment for the student (or leave blank):");
            var comment = Console.ReadLine();
            AssignmentService.SetComment(submission, comment);
 
            Console.WriteLine("Grade saved.");
        }
 
        static void ShowAssignmentGroupMenu(Course course)
        {
            bool inGroupMenu = true;
 
            while (inGroupMenu)
            {
                Console.WriteLine();
                Console.WriteLine("Assignment Group Menu");
                Console.WriteLine("1. Add a group");
                Console.WriteLine("2. Edit a group");
                Console.WriteLine("3. List groups");
                Console.WriteLine("4. Delete a group");
                Console.WriteLine("5. Add an assignment to a group");
                Console.WriteLine("6. Set a group's weight");
                Console.WriteLine("7. Calculate a student's final grade");
                Console.WriteLine("8. Back");
 
                var selection = Console.ReadLine();
 
                switch (selection)
                {
                    case "1":
                        AddAssignmentGroup(course);
                        break;
                    case "2":
                        EditAssignmentGroup(course);
                        break;
                    case "3":
                        ListAssignmentGroups(course);
                        break;
                    case "4":
                        DeleteAssignmentGroup(course);
                        break;
                    case "5":
                        AddAssignmentToGroup(course);
                        break;
                    case "6":
                        SetGroupWeight(course);
                        break;
                    case "7":
                        CalculateFinalGradeForStudent(course);
                        break;
                    case "8":
                        inGroupMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
        }
 
        static void AddAssignmentGroup(Course course)
        {
            Console.WriteLine("Enter the group name:");
            var name = Console.ReadLine();
 
            var group = AssignmentGroupService.AddGroup(course, name);
 
            Console.WriteLine($"Group '{group.Name}' added with Id {group.Id}");
        }
 
        static void EditAssignmentGroup(Course course)
        {
            if (course.AssignmentGroups.Count == 0)
            {
                Console.WriteLine("No groups exist yet.");
                return;
            }
 
            Console.WriteLine("Existing groups:");
            foreach (var g in course.AssignmentGroups)
            {
                Console.WriteLine($"Id {g.Id}: {g.Name}");
            }
 
            Console.WriteLine("Enter the Id of the group to edit:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var match = course.AssignmentGroups.FirstOrDefault(g => g.Id == selectedId);
 
                if (match != null)
                {
                    Console.WriteLine($"Enter the new name (current: {match.Name}):");
                    var newName = Console.ReadLine();
                    AssignmentGroupService.RenameGroup(match, newName);
                    Console.WriteLine("Group updated.");
                }
                else
                {
                    Console.WriteLine("No group found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void ListAssignmentGroups(Course course)
        {
            if (course.AssignmentGroups.Count == 0)
            {
                Console.WriteLine("No groups exist yet.");
                return;
            }
 
            foreach (var g in course.AssignmentGroups)
            {
                Console.WriteLine($"Id {g.Id}: {g.Name} - Weight: {g.Weight}%");
 
                if (g.Assignments.Count == 0)
                {
                    Console.WriteLine("  (no assignments in this group)");
                }
                else
                {
                    foreach (var a in g.Assignments)
                    {
                        Console.WriteLine($"  - {a.Name}");
                    }
                }
            }
        }
 
        static void DeleteAssignmentGroup(Course course)
        {
            if (course.AssignmentGroups.Count == 0)
            {
                Console.WriteLine("No groups exist yet.");
                return;
            }
 
            Console.WriteLine("Existing groups:");
            foreach (var g in course.AssignmentGroups)
            {
                Console.WriteLine($"Id {g.Id}: {g.Name}");
            }
 
            Console.WriteLine("Enter the Id of the group to delete:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var groupName = course.AssignmentGroups.FirstOrDefault(g => g.Id == selectedId)?.Name;
 
                // This only removes the group itself. The assignments in it are
                // references into course.Assignments, so they're untouched.
                bool deleted = AssignmentGroupService.DeleteGroup(course, selectedId);
 
                if (deleted)
                {
                    Console.WriteLine($"Group '{groupName}' has been deleted.");
                }
                else
                {
                    Console.WriteLine("No group found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void AddAssignmentToGroup(Course course)
        {
            if (course.AssignmentGroups.Count == 0)
            {
                Console.WriteLine("No groups exist yet. Add a group first.");
                return;
            }
 
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet. Add one first.");
                return;
            }
 
            Console.WriteLine("Existing groups:");
            foreach (var g in course.AssignmentGroups)
            {
                Console.WriteLine($"Id {g.Id}: {g.Name}");
            }
 
            Console.WriteLine("Enter the Id of the group:");
            var groupInput = Console.ReadLine();
 
            if (!int.TryParse(groupInput, out int groupId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var group = course.AssignmentGroups.FirstOrDefault(g => g.Id == groupId);
 
            if (group == null)
            {
                Console.WriteLine("No group found with that Id.");
                return;
            }
 
            Console.WriteLine("Existing assignments:");
            foreach (var a in course.Assignments)
            {
                Console.WriteLine($"Id {a.Id}: {a.Name}");
            }
 
            Console.WriteLine("Enter the Id of the assignment to add:");
            var assignmentInput = Console.ReadLine();
 
            if (int.TryParse(assignmentInput, out int assignmentId))
            {
                var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
 
                if (assignment == null)
                {
                    Console.WriteLine("No assignment found with that Id.");
                    return;
                }
 
                if (group.Assignments.Any(a => a.Id == assignment.Id))
                {
                    Console.WriteLine($"{assignment.Name} is already in this group.");
                }
                else
                {
                    AssignmentGroupService.AddAssignmentToGroup(group, assignment);
                    Console.WriteLine($"{assignment.Name} added to group '{group.Name}'.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void SetGroupWeight(Course course)
        {
            if (course.AssignmentGroups.Count == 0)
            {
                Console.WriteLine("No groups exist yet.");
                return;
            }
 
            Console.WriteLine("Existing groups:");
            foreach (var g in course.AssignmentGroups)
            {
                Console.WriteLine($"Id {g.Id}: {g.Name} - Weight: {g.Weight}%");
            }
 
            Console.WriteLine("Enter the Id of the group to set a weight for:");
            var input = Console.ReadLine();
 
            if (!int.TryParse(input, out int selectedId))
            {
                Console.WriteLine("That is not a valid Id.");
                return;
            }
 
            var group = course.AssignmentGroups.FirstOrDefault(g => g.Id == selectedId);
 
            if (group == null)
            {
                Console.WriteLine("No group found with that Id.");
                return;
            }
 
            Console.WriteLine("Enter the weight as a percentage (e.g. 20 for 20%):");
            var weightInput = Console.ReadLine();
 
            if (double.TryParse(weightInput, out double weight))
            {
                AssignmentGroupService.SetWeight(group, weight);
                Console.WriteLine($"Weight for '{group.Name}' set to {weight}%.");
            }
            else
            {
                Console.WriteLine("That is not a valid weight.");
            }
        }
 
        static void ShowMyGrades(Course course, Student currentStudent)
        {
            if (currentStudent == null)
            {
                Console.WriteLine("Only students can view their own grades this way.");
                return;
            }
 
            if (course.Assignments.Count == 0)
            {
                Console.WriteLine("No assignments exist yet.");
                return;
            }
 
            Console.WriteLine("Your grades:");
 
            foreach (var assignment in course.Assignments)
            {
                var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == currentStudent.Id);
 
                if (submission == null)
                {
                    Console.WriteLine($"{assignment.Name}: Not submitted");
                }
                else if (submission.Grade.HasValue)
                {
                    Console.WriteLine($"{assignment.Name}: {submission.Grade.Value}/{assignment.AvailablePoints}");
 
                    if (!string.IsNullOrEmpty(submission.Comment))
                    {
                        Console.WriteLine($"  Feedback: {submission.Comment}");
                    }
                }
                else
                {
                    Console.WriteLine($"{assignment.Name}: Submitted, not yet graded");
                }
            }
 
            var finalGrade = AssignmentGroupService.CalculateFinalGrade(course, currentStudent);
 
            if (finalGrade.HasValue)
            {
                Console.WriteLine($"Weighted course average: {finalGrade.Value:0.##}%");
            }
            else
            {
                Console.WriteLine("Weighted course average: not available yet (no weighted groups with graded assignments).");
            }
        }
 
        static void CalculateFinalGradeForStudent(Course course)
        {
            if (course.Roster.Count == 0)
            {
                Console.WriteLine("No students are enrolled in this course.");
                return;
            }
 
            Console.WriteLine("Enrolled students:");
            foreach (var s in course.Roster)
            {
                Console.WriteLine($"Id {s.Id}: {s.Name}");
            }
 
            Console.WriteLine("Enter the Id of the student to calculate a final grade for:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedId))
            {
                var student = course.Roster.FirstOrDefault(s => s.Id == selectedId);
 
                if (student != null)
                {
                    var finalGrade = AssignmentGroupService.CalculateFinalGrade(course, student);
 
                    if (finalGrade.HasValue)
                    {
                        Console.WriteLine($"{student.Name}'s final grade: {finalGrade.Value:0.##}%");
                    }
                    else
                    {
                        Console.WriteLine("No weighted groups with graded assignments exist yet.");
                    }
                }
                else
                {
                    Console.WriteLine("No student found with that Id.");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid Id.");
            }
        }
 
        static void OpenFile(Course course)
        {
            // Which files exist is business logic (now in ModuleService); actually
            // opening one with Process.Start is platform-specific, so that stays here.
            var files = ModuleService.GetAllFiles(course);
 
            if (files.Count == 0)
            {
                Console.WriteLine("No files exist in any module yet.");
                return;
            }
 
            Console.WriteLine("Files:");
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {files[i].FileName}");
            }
 
            Console.WriteLine("Enter the number of the file to open:");
            var input = Console.ReadLine();
 
            if (int.TryParse(input, out int selectedNumber) && selectedNumber >= 1 && selectedNumber <= files.Count)
            {
                var file = files[selectedNumber - 1];
 
                try
                {
                    // UseShellExecute lets the OS open the file with whatever
                    // program is normally associated with it (like double-clicking it).
                    Process.Start(new ProcessStartInfo(file.FilePath) { UseShellExecute = true });
                    Console.WriteLine($"Opening {file.FileName}...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not open the file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("That is not a valid selection.");
            }
        }
 
        // Returns true if the current student unenrolled themselves,
        // so the caller knows to exit the course menu (they're no longer in it).
        static bool UnenrollStudent(Course course, Student currentStudent)
        {
            if (currentStudent != null)
            {
                // Student unenrolling themselves - no need to pick anyone, it's just them.
                bool removed = EnrollmentService.RemoveFromRoster(course, currentStudent.Id);
 
                if (!removed)
                {
                    Console.WriteLine("You are not enrolled in this course.");
                    return false;
                }
 
                Console.WriteLine("You have been unenrolled from this course.");
                return true;
            }
            else
            {
                // Teacher unenrolling a selected student.
                if (course.Roster.Count == 0)
                {
                    Console.WriteLine("No students are enrolled in this course.");
                    return false;
                }
 
                Console.WriteLine("Enrolled students:");
                foreach (var s in course.Roster)
                {
                    Console.WriteLine($"Id {s.Id}: {s.Name}");
                }
 
                Console.WriteLine("Enter the Id of the student to unenroll:");
                var input = Console.ReadLine();
 
                if (int.TryParse(input, out int selectedId))
                {
                    var studentName = course.Roster.FirstOrDefault(s => s.Id == selectedId)?.Name;
 
                    bool removed = EnrollmentService.RemoveFromRoster(course, selectedId);
 
                    if (removed)
                    {
                        Console.WriteLine($"{studentName} has been unenrolled from this course.");
                    }
                    else
                    {
                        Console.WriteLine("No student found with that Id.");
                    }
                }
                else
                {
                    Console.WriteLine("That is not a valid Id.");
                }
 
                return false;
            }
        }
    }
}
