using System;

namespace LibraryManagementSystem
{
    // MODEL: one borrowing event: "this borrower took this book on this date".
    public class Loan
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public DateTime IssueDate { get; set; }   // DateTime = a date (and time) value
        public DateTime DueDate { get; set; }     // the last day the book can be returned without a fine
        public bool IsReturned { get; set; }      // false while the book is still out
        public DateTime ReturnDate { get; set; }  // only meaningful when IsReturned is true
        public decimal Fine { get; set; }         // overdue charge, 0 if returned on time

        public Loan(int loanId, int bookId, int borrowerId, DateTime issueDate, DateTime dueDate,
                    bool isReturned, DateTime returnDate, decimal fine)
        {
            LoanId = loanId;
            BookId = bookId;
            BorrowerId = borrowerId;
            IssueDate = issueDate;
            DueDate = dueDate;
            IsReturned = isReturned;
            ReturnDate = returnDate;
            Fine = fine;
        }
    }
}
