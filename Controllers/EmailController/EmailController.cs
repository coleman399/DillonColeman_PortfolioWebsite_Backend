using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioWebsite_Backend.Controllers.EmailController
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // POST api/<EmailController>

        [HttpPost("sendEmail"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmailServiceResponse<Email>>> SendEmail(Email email)
        {
            EmailServiceResponse<Email> result = await _emailService.SendEmail(email);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }
    }
}
