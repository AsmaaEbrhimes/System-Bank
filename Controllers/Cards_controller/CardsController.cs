using Banking.Model.Cards;
using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;


namespace Banking.Controllers.Cards_controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ContextApi _contextApi;

        public CardsController(ContextApi context)
        {
            _contextApi = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _contextApi.Cards.Include(card => card.Account)
                .ToListAsync();
            return Ok(cards);
        }



        [HttpGet("{Id}")]
        public async Task<ActionResult<Customer>> GetCardsById(int Id)
        {
            var cardItem = await _contextApi.Cards.FirstOrDefaultAsync(item => item.Id == Id);
            if (cardItem == null) return NotFound();
            return Ok(cardItem);
        }


        [HttpPost("request")]
        public async Task<IActionResult> RequestCard([FromBody] CardRequestDto dto)
        {
            var account = await _contextApi.Accounts.FirstOrDefaultAsync(item => item.Id == dto.AccountId);
            if (account == null) return NotFound();
            if (dto.Pin.Length != 4) return BadRequest("الرقم السري (PIN) يجب أن يتكون من 4 أرقام.");
            var random = new Random();
            string cardNumber = "4" + DateTime.Now.Ticks.ToString().Substring(3, 15);
            string cvv = random.Next(100, 999).ToString();
            var card = new Card
            {
                AccountId = dto.AccountId,
                Pin = dto.Pin,
                CardNumber = cardNumber,
                Cvv = cvv,
                ExpiryDate = DateTime.Now.AddYears(3),
                CardType = dto.CardType ?? "Debit",
                IsBlocked = false
            };

            _contextApi.Cards.Add(card);
            await _contextApi.SaveChangesAsync();
            return Ok(new
            {
                message = "تم إصدار الكارت بنجاح.",
                cardId = card.Id,
                cardNumber = card.CardNumber,
                expiryDate = card.ExpiryDate.ToString("MM/yy"),
            });
        }




        [HttpPut("{Id}/block")]
        public async Task<IActionResult> BlockCard(int Id)
        {
            var card_id = await _contextApi.Cards.FindAsync(Id);
            if (card_id == null) return NotFound("الكارت غير موجود.");
            if (card_id.IsBlocked) return BadRequest("الكارت موقوف بالفعل.");
            card_id.IsBlocked = true;
            await _contextApi.SaveChangesAsync();
            return Ok(new { message = "تم إيقاف الكارت بنجاح لحماية حسابك." });
        }


        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> UnblockCard(int id)
        {
            var card_id = await _contextApi.Cards.FindAsync(id);
            if (card_id == null) return NotFound("الكارت غير موجود.");
            if (!card_id.IsBlocked) return BadRequest("الكارت يعمل بالفعل وليس موقوفاً.");
            card_id.IsBlocked = false;
            await _contextApi.SaveChangesAsync();
            return Ok(new { message = "تم إعادة تفعيل الكارت بنجاح." });
        }


        [HttpPut("{id}/change-pin")]
        public async Task<IActionResult> ChangePin(int id, [FromBody] ChangePinDto dto)
        {
            var card = await _contextApi.Cards.FindAsync(id);
            if (card == null) return NotFound("الكارت غير موجود.");
            if (dto.OldPin != card.Pin) return BadRequest("الرقم السري القديم غير صحيح.");
            if (dto.NewPin.Length != 4) return BadRequest("الرقم السري الجديد يجب أن يتكون من 4 أرقام.");
            card.Pin = dto.NewPin;
            await _contextApi.SaveChangesAsync();
            return Ok(new { message = "تم تغيير الرقم السري بنجاح." });
        }
    }
}
