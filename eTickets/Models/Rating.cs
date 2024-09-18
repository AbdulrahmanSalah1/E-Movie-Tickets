namespace eTickets.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Value { get; set; } // Assuming a rating from 1 to 5
        public string UserName { get; set; }
    }
}
