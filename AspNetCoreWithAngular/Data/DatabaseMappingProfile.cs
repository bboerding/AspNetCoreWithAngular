using AspNetCoreWithAngular.Data.Entities;
using AspNetCoreWithAngular.ViewModels;
using AutoMapper;

namespace AspNetCoreWithAngular.Data
{
    public class DatabaseMappingProfile : Profile
    {
        //Mit dem Mapper wird automatisch zwischen Database-Entity und ViewModel gemappt
        //Hier müssen die einzelnen Mappings initialisiert werden
        public DatabaseMappingProfile()
        {
            //Hier wird ein Mapping zwischen Order und OrderViewModel vorgenommen
            //Dabei muss die Order.Id nach OrderViewModel.OrderId explizit gemappt werden
            //  weil diese beiden Entities über unterschiedliche Id-Felder verfügen
            //Mit ReverseMap wird auch das umgekehrte Mappint unterstützt
            CreateMap<Order, OrderViewModel>()
                .ForMember(o => o.OrderId, ex => ex.MapFrom(o => o.Id))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemViewModel>()
                .ReverseMap();
        }
    }
}
