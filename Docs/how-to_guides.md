# How-to Guides

Our practical guides covering specific tasks and problems.

## Connecting payroll software to FreeAgent's API

Welcome to the integration guide for third-party developers looking to connect payroll software to FreeAgent’s API. These documents provide step-by-step instructions, best practices, and technical details to ensure an optimal integration and user experience. Let’s get started!

When deciding how to push payroll-related data into FreeAgent, there are two options to choose from:

- include individual records for each employee
- merge the employee records together

## Include individual records for each employee

### FreeAgent API documentation

https://dev.freeagent.com/docs/journal_setshttps://dev.freeagent.com/docs/journal_sets

### Information required

You will need to know the following about each individual on the pay run:

- Name
- Whether they are an employee or a director
- Gross pay
- PAYE tax deducted
- Amount of employee’s National Insurance (NI) that has been deducted
- Amount of employer's National Insurance (NI) that the business will pay for them
- Net pay
- Amount of pension contributions

### Payroll-related categories in FreeAgent

- **401 Salaries** - gross pay for employee(s)
- **402 Employer NICs** - the amount of employer's NICs
- **403 Staff Pensions** - the employer’s pension contribution amount
- **407 Directors' Salaries** - gross pay for director(s)
- **408 Directors' Employer NICs** - the amount of employer's NICs
- **813 Pension Creditor** - the amount being paid to the pension provider(s)
- **814 PAYE/NI** - the total amount of tax and NICs due to HMRC
- **815** - Other Payroll Deductions - used for any other payroll deductions
- **902-X** Salary and Bonuses - net pay

*Note that 902 is a parent category that contains subcategories for each user. For example, Nathan Barley as the first user on the account (ID=1) would be 902-1 and Jack Smith as the second user on the account (ID=2) would be 902-2.*

