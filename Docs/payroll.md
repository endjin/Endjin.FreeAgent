# Payroll

*Minimum access level*: `Tax and Limited Accounting`. Only available for UK companies (i.e. those which support payroll in FreeAgent).

Provides read-only access to RTI payroll data generated within FreeAgent, including details on payments due.

## Attributes

| Attribute                              | Description                                                                                                                 | Kind    |
| -------------------------------------- | --------------------------------------------------------------------------------------------------------------------------- | ------- |
| url                                    | The unique identifier for the period                                                                                        | URI     |
| period                                 | The pay period of the year (0-11)                                                                                           | Integer |
| frequency                              | The pay frequency (Monthly/Weekly)                                                                                          | String  |
| dated_on                               | The date of pay for this period, in `YYYY-MM-DD` format                                                                     | Date    |
| status                                 | The filing status of this payroll period. One of: unfiled, pending, rejected, partially_filed, filed                        | String  |
| employment_allowance_claimed           | Was Employment Allowance claimed in this pay period                                                                         | Boolean |
| employment_allowance_amount            | The amount of Employment Allowance claimed in this pay period                                                               | Decimal |
| construction_industry_scheme_deduction | The amount of liability already witheld under the CIS scheme this period                                                    | Decimal |
| due_on                                 | The date on which a particular payment is due                                                                               | Date    |
| amount_due                             | The amount due on that date                                                                                                 | Decimal |
| status                                 | One of `unpaid` or `marked_as_paid`. `paid` is reserved for future use. Key is omitted if there are no payments to be made. | Decimal |
| user                                   | The unique identifier for the user being paid                                                                               | URL     |
| tax_code                               | The HMRC tax code for the payslip                                                                                           | String  |
| dated_on                               | The date of the payslip in `YYYY-MM-DD` format                                                                              | Date    |
| basic_pay                              | Basic pay this period                                                                                                       | Decimal |
| tax_deducted                           | Tax deducted this period                                                                                                    | Decimal |
| employee_ni                            | Employee NI payable this period                                                                                             | Decimal |
| employer_ni                            | Employer NI payable this period                                                                                             | Decimal |
| other_deductions                       | Other deductions this period                                                                                                | Decimal |
| student_loan_deduction                 | Student loan deductions this period                                                                                         | Decimal |
| postgrad_loan_deduction                | Postgraduate loan deductions this period                                                                                    | Decimal |
| overtime                               | Overtime paid this period                                                                                                   | Decimal |
| commission                             | Commission paid this period                                                                                                 | Decimal |
| bonus                                  | Bonus paid this period                                                                                                      | Decimal |
| allowance                              | Allowance paid this period                                                                                                  | Decimal |
| statutory_sick_pay                     | SSP this period                                                                                                             | Decimal |
| statutory_maternity_pay                | SMP this period                                                                                                             | Decimal |
| statutory_paternity_pay                | SPP this period                                                                                                             | Decimal |
| statutory_adoption_pay                 | SAP this period                                                                                                             | Decimal |
| statutory_parental_bereavement_pay     | SPBP this period                                                                                                            | Decimal |
| statutory_neonatal_care_pay            | SNCP this period                                                                                                            | Decimal |
| absence_payments                       | Absence payments this period                                                                                                | Decimal |
| other_payments                         | Other payments this period                                                                                                  | Decimal |
| employee_pension                       | Employee pension deductions this period                                                                                     | Decimal |
| employer_pension                       | Employer pension deductions this period                                                                                     | Decimal |
| attachments                            | Attachment deductions this period                                                                                           | Decimal |
| payroll_giving                         | Payroll giving deductions this period                                                                                       | Decimal |
| ni_calc_type                           | Calculation method for NI (Director/Employee)                                                                               | String  |
| frequency                              | Pay frequency (Monthly/Weekly)                                                                                              | String  |
| additional_statutory_paternity_pay     | ASPP this period                                                                                                            | Decimal |
| deductions_subject_to_nic_but_not_paye | Payroll deductions that are subject to NI but not income tax this period                                                    | Decimal |
| other_deductions_from_net_pay          | Other deductions this period                                                                                                | Decimal |
| employee_pension_not_under_net_pay     | Employee pension deductions not under net pay agreement this period                                                         | Decimal |
| other_salary_sacrifice_deductions      | Other salary sacrifice deductions, excluding pension contributions                                                          | Decimal |
| employee_pension_salary_sacrifice      | Employee pension contributions under the salary sacrifice scheme                                                            | Decimal |
| ni_letter                              | HMRC NI classification for this employee                                                                                    | String  |
| deduct_student_loan                    | Are student loan payments being deducted?                                                                                   | Boolean |
| student_loan_deductions_plan           | The student loan plan type (Plan 1/Plan 2)                                                                                  | String  |
| deduct_postgrad_loan                   | Are postgraduate loan payments being deducted?                                                                              | Boolean |
| week_1_month_1_basis                   | Deductions calculated on a W1M1 basis                                                                                       | Boolean |
| deduction_free_pay                     | Pay this period not subject to NI or income tax                                                                             | Decimal |
| hours_worked                           | The number of hours worked this period                                                                                      | Decimal |

## List all periods for a given tax year

Returns the list of all payroll periods for the given year end. For example, for the payroll year April 2025 - March 2026, the `:year` parameter should be 2026.

```http
GET https://api.freeagent.com/v2/payroll/:year
```

### Response

```http
Status: 200 OK
```

```json
{
    "periods": [
        {
            "url": "https://api.freeagent.com/v2/payroll/2026/0",
            "period": 0,
            "frequency": "Monthly",
            "dated_on": "2020-04-25",
            "status": "filed",
            "employment_allowance_claimed": true,
            "employment_allowance_amount": "0.0",
            "construction_industry_scheme_deduction": "0.0",
            "created_at": "2025-04-17T11:31:58.000Z",
            "updated_at": "2025-04-17T12:42:54.000Z"
        }
    ],
    "payments": [
        {
            "due_on": "2025-05-22",
            "amount_due": "738.19",
            "status": "unpaid"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <periods type="array">
        <period>
            <url>https://api.freeagent.com/v2/payroll/2026/0</url>
            <period type="integer">0</period>
            <frequency>Monthly</frequency>
            <dated-on type="date">2017-02-25</dated-on>
            <status>filed</status>
            <employment-allowance-claimed type="boolean">true</employment-allowance-claimed>
            <employment-allowance-amount type="decimal">0.0</employment-allowance-amount>
            <constriction-industry-scheme-deduction type="decimal">0.0</constriction-industry-scheme-deduction>
            <created-at type="dateTime">2017-02-17T11:31:58Z</created-at>
            <updated-at type="dateTime">2017-02-17T12:42:54Z</updated-at>
        </period>
    </periods>
    <payments type="array">
        <payment>
            <due-on type="date">2017-03-22</due-on>
            <amount-due type="decimal">1454.95</amount-due>
            <status>unpaid</status>
        </payment>
    </payments>
</freeagent>
```
Show as JSON

## List all payslips for a given period

Returns the list of all payslips for the given year and period.

```http
GET https://api.freeagent.com/v2/payroll/:year/:period
```

### Response

```http
Status: 200 OK
```

