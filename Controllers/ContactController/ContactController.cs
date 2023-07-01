using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DillonColeman_PortfolioWebsite.Controllers.ContactController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {

        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            this._contactService = contactService;
        }

        // GET: api/<ContactController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> GetContacts()
        {
            return Ok(await _contactService.GetContacts());
        }

        // GET api/<ContactController>/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactById(int id)
        {
            return Ok(await _contactService.GetContactById(id));
        }

        // GET api/<ContactController>/{email}
        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactByEmail(string email)
        {
            return Ok(await _contactService.GetContactByEmail(email));
        }

        // GET api/<ContactController>/{name}
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactByName(string name)
        {
            return Ok(await _contactService.GetContactByName(name));
        }

        // POST api/<ContactController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> PostContact([FromBody] AddContactDto contact)
        {
            return Created("", await _contactService.AddContact(contact));
        }

        // PUT api/<ContactController>/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> PutContact(int id, [FromBody] UpdateContactDto contact)
        {
            return Ok(await _contactService.UpdateContact(id, contact));
        }

        // DELETE api/<ContactController>/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> DeleteContact(int id)
        {
            return Ok(await _contactService.DeleteContact(id));
        }
    }
}
