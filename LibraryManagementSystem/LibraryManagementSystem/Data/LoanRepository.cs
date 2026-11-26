using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LibraryManagementSystem
{
    // DATA LAYER: stores loans in loans.txt.
    // One per line: loanId|bookId|borrowerId|issueDate|dueDate|returnDate (empty if not returned yet)|fine
    public class LoanRepository
    {
        private string filePath;
        private List<Loan> loans = new List<Loan>();

        public LoanRepository()
        {
            filePath = Path.Combine(AppContext.BaseDirectory, "loans.txt");
            Load();
        }

        private void Load()
        {
            loans.Clear();
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split('|');
                if (parts.Length == 7)
                {
                    int loanId;
                    int bookId;
                    int borrowerId;
                    DateTime issue;
                    DateTime due;
                    decimal fine;

                    // Every part must be readable, otherwise the damaged line is skipped.
                    if (int.TryParse(parts[0], out loanId) && int.TryParse(parts[1], out bookId) && int.TryParse(parts[2], out borrowerId)
                        && DateHelper.TryParse(parts[3], out issue) && DateHelper.TryParse(parts[4], out due)
                        && decimal.TryParse(parts[6], NumberStyles.Number, CultureInfo.InvariantCulture, out fine))
                    {
                        bool isReturned = parts[5] != "";   // an empty return date means "still out"
                        DateTime returned = DateTime.MinValue;
                        bool dateOk = true;
                        if (isReturned)
                        {
                            dateOk = DateHelper.TryParse(parts[5], out returned);
                        }
                        if (dateOk)
                        {
                            loans.Add(new Loan(loanId, bookId, borrowerId, issue, due, isReturned, returned, fine));
                        }
                    }
                }
            }
        }

        private void Save()
        {
            List<string> lines = new List<string>();
            foreach (Loan l in loans)
            {
                string returnText = "";
                if (l.IsReturned)
                {
                    returnText = DateHelper.Format(l.ReturnDate);
                }
                lines.Add(l.LoanId + "|" + l.BookId + "|" + l.BorrowerId + "|" + DateHelper.Format(l.IssueDate) + "|"
                    + DateHelper.Format(l.DueDate) + "|" + returnText + "|" + l.Fine.ToString(CultureInfo.InvariantCulture));
            }
            File.WriteAllLines(filePath, lines);
        }

        private int NextId()
        {
            int highest = 0;
            foreach (Loan l in loans)
            {
                if (l.LoanId > highest)
                {
                    highest = l.LoanId;
                }
            }
            return highest + 1;
        }

        // Records a new loan (book is out, not returned, no fine yet).
        public Loan Create(int bookId, int borrowerId, DateTime issueDate, DateTime dueDate)
        {
            Loan loan = new Loan(NextId(), bookId, borrowerId, issueDate, dueDate, false, DateTime.MinValue, 0m);
            loans.Add(loan);
            Save();
            return loan;
        }

        public List<Loan> GetAll()
        {
            return loans;
        }

        // Only the loans whose book has not come back yet.
        public List<Loan> GetActive()
        {
            List<Loan> active = new List<Loan>();
            foreach (Loan l in loans)
            {
                if (!l.IsReturned)
                {
                    active.Add(l);
                }
            }
            return active;
        }

        // How many books this borrower currently has out.
        public int CountActiveForBorrower(int borrowerId)
        {
            int count = 0;
            foreach (Loan l in loans)
            {
                if (l.BorrowerId == borrowerId && !l.IsReturned)
                {
                    count++;   // ++ adds 1 (slide 17)
                }
            }
            return count;
        }

        public bool HasLoan(int loanId)
        {
            foreach (Loan l in loans)
            {
                if (l.LoanId == loanId)
                {
                    return true;
                }
            }
            return false;
        }

        // Callers must check HasLoan first. Unknown id gives an empty placeholder with LoanId 0.
        public Loan GetById(int loanId)
        {
            foreach (Loan l in loans)
            {
                if (l.LoanId == loanId)
                {
                    return l;
                }
            }
            return new Loan(0, 0, 0, DateTime.MinValue, DateTime.MinValue, false, DateTime.MinValue, 0m);
        }

        // Marks a loan as returned on the given date, stores the fine, and saves.
        public void Complete(int loanId, DateTime returnDate, decimal fine)
        {
            foreach (Loan l in loans)
            {
                if (l.LoanId == loanId)
                {
                    l.IsReturned = true;
                    l.ReturnDate = returnDate;
                    l.Fine = fine;
                }
            }
            Save();
        }
    }
}
