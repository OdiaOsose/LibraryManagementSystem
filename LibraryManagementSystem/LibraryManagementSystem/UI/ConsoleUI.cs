using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // PRESENTATION LAYER: the ONLY class that prints to the screen or reads the keyboard.
    // At any question the user can type BACK to cancel and return to the menu.
    // Increment 1: add/view books, register/view borrowers.
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
    }
}
