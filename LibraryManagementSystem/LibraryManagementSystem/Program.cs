using System;

namespace LibraryManagementSystem
{
    class Program
    {
        // The program starts here.
        static void Main()
        {
            // Build the layers and connect them. "new" creates an object from a class.
            BookRepository books = new BookRepository();              // data: books.txt
            BorrowerRepository borrowers = new BorrowerRepository();  // data: borrowers.txt
            LoanRepository loans = new LoanRepository();              // data: loans.txt
            InputValidator validator = new InputValidator();          // services: input rules
            LibraryService service = new LibraryService(books, borrowers, loans);  // services: library rules
            ConsoleUI ui = new ConsoleUI(service, validator);         // presentation

            ui.Run();   // start the menu
        }
    }
}
