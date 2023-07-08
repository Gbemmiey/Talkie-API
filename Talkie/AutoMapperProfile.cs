using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Talkie.DTOs.Account;
using Talkie.Models;

namespace Talkie
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, GetAccountDto>();
            CreateMap<Account, GetProfileDto>();

            CreateMap<AddAccountDto, Account>();
            CreateMap<UpdateAccountDto, Account>();
            CreateMap<LoginAccountDto, Account>();

            CreateMap<Account, AccountBalanceDto>();
            // CreateMap<AddMessageDto, Message>();

            //            CreateMap<Message, GetMessageDto>();
            //          CreateMap<Text, GetMessageDto>();

            //        CreateMap<AddTransactionDto, Transaction>();

            CreateMap<string, Text>();
            CreateMap<string, Transaction>();

            //      CreateMap<GetReceivedMessageDto, GetMessageDto>();
            //    CreateMap<GetSentMessageDto, GetMessageDto>();

            //            CreateMap<Message, GetSentMessageDto>();
            //          CreateMap<GetSentMessageDto, GetMessageDto>();

            //        CreateMap<Message, GetReceivedMessageDto>();

            //        CreateMap<Contact, GetContactDto>();
            //      CreateMap<AddContactDto, Contact>();

            //    CreateMap<Message, GetMessageDto>();
            //  CreateMap<Airtime, GetAirtimePurchasesDto>();

            //            CreateMap<Message, GetStatementDto>();
        }
    }
}