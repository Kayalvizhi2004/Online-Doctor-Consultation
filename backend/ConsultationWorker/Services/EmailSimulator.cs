using Microsoft.Extensions.Logging;

namespace ConsultationWorker.Services;

public class EmailSimulator
{
    private readonly ILogger<EmailSimulator> _logger;

    public EmailSimulator(ILogger<EmailSimulator> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("[EmailSimulator] To={To} Subject={Subject} Body={Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
