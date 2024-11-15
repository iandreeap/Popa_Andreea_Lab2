using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Popa_Andreea_Lab2.Data;
using Popa_Andreea_Lab2.Models;

namespace Popa_Andreea_Lab2.Pages.Books
{
    public class CreateModel : BookCategoriesPageModel
    {
        private readonly Popa_Andreea_Lab2.Data.Popa_Andreea_Lab2Context _context;

        public CreateModel(Popa_Andreea_Lab2.Data.Popa_Andreea_Lab2Context context)
        {
            _context = context;
        }

        public class BookCategoriesPageModel : PageModel
        {
            public List<AssignedCategoryData> AssignedCategoryDataList;
            public void PopulateAssignedCategoryData(Popa_Andreea_Lab2Context context,
            Book book)
            {
                var allCategories = context.Category;
                var bookCategories = new HashSet<int>(
                book.BookCategories.Select(c => c.CategoryID)); //
                AssignedCategoryDataList = new List<AssignedCategoryData>();
                foreach (var cat in allCategories)
                {
                    AssignedCategoryDataList.Add(new AssignedCategoryData
                    {
                        CategoryID = cat.ID,
                        Name = cat.CategoryName,
                        Assigned = bookCategories.Contains(cat.ID)
                    });
                }
            }
            public void UpdateBookCategories(Popa_Andreea_Lab2Context context,
            string[] selectedCategories, Book bookToUpdate)
            {
                if (selectedCategories == null)
                {
                    bookToUpdate.BookCategories = new List<BookCategory>();
                    return;
                }
                var selectedCategoriesHS = new HashSet<string>(selectedCategories);
                var bookCategories = new HashSet<int>
                (bookToUpdate.BookCategories.Select(c => c.Category.ID));
                foreach (var cat in context.Category)
                {
                    if (selectedCategoriesHS.Contains(cat.ID.ToString()))
                    {
                        if (!bookCategories.Contains(cat.ID))
                        {
                            bookToUpdate.BookCategories.Add(
                            new BookCategory
                            {
                                BookID = bookToUpdate.ID,
                                CategoryID = cat.ID
                            });
                        }
                    }
                    else
                    {
                        if (bookCategories.Contains(cat.ID))
                        {
                            BookCategory bookToRemove
                            = bookToUpdate
                            .BookCategories
                           .SingleOrDefault(i => i.CategoryID == cat.ID);
                            context.Remove(bookToRemove);
                        }
                    }
                }
            }
        }

        public IActionResult OnGet()
        {
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "ID",
"PublisherName");
            ViewData["AuthorID"] = new SelectList(_context.Author, "ID", "LastName");
            var book = new Book();
            book.BookCategories = new List<BookCategory>();
            PopulateAssignedCategoryData(_context, book);
            return Page();
        }
       

        [BindProperty]
        public Book Book { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string[] selectedCategories)
        {
            var newBook = new Book();
            if (selectedCategories != null)
            {
                newBook.BookCategories = new List<BookCategory>();
                foreach (var cat in selectedCategories)
                {
                    var catToAdd = new BookCategory
                    {
                        CategoryID = int.Parse(cat)
                    };
                    newBook.BookCategories.Add(catToAdd);
                }
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Book.BookCategories = newBook.BookCategories;
            _context.Book.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
