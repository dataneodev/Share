using FluentAssertions;
using Newtonsoft.Json;
using ProfiBiznes.Shared.Application.Bus.Events;
using ProfiBiznes.Shared.Application.Security.Initiator;

namespace ProfiBiznes.Shared.UnitTests.Infrastructure.Events
{
    public sealed record FakeIntegrationEvent(
        Initiator.InitiatorJson InitiatorJson,
        Guid CorrelationId,
        DateTimeOffset OccurredAt,
        int IntNotNull,
        int? IntNull,
        string StringNotNull,
        string? StringNull,
        DateTimeOffset DateTimeOffsetNotNull,
        DateTimeOffset? DateTimeOffsetNull
    ) : IntegrationEvent(InitiatorJson, CorrelationId, OccurredAt);

    public sealed class IntegrationEventHandlersTests
    {
        [Fact]
        public void Serialization_DeserializationAfterSerialization_ShouldReturnIdenticalData()
        {
            // Arrange
            var initiator = InitiatorBuilder.User.ToJson();
            var correlationId = Guid.NewGuid();
            var occurredAt = DateTimeOffset.UtcNow;
            var intNotNull = 1;
            int? intNull = null;
            var stringNotNull = "string";
            string? stringNull = null;
            var dateTimeOffsetNotNull = DateTimeOffset.UtcNow;
            DateTimeOffset? dateTimeOffsetNull = null;


            // Act
            var integrationEvent = new FakeIntegrationEvent(
                initiator,
                correlationId,
                occurredAt,
                intNotNull,
                intNull,
                stringNotNull,
                stringNull,
                dateTimeOffsetNotNull,
                dateTimeOffsetNull
            );

            var serialized = JsonConvert.SerializeObject(integrationEvent);
            var deserialized = JsonConvert.DeserializeObject<FakeIntegrationEvent>(serialized);

            //Assert

            deserialized.Should()
                .NotBeNull();

            deserialized?.Id
                .Should()
                .Be(integrationEvent.Id);

            deserialized?.Initiator
                .AccountId
                .Value
                .Should()
                .Be(initiator.AccountId);

            deserialized?.CorrelationId
                .Should()
                .Be(correlationId);

            deserialized?.OccurredAt
                .Should()
                .Be(occurredAt);

            deserialized?.IntNotNull
                .Should()
                .Be(intNotNull);

            deserialized?.IntNull
                .Should()
                .Be(intNull);

            deserialized?.StringNotNull
                .Should()
                .Be(stringNotNull);

            deserialized?.StringNull
                .Should()
                .Be(stringNull);

            deserialized?.DateTimeOffsetNotNull
                .Should()
                .Be(dateTimeOffsetNotNull);

            deserialized?.DateTimeOffsetNull
                .Should()
                .Be(dateTimeOffsetNull);
        }
    }
}