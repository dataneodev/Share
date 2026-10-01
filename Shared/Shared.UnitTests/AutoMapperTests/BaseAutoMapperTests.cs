using System.Reflection;
using AutoMapper;
using Vero.Shared;
using Vero.Shared.AutoMapper;

namespace Shared.UnitTests.AutoMapperTests
{
    public abstract class BaseAutoMapperTests
    {
        protected readonly MapperConfiguration _config;
        protected readonly IMapper _mapper;
        protected readonly EventProfile _profile;

        public BaseAutoMapperTests(EventProfile profile, bool mapNestedTypes)
        {
            _profile = profile;

            var assembliesContainer = new AssembliesContainer(
                Assembly.LoadFile(
                    Assembly.GetCallingAssembly()
                        .Location.Replace("UnitTests.dll", "Infrastructure.dll")
                )
            );

            _profile.AddValueObjectOfMap(assembliesContainer.Domain!, assembliesContainer.Shared!);
            _profile.AddEnumerationMap(assembliesContainer.Domain!, assembliesContainer.Shared!);
            _profile.AddDomainEventToIntegrationEventMap(assembliesContainer.Domain!, assembliesContainer.Contract!, false, mapNestedTypes);

            _profile.AddGlobalIgnore("OccuredAt");
            _profile.AddGlobalIgnore("InitiatorId");
            _profile.AddGlobalIgnore("CorrelationId");

            _config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = _config.CreateMapper();
        }
    }
}