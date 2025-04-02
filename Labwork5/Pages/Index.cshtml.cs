using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Labwork5.Models;
using System.Collections.Generic;
using System.Linq;

namespace Labwork5.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> Classes = new List<ClassInformationModel>();

        [BindProperty]
        public string ClassName { get; set; } = string.Empty;

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public int? ClassIdToEdit { get; set; }

        // Sayfa Yüklenince Çalışan Metot
        public void OnGet()
        {
            // Sayfa ilk açıldığında sınıfları yükler
        }

        // Yeni Sınıf Ekleme
        public IActionResult OnPostAdd()
        {
            var newClass = new ClassInformationModel
            {
                Id = Classes.Count + 1,  // Otomatik ID atama
                ClassName = ClassName,
                StudentCount = StudentCount,
                Description = Description
            };
            Classes.Add(newClass);

            // Formu sıfırla
            ClassName = string.Empty;
            StudentCount = 0;
            Description = string.Empty;

            return Page();
        }

        // Sınıfı Silme
        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = Classes.Find(c => c.Id == id);
            if (classToRemove != null)
            {
                Classes.Remove(classToRemove);
            }

            return Page();
        }

        // Düzenleme İşlemi (Formu Doldurur)
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

        // Güncelleme İşlemi (Düzenlenen Veriyi Kaydeder)
        public IActionResult OnPostUpdate()
        {
            if (ClassIdToEdit == null)
            {
                Console.WriteLine("ClassIdToEdit is null, update failed.");
                return Page();
            }

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
