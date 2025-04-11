using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Labwork5.Models;
using System.Collections.Generic;
using System.Linq;

namespace Labwork5.Pages
{
    public class IndexModel : PageModel
{
    public static List<ClassInformationTable> Classes = new List<ClassInformationTable>();

    [BindProperty]
    public string ClassName { get; set; } = string.Empty;

    [BindProperty]
    public int StudentCount { get; set; }

    [BindProperty]
    public string Description { get; set; } = string.Empty;

    [BindProperty]
    public int? ClassIdToEdit { get; set; }

    public List<ClassInformationTable> FilteredClasses { get; set; } = new List<ClassInformationTable>();

    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int? MinStudentCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MaxStudentCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DescriptionFilter { get; set; }

    public int TotalPages { get; set; }

    private const int PageSize = 10;

    public void OnGet()
    {
        if (CurrentPage < 1)
            CurrentPage = 1;

        var filteredClasses = Classes.AsQueryable();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            filteredClasses = filteredClasses.Where(c => c.ClassName.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }


        if (MinStudentCount.HasValue)
        {
            filteredClasses = filteredClasses.Where(c => c.StudentCount >= MinStudentCount.Value);
        }

        if (MaxStudentCount.HasValue)
        {
            filteredClasses = filteredClasses.Where(c => c.StudentCount <= MaxStudentCount.Value);
        }

        if (!string.IsNullOrEmpty(DescriptionFilter))
        {
            filteredClasses = filteredClasses.Where(c => c.Description.Equals(DescriptionFilter, StringComparison.OrdinalIgnoreCase));
        }


        var totalClasses = filteredClasses.Count();
        TotalPages = (int)Math.Ceiling(totalClasses / (double)PageSize);

        FilteredClasses = filteredClasses
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    public IActionResult OnPostAdd()
    {
        var newClass = new ClassInformationTable
        {
            Id = Classes.Any() ? Classes.Max(c => c.Id) + 1 : 1,
            ClassName = ClassName,
            StudentCount = StudentCount,
            Description = Description
        };
        Classes.Add(newClass);

        ClassName = string.Empty;
        StudentCount = 0;
        Description = string.Empty;

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var classToRemove = Classes.Find(c => c.Id == id);
        if (classToRemove != null)
        {
            Classes.Remove(classToRemove);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostEdit(int id)
    {
        var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
        if (classToEdit != null)
        {
            ClassName = classToEdit.ClassName;
            StudentCount = classToEdit.StudentCount;
            Description = classToEdit.Description;
            ClassIdToEdit = classToEdit.Id;
        }
        return Page();
    }

    public IActionResult OnPostUpdate()
    {
        if (ClassIdToEdit == null)
            return Page();

        var classToUpdate = Classes.FirstOrDefault(c => c.Id == ClassIdToEdit);
        if (classToUpdate != null)
        {
            classToUpdate.ClassName = ClassName;
            classToUpdate.StudentCount = StudentCount;
            classToUpdate.Description = Description;
        }

        return RedirectToPage();
    }
}

}
