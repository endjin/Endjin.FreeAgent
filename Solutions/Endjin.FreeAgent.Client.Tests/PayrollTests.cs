// <copyright file="PayrollTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PayrollTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Payroll payroll = null!;
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
        this.payroll = new Payroll(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetPayrollYearAsync_ReturnsPeriodsAndPayments()
    {
        // Arrange
        PayrollYearRoot responseRoot = new()
        {
            Periods =
            [
                new PayrollPeriod
                {
                    Url = new Uri("https://api.freeagent.com/v2/payroll/2026/0"),
                    Period = 0,
                    Frequency = "Monthly",
                    DatedOn = new DateOnly(2025, 4, 30),
                    Status = "filed",
                    EmploymentAllowanceClaimed = true,
                    EmploymentAllowanceAmount = 100.00m,
                    ConstructionIndustrySchemeDeduction = 0.00m
                },
                new PayrollPeriod
                {
                    Url = new Uri("https://api.freeagent.com/v2/payroll/2026/1"),
                    Period = 1,
                    Frequency = "Monthly",
                    DatedOn = new DateOnly(2025, 5, 31),
                    Status = "unfiled"
                }
            ],
            Payments =
            [
                new PayrollPayment
                {
                    DueOn = new DateOnly(2025, 5, 22),
                    AmountDue = 1500.00m,
                    Status = "unpaid"
                },
                new PayrollPayment
                {
                    DueOn = new DateOnly(2025, 6, 22),
                    AmountDue = 1600.00m,
                    Status = "unpaid"
                }
            ]
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollYearRoot result = await this.payroll.GetPayrollYearAsync(2026);

        // Assert
        result.ShouldNotBeNull();
        result.Periods.Count.ShouldBe(2);
        result.Payments.Count.ShouldBe(2);
        result.Periods[0].Period.ShouldBe(0);
        result.Periods[0].Status.ShouldBe("filed");
        result.Payments[0].AmountDue.ShouldBe(1500.00m);
        result.Payments[0].Status.ShouldBe("unpaid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll/2026");
    }

    [TestMethod]
    public async Task GetPayrollPeriodAsync_ReturnsPayslips()
    {
        // Arrange
        PayrollPeriodRoot responseRoot = new()
        {
            Period = new PayrollPeriod
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll/2026/0"),
                Period = 0,
                Frequency = "Monthly",
                DatedOn = new DateOnly(2025, 4, 30),
                Status = "filed",
                EmploymentAllowanceClaimed = true,
                EmploymentAllowanceAmount = 100.00m,
                Payslips =
                [
                    new Payslip
                    {
                        User = new Uri("https://api.freeagent.com/v2/users/123"),
                        TaxCode = "1257L",
                        DatedOn = new DateOnly(2025, 4, 30),
                        BasicPay = 3000.00m,
                        TaxDeducted = 450.00m,
                        EmployeeNi = 250.00m,
                        EmployerNi = 300.00m,
                        EmployeePension = 150.00m,
                        EmployerPension = 90.00m,
                        NiLetter = "A",
                        NiCalcType = "Employee",
                        Frequency = "Monthly"
                    },
                    new Payslip
                    {
                        User = new Uri("https://api.freeagent.com/v2/users/124"),
                        TaxCode = "1257L",
                        DatedOn = new DateOnly(2025, 4, 30),
                        BasicPay = 3500.00m,
                        TaxDeducted = 550.00m,
                        EmployeeNi = 280.00m,
                        EmployerNi = 350.00m
                    }
                ]
            }
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollPeriod result = await this.payroll.GetPayrollPeriodAsync(2026, 0);

        // Assert
        result.ShouldNotBeNull();
        result.Period.ShouldBe(0);
        result.Payslips.ShouldNotBeNull();
        result.Payslips!.Count.ShouldBe(2);
        result.Payslips[0].BasicPay.ShouldBe(3000.00m);
        result.Payslips[0].TaxCode.ShouldBe("1257L");
        result.Payslips[0].NiLetter.ShouldBe("A");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll/2026/0");
    }

    [TestMethod]
    public async Task GetPayrollPeriodAsync_WithInvalidPeriod_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
            await this.payroll.GetPayrollPeriodAsync(2026, 12));
    }

    [TestMethod]
    public async Task GetPayrollPeriodAsync_WithNegativePeriod_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(async () =>
            await this.payroll.GetPayrollPeriodAsync(2026, -1));
    }

    [TestMethod]
    public async Task MarkPaymentAsPaidAsync_UpdatesPaymentStatus()
    {
        // Arrange
        PayrollYearRoot responseRoot = new()
        {
            Periods =
            [
                new PayrollPeriod
                {
                    Url = new Uri("https://api.freeagent.com/v2/payroll/2026/0"),
                    Period = 0,
                    Status = "filed"
                }
            ],
            Payments =
            [
                new PayrollPayment
                {
                    DueOn = new DateOnly(2025, 5, 22),
                    AmountDue = 1500.00m,
                    Status = "marked_as_paid"
                }
            ]
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollYearRoot result = await this.payroll.MarkPaymentAsPaidAsync(2026, "2025-05-22");

        // Assert
        result.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("marked_as_paid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll/2026/payments/2025-05-22/mark_as_paid");
    }

    [TestMethod]
    public async Task MarkPaymentAsUnpaidAsync_UpdatesPaymentStatus()
    {
        // Arrange
        PayrollYearRoot responseRoot = new()
        {
            Periods =
            [
                new PayrollPeriod
                {
                    Url = new Uri("https://api.freeagent.com/v2/payroll/2026/0"),
                    Period = 0,
                    Status = "filed"
                }
            ],
            Payments =
            [
                new PayrollPayment
                {
                    DueOn = new DateOnly(2025, 5, 22),
                    AmountDue = 1500.00m,
                    Status = "unpaid"
                }
            ]
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollYearRoot result = await this.payroll.MarkPaymentAsUnpaidAsync(2026, "2025-05-22");

        // Assert
        result.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("unpaid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll/2026/payments/2025-05-22/mark_as_unpaid");
    }

    [TestMethod]
    public async Task GetPayrollYearAsync_CachesResult()
    {
        // Arrange
        PayrollYearRoot responseRoot = new()
        {
            Periods = [new PayrollPeriod { Period = 0, Status = "filed" }],
            Payments = [new PayrollPayment { AmountDue = 1500.00m }]
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        PayrollYearRoot result1 = await this.payroll.GetPayrollYearAsync(2026);
        PayrollYearRoot result2 = await this.payroll.GetPayrollYearAsync(2026);

        // Assert - Should only make one API call due to caching
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task MarkPaymentAsPaidAsync_InvalidatesCache()
    {
        // Arrange - First get the year data
        PayrollYearRoot yearRoot = new()
        {
            Periods = [new PayrollPeriod { Period = 0 }],
            Payments = [new PayrollPayment { AmountDue = 1500.00m, Status = "unpaid" }]
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(yearRoot, SharedJsonOptions.Instance),
                Encoding.UTF8,
                "application/json")
        };

        await this.payroll.GetPayrollYearAsync(2026);

        // Arrange - Now mark as paid
        PayrollYearRoot paidRoot = new()
        {
            Periods = [new PayrollPeriod { Period = 0 }],
            Payments = [new PayrollPayment { AmountDue = 1500.00m, Status = "marked_as_paid" }]
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(paidRoot, SharedJsonOptions.Instance),
                Encoding.UTF8,
                "application/json")
        };

        // Act
        await this.payroll.MarkPaymentAsPaidAsync(2026, "2025-05-22");

        // Arrange - Get year data again (should not be cached)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(paidRoot, SharedJsonOptions.Instance),
                Encoding.UTF8,
                "application/json")
        };

        await this.payroll.GetPayrollYearAsync(2026);

        // Assert - Should have made 3 calls (initial get, mark as paid, second get)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task PayslipDeserialization_WithAllFields_DeserializesCorrectly()
    {
        // Arrange - Test deserialization of all payslip fields
        PayrollPeriodRoot responseRoot = new()
        {
            Period = new PayrollPeriod
            {
                Period = 0,
                Payslips =
                [
                    new Payslip
                    {
                        User = new Uri("https://api.freeagent.com/v2/users/123"),
                        TaxCode = "1257L",
                        DatedOn = new DateOnly(2025, 4, 30),
                        BasicPay = 3000.00m,
                        TaxDeducted = 450.00m,
                        EmployeeNi = 250.00m,
                        EmployerNi = 300.00m,
                        OtherDeductions = 50.00m,
                        StudentLoanDeduction = 100.00m,
                        PostgradLoanDeduction = 25.00m,
                        Overtime = 200.00m,
                        Commission = 150.00m,
                        Bonus = 500.00m,
                        Allowance = 100.00m,
                        StatutorySickPay = 0.00m,
                        StatutoryMaternityPay = 0.00m,
                        StatutoryPaternityPay = 0.00m,
                        StatutoryAdoptionPay = 0.00m,
                        StatutoryParentalBereavementPay = 0.00m,
                        StatutoryNeonatalCarePay = 0.00m,
                        AbsencePayments = 0.00m,
                        OtherPayments = 75.00m,
                        EmployeePension = 150.00m,
                        EmployerPension = 90.00m,
                        Attachments = 0.00m,
                        PayrollGiving = 10.00m,
                        NiCalcType = "Employee",
                        Frequency = "Monthly",
                        NiLetter = "A",
                        DeductStudentLoan = true,
                        StudentLoanDeductionsPlan = "Plan 2",
                        DeductPostgradLoan = true,
                        Week1Month1Basis = false,
                        DeductionFreePay = 1047.50m,
                        HoursWorked = 160.00m
                    }
                ]
            }
        };

        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollPeriod result = await this.payroll.GetPayrollPeriodAsync(2026, 0);

        // Assert
        Payslip payslip = result.Payslips![0];
        payslip.TaxCode.ShouldBe("1257L");
        payslip.BasicPay.ShouldBe(3000.00m);
        payslip.TaxDeducted.ShouldBe(450.00m);
        payslip.EmployeeNi.ShouldBe(250.00m);
        payslip.EmployerNi.ShouldBe(300.00m);
        payslip.StudentLoanDeduction.ShouldBe(100.00m);
        payslip.PostgradLoanDeduction.ShouldBe(25.00m);
        payslip.Overtime.ShouldBe(200.00m);
        payslip.Commission.ShouldBe(150.00m);
        payslip.Bonus.ShouldBe(500.00m);
        payslip.Allowance.ShouldBe(100.00m);
        payslip.EmployeePension.ShouldBe(150.00m);
        payslip.EmployerPension.ShouldBe(90.00m);
        payslip.PayrollGiving.ShouldBe(10.00m);
        payslip.NiCalcType.ShouldBe("Employee");
        payslip.NiLetter.ShouldBe("A");
        payslip.DeductStudentLoan.ShouldBe(true);
        payslip.StudentLoanDeductionsPlan.ShouldBe("Plan 2");
        payslip.DeductPostgradLoan.ShouldBe(true);
        payslip.Week1Month1Basis.ShouldBe(false);
        payslip.DeductionFreePay.ShouldBe(1047.50m);
        payslip.HoursWorked.ShouldBe(160.00m);
    }
}
