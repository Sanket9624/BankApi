using AutoMapper;
using BankApi.Dto;
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
            CreateMap<Users, BankManagerDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleMaster.RoleName));
            CreateMap<Users, BankManagerDto>().ReverseMap();





            // Admin mapping (ensuring AdminDto fields map correctly)
            CreateMap<Users, AdminDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNo))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ReverseMap();

            CreateMap<Users, UserWithAccountDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleMaster.RoleName));

            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.AccountId, opt => opt.Ignore()) // Prevent exposing AccountId
                .ReverseMap();

            CreateMap<Transactions, TransactionResponseDto>()
    .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
    .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.SenderAccount.Users.FirstName + " " + src.SenderAccount.Users.LastName))
    .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiverAccount.Users.FirstName + " " + src.ReceiverAccount.Users.LastName))
    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString())) // Convert Enum to String
    .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));


            CreateMap<Transactions, TransferRequestDto>()
           .ForMember(dest => dest.ReceiverAccountNumber, opt => opt.MapFrom(src => src.ReceiverAccount.AccountNumber)) // Map to AccountNumber
           .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));



            CreateMap<TransactionRequestDto, Transactions>()
                .ForMember(dest => dest.SenderAccountId, opt => opt.Ignore()) // Prevent exposing sensitive details
                .ForMember(dest => dest.ReceiverAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow)); // Auto-set transaction date
        }
    }
}
