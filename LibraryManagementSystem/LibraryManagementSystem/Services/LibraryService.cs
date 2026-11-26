using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // SERVICES (BUSINESS) LAYER: the "brain". It never prints and never reads the keyboard.
    // Increment 2: books and borrowers + issuing and returning books.
    public class LibraryService
    {
        public const int LoanDays = 14;          // a book may be kept 14 days
        public const int MaxActiveLoans = 3;     // a borrower may have at most 3 books out

        private BookRepository books;
        private BorrowerRepository borrowers;
        private LoanRepository loans;

        // Whoever creates the service must give it the three repositories.
        public LibraryService(BookRepository bookRepository, BorrowerRepository borrowerRepository, LoanRepository loanRepository)
        {
            books = bookRepository;
            borrowers = borrowerRepository;
            loans = loanRepository;
        }

        // ---- Books and borrowers ----

        public Book AddBook(string title, string author, int copies)
        {
            return books.Add(title.Trim(), author.Trim(), copies);
        }

        public List<Book> GetAllBooks()
        {
            return books.GetAll();
        }

        public Borrower RegisterBorrower(string name, string phone)
        {
            return borrowers.Add(name.Trim(), phone.Trim());
        }

        public List<Borrower> GetAllBorrowers()
        {
            return borrowers.GetAll();
        }

        // ---- Issuing ----

        // Returns "" if the book can be issued, otherwise a message saying why not.
        public string CheckIssue(int bookId, int borrowerId)
        {
            if (!books.HasBook(bookId))
            {
                return "Book ID " + bookId + " does not exist.";
            }
            if (!borrowers.HasBorrower(borrowerId))
            {
                return "Borrower ID " + borrowerId + " does not exist.";
            }
            Book book = books.GetById(bookId);
            if (book.AvailableCopies < 1)
            {
                return "No copies of '" + book.Title + "' are available right now.";
            }
            if (loans.CountActiveForBorrower(borrowerId) >= MaxActiveLoans)
            {
                return "This borrower already has " + MaxActiveLoans + " books out (the limit).";
            }
            return "";
        }

        // Issues the book: records the loan (due in 14 days) and takes one copy off the shelf.
        // Call CheckIssue first.
        public Loan IssueBook(int bookId, int borrowerId)
        {
            DateTime issueDate = DateTime.Today;               // today's date
            DateTime dueDate = issueDate.AddDays(LoanDays);    // AddDays moves a date forward
            Loan loan = loans.Create(bookId, borrowerId, issueDate, dueDate);
            books.ChangeAvailable(bookId, -1);
            return loan;
        }

        // ---- Returning ----

        // Returns "" if the return is allowed, otherwise a message saying why not.
        public string CheckReturn(int loanId)
        {
            if (!loans.HasLoan(loanId))
            {
                return "Loan ID " + loanId + " does not exist.";
            }
            Loan loan = loans.GetById(loanId);
            if (loan.IsReturned)
            {
                return "Loan " + loanId + " has already been returned.";
            }
            return "";
        }

        // Completes the return today and puts the copy back on the shelf. Call CheckReturn first.
        // (Overdue charges are added in Increment 3, so the fine is 0 for now.)
        public Loan ReturnBook(int loanId)
        {
            Loan loan = loans.GetById(loanId);
            loans.Complete(loanId, DateTime.Today, 0m);   // 0m = a decimal zero
            books.ChangeAvailable(loan.BookId, 1);
            return loans.GetById(loanId);
        }

        // ---- Looking things up ----

        public List<Loan> GetActiveLoans()
        {
            return loans.GetActive();
        }

        public string GetBookTitle(int bookId)
        {
            if (books.HasBook(bookId))
            {
                return books.GetById(bookId).Title;
            }
            return "(unknown book)";
        }

        public string GetBorrowerName(int borrowerId)
        {
            if (borrowers.HasBorrower(borrowerId))
            {
                return borrowers.GetById(borrowerId).Name;
            }
            return "(unknown borrower)";
        }
    }
}