```json
{
    "period": {
        "url": "https://api.freeagent.com/v2/payroll/2026/0",
        "period": 0,
        "frequency": "Monthly",
        "dated_on": "2025-04-25",
        "status": "filed",
        "employment_allowance_claimed": true,
        "employment_allowance_amount": "0.0",
        "construction_industry_scheme_deduction": "0.0",
        "created_at": "2025-04-17T11:31:58.000Z",
        "updated_at": "2025-04-17T12:42:54.000Z",
        "payslips": [
            {
                "user": "https://api.freeagent.com/v2/users/86",
                "tax_code": "1100L",
                "dated_on": "2025-04-25",
                "basic_pay": "2500.0",
                "tax_deducted": "0.0",
                "employee_ni": "0.0",
                "employer_ni": "0.0",
                "other_deductions": "0.0",
                "student_loan_deduction": "0.0",
                "postgrad_loan_deduction": "0.0",
                "overtime": "0.0",
                "commission": "0.0",
                "bonus": "0.0",
                "allowance": "0.0",
                "statutory_sick_pay": "0.0",
                "statutory_maternity_pay": "0.0",
                "statutory_paternity_pay": "0.0",
                "statutory_adoption_pay": "0.0",
                "statutory_parental_bereavement_pay": "0.0",
                "statutory_neonatal_care_pay": "0.0",
                "absence_payments": "0.0",
                "other_payments": "0.0",
                "employee_pension": "0.0",
                "employer_pension": "0.0",
                "attachments": "0.0",
                "payroll_giving": "0.0",
                "ni_calc_type": "Director",
                "frequency": "Monthly",
                "additional_statutory_paternity_pay": "0.0",
                "deductions_subject_to_nic_but_not_paye": "0.0",
                "other_deductions_from_net_pay": "0.0",
                "employee_pension_not_under_net_pay": "0.0",
                "other_salary_sacrifice_deductions": "0.0",
                "employee_pension_salary_sacrifice": "0.0",
                "ni_letter": "A",
                "deduct_student_loan": false,
                "deduct_postgrad_loan": false,
                "week_1_month_1_basis": false,
                "deduction_free_pay": "0.0",
                "created_at": "2025-04-17T11:31:59.000Z",
                "updated_at": "2025-04-17T11:31:59.000Z"
            },
            {
                "user": "https://api.freeagent.com/v2/users/71",
                "tax_code": "BR",
                "dated_on": "2026-04-25",
                "basic_pay": "1100.0",
                "tax_deducted": "220.0",
                "employee_ni": "0.0",
                "employer_ni": "0.0",
                "other_deductions": "0.0",
                "student_loan_deduction": "0.0",
                "postgrad_loan_deduction": "0.0",
                "overtime": "0.0",
                "commission": "0.0",
                "bonus": "0.0",
                "allowance": "0.0",
                "statutory_sick_pay": "0.0",
                "statutory_maternity_pay": "0.0",
                "statutory_paternity_pay": "0.0",
                "statutory_adoption_pay": "0.0",
                "statutory_parental_bereavement_pay": "0.0",
                "statutory_neonatal_care_pay": "0.0",
                "absence_payments": "0.0",
                "other_payments": "0.0",
                "employee_pension": "0.0",
                "employer_pension": "0.0",
                "attachments": "0.0",
                "payroll_giving": "0.0",
                "ni_calc_type": "Director",
                "frequency": "Monthly",
                "additional_statutory_paternity_pay": "0.0",
                "deductions_subject_to_nic_but_not_paye": "0.0",
                "other_deductions_from_net_pay": "0.0",
                "employee_pension_not_under_net_pay": "0.0",
                "other_salary_sacrifice_deductions": "0.0",
                "employee_pension_salary_sacrifice": "0.0",
                "ni_letter": "A",
                "deduct_student_loan": true,
                "deduct_postgrad_loan": false,
                "week_1_month_1_basis": false,
                "deduction_free_pay": "0.0",
                "student_loan_deductions_plan": "Plan 1",
                "hours_worked": "120.0",
                "created_at": "2025-04-17T11:31:59.000Z",
                "updated_at": "2025-04-17T11:31:59.000Z"
            }
        ]
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <period>
        <url>https://api.freeagent.com/v2/payroll/2026/0</url>
        <period type="integer">0</period>
        <frequency>Monthly</frequency>
        <dated-on type="date">2025-04-25</dated-on>
        <status>filed</status>
        <employment-allowance-claimed type="boolean">true</employment-allowance-claimed>
        <employment-allowance-amount type="decimal">0.0</employment-allowance-amount>
        <constriction-industry-scheme-deduction type="decimal">0.0</constriction-industry-scheme-deduction>
        <created-at type="dateTime">2025-04-17T11:31:58Z</created-at>
        <updated-at type="dateTime">2025-04-17T12:42:54Z</updated-at>
        <payslips type="array">
            <payslip>
                <user>https://api.freeagent.com/v2/users/86</user>
                <tax-code>1250L</tax-code>
                <dated-on type="date">2025-04-25</dated-on>
                <basic-pay type="decimal">2500.0</basic-pay>
                <tax-deducted type="decimal">0.0</tax-deducted>
                <employee-ni type="decimal">0.0</employee-ni>
                <employer-ni type="decimal">0.0</employer-ni>
                <other-deductions type="decimal">0.0</other-deductions>
                <student-loan-deduction type="decimal">0.0</student-loan-deduction>
                <postgrad-loan-deduction type="decimal">0.0</postgrad-loan-deduction>
                <overtime type="decimal">0.0</overtime>
                <commission type="decimal">0.0</commission>
                <bonus type="decimal">0.0</bonus>
                <allowance type="decimal">0.0</allowance>
                <statutory-sick-pay type="decimal">0.0</statutory-sick-pay>
                <statutory-maternity-pay type="decimal">0.0</statutory-maternity-pay>
                <statutory-paternity-pay type="decimal">0.0</statutory-paternity-pay>
                <statutory-adoption-pay type="decimal">0.0</statutory-adoption-pay>
                <statutory-parental-bereavement-pay type="decimal">0.0</statutory-parental-bereavement-pay>
                <statutory-neonatal-care-pay type="decimal">0.0</statutory-neonatal-care-pay>
                <absence-payments type="decimal">0.0</absence-payments>
                <other-payments type="decimal">0.0</other-payments>
                <employee-pension type="decimal">0.0</employee-pension>
                <employer-pension type="decimal">0.0</employer-pension>
                <attachments type="decimal">0.0</attachments>
                <payroll-giving type="decimal">0.0</payroll-giving>
                <ni-calc-type>Director</ni-calc-type>
                <frequency>Monthly</frequency>
                <additional-statutory-paternity-pay type="decimal">0.0</additional-statutory-paternity-pay>
                <deductions-subject-to-nic-but-not-paye type="decimal">0.0</deductions-subject-to-nic-but-not-paye>
                <other-deductions-from-net-pay type="decimal">0.0</other-deductions-from-net-pay>
                <employee-pension-not-under-net-pay type="decimal">0.0</employee-pension-not-under-net-pay>
                <other-salary-sacrifice-deductions type="decimal">0.0</other-salary-sacrifice-deductions>
                <employee-pension-salary-sacrifice type="decimal">0.0</employee-pension-salary-sacrifice>
                <ni-letter>A</ni-letter>
                <deduct-student-loan type="boolean">false</deduct-student-loan>
                <deduct-postgrad-loan type="boolean">false</deduct-postgrad-loan>
                <week-1-month-1-basis type="boolean">false</week-1-month-1-basis>
                <deduction-free-pay type="decimal">0.0</deduction-free-pay>
                <created-at type="dateTime">2025-04-17T11:31:59Z</created-at>
                <updated-at type="dateTime">2025-04-17T11:31:59Z</updated-at>
            </payslip>
            <payslip>
                <user>https://api.freeagent.com/v2/users/71</user>
                <tax-code>BR</tax-code>
                <dated-on type="date">2025-04-25</dated-on>
                <basic-pay type="decimal">1100.0</basic-pay>
                <tax-deducted type="decimal">220.0</tax-deducted>
                <employee-ni type="decimal">0.0</employee-ni>
                <employer-ni type="decimal">0.0</employer-ni>
                <other-deductions type="decimal">0.0</other-deductions>
                <student-loan-deduction type="decimal">0.0</student-loan-deduction>
                <postgrad-loan-deduction type="decimal">0.0</postgrad-loan-deduction>
                <overtime type="decimal">0.0</overtime>
                <commission type="decimal">0.0</commission>
                <bonus type="decimal">0.0</bonus>
                <allowance type="decimal">0.0</allowance>
                <statutory-sick-pay type="decimal">0.0</statutory-sick-pay>
                <statutory-maternity-pay type="decimal">0.0</statutory-maternity-pay>
                <statutory-paternity-pay type="decimal">0.0</statutory-paternity-pay>
                <statutory-adoption-pay type="decimal">0.0</statutory-adoption-pay>
                <statutory-parental-bereavement-pay type="decimal">0.0</statutory-parental-bereavement-pay>
                <statutory-neonatal-care-pay type="decimal">0.0</statutory-neonatal-care-pay>
                <absence-payments type="decimal">0.0</absence-payments>
                <other-payments type="decimal">0.0</other-payments>
                <employee-pension type="decimal">0.0</employee-pension>
                <employer-pension type="decimal">0.0</employer-pension>
                <attachments type="decimal">0.0</attachments>
                <payroll-giving type="decimal">0.0</payroll-giving>
                <ni-calc-type>Director</ni-calc-type>
                <frequency>Monthly</frequency>
                <additional-statutory-paternity-pay type="decimal">0.0</additional-statutory-paternity-pay>
                <deductions-subject-to-nic-but-not-paye type="decimal">0.0</deductions-subject-to-nic-but-not-paye>
                <other-deductions-from-net-pay type="decimal">0.0</other-deductions-from-net-pay>
                <employee-pension-not-under-net-pay type="decimal">0.0</employee-pension-not-under-net-pay>
                <other-salary-sacrifice-deductions type="decimal">0.0</other-salary-sacrifice-deductions>
                <employee-pension-salary-sacrifice type="decimal">0.0</employee-pension-salary-sacrifice>
                <ni-letter>A</ni-letter>
                <deduct-student-loan type="boolean">true</deduct-student-loan>
                <student-loan-deductions-plan>Plan 1</student-loan-deductions-plan>
                <deduct-postgrad-loan type="boolean">false</deduct-postgrad-loan>
                <week-1-month-1-basis type="boolean">false</week-1-month-1-basis>
                <deduction-free-pay type="decimal">0.0</deduction-free-pay>
                <hours-worked type="decimal">120.0</hours-worked>
                <created-at type="dateTime">2025-04-17T11:31:59Z</created-at>
                <updated-at type="dateTime">2025-04-17T11:31:59Z</updated-at>
            </payslip>
        </payslips>
    </period>
</freeagent>
```
Show as JSON

