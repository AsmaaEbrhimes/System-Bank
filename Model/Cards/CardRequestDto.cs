using Banking_System.Model;

namespace Banking.Model.Cards
{
    public class CardRequestDto
    {
        public int AccountId { get; set; }
        public string Pin { get; set; }
        public string CardType { get; set; }

    }
}
