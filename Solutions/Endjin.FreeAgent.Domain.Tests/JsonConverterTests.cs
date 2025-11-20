using Endjin.FreeAgent.Converters;
using Endjin.FreeAgent.Domain;
using Shouldly;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class JsonConverterTests
{
    private readonly JsonSerializerOptions options;

    public JsonConverterTests()
    {
        this.options = new JsonSerializerOptions
        {
            Converters = 
            { 
                new EcStatusJsonConverter(),
                new RecurringPatternJsonConverter()
            }
        };
    }

    [TestMethod]
    public void EcStatusConverter_HandlesEcVatMoss()
    {
        // Arrange
        string json = """{"status": "EC VAT MOSS"}""";

        // Act
        TestEcStatus? result = JsonSerializer.Deserialize<TestEcStatus>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(EcStatus.EcVatMoss);

        // Round trip
        string serialized = JsonSerializer.Serialize(result, options);
        serialized.ShouldContain("\"status\":\"EC VAT MOSS\"");
    }

    [TestMethod]
    public void RecurringPatternConverter_HandlesMonthly()
    {
        // Arrange
        string json = """{"pattern": "Monthly"}""";

        // Act
        TestRecurringPattern? result = JsonSerializer.Deserialize<TestRecurringPattern>(json, options);

        // Assert
        result.ShouldNotBeNull();
        result.Pattern.ShouldBe(RecurringPattern.Monthly);

        // Round trip
        string serialized = JsonSerializer.Serialize(result, options);
        serialized.ShouldContain("\"pattern\":\"Monthly\"");
    }

    private class TestEcStatus
    {
        [JsonPropertyName("status")]
        [JsonConverter(typeof(EcStatusJsonConverter))]
        public EcStatus? Status { get; init; }
    }

    private class TestRecurringPattern
    {
        [JsonPropertyName("pattern")]
        [JsonConverter(typeof(RecurringPatternJsonConverter))]
        public RecurringPattern? Pattern { get; init; }
    }
}
