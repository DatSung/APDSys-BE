using ADPSys.DataAccess.IRepository;
using APDSys.Model.Domain;
using APDSys.Model.DTO.General;
using APDSys.Model.DTO.Message;
using APDSys.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageService(IUnitOfWork unitOfWork, ILogService logService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<GeneralServiceResponseDTO> CreateMessageAysnc(ClaimsPrincipal user, CreateMessageDTO createMessageDTO)
        {
            if (user.Identity.Name == createMessageDTO.ReceiverUserName)
            {
                return new GeneralServiceResponseDTO()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Sender and Receiver can not be same"
                };
            }

            var isReceiverUserNameValid = _userManager.Users.Any(x => x.UserName == createMessageDTO.ReceiverUserName);

            if (!isReceiverUserNameValid)
            {
                return new GeneralServiceResponseDTO()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = "Receiver UserName is not valid"
                };
            }

            Message message = new Message()
            {
                SenderUserName = user.Identity.Name,
                ReceiverUserName = createMessageDTO.ReceiverUserName,
                Text = createMessageDTO.Text
            };

            await _unitOfWork.MessageRepository.AddAsync(message);
            await _logService.SaveNewLog(user.Identity.Name, "Send Message");
            await _unitOfWork.SaveAsync();

            return new GeneralServiceResponseDTO()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "Message saved successfully"
            };
        }

        public async Task<IEnumerable<GetMessageDTO>> GetMessagesAsync()
        {
            var messages = await _unitOfWork.MessageRepository.GetAllAsync();

            var messagesDTO = _mapper.Map<List<GetMessageDTO>>(messages).OrderByDescending(x => x.CreatedAt);

            return messagesDTO;
        }

        public async Task<IEnumerable<GetMessageDTO>> GetMyMessagesAsync(ClaimsPrincipal user)
        {
            var loggedInUser = user.Identity.Name;

            var messages = await _unitOfWork.MessageRepository
                .GetAllAsync(x => x.SenderUserName == loggedInUser || x.ReceiverUserName == loggedInUser);

            var messagesDTO = _mapper.Map<List<GetMessageDTO>>(messages).OrderByDescending(x => x.CreatedAt);

            return messagesDTO;
        }
    }
}
