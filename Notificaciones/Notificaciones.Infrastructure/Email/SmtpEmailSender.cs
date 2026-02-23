using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Notificaciones.Domain.Interfaces;

namespace Notificaciones.Infrastructure.Email;

/// <summary>
/// Implementa IEmailSender usando MailKit + SMTP.
/// En desarrollo usa MailHog (smtp:1025, sin TLS).
/// En producción usa SMTP real (Gmail, SendGrid, etc.) configurado via appsettings.
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly string _smtpHost;
    private readonly int    _smtpPort;
    private readonly bool   _useSsl;
    private readonly string _usuario;
    private readonly string _password;
    private readonly string _remitenteEmail;
    private readonly string _remitenteName;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
    {
        _smtpHost       = config["Email:SmtpHost"]       ?? "mailhog";
        _smtpPort       = config.GetValue<int>("Email:SmtpPort", 1025);
        _useSsl         = config.GetValue<bool>("Email:UseSsl", false);
        _usuario        = config["Email:Usuario"]        ?? string.Empty;
        _password       = config["Email:Password"]       ?? string.Empty;
        _remitenteEmail = config["Email:RemitenteEmail"] ?? "facturacion@empresa.com";
        _remitenteName  = config["Email:RemitenteName"]  ?? "Sistema de Facturación";
        _logger         = logger;
    }

    public async Task<bool> EnviarAsync(
        string destinatario,
        string nombreDestinatario,
        string asunto,
        string cuerpoHtml)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_remitenteName, _remitenteEmail));
            message.To.Add(new MailboxAddress(nombreDestinatario, destinatario));
            message.Subject = asunto;
            message.Body    = new TextPart("html") { Text = cuerpoHtml };

            using var client = new SmtpClient();

            var secureOption = _useSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await client.ConnectAsync(_smtpHost, _smtpPort, secureOption);

            if (!string.IsNullOrWhiteSpace(_usuario))
                await client.AuthenticateAsync(_usuario, _password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("[SMTP] Email enviado a {Destinatario} | Asunto: {Asunto}", destinatario, asunto);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SMTP] Error enviando email a {Destinatario}", destinatario);
            return false;
        }
    }
}
