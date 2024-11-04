using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using TicketSystem.Models;

public static class GetTicketsFunction
{
    [FunctionName("GetTickets")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "tickets/{ticketId?}")] HttpRequest req,
        ILogger log, string ticketId)
    {
        log.LogInformation("Processing a GET request to retrieve ticket information.");

        try
        {
            // Use the file path from the environment variable if available.
            string filePath = Environment.GetEnvironmentVariable("FilePath") 
                              ?? Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? Environment.CurrentDirectory, "tickets.json");

            log.LogInformation($"Using file path: {filePath}");

            if (!File.Exists(filePath))
            {
                log.LogWarning("No ticket file found.");
                return new NotFoundObjectResult("Tickets file not found.");
            }

            string jsonData = await File.ReadAllTextAsync(filePath);
            var tickets = JsonConvert.DeserializeObject<List<Ticket>>(jsonData) ?? new List<Ticket>();

            log.LogInformation($"Total tickets loaded: {tickets.Count}");

            if (!string.IsNullOrEmpty(ticketId))
            {
                log.LogInformation($"Searching for ticket with ID: {ticketId}");
                
                // Find the ticket with the matching ID
                var ticket = tickets.Find(t => t.TicketID == ticketId);
                if (ticket != null)
                {
                    log.LogInformation($"Found ticket: {JsonConvert.SerializeObject(ticket)}");
                    return new OkObjectResult(ticket);
                }
                else
                {
                    log.LogWarning($"Ticket with ID: {ticketId} not found.");
                    return new NotFoundObjectResult($"Ticket with ID: {ticketId} not found.");
                }
            }

            log.LogInformation("Returning all tickets.");
            return new OkObjectResult(tickets);
        }
        catch (JsonException ex)
        {
            log.LogError(ex, "Failed to parse ticket data.");
            return new ObjectResult("Error reading ticket data.") { StatusCode = StatusCodes.Status500InternalServerError };
        }
        catch (Exception ex)
        {
            log.LogError(ex, "An error occurred while retrieving tickets.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}


