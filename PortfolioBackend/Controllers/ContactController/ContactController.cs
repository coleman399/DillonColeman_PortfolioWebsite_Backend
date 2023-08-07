using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioBackend.Controllers.ContactController
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {

        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // GET api/Contact/getContacts
        // Admin should be able to see all contacts, user should only be able to see their own contacts
        [HttpGet("getContacts"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> GetContacts()
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.GetContacts();
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // GET api/Contact/getContactById?id={id}
        // Only admin should be able to search by id
        [HttpGet("getContactById"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> GetContactById([FromQuery] int id)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.GetContactById(id);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // GET api/Contact/geContactsByEmail?email={email}
        // Admin should be able to get all contacts, user should only be able to get their own contacts
        [HttpGet("getContactsByEmail"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> GetContactsByEmail([FromQuery] string email)
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.GetContactsByEmail(email);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // GET api/Contact/getContactsByName?name={name}
        // Only admin should be able to search by name
        [HttpGet("getContactsByName"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<List<GetContactDto>>>> GetContactsByName([FromQuery] string name)
        {
            ContactServiceResponse<List<GetContactDto>> result = await _contactService.GetContactsWithSimilarNameTo(name);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // POST api/Contact/addContact
        // Anyone can add a contact
        [HttpPost("addContact"), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> PostContact([FromBody] AddContactDto contact)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.AddContact(contact);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Created("addContact", result);
        }

        // PUT api/Contact/updateContact?id={id}
        // Admin should be able to update any contact, user should only be able to update their own contacts
        [HttpPut("updateContact"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<GetContactDto>>> UpdateContact([FromQuery] int id, [FromBody] UpdateContactDto contact)
        {
            ContactServiceResponse<GetContactDto> result = await _contactService.UpdateContact(id, contact);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }

        // DELETE api/Contact/deleteContact?id={id}
        // Admin should be able to delete any contact, user should only be able to delete their own contacts
        [HttpDelete("deleteContact"), Authorize(Roles = "SuperUser, Admin, User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContactServiceResponse<DeleteContactDto>>> DeleteContact([FromQuery] int id)
        {
            ContactServiceResponse<DeleteContactDto> result = await _contactService.DeleteContact(id);
            if (result.Success == false) return BadRequest(result);
            if (result.Data == null && result.Success == true) return Unauthorized();
            return Ok(result);
        }
    }
}
