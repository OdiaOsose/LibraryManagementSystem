using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // SERVICES (BUSINESS) LAYER: the "brain". It never prints and never reads the keyboard.
    // Increment 1: books and borrowers.
    public class LibraryService
    {
        private BookRepository books;
        private BorrowerRepository borrowers;

        // Whoever creates the service must give it the repositories it needs.
        public LibraryService(BookRepository bookRepository, BorrowerRepository borrowerRepository)
        {
            books = bookRepository;
            borrowers = borrowerRepository;
        }

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
    }
}
