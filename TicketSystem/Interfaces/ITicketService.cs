using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSystem.Models;

namespace TicketSystem.Interfaces
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(string ticketId);
        Task AddTicketAsync(Ticket ticket);
        
        Task UpdateTicketAsync(Ticket ticket);      
        Task<bool> DeleteTicketAsync(string ticketId);  
        Ticket ValidateAndPrepareTicket(Ticket ticket);
    }
}