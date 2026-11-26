using System;

namespace LibraryManagementSystem
{
    // MODEL: one book title in the library. A book can have several copies.
    public class Book
    {
        public int Id { get; set; }              // unique number the program gives the book (1, 2, 3...)
        public string Title { get; set; }
        public string Author { get; set; }
        public int TotalCopies { get; set; }     // how many copies the library owns
        public int AvailableCopies { get; set; } // how many are on the shelf right now

        // Constructor: fills in all the values when we create a Book with "new".
        public Book(int id, string title, string author, int totalCopies, int availableCopies)
        {
            Id = id;
            Title = title;
            Author = author;
            TotalCopies = totalCopies;
            AvailableCopies = availableCopies;
        }
    }
}
