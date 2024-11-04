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
using TicketSystem.Models; // Confirm that this is the correct namespace for the Ticket model

public static class PostTicketFunction
{
    [FunctionName("PostTicket")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tickets")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing a POST request to add a new ticket.");

        // Read the request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        Ticket ticket;
        try
        {
            ticket = JsonConvert.DeserializeObject<Ticket>(requestBody);
        }
        catch (JsonException ex)
        {
            log.LogWarning($"JSON parsing error: {ex.Message}");
            return new BadRequestObjectResult("Invalid ticket data provided.");
        }

        // Validate the ticket data: Ensure required fields are not missing or empty.
        if (ticket == null ||
            string.IsNullOrWhiteSpace(ticket.TicketID) ||
            string.IsNullOrWhiteSpace(ticket.Description) ||
            string.IsNullOrWhiteSpace(ticket.Status) ||
            string.IsNullOrWhiteSpace(ticket.Priority))
        {
            log.LogWarning("Invalid ticket data received: missing required fields.");
            return new BadRequestObjectResult("Invalid ticket data provided.");
        }

        // Set the DateCreated and LastUpdated fields
        ticket.DateCreated = DateTime.UtcNow;
        ticket.LastUpdated = DateTime.UtcNow;

        // Get the file path from environment variables or use a default path.
        string filePath = Environment.GetEnvironmentVariable("FilePath") 
                          ?? Path.Combine(Environment.CurrentDirectory, "tickets.json");
        List<Ticket> tickets = new List<Ticket>();

        // Read existing tickets if the file exists.
        if (File.Exists(filePath))
        {
            string existingData = await File.ReadAllTextAsync(filePath);
            tickets = JsonConvert.DeserializeObject<List<Ticket>>(existingData) ?? new List<Ticket>();
        }

        // Add the new ticket to the list and save it back to the JSON file.
        tickets.Add(ticket);
        await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(tickets, Formatting.Indented));

        log.LogInformation($"Ticket with ID: {ticket.TicketID} added successfully.");
        return new OkObjectResult($"Ticket added successfully: {ticket.TicketID}");
    }
}


