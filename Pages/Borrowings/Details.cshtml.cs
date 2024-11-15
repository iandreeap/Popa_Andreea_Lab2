﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Popa_Andreea_Lab2.Data;
using Popa_Andreea_Lab2.Models;

namespace Popa_Andreea_Lab2.Pages.Borrowings
{
    public class DetailsModel : PageModel
    {
        private readonly Popa_Andreea_Lab2.Data.Popa_Andreea_Lab2Context _context;

        public DetailsModel(Popa_Andreea_Lab2.Data.Popa_Andreea_Lab2Context context)
        {
            _context = context;
        }

        public Borrowing Borrowing { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowing
                .Include(b=>b.Member)
                .Include(b=>b.Book)
                .ThenInclude(book=>book.Author)
                .FirstOrDefaultAsync(m=> m.ID == id);
            if (borrowing == null)
            {
                return NotFound();
            }
            Borrowing = borrowing;
                
            
            return Page();
        }
    }
}
