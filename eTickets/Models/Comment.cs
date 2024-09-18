using System;

namespace eTickets.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
    }
}
