using System.Collections.Generic;
using System.Linq;
 
namespace Library.CMS
{
    public static class AssignmentGroupService
    {
        public static AssignmentGroup AddGroup(Course course, string name)
        {
            var group = new AssignmentGroup
            {
                Id = CmsRepository.NextAssignmentGroupId,
                Name = name
            };
 
            course.AssignmentGroups.Add(group);
            CmsRepository.NextAssignmentGroupId++;
 
            return group;
        }
 
        public static void RenameGroup(AssignmentGroup group, string newName)
        {
            group.Name = newName;
        }
 
        // Only removes the group itself. The assignments in its list are
        // references into course.Assignments, so they're untouched.
        public static bool DeleteGroup(Course course, int groupId)
        {
            var match = course.AssignmentGroups.FirstOrDefault(g => g.Id == groupId);
 
            if (match == null)
            {
                return false;
            }
 
            course.AssignmentGroups.Remove(match);
            return true;
        }
 
        // Returns false if the assignment was already in this group.
        public static bool AddAssignmentToGroup(AssignmentGroup group, Assignment assignment)
        {
            if (group.Assignments.Any(a => a.Id == assignment.Id))
            {
                return false;
            }
 
            group.Assignments.Add(assignment);
            return true;
        }
 
        public static void SetWeight(AssignmentGroup group, double weight)
        {
            group.Weight = weight;
        }
 
        // Averages a student's graded percentage within each weighted group,
        // then combines those averages using each group's weight.
        public static double? CalculateFinalGrade(Course course, Student student)
        {
            var weightedGroups = course.AssignmentGroups.Where(g => g.Weight > 0).ToList();
 
            if (weightedGroups.Count == 0)
            {
                return null;
            }
 
            double weightedTotal = 0;
 
            foreach (var group in weightedGroups)
            {
                var gradedPercentages = new List<double>();
 
                foreach (var assignment in group.Assignments)
                {
                    var submission = assignment.Submissions
                        .FirstOrDefault(s => s.StudentId == student.Id && s.Grade.HasValue);
 
                    if (submission != null)
                    {
                        gradedPercentages.Add((double)submission.Grade.Value / assignment.AvailablePoints * 100);
                    }
                }
 
                if (gradedPercentages.Count > 0)
                {
                    double groupAverage = gradedPercentages.Average();
                    weightedTotal += groupAverage * (group.Weight / 100.0);
                }
            }
 
            return weightedTotal;
        }
 
        // Uses this course's own cutoffs (issue #44) instead of a fixed scale,
        // so each course can have its own grading scheme.
        public static string GetLetterGrade(Course course, double percentage)
        {
            if (percentage >= course.AMinimum)
            {
                return "A";
            }
            else if (percentage >= course.BMinimum)
            {
                return "B";
            }
            else if (percentage >= course.CMinimum)
            {
                return "C";
            }
            else if (percentage >= course.DMinimum)
            {
                return "D";
            }
            else
            {
                return "F";
            }
        }
    }
}