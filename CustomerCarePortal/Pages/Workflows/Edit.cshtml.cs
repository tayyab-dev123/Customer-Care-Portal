using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomerCarePortal.Data;
using CustomerCarePortal.Models;

namespace CustomerCarePortal.Pages.Workflows
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly CustomerCarePortal.Data.ApplicationDbContext _context;

        public EditModel(CustomerCarePortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public State InitialState { get; set; } = default!;

        [BindProperty]
        public Workflow Workflow { get; set; } = default!;
        [BindProperty]
        public State NewState { get; set; } = default!;
        [BindProperty]
        public Transition NewTransition { get; set; } = default!;
        public List<Transition> Transitions { get; set; } = new List<Transition>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Workflows == null)
            {
                return NotFound();
            }

            var workflow = await _context.Workflows.FirstOrDefaultAsync(m => m.Id == id);
            if (workflow == null)
            {
                return NotFound();
            }
            var states = await _context.States.ToListAsync();
            workflow.States = states.FindAll(s => s.Workflow == workflow);
            Workflow = workflow;
            IList<Transition> transitions = await _context.Transitions.ToListAsync();
            foreach(State state in workflow.States)
            {
                foreach(Transition transition in transitions)
                {
                    if(transition.SourceStateId == state.Id)
                    {
                        Transitions.Add(transition);
                    }
                }
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostUpdateWorkflowAsync()
        {
            if (Workflow is null)
            {
                return Page();
            }
            _context.Workflows.Update(Workflow);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkflowExists(Workflow.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Edit", new { id = Workflow.Id });
        }

        private bool WorkflowExists(int id)
        {
            return (_context.Workflows?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> OnPostAddStateAsync()
        {
            if (NewState is not null)
            {
                try
                {
                    NewState.Workflow = await _context.Workflows.FirstOrDefaultAsync(w => w.Id == Workflow.Id);
                    await _context.States.AddAsync(NewState);
                    await _context.SaveChangesAsync();

                }
                catch
                {
                    throw;
                }
            }
            return RedirectToPage("./Edit", new { id = Workflow.Id });
        }
        public async Task<IActionResult> OnPostAddTransitionAsync()
        {
            if (NewTransition is not null)
            {
                try
                {
                    await _context.Transitions.AddAsync(NewTransition);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }
            }
            return RedirectToPage("./Edit", new { id = Workflow.Id });
        }

    }
}
