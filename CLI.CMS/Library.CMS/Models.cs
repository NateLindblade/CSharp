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
        public string Classification { get; set; } // e.g. Freshman, Sophomore
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
 
    public abstract class ModuleContent
    {
        public int Id { get; set; }
    }
 
    public class PageContent : ModuleContent
    {
        public string Content { get; set; }
    }
 
    public class FileContent : ModuleContent
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
 
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
