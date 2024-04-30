using APDSys.Model.DTO.Message;
using APDSys.Service.IService;
using APDSys.Utility.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APDSys.API.Controllers
{
    [Route("apdsys/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Create a new message to send to another user
        /// </summary>
        /// <param name="createMessageDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateNewMessage([FromBody] CreateMessageDTO createMessageDTO)
        {
            var result = await _messageService.CreateMessageAysnc(User, createMessageDTO);

            if (result.IsSucceed)
            {
                return Ok(result.Message);
            }

            return StatusCode(result.StatusCode, result.Message);
        }

        /// <summary>
        /// Get all messsages for current user, either as sender or as receiver
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetMessageDTO>>> GetMyMessages()
        {
            var messages = await _messageService.GetMyMessagesAsync(User);

            return Ok(messages);
        }

        /// <summary>
        /// Get all messsages with owner access and admin access
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<ActionResult<IEnumerable<GetMessageDTO>>> GetMessages()
        {
            var messages = await _messageService.GetMessagesAsync();

            return Ok(messages);
        }
    }
}