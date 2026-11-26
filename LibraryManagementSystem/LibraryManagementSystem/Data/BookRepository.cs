using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryManagementSystem
{
    // DATA LAYER: stores books in books.txt. One book per line: id|title|author|totalCopies|availableCopies
    public class BookRepository
    {
        private string filePath;
        private List<Book> books = new List<Book>();

        public BookRepository()
        {
            filePath = Path.Combine(AppContext.BaseDirectory, "books.txt");
            Load();
        }

        // Reads the file into the list. A missing file just means "no books yet". Damaged lines are skipped.
        private void Load()
        {
            books.Clear();
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split('|');   // cut the line at each "|"
                if (parts.Length == 5)
                {
                    int id;
                    int total;
                    int available;
                    // All three numbers must be readable (&& means AND), otherwise the line is skipped.
                    if (int.TryParse(parts[0], out id) && int.TryParse(parts[3], out total) && int.TryParse(parts[4], out available)
                        && total >= 0 && available >= 0 && available <= total)
                    {
                        books.Add(new Book(id, parts[1], parts[2], total, available));
                    }
                }
            }
        }

        // Writes the whole list back to the file.
        private void Save()
        {
            List<string> lines = new List<string>();
            foreach (Book b in books)
            {
                lines.Add(b.Id + "|" + b.Title + "|" + b.Author + "|" + b.TotalCopies + "|" + b.AvailableCopies);
            }
            File.WriteAllLines(filePath, lines);
        }

        // Next free id = highest id so far + 1.
        private int NextId()
        {
            int highest = 0;
            foreach (Book b in books)
            {
                if (b.Id > highest)
                {
                    highest = b.Id;
                }
            }
            return highest + 1;
        }

        // Adds a new book. All copies start on the shelf (available = total).
        public Book Add(string title, string author, int copies)
        {
            Book book = new Book(NextId(), title, author, copies, copies);
            books.Add(book);
            Save();
            return book;
        }

        public List<Book> GetAll()
        {
            return books;
        }

        public bool HasBook(int id)
        {
            foreach (Book b in books)
            {
                if (b.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        // Callers must check HasBook first. If the id doesn't exist you get an empty placeholder book with Id 0.
        public Book GetById(int id)
        {
            foreach (Book b in books)
            {
                if (b.Id == id)
                {
                    return b;
                }
            }
            return new Book(0, "", "", 0, 0);
        }

        // Adds (+1) or removes (-1) one available copy and saves.
        public void ChangeAvailable(int id, int change)
        {
            foreach (Book b in books)
            {
                if (b.Id == id)
                {
                    b.AvailableCopies = b.AvailableCopies + change;
                }
            }
            Save();
        }
    }
}