As the payroll journals include a breakdown for each employee, you will need to know which user corresponds to each subaccount of 902 Salary and Bonuses. To do this you can call the [Categories API](https://dev.freeagent.com/docs/categories) with subaccounts set to `true` which will now include the FreeAgent ID and name of the user alongside any 90x-y accounts. As long as you have a mapping to FreeAgent users in your system, you’ll be able to identify the salary account for any specific user by comparing that value.

Should you wish to know if the user is an employee or a director, you can call the [Users API](https://dev.freeagent.com/docs/users), and the role value tells you if they're a director or not.

### Example payrun

For an employee who earned £2,000 each week, before tax and NI, the following would apply:

- For the first week of June, their PAYE tax due might be £275.20 and employee's NI would be £167.76. Subtracting these means that their net pay will be £1,557.04
- The employer could have to pay £194.71 in employer's NICs
- The employee has also asked the employer to deduct £100 from their net wages each week to pay into a pension fund
- The employer also contributes £50 to the fund

To reflect this in FreeAgent, the following journal entries would be created, dated the same day as the employee’s payslip for the first week of June:

- Debit code ‘401 Salaries’: £2,000 (the employee’s gross pay)
- Debit code ‘402 Employer NICs’: £194.71
- Credit code ‘814 PAYE/NI’: £637.67 (the total amount of tax and NICs due to HMRC: £275.20 + £167.76 + £194.71)
- Credit code ‘902 Salary and Bonuses’ for the correct user: £1,557.04 (the employee’s net pay)

*For pensions specifically*:
\* Debit code ‘902 Salary and Bonuses’ for the correct user: £100 (the amount that the employee’s net pay will be reduced by)
\* Credit code ‘813 Pension Creditor’: £150 (both pension contribution amounts)
\* Debit code ‘403 Staff Pensions’: £50 (the employer’s pension contribution amount)
This would be done via the API as follows, assuming the employee user is identified by `https://api.freeagent.com/v2/users/1`

```http
POST https://api.freeagent.com/v2/journal_sets
```

Payload should have a root journal_set element, containing elements listed under Journal Set Attributes.

```json
{
  "journal_set":{
    "dated_on":"2024-07-07",
    "description":"June Payroll Week 1",
    "journal_entries":[{
        "category":"https://api.freeagent.com/v2/categories/401",
        "description":"Gross Pay",
        "debit_value":"2000.00"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/402",
        "description":"Employer’s NIC",
        "debit_value":"194.71"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/814",
        "description":"Tax & NI Due to HMRC",
        "debit_value":"-637.67"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/902",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Net Pay",
        "debit_value":"-1557.04"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/902",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Employee's Pension Contribution",
        "debit_value":"100"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/813",
        "description":"Pension Contributions",
        "debit_value":"-150"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/403",
        "description":"Employer's Pension Contribution",
        "debit_value":"50"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-set>
    <dated-on type="date">2024-07-07</dated-on>
    <description>June Payroll Week 1</description>
    <journal-entries type="array">
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/401</category>
        <description>Gross Pay</description>
        <debit-value type="decimal">2000.00</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/402</category>
        <description>Employer’s NIC</description>
        <debit-value type="decimal">194.71</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/814</category>
        <description>Tax & NI Due to HMRC</description>
        <debit-value type="decimal">-637.67</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/902</category>
        <user>https://api.freeagent.com/v2/users/1</user>
        <description>Net Pay</description>
        <debit-value type="decimal">-1557.04</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/902</category>
        <user>https://api.freeagent.com/v2/users/1</user>
        <description>Employee's Pension Contribution</description>
        <debit-value type="decimal">100</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/813</category>
        <description>Pension Contributions</description>
        <debit-value type="decimal">-150</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/403</category>
        <description>Employer's Pension Contribution</description>
        <debit-value type="decimal">50</debit-value>
      </journal-entry>
    </journal-entries>
  </journal-set>
</freeagent>
```
Show as JSON

### Employment Allowance

If the employer is claiming the Employment Allowance, then you will need to create the following extra journal entries to show the reduction in the employer's NI liability:

#### For an employee

- Debit code '814 PAYE/NI'
- Credit code '402 Employer NICs'

#### For a director

- Debit code '814 PAYE/NI'
- Credit code ‘408 Directors' Employer NICs’

### Explaining payroll bank transactions

Although the following are not actions that the payroll integration should make via the API, below are the explanations that should be given to payroll-related transactions after the payrun by those with access to the relevant account:

Each bank transaction leaving the bank account to pay the individuals included in the pay run can be explained as:

- Type &gt; Money Paid to User
- Payment to &gt; [individual’s name]
- Reason &gt; Net Salary and Bonuses

For a payment made to HMRC for PAYE and NI, select ‘Payment’ as the transaction ‘Type’ and ‘PAYE/NI’ as the transaction category when explaining the bank transaction.

For a payment made to a pension provider, select ‘Other Money Out’ as the transaction ‘Type’ and ‘Pension Creditor’ as the transaction ‘Category’ when explaining the bank transaction.

## Merge the employee records together

### FreeAgent API documentation

https://dev.freeagent.com/docs/journal_sets

### Information required

If you want to **merge the employee records together**, you will need to know the following:

- Combined gross pay for all employees
- Combined gross pay for all directors
- Combined PAYE tax deducted across all individuals
- Combined employee’s National Insurance (NI) that has been deducted across all individuals
- Combined employer's National Insurance (NI) that the business will pay across all individuals
- Combined net pay across all individuals
- Combined pension contributions across all individuals

### Payroll related categories in FreeAgent

- **401 Salaries** - gross pay for employee(s)
- **402 Employer NICs** - the amount of employer's NICs
- **403 Staff Pensions** - the employer’s pension contribution amount
- **407 Directors' Salaries** - gross pay for director(s)
- **408 Directors' Employer NICs** - the amount of employer's NICs
- **813 Pension Creditor** - the amount being paid to the pensions provider(s)
- **814 PAYE/NI** - the total amount of tax and NICs due to HMRC
- **815 - Other Payroll Deductions** - used for any other payroll deductions
- **902-X Salary and Bonuses** - net pay

### Before the first payrun

Category 902 is a parent category that contains subcategories for each user. For example, Nathan Barley as the first user on the account (ID=1) would be 902-1 and Jack Smith as the second user on the account (ID=2) would be 902-2.

As you want to merge the employee records together, we suggest either:

- [creating a new user](users.md) with a name such as ‘Payroll Control’ for the purpose of grouping all payroll-related data against
- [creating a new custom liabilities category](categories.md) with the name ‘Payroll Control’

### Example payrun

Payroll is run for three directors and three employees in the first week of June.

**Employees**

- Earned a total of £8,000 each week before tax and NI
- For the first week of June, their PAYE tax due might be £1100.80 and employee's NI would be £671.04. Subtracting these means that their net pay will be £6,228.16
- The employer could have to pay £778.84 in employer's NICs for the directors

**Directors**

- Earned a total of £12,000 each week before tax and NI
- For the first week of June, their PAYE tax due might be £1651.20 and employee's NI would be £1006.56. Subtracting these means that their net pay will be £9.342.24
- The employer could have to pay £1168.26 in employer's NICs for the employees

**Both**

- The total amount across the six individuals net wages to pay into a pension fund is £1200
- The employer also contributes £600 to the fund

To reflect this in FreeAgent, the following journal entries would be created, dated the same day as the employee’s payslip for the first week of June:

- Debit code ‘401 Salaries’: £12,000 (total gross pay for all employees)
- Debit code ‘407 Directors' Salaries’: £8,000 (total gross pay for all directors)
- Debit code ‘402 Employer NICs’: £1168.26 (total amount of employer's NICs for employees)
- Debit code ‘408 Directors' Employer NICs’: £778.84 (total amount of employer's NICs for directors)
- Credit code ‘814 PAYE/NI’: £6376.65 (the total amount of tax and NICs due to HMRC for
    - Employees = (£1100.80 + £671.04 + £778.84) = £2550.68 +
    - Directors = (£1651.20 + £1006.56 + £1168.26) = £3825.97)
- Credit code ‘902 Salary and Bonuses’ for the user ID Payroll Control **or** Credit code ‘[new custom liability category]’ entitled Payroll Control : £15,552.04 (Total net pay for all six individuals)
*For pensions specifically:*
- Debit code ‘902 Salary and Bonuses’ for the user ID Payroll Control **or** Debit code ‘[new custom liability category]’ entitled Payroll Control: £1200 (the total amount that the employee’s and director’s net pay will be reduced by)
- Credit code ‘813 Pension Creditor’: £1800 (both pension contribution amounts)
- Debit code ‘403 Staff Pensions’: £600 (the employer’s pension contribution amount)

This would be done via the API as follows, assuming the “Payroll Control” user is identified by: `https://api.freeagent.com/v2/users/1`

```http
POST https://api.freeagent.com/v2/journal_sets
```

Payload should have a root journal_set element, containing elements listed under Journal Set Attributes.

```json
{
  "journal_set":{
    "dated_on":"2024-07-07",
    "description":"June Payroll Week 1",
    "journal_entries":[
      {
        "category":"https://api.freeagent.com/v2/categories/401",
        "description":"Total Gross Pay",
        "debit_value":"12000.00"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/402",
        "description":"Total Employer’s NIC for Employees",
        "debit_value":"1168.26"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/407",
        "description":"Total Directors' Salary",
        "debit_value":"8000.00"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/408",
        "description":"Total Employer's NIC for Directors",
        "debit_value":"778.84"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/814",
        "description":"Tax & NI Due to HMRC",
        "debit_value":"-6376.65"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/902",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Total Salary and Bonuses",
        "debit_value":"-15570.45"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/902",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Employee's Pension Contribution",
        "debit_value":"1200"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/813",
        "description":"Pension Contributions",
        "debit_value":"-1800"
      },
      {
        "category":"https://api.freeagent.com/v2/categories/403",
        "description":"Employer's Total Pension Contribution",
        "debit_value":"600"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-set>
    <dated-on type="date">2024-07-07</dated-on>
    <description>June Payroll Week 1</description>
    <journal-entries type="array">
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/401</category>
        <description>Total Gross Pay</description>
        <debit-value type="decimal">12000.00</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/402</category>
        <description>Total Employer’s NIC for Employees</description>
        <debit-value type="decimal">1168.26</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/407</category>
        <description>Total Directors' Salary</description>
        <debit-value type="decimal">8000.00</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/408</category>
        <description>Total Employer's NIC for Directors</description>
        <debit-value type="decimal">778.84</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/814</category>
        <description>Tax & NI Due to HMRC</description>
        <debit-value type="decimal">-6376.65</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/902</category>
        <user>https://api.freeagent.com/v2/users/1</user>
        <description>Total Salary and Bonuses</description>
        <debit-value type="decimal">-15570.45</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/902</category>
        <user>https://api.freeagent.com/v2/users/1</user>
        <description>Employee's Pension Contribution</description>
        <debit-value type="decimal">1200</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/813</category>
        <description>Pension Contributions</description>
        <debit-value type="decimal">-1800</debit-value>
      </journal-entry>
      <journal-entry>
        <category>https://api.freeagent.com/v2/categories/403</category>
        <description>Employer's Total Pension Contribution</description>
        <debit-value type="decimal">600</debit-value>
      </journal-entry>
    </journal-entries>
  </journal-set>
</freeagent>
```
Show as JSON

### **Employment Allowance**

If the employer is claiming the Employment Allowance, then you will need to create the following extra journal entries to show the reduction in the employer's NI liability:

**For an employee**

- Debit code '814 PAYE/NI'
- Credit code '402 Employer NICs'

**For a director**

- Debit code '814 PAYE/NI'
- Credit code ‘408 Directors' Employer NICs’

### **Explaining payroll bank transactions**

Although the following are not actions that the payroll integration should make via the API, below are the explanations that should be given to payroll-related transactions after the payrun by those with access to the relevant account:

Each bank transaction leaving the bank account to pay the individuals included in the pay run can be explained as:

*If a new user was created for Payroll control:*

- Type &gt; Money Paid to User
- Payment to &gt; Payroll Control
- Reason &gt; Net Salary and Bonuses

*If a new liability category was created for Payroll control:*

- Type &gt; Other Money Our
- Category &gt; [new custom liability category]

For a payment made to HMRC for PAYE and NI, select ‘Payment’ as the transaction ‘Type’ and ‘PAYE/NI’ as the transaction category when explaining the bank transaction.

For a payment made to a pension provider, select ‘Other Money Out’ as the transaction ‘Type’ and ‘Pension Creditor’ as the transaction ‘Category’ when explaining the bank transaction.