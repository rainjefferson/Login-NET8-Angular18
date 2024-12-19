using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LoginApp.Models;
using LoginApp.DB;
using LoginApp.Dto;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly ParamsConfig _params;

        public AuthController(
            ApplicationDbContext context, 
            ILogger<AuthController> logger,
            IOptions<ParamsConfig> paramsConfig)
        {
            _context = context;
            _logger = logger;
            _params = paramsConfig.Value; 
        }

        [HttpPost("send-confirmation-email")]
        public async Task SendConfirmationEmail(User user, string token)
        {
            if (_params == null || string.IsNullOrEmpty(_params.BaseUrlApp))
            {
                _logger.LogWarning("BaseUrlApp não foi configurada no arquivo appsettings.json.");
                throw new InvalidOperationException("BaseUrlApp não foi configurada no arquivo appsettings.json.");
            }

            var baseUrl = _params.BaseUrlApp;
            var confirmationLink = $"{baseUrl}confirm-email?userId={user.Id}&token={token}";
            var subject = "Sua conta está quase pronta.";
            var body = GerarHtmlConfirmacaoEmail(confirmationLink);
            string emailRemetente = "cadastro@jeffer.com.br";
            string nomeRemetente = "Confirme seu e-mail";

            await EnviarEmailAsync(emailRemetente, nomeRemetente, user.Email, subject, body);
        }
                

        /// <summary>
        /// Logs a user in.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>A token if login is successful</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { message = "Dados inválidos.", errors });
                }

                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

                if (user == null)
                {
                    _logger.LogWarning("Tentativa de login falhou para o e-mail: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Não foi encontrado registro com o Email e Senha informados." });
                }                    

                // Gerar token JWT ou outras ações necessárias
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("O e-mail: {Email} ainda não foi confirmado.", loginDto.Email);
                    await SendConfirmationEmail(user, "token123");
                    return Unauthorized(new { message = "Email não confirmado! Verifique sua caixa de emails ou lixo elerônico." });
                }

                user.LastPasswordChangeDate = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Login bem-sucedido para o e-mail: {Email}", loginDto.Email);
                return Ok(new { token = "dummy-jwt-token" }); // Substitua por seu token real
               }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro durante o login para o e-mail: {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns>A success message if registration is successful</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { message = "Dados inválidos.", errors });
            }

            if (registerDto.Password != registerDto.ConfirmPassword)
                return BadRequest("As senhas não coincidem.");

            var userExists = await _context.Users
                .AnyAsync(u => u.Email == registerDto.Email);

            if (userExists)
                return BadRequest("Usuário já existe. Este Email já foi utilizado.");

            var user = new User
            {
                Email = registerDto.Email,
                Password = registerDto.Password, 
                Name = registerDto.Name,
                Address = registerDto.Address,
                City = registerDto.City,
                State = registerDto.State,
                Phone = registerDto.Phone,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SendConfirmationEmail(user, "token123");

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        /// <summary>
        /// Sends a password reset link to the user's email.
        /// </summary>
        /// <param name="forgotPasswordDto"></param>
        /// <returns>A success message if email is sent</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { message = "Dados inválidos.", errors });
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);

            if (user == null)
                return BadRequest(new { message = "Nenhum registro encontrado com esse Email!" });

            // Lógica para enviar e-mail de recuperação de senha
            var subject = "Sua senha está aqui.";
            var body = GerarHtmlForgotPassword(user.Password);
            string emailRemetente = "cadastro@seudominio.com.br";
            string nomeRemetente = "Lembrete de senha solicitada";

            await EnviarEmailAsync(emailRemetente, nomeRemetente, user.Email, subject, body);

            return Ok(new { message = "Um link de redefinição de senha foi enviado." });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userId"></param>
        /// /// <param name="token"></param>
        /// <returns>A success message if registration is successful</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Verificar o token e atualizar o estado do usuário
            //if (VerifyToken(token, user))
            //{
                user.EmailConfirmed = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "E-mail confirmado com sucesso!" });
            //}

            //return BadRequest("Token inválido.");
        }

        private static string GerarHtmlConfirmacaoEmail(string linkClick)
        {              
           return HtmlTemplateConfirmEmail.HtmlTemplate.Replace("#LINK_CLICK", linkClick);
        }

        private static string GerarHtmlForgotPassword(string senhaCadastrada)
        {
            return HtmlTemplateForgotPassword.HtmlTemplate.Replace("#SENHA_CADASTRADA", senhaCadastrada);
        }

        private async Task EnviarEmailAsync(string remetente, string nomeRemetente, string destinatario, string assunto, string corpo)
        {
            var message = new MailMessage
            {
                From = new MailAddress(remetente, nomeRemetente),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(destinatario));

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("seu_email_aqui@gmail.com", "a_senha_do_email_aqui");
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                try
                {
                    await client.SendMailAsync(message);                   
                    _logger.LogInformation("Email enviado com sucesso para {Destinatario}.", destinatario);
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogWarning(smtpEx, "Erro SMTP ao enviar email para {Destinatario}. StatusCode: {StatusCode}.",
                                   destinatario, smtpEx.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro geral ao enviar e-mail para {Destinatario}.", destinatario);
                }
            }
        }              

    }
}

