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
        [HttpGet("getContacts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> GetContacts()
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.GetContacts();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // GET api/<ContactController>/{id}
        [HttpGet("getById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactById(int id)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.GetContactById(id);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // GET api/<ContactController>/{email}
        [HttpGet("getbyEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactByEmail(string email)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.GetContactByEmail(email);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // GET api/<ContactController>/{name}
        [HttpGet("getbyName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactByName(string name)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.GetContactByName(name);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // POST api/<ContactController>
        [HttpPost("addContact")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> PostContact([FromBody] AddContactDto contact)
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.AddContact(contact);
            if (result.Success == false) return BadRequest(result);
            return Created("", result);
        }

        // PUT api/<ContactController>/{id}
        [HttpPut("updateContact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> PutContact(int id, [FromBody] UpdateContactDto contact)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.UpdateContact(id, contact);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        // DELETE api/<ContactController>/{id}
        [HttpDelete("deleteContact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> DeleteContact(int id)
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.DeleteContact(id);
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }
    }
}
