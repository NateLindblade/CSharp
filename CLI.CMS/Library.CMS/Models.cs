using System;
using System.Collections.Generic;
 
namespace Library.CMS
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } // FSUID
    }
 
    public class Student : User
    {
        public string Classification { get; set; } // e.g. Freshman, Sophomore, etc.
    }
 
    public class Instructor : User
    {
        public int YearsOfExperience { get; set; }
    }
 
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Semester { get; set; } // e.g. "Fall 2026"
        public string Section { get; set; } // e.g. "01", "02"
        public List<Student> Roster { get; set; } = new List<Student>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
        public List<AssignmentGroup> AssignmentGroups { get; set; } = new List<AssignmentGroup>();
        public List<string> Announcements { get; set; } = new List<string>();
 
        // Minimum percentage needed for each letter grade. Default to the
        // standard 90/80/70/60 scale until a teacher customizes them (issue #44).
        public double AMinimum { get; set; } = 90;
        public double BMinimum { get; set; } = 80;
        public double CMinimum { get; set; } = 70;
        public double DMinimum { get; set; } = 60;
    }
 
    public class Module
    {
        public int Id { get; set; }
        public List<ModuleContent> Content { get; set; } = new List<ModuleContent>();
    }
 
    // Base type for anything that can live inside a module.
    public abstract class ModuleContent
    {
        public int Id { get; set; }
    }
 
    // A page is just the old string-based content, wrapped so it fits the hierarchy.
    public class PageContent : ModuleContent
    {
        public string Content { get; set; }
    }
 
    // A file shows its name and can be opened directly from the module.
    public class FileContent : ModuleContent
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
 
    // An assignment embedded directly in a module - points back to a real Assignment.
    public class AssignmentContent : ModuleContent
    {
        public Assignment Assignment { get; set; }
    }
 
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AvailablePoints { get; set; }
        public DateTime DueDate { get; set; }
        public List<Submission> Submissions { get; set; } = new List<Submission>();
    }
 
    // A quiz is a kind of assignment with a specific question attached.
    // Since it derives from Assignment, it can live in course.Assignments,
    // join assignment groups, and reuse all the existing submit/grade logic
    // as-is - the student's answer is just their Submission.Content, same as
    // any other assignment.
    public class Quiz : Assignment
    {
        public string Question { get; set; }
    }
 
    public class Submission
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public string Content { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int? Grade { get; set; } // null until a teacher grades it
        public string Comment { get; set; } // teacher feedback on the submission
        public string FileName { get; set; } // null if no file was attached
        public string FilePath { get; set; }
    }
 
    public class AssignmentGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; } // percentage, e.g. 20 for 20%
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}