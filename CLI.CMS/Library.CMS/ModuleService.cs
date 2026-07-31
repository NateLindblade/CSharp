using System.Collections.Generic;
using System.Linq;
 
namespace Library.CMS
{
    public static class ModuleService
    {
        public static Module AddModule(Course course)
        {
            var module = new Module
            {
                Id = CmsRepository.NextModuleId
            };
 
            course.Modules.Add(module);
            CmsRepository.NextModuleId++;
 
            return module;
        }
 
        public static PageContent AddPageContent(Module module, string text)
        {
            var content = new PageContent
            {
                Id = CmsRepository.NextModuleContentId,
                Content = text
            };
 
            module.Content.Add(content);
            CmsRepository.NextModuleContentId++;
 
            return content;
        }
 
        public static FileContent AddFileContent(Module module, string fileName, string filePath)
        {
            var content = new FileContent
            {
                Id = CmsRepository.NextModuleContentId,
                FileName = fileName,
                FilePath = filePath
            };
 
            module.Content.Add(content);
            CmsRepository.NextModuleContentId++;
 
            return content;
        }
 
        public static AssignmentContent AddAssignmentContent(Module module, Assignment assignment)
        {
            var content = new AssignmentContent
            {
                Id = CmsRepository.NextModuleContentId,
                Assignment = assignment
            };
 
            module.Content.Add(content);
            CmsRepository.NextModuleContentId++;
 
            return content;
        }
 
        // Content items have no stable Id shown to the user, so callers select
        // by position in the list - this just removes whatever is at that index.
        public static ModuleContent RemoveContentAt(Module module, int index)
        {
            if (index < 0 || index >= module.Content.Count)
            {
                return null;
            }
 
            var removed = module.Content[index];
            module.Content.RemoveAt(index);
            return removed;
        }
 
        // Same removal, but by the content object itself rather than a position -
        // useful when the caller already has the object (e.g. from a UI button click).
        public static bool RemoveContent(Module module, ModuleContent content)
        {
            return module.Content.Remove(content);
        }
 
        public static bool DeleteModule(Course course, int moduleId)
        {
            var match = course.Modules.FirstOrDefault(m => m.Id == moduleId);
 
            if (match == null)
            {
                return false;
            }
 
            course.Modules.Remove(match);
            return true;
        }
 
        // A readable label for any of the three content types - used by whichever
        // UI is displaying a module's contents.
        public static string DescribeContent(ModuleContent content)
        {
            if (content is PageContent page)
            {
                return $"Page: {page.Content}";
            }
            else if (content is FileContent file)
            {
                return $"File: {file.FileName}";
            }
            else if (content is AssignmentContent assignmentContent)
            {
                return $"Assignment: {assignmentContent.Assignment.Name} (due {assignmentContent.Assignment.DueDate.ToShortDateString()})";
            }
 
            return "Unknown content type";
        }
 
        // Every file across every module in a course - used when opening a file (issue #21).
        // Actually opening the file is platform-specific, so that stays in the UI layer.
        public static List<FileContent> GetAllFiles(Course course)
        {
            return course.Modules
                .SelectMany(m => m.Content)
                .OfType<FileContent>()
                .ToList();
        }
    }
}