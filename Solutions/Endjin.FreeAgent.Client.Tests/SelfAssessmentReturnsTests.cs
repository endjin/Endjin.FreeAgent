// <copyright file="SelfAssessmentReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class SelfAssessmentReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private SelfAssessmentReturns selfAssessmentReturns = null!;
    private HttpClient httpClient = null!;
    private TestHttpMessageHandler messageHandler = null!;

    [TestInitialize]
    public async Task Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.messageHandler = new TestHttpMessageHandler();
        this.httpClient = new HttpClient(this.messageHandler);
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(this.httpClient);
        this.loggerFactory = Substitute.For<ILoggerFactory>();

        this.freeAgentClient = new FreeAgentClient(
            new FreeAgentOptionsBuilder().Build(),
            this.cache,
            this.httpClientFactory,
            this.loggerFactory);

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.selfAssessmentReturns = new SelfAssessmentReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReturns()
    {
        // Arrange
        List<SelfAssessmentReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/self_assessment_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 6),
                PeriodEndsOn = new DateOnly(2024, 4, 5)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/self_assessment_tax_returns/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 6),
                PeriodEndsOn = new DateOnly(2025, 4, 5)
            }
        ];

        SelfAssessmentReturnsRoot responseRoot = new() { SelfAssessmentReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<SelfAssessmentReturn> result = await this.selfAssessmentReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/self_assessment_returns");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsReturn()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/self_assessment_tax_returns/123"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5)
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.GetByIdAsync("123");

        // Assert
        result.ShouldNotBeNull();
        result.PeriodStartsOn.ShouldBe(new DateOnly(2023, 4, 6));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/self_assessment_returns/123");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_MarksReturnAsFiled()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/self_assessment_tax_returns/456"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5)
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.MarkAsFiledAsync("456", new DateOnly(2024, 1, 31));

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/self_assessment_returns/456");
    }
}