## Mark a payment as paid

```http
PUT https://api.freeagent.com/v2/payroll/:year/payments/:payment_date/mark_as_paid
```

### Response

```http
Status: 200 OK
```

```json
{
    "periods": [
        {
            "url": "https://api.freeagent.com/v2/payroll/2026/0",
            "period": 0,
            "frequency": "Monthly",
            "dated_on": "2020-04-25",
            "status": "filed",
            "employment_allowance_claimed": true,
            "employment_allowance_amount": "0.0",
            "construction_industry_scheme_deduction": "0.0",
            "created_at": "2020-04-17T11:31:58.000Z",
            "updated_at": "2020-04-17T12:42:54.000Z"
        }
    ],
    "payments": [
        {
            "due_on": "2020-05-22",
            "amount_due": "738.19",
            "status": "paid"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <periods type="array">
        <period>
            <url>https://api.freeagent.com/v2/payroll/2021/0</url>
            <period type="integer">0</period>
            <frequency>Monthly</frequency>
            <dated-on type="date">2017-02-25</dated-on>
            <status>filed</status>
            <employment-allowance-claimed type="boolean">true</employment-allowance-claimed>
            <employment-allowance-amount type="decimal">0.0</employment-allowance-amount>
            <constriction-industry-scheme-deduction type="decimal">0.0</constriction-industry-scheme-deduction>
            <created-at type="dateTime">2017-02-17T11:31:58Z</created-at>
            <updated-at type="dateTime">2017-02-17T12:42:54Z</updated-at>
        </period>
    </periods>
    <payments type="array">
        <payment>
            <due-on type="date">2017-03-22</due-on>
            <amount-due type="decimal">1454.95</amount-due>
            <status>paid</status>
        </payment>
    </payments>
</freeagent>
```
Show as JSON

