using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Message;
using Talkie.Models;
using Talkie.Services.GenericServices;

namespace Talkie.Services.TextService
{
    public class TextService : ITextService

    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IGenericService _genericService;

        public TextService(IMapper mapper, DataContext context, IGenericService genericService)
        {
            _mapper = mapper;
            _context = context;
            _genericService = genericService;
        }

        public async Task<ServiceResponse<GetMessageDto>> SaveText(AddMessageDto message)
        {
            DateTime wcaTime = _genericService.getLocalTime();

            var jsonString = JsonConvert.SerializeObject(message.Payload);

            Text newText = _mapper.Map<Text>(jsonString);

            Message newMess = new()
            {
                Modified = wcaTime,
                RecipientNumber = message.RecipientNumber,
                Type = message.Type,
                Number = _genericService.GetUserID(),
                DeliveryStatus = DeliveryStatus.Sent
            };

            Text nT = new Text
            {
                Content = message.Payload
            };

            nT.Message = newMess;

            _context.Texts.Add(nT);

            System.Diagnostics.Debug.WriteLine(newMess.Number);
            Console.WriteLine(newMess.Number);

            await _context.SaveChangesAsync();

            Message? saveMessage = await _context.Messages
                                        .Include(c => c.Texts)
                                        .FirstOrDefaultAsync(c => c.Id == nT.MessageId);

            Text? savedText = await _context.Texts
                                            .FirstOrDefaultAsync(c => c.MessageId == nT.MessageId);

            return getSavedMessage(saveMessage, savedText);
        }

        private ServiceResponse<GetMessageDto> getSavedMessage(Message? saveMessage, Text? savedText)
        {
            var serviceResponse = new ServiceResponse<GetMessageDto>();
            GetMessageDto responseData = _mapper.Map<GetMessageDto>(saveMessage);

            responseData.Payload = savedText.Content;
            responseData.Interaction = "Sent";

            serviceResponse.Data = _mapper.Map<GetMessageDto>(responseData);
            return serviceResponse;
        }
    }
}