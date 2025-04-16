using AutoMapper;
using BankApi.Dto;
using BankApi.Dto.Request;
using BankApi.Entities;

namespace BankingManagement.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Users, UserRequestDto>().ReverseMap();
            CreateMap<Users, UserResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleMaster.RoleName));
            CreateMap<Users, LoginDto>().ReverseMap();
            CreateMap<RoleMaster, RoleRequestDto>().ReverseMap();
            CreateMap<RoleMaster, RoleResponseDto>().ReverseMap();
            CreateMap<Users, BankManagerDto>().ReverseMap();
            CreateMap<Users, AdminResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleMaster.RoleName));
            CreateMap<Users, BankMangerUpdateDto>().ReverseMap();

            CreateMap<Users, UserWithAccountDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleMaster.RoleName));

            CreateMap<Account, AccountDto>().ReverseMap();

            // Updated Transaction Mapping
            CreateMap<Transactions, TransactionResponseDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.SenderAccount.Users.FirstName + " " + src.SenderAccount.Users.LastName))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiverAccount.Users.FirstName + " " + src.ReceiverAccount.Users.LastName))
                .ForMember(dest => dest.SenderAccount, opt => opt.MapFrom(src => src.SenderAccount.AccountNumber))
                .ForMember(dest => dest.ReceiverAccount, opt => opt.MapFrom(src => src.ReceiverAccount.AccountNumber))
                .ReverseMap();

            CreateMap<Transactions, TransferRequestDto>()
                .ForMember(dest => dest.ReceiverAccountNumber, opt => opt.MapFrom(src => src.ReceiverAccount.AccountNumber))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<TransactionRequestDto, Transactions>()
                .ForMember(dest => dest.SenderAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiverAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
