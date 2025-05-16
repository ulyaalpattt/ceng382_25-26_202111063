using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Labwork5.Models;
using Labwork5.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Labwork5.Data;

namespace Labwork5.Pages
{
    public class IndexModel : PageModel
    {
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

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();
        public int TotalPages { get; set; }
        private const int PageSize = 10;
        public string? JsonPreviewContent { get; set; }
        public string cookieUsername { get; set; }
        public string cookieToken { get; set; }
        public string cookieSessionId { get; set; }

        private readonly SchoolDbContext _context;
        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        public IList<Class> ClassList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Giriş kontrolü (Session & Cookie doğrulama)
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            cookieUsername = Request.Cookies["username"];
            cookieToken = Request.Cookies["token"];
            cookieSessionId = Request.Cookies["session_id"];

            if (sessionUsername == null || cookieUsername != sessionUsername ||
                sessionToken == null || cookieToken != sessionToken ||
                sessionId == null || cookieSessionId != sessionId)
            {
                TempData["Error"] = "Please login to access the page.";
                return RedirectToPage("/Login");
            }

            // Veritabanından sınıfları al
            ClassList = await _context.Classes.Where(c => c.IsActive).ToListAsync(); // Sadece aktif sınıfları al

            // Null kontrolü ekleyin
            if (ClassList == null || !ClassList.Any())
            {
                TempData["Error"] = "No classes found in the database.";
                return RedirectToPage();
            }

            // Filtreleme işlemleri
            var filteredClasses = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                filteredClasses = filteredClasses.Where(c => c.Name.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase));

            if (MinStudentCount.HasValue)
                filteredClasses = filteredClasses.Where(c => c.PersonCount >= MinStudentCount.Value);

            if (MaxStudentCount.HasValue)
                filteredClasses = filteredClasses.Where(c => c.PersonCount <= MaxStudentCount.Value);

            if (!string.IsNullOrEmpty(DescriptionFilter))
                filteredClasses = filteredClasses.Where(c => c.Description.Contains(DescriptionFilter, System.StringComparison.OrdinalIgnoreCase));

            var totalClasses = filteredClasses.Count();
            TotalPages = (int)System.Math.Ceiling(totalClasses / (double)PageSize);

            FilteredClasses = filteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.Name,
                    StudentCount = c.PersonCount,
                    Description = c.Description
                })
                .ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            var newClass = new Class
            {
                Name = ClassName,
                PersonCount = StudentCount,
                Description = Description,
                IsActive = true // Yeni sınıf aktif olarak eklenecek
            };

            _context.Classes.Add(newClass);
            _context.SaveChanges();

            ClassName = string.Empty;
            StudentCount = 0;
            Description = string.Empty;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            // Veritabanından sınıfı bul
            var classToRemove = await _context.Classes.FindAsync(id);

            if (classToRemove != null)
            {
                // Silmek yerine sadece IsActive'yi false yap
                classToRemove.IsActive = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEdit(int id)
        {
            // Sınıfı veritabanından bul
            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                ClassName = classToEdit.Name;
                StudentCount = classToEdit.PersonCount;
                Description = classToEdit.Description;
                ClassIdToEdit = classToEdit.Id;
            }
            else
            {
                TempData["Error"] = "Class not found.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            // Eğer ClassIdToEdit null ise, hata ver
            if (ClassIdToEdit == null)
            {
                TempData["Error"] = "Class ID is missing.";
                return Page();
            }

            // Sınıfı veritabanından bul
            var classToUpdate = await _context.Classes.FirstOrDefaultAsync(c => c.Id == ClassIdToEdit);

            // Eğer sınıf bulunduysa, bilgileri güncelle
            if (classToUpdate != null)
            {
                classToUpdate.Name = ClassName;
                classToUpdate.PersonCount = StudentCount;
                classToUpdate.Description = Description;

                // Değişiklikleri veritabanına kaydet
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class updated successfully!";
            }
            else
            {
                TempData["Error"] = "Class not found.";
            }

            // Ana sayfaya dön
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            // Oturum verilerini temizle
            HttpContext.Session.Clear();

            // Girişle ilgili çerezleri sil
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            // Giriş sayfasına yönlendir
            return RedirectToPage("/Login");
        }

        public IActionResult OnPostExportJson(string mode)
        {
            var selectedColumns = string.IsNullOrWhiteSpace(SelectedColumnsJson)
                ? null
                : JsonSerializer.Deserialize<List<string>>(SelectedColumnsJson);

            List<ClassInformationTable> dataToExport;

            if (mode == "filtered")
            {
                var jsonData = HttpContext.Session.GetString("FilteredClasses");
                dataToExport = string.IsNullOrEmpty(jsonData)
                    ? new List<ClassInformationTable>()
                    : JsonSerializer.Deserialize<List<ClassInformationTable>>(jsonData);
            }
            else
            {
                dataToExport = FilteredClasses;
            }

            var json = Utils.Instance.ExportToJson(dataToExport, selectedColumns);
            var bytes = Encoding.UTF8.GetBytes(json);

            TempData["JsonPreview"] = json.Length > 1000 ? json.Substring(0, 1000) + "..." : json;

            return File(bytes, "application/json", $"{mode}_data.json");
        }
    }
}