## Mark a payment as unpaid

```http
GET https://api.freeagent.com/v2/payroll/:year/payments/:payment_date/mark_as_unpaid
```

### Response

```http
Status: 200 OK
```

```json
{
    "periods": [
        {
            "url": "https://api.freeagent.com/v2/payroll/2021/0",
            "period": 0,
            "frequency": "Monthly",
            "dated_on": "2020-04-25",
            "status": "filed",
            "employment_allowance_claimed": true,
            "employment_allowance_amount": "0.0",
            "construction_industry_scheme_deduction": "0.0",
            "created_at": "2020-04-17T11:31:58.000Z",
            "updated_at": "2020-04-17T12:42:54.000Z"
        }
    ],
    "payments": [
        {
            "due_on": "2020-05-22",
            "amount_due": "738.19",
            "status": "unpaid"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <periods type="array">
        <period>
            <url>https://api.freeagent.com/v2/payroll/2021/0</url>
            <period type="integer">0</period>
            <frequency>Monthly</frequency>
            <dated-on type="date">2017-02-25</dated-on>
            <status>filed</status>
            <employment-allowance-claimed type="boolean">true</employment-allowance-claimed>
            <employment-allowance-amount type="decimal">0.0</employment-allowance-amount>
            <constriction-industry-scheme-deduction type="decimal">0.0</constriction-industry-scheme-deduction>
            <created-at type="dateTime">2017-02-17T11:31:58Z</created-at>
            <updated-at type="dateTime">2017-02-17T12:42:54Z</updated-at>
        </period>
    </periods>
    <payments type="array">
        <payment>
            <due-on type="date">2017-03-22</due-on>
            <amount-due type="decimal">1454.95</amount-due>
            <status>unpaid</status>
        </payment>
    </payments>
</freeagent>
```
Show as JSON