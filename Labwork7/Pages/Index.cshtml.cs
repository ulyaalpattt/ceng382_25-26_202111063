using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Labwork5.Models;
using Labwork5.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace Labwork5.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationTable> Classes = new List<ClassInformationTable>();

        [BindProperty] public string ClassName { get; set; } = string.Empty;
        [BindProperty] public int StudentCount { get; set; }
        [BindProperty] public string Description { get; set; } = string.Empty;
        [BindProperty] public int? ClassIdToEdit { get; set; }
        [BindProperty(SupportsGet = true)] public string SearchTerm { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int? MinStudentCount { get; set; }
        [BindProperty(SupportsGet = true)] public int? MaxStudentCount { get; set; }
        [BindProperty(SupportsGet = true)] public string? DescriptionFilter { get; set; }
        [BindProperty] public string SelectedColumnsJson { get; set; } = string.Empty;

        public List<ClassInformationTable> AllClasses { get; set; } = new(); // tüm sınıflar

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();
        public int TotalPages { get; set; }
        private const int PageSize = 10;
        public string? JsonPreviewContent { get; set; }

        public void OnGet()
        {
            if (Classes.Count == 0)
            {
                for (int i = 1; i <= 100; i++)
                {
                    Classes.Add(new ClassInformationTable
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = i % 50,
                        Description = $"Description {i}"
                    });
                }
            }

            if (CurrentPage < 1)
                CurrentPage = 1;

            var filteredClasses = Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                filteredClasses = filteredClasses.Where(c => c.ClassName != null && c.ClassName.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase));

            if (MinStudentCount.HasValue)
                filteredClasses = filteredClasses.Where(c => c.StudentCount >= MinStudentCount.Value);

            if (MaxStudentCount.HasValue)
                filteredClasses = filteredClasses.Where(c => c.StudentCount <= MaxStudentCount.Value);

            if (!string.IsNullOrEmpty(DescriptionFilter))
                filteredClasses = filteredClasses.Where(c => c.Description != null && c.Description.Contains(DescriptionFilter, System.StringComparison.OrdinalIgnoreCase));

            var totalClasses = filteredClasses.Count();
            TotalPages = (int)System.Math.Ceiling(totalClasses / (double)PageSize);



// JSON'a dönüştürme
            HttpContext.Session.SetString("FilteredClasses", JsonSerializer.Serialize(filteredClasses.ToList()));
            
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
                Classes.Remove(classToRemove);

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
        

        public IActionResult OnPostExportJson(string mode)
        {
            // Filtreleme işlemi için seçilen sütunlar
            var selectedColumns = string.IsNullOrWhiteSpace(SelectedColumnsJson)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(SelectedColumnsJson);

            List<ClassInformationTable> dataToExport;

            if (mode == "filtered")
            {
                var jsonData = HttpContext.Session.GetString("FilteredClasses");
                if (!string.IsNullOrEmpty(jsonData))
                {
                    dataToExport = JsonSerializer.Deserialize<List<ClassInformationTable>>(jsonData);
                }
                else
                {
                    dataToExport = new List<ClassInformationTable>(); // fallback
                }
            }

            else
            {
                // Tüm veriyi al
                dataToExport = Classes;
            }

            // JSON verisini üretme
            var json = Utils.Instance.ExportToJson(dataToExport, selectedColumns);
            var bytes = Encoding.UTF8.GetBytes(json);

            // JSON önizleme içeriği (kısa bir kısmı gösterilecek)
            TempData["JsonPreview"] = json.Length > 1000 ? json.Substring(0, 1000) + "..." : json;

            // JSON dosyasını export et
            return File(bytes, "application/json", $"{mode}_data.json");
        }



    }
}