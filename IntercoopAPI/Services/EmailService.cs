using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IntercoopAPI.Services
{
    
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string para, string asunto, string mensajeHtml);
    }

    
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoAsync(string para, string asunto, string mensajeHtml)
        {
            var smtpServer = "smtp.mailtrap.io"; 
            var smtpPort = 587;
            var smtpUser = "tu_usuario_prueba";
            var smtpPass = "tu_password_prueba";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var correoEvaluador = "carlosescobar@intercop.gt";

            var mensajeInterceptado = mensajeHtml + 
                $"<br/><br/><hr/><p style='color:gray; font-size:12px;'>" +
                $"<em>Nota del sistema (Entorno de Evaluación): Este correo estaba originalmente dirigido a <strong>{para}</strong> " +
                $"pero ha sido interceptado y redirigido a esta bandeja por requerimiento de la prueba técnica.</em></p>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@intercoop.com.gt", "Sistema Interno Intercoop (Pruebas)"),
                Subject = $"[PRUEBA] {asunto}",
                Body = mensajeInterceptado,
                IsBodyHtml = true
            };

            mailMessage.To.Add(correoEvaluador);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch
            {
            }
        }
    }
}