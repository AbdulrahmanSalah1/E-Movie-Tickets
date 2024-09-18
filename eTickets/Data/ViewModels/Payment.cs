namespace eTickets.Data.ViewModels
{
    public class Payment
    {
        // Credit card number for the transaction
        public string CardNumber { get; set; }
        // Expiry date of the credit card in MM/YY format
        public string ExpiryDate { get; set; }
        // CVV (Card Verification Value) is a security
        // feature for card transactions
        public string CVV { get; set; }
    }
}
