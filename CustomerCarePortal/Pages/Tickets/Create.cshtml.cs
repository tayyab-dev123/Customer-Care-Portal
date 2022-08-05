using CustomerCarePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerCarePortal.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public CreateModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Ticket NewTicket { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (NewTicket == null)
            {
                return Page();
            }
            try
            {
                History history = new History();
                history.TicketId = NewTicket.Id;
                history.Ticket = NewTicket;
                NewTicket.History = history;
                NewTicket.TrackingId = GenerateId();
                _context.Histories.Add(history);
                _context.Tickets.Add(NewTicket);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return RedirectToPage("/Tickets/Success", new { id = NewTicket.Id });
        }
        private string GenerateId()
        {
            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            var ticketTrack = _context.Tickets.FirstOrDefault(t => t.TrackingId.Equals(str));
            if (ticketTrack is null)
            {
                return str;
            }
            else {
                return GenerateId();
            }
        }
    }
}
