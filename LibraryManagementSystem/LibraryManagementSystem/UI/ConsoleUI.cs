using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // PRESENTATION LAYER: the ONLY class that prints to the screen or reads the keyboard.
    // At any question the user can type BACK to cancel and return to the menu.
    public class ConsoleUI
    {
        private LibraryService service;
        private InputValidator validator;

        public ConsoleUI(LibraryService libraryService, InputValidator inputValidator)
        {
            service = libraryService;
            validator = inputValidator;
        }

        // Main loop: show menu, read choice, act, repeat until the user picks 0.
        public void Run()
        {
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = ReadText("Choose an option: ");

                if (choice == "1") { AddBook(); }
                else if (choice == "2") { ShowBooks(); }
                else if (choice == "3") { RegisterBorrower(); }
                else if (choice == "4") { ShowBorrowers(); }
                else if (choice == "5") { IssueBook(); }
                else if (choice == "6") { ReturnBook(); }
                else if (choice == "7") { ShowActiveLoans(); }
                else if (choice == "0") { running = false; }
                else { Console.WriteLine("Invalid choice. Please enter a number from the menu."); }
            }
            Console.WriteLine("Goodbye!");
        }

        private void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== LIBRARY MANAGEMENT SYSTEM =====");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. View all books and availability");
            Console.WriteLine("3. Register a borrower");
            Console.WriteLine("4. View all borrowers");
            Console.WriteLine("5. Issue a book");
            Console.WriteLine("6. Return a book");
            Console.WriteLine("7. View books currently on loan (and overdue charges)");
            Console.WriteLine("0. Exit");
        }

        // ---------- small helpers that read and check input ----------

        // Asks a question and returns what the user typed, without extra spaces.
        private string ReadText(string question)
        {
            Console.Write(question);
            string? text = Console.ReadLine();
            if (text == null)
            {
                return "";
            }
            return text.Trim();
        }

        // Keeps asking until the text is valid. Returns false if the user typed BACK.
        private bool AskText(string question, out string answer)
        {
            while (true)
            {
                string text = ReadText(question);
                if (text.ToUpper() == "BACK")
                {
                    answer = "";
                    return false;
                }
                if (validator.IsValidText(text))
                {
                    answer = text;
                    return true;
                }
                Console.WriteLine("This cannot be empty and cannot contain the | character.");
            }
        }

        // Same as AskText, but for names: no digits or symbols. Returns false if the user typed BACK.
        private bool AskName(string question, out string answer)
        {
            while (true)
            {
                string text = ReadText(question);
                if (text.ToUpper() == "BACK")
                {
                    answer = "";
                    return false;
                }
                if (validator.IsValidName(text))
                {
                    answer = text;
                    return true;
                }
                Console.WriteLine("Names can only contain letters, spaces, hyphens, apostrophes and full stops.");
            }
        }

        // Keeps asking until the user types a whole number above 0. Returns false on BACK.
        private bool AskId(string question, out int number)
        {
            while (true)
            {
                string text = ReadText(question);
                if (text.ToUpper() == "BACK")
                {
                    number = 0;
                    return false;
                }
                if (validator.TryParsePositiveInt(text, out number))
                {
                    return true;
                }
                Console.WriteLine("Please enter a whole number greater than 0.");
            }
        }

        private bool AskCopies(out int copies)
        {
            while (true)
            {
                string text = ReadText("Number of copies (1 to " + InputValidator.MaxCopies + "): ");
                if (text.ToUpper() == "BACK")
                {
                    copies = 0;
                    return false;
                }
                if (!validator.TryParsePositiveInt(text, out copies))
                {
                    Console.WriteLine("Please enter a whole number.");
                }
                else if (!validator.IsCopiesInRange(copies))
                {
                    Console.WriteLine("Copies must be between 1 and " + InputValidator.MaxCopies + ".");
                }
                else
                {
                    return true;
                }
            }
        }

        private bool AskPhone(out string phone)
        {
            while (true)
            {
                string text = ReadText("Phone number (digits only, 7 to 15 digits): ");
                if (text.ToUpper() == "BACK")
                {
                    phone = "";
                    return false;
                }
                if (validator.IsValidPhone(text))
                {
                    phone = text;
                    return true;
                }
                Console.WriteLine("Invalid phone number. Use digits only, 7 to 15 digits.");
            }
        }

        // Asks for the return date. Pressing Enter means today.
        private bool AskReturnDate(out DateTime date)
        {
            while (true)
            {
                string text = ReadText("Return date as yyyy-MM-dd (press Enter for today): ");
                if (text.ToUpper() == "BACK")
                {
                    date = DateTime.Today;
                    return false;
                }
                if (text == "")
                {
                    date = DateTime.Today;
                    return true;
                }
                if (validator.TryParseDate(text, out date))
                {
                    return true;
                }
                Console.WriteLine("Invalid date. Use the form yyyy-MM-dd, for example 2026-12-01.");
            }
        }

        // ---------- menu actions ----------

        private void AddBook()
        {
            Console.WriteLine("(Type BACK at any question to return to the menu.)");
            string title;
            if (!AskText("Title: ", out title)) { return; }
            string author;
            if (!AskName("Author: ", out author)) { return; }
            int copies;
            if (!AskCopies(out copies)) { return; }

            Book book = service.AddBook(title, author, copies);
            Console.WriteLine("Book added. Book ID: " + book.Id);
        }

        private void ShowBooks()
        {
            List<Book> list = service.GetAllBooks();
            Console.WriteLine();
            if (list.Count == 0)
            {
                Console.WriteLine("No books have been added yet.");
                return;
            }

            Console.WriteLine($"{"ID",-5}{"Title",-30}{"Author",-22}{"Available",-10}");
            foreach (Book b in list)
            {
                Console.WriteLine($"{b.Id,-5}{b.Title,-30}{b.Author,-22}{b.AvailableCopies}/{b.TotalCopies}");
            }
        }

        private void RegisterBorrower()
        {
            Console.WriteLine("(Type BACK at any question to return to the menu.)");
            string name;
            if (!AskName("Full name: ", out name)) { return; }
            string phone;
            if (!AskPhone(out phone)) { return; }

            Borrower borrower = service.RegisterBorrower(name, phone);
            Console.WriteLine("Borrower registered. Borrower ID: " + borrower.Id);
        }

        private void ShowBorrowers()
        {
            List<Borrower> list = service.GetAllBorrowers();
            Console.WriteLine();
            if (list.Count == 0)
            {
                Console.WriteLine("No borrowers have been registered yet.");
                return;
            }

            Console.WriteLine($"{"ID",-5}{"Name",-30}{"Phone",-16}");
            foreach (Borrower b in list)
            {
                Console.WriteLine($"{b.Id,-5}{b.Name,-30}{b.Phone,-16}");
            }
        }

        private void IssueBook()
        {
            Console.WriteLine("(Type BACK at any question to return to the menu.)");
            int bookId;
            if (!AskId("Book ID: ", out bookId)) { return; }
            int borrowerId;
            if (!AskId("Borrower ID: ", out borrowerId)) { return; }

            string problem = service.CheckIssue(bookId, borrowerId);
            if (problem != "")
            {
                Console.WriteLine("Cannot issue: " + problem);
                return;
            }

            Loan loan = service.IssueBook(bookId, borrowerId);
            Console.WriteLine("Book issued. Loan ID: " + loan.LoanId);
            Console.WriteLine("Due date: " + DateHelper.Format(loan.DueDate) + " (fine of " + LibraryService.FinePerDay.ToString("N2") + " NGN per day after that)");
        }

        private void ReturnBook()
        {
            Console.WriteLine("(Type BACK at any question to return to the menu.)");
            int loanId;
            if (!AskId("Loan ID: ", out loanId)) { return; }
            DateTime returnDate;
            if (!AskReturnDate(out returnDate)) { return; }

            string problem = service.CheckReturn(loanId, returnDate);
            if (problem != "")
            {
                Console.WriteLine("Cannot return: " + problem);
                return;
            }

            Loan loan = service.ReturnBook(loanId, returnDate);
            Console.WriteLine("Book returned on " + DateHelper.Format(loan.ReturnDate) + ".");
            if (loan.Fine > 0)
            {
                int daysLate = (loan.ReturnDate - loan.DueDate).Days;
                Console.WriteLine("OVERDUE by " + daysLate + " day(s). Fine to pay: " + loan.Fine.ToString("N2") + " NGN");
            }
            else
            {
                Console.WriteLine("Returned on time. No fine.");
            }
        }

        private void ShowActiveLoans()
        {
            List<Loan> list = service.GetActiveLoans();
            Console.WriteLine();
            if (list.Count == 0)
            {
                Console.WriteLine("No books are currently on loan.");
                return;
            }

            Console.WriteLine("Books currently on loan (as of " + DateHelper.Format(DateTime.Today) + "):");
            foreach (Loan l in list)
            {
                int overdue = service.GetDaysOverdue(l, DateTime.Today);
                string status = "on time";
                if (overdue > 0)
                {
                    decimal fineSoFar = service.CalculateFine(l.DueDate, DateTime.Today);
                    status = "OVERDUE " + overdue + " day(s), fine so far " + fineSoFar.ToString("N2") + " NGN";
                }
                Console.WriteLine($"  Loan {l.LoanId}: '{service.GetBookTitle(l.BookId)}' to {service.GetBorrowerName(l.BorrowerId)}, "
                    + $"issued {DateHelper.Format(l.IssueDate)}, due {DateHelper.Format(l.DueDate)} - {status}");
            }
        }
    }
}
