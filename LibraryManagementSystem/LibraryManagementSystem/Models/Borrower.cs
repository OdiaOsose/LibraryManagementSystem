using System;

namespace LibraryManagementSystem
{
    // MODEL: one registered library member who can borrow books.
    public class Borrower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public Borrower(int id, string name, string phone)
        {
            Id = id;
            Name = name;
            Phone = phone;
        }
    }
}
