using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Library.Controllers
{
	[Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .SingleOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create([Bind("BookID,Title,Author,Genre,Price,Avaliable")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.SingleOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("BookID,Title,Author,Genre,Price,Avaliable")] Book book)
        {
            if (id != book.BookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .SingleOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.SingleOrDefaultAsync(m => m.BookID == id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Books/IssueBook/5
        [Authorize(Roles = "Admin,Librarian, User")]
        public async Task<IActionResult> IssueBook(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.SingleOrDefaultAsync(m => m.BookID == id);
            if (book == null)
            {
                return NotFound();
            }
            BooksIssueViewModel IssueVM = new BooksIssueViewModel();
            IssueVM.book = book;
            IQueryable<string> usersQuery = from m in _context.ApplicationUser
                                            orderby m.UserName
                                            select m.UserName;

            var movies = from m in _context.ApplicationUser
                         select m;
            IssueVM.Users = new SelectList(await usersQuery.Distinct().ToListAsync());
            return View(IssueVM);
        }

        // POST: Books/IssueBook/5
        [HttpPost, ActionName("IssueBook")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian,User")]
        public async Task<IActionResult> IssueBook(int id, BooksIssueViewModel IssueBook)
        {
            var book = await _context.Book.SingleOrDefaultAsync(m => m.BookID == id);
            book.Avaliable = false;

            var user = await _userManager.FindByNameAsync(IssueBook.Username);
            book.TakenBy = user;
            _context.Book.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Books/ReturnBook/5
        [Authorize(Roles = "Admin,Librarian, User")]
        public async Task<IActionResult> ReturnBook(string id, int Bookid)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var book = await _context.Book.SingleOrDefaultAsync(m => m.TakenBy.UserName == id);
            if (book == null)
            {
                return NotFound();
            }
            if(book.BookID != Bookid)
            {
                return NotFound();
            }
            ReturnBookViewModel IssueVM = new ReturnBookViewModel();
            IssueVM.book = book;
            IssueVM.Username = user.UserName;
            IssueVM.bookID = book.BookID;
            return View(IssueVM);
        }

        // POST: Books/IssueBook/5
        [HttpPost, ActionName("ReturnBook")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian,User")]
        public async Task<IActionResult> ReturnBook(string id, [Bind("Username,bookID")] ReturnBookViewModel ReturnBook)
        {

            var user = await _userManager.FindByNameAsync(id);
            var bookid = ReturnBook.bookID;
            var book = await _context.Book.SingleOrDefaultAsync(m => m.BookID == bookid);
                //return NotFound(book);
            if (user == null)
            {
                return NotFound();
            }
            book.Avaliable = true;
            book.TakenBy = null;
            _context.Book.Update(book); 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.BookID == id);
        }

        
    }
}
