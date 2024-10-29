using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Interfaces;

namespace TicketSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllTicketsAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(string ticketId)
        {
            return await _ticketRepository.GetTicketByIdAsync(ticketId);
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            // Validate the ticket before adding it to the repository.
            var validatedTicket = ValidateAndPrepareTicket(ticket);

            // Add the validated ticket to the repository.
            await _ticketRepository.AddTicketAsync(validatedTicket);
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var validatedTicket = ValidateAndPrepareTicket(ticket);
            await _ticketRepository.UpdateTicketAsync(validatedTicket);
        }

        public async Task<bool> DeleteTicketAsync(string ticketId)
        {
            return await _ticketRepository.DeleteTicketAsync(ticketId);
        }

        public Ticket ValidateAndPrepareTicket(Ticket ticket)
        {
            // Check if the ticket description is null or empty.
            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new ArgumentException("Ticket description cannot be empty");
            }

            // If the ticket is valid, set its status to "New".
            ticket.Status = "New";

            return ticket;
        }
     
    
        
    }
}