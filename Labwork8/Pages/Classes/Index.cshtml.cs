using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Labwork5.Data; // DbContext'in bulunduğu namespace'e göre güncelle
using Labwork5.Models; // Class modelinin bulunduğu namespace'e göre güncelle
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labwork5.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        public IList<Class> ClassList { get; set; }

        public async Task OnGetAsync()
        {
            ClassList = await _context.Classes.ToListAsync();
        }
    }
}
