using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Interfaces;

namespace TicketSystem.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly string _filePath;

        public TicketRepository(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await ReadTicketsFromFileAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(string ticketId)
        {
            var tickets = await ReadTicketsFromFileAsync();
            return tickets.FirstOrDefault(t => t.TicketID == ticketId);
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            var tickets = await ReadTicketsFromFileAsync();
            tickets.Add(ticket);
            await WriteTicketsToFileAsync(tickets);
        }

        public async Task UpdateTicketAsync(Ticket updatedTicket)
        {
            var tickets = await ReadTicketsFromFileAsync();
            var existingTicket = tickets.FirstOrDefault(t => t.TicketID == updatedTicket.TicketID);

            if (existingTicket == null)
            {
                throw new Exception($"Ticket with ID: {updatedTicket.TicketID} not found.");
            }

            existingTicket.Description = updatedTicket.Description;
            existingTicket.Status = updatedTicket.Status;
            existingTicket.Priority = updatedTicket.Priority;

            await WriteTicketsToFileAsync(tickets);
        }

        public async Task<bool> DeleteTicketAsync(string ticketId)
        {
            var tickets = await ReadTicketsFromFileAsync();
            var ticketToRemove = tickets.FirstOrDefault(t => t.TicketID == ticketId);

            if (ticketToRemove == null)
            {
                return false;
            }

            tickets.Remove(ticketToRemove);
            await WriteTicketsToFileAsync(tickets);
            return true;
        }

        private async Task<List<Ticket>> ReadTicketsFromFileAsync()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            try
            {
                var jsonData = await File.ReadAllTextAsync(_filePath);
                var tickets = JsonSerializer.Deserialize<List<Ticket>>(jsonData) ?? new List<Ticket>();
                return tickets;
            }
            catch (JsonException ex)
            {
                throw new JsonException("Error reading tickets from file.", ex);
            }
        }

        private async Task WriteTicketsToFileAsync(List<Ticket> tickets)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(tickets, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, jsonData);
            }
            catch (IOException ex)
            {
                throw new IOException("Error writing tickets to file.", ex);
            }
        }
    }
}

