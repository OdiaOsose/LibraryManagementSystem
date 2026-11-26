using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryManagementSystem
{
    // DATA LAYER: stores borrowers in borrowers.txt. One per line: id|name|phone
    public class BorrowerRepository
    {
        private string filePath;
        private List<Borrower> borrowers = new List<Borrower>();

        public BorrowerRepository()
        {
            filePath = Path.Combine(AppContext.BaseDirectory, "borrowers.txt");
            Load();
        }

        private void Load()
        {
            borrowers.Clear();
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split('|');
                if (parts.Length == 3)
                {
                    int id;
                    if (int.TryParse(parts[0], out id))
                    {
                        borrowers.Add(new Borrower(id, parts[1], parts[2]));
                    }
                }
            }
        }

        private void Save()
        {
            List<string> lines = new List<string>();
            foreach (Borrower b in borrowers)
            {
                lines.Add(b.Id + "|" + b.Name + "|" + b.Phone);
            }
            File.WriteAllLines(filePath, lines);
        }

        private int NextId()
        {
            int highest = 0;
            foreach (Borrower b in borrowers)
            {
                if (b.Id > highest)
                {
                    highest = b.Id;
                }
            }
            return highest + 1;
        }

        public Borrower Add(string name, string phone)
        {
            Borrower borrower = new Borrower(NextId(), name, phone);
            borrowers.Add(borrower);
            Save();
            return borrower;
        }

        public List<Borrower> GetAll()
        {
            return borrowers;
        }

        public bool HasBorrower(int id)
        {
            foreach (Borrower b in borrowers)
            {
                if (b.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        // Callers must check HasBorrower first. Unknown id gives an empty placeholder with Id 0.
        public Borrower GetById(int id)
        {
            foreach (Borrower b in borrowers)
            {
                if (b.Id == id)
                {
                    return b;
                }
            }
            return new Borrower(0, "", "");
        }
    }
}
