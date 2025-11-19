# Changes

## 2025

## 12 August

- Attachments require the same access level as the item they are attached to.

### 3 June

- Allow for the `from_date` and/or `to_date` filters to be combined with the `updated_since` filter (where they are provided)

### 21 Apr

- Add `updated_since` filter to [Contacts](contacts.md) /v2/contacts endpoint

### 12 February

- Add `statutory_neonatal_care_pay` to [Payroll](payroll.md) endpoints

## 2024

### 28 November

- Add `currency` and `is_deletable` fields to the [task endpoint](tasks.md).

### 7 November

- Add `bank_account`, `stock_item`, `capital_asset_type`, `hire_purchase` and `user` attributes to relevant [categories](categories.md).

### 9 October

- Add address fields to the [payroll profile endpoint](payroll_profiles.md).
- Add the `title` field to the [payroll profile endpoint](payroll_profiles.md).
- Add the `date_of_birth` field to the [payroll profile endpoint](payroll_profiles.md).
- Add the `gender` field to the [payroll profile endpoint](payroll_profiles.md).

### 2 October

- Remove deprecated `mark_as_cancelled` endpoint for [Credit Notes](credit_notes.md)

### 12 August

- Add the `created_at` field to the [payroll profile endpoint](payroll_profiles.md).
- Add the `updated_at` field to the [payroll profile endpoint](payroll_profiles.md).

### 6 August

- Include CoPilot clients in the [accountancy practice
clients endpoint](accountancy_practice_api.md#list-clients).
- Add the `copilot` option to the `view` parameter in the [accountancy practice clients endpoint](accountancy_practice_api.md#list-clients).

### 17 July

- Add the `subdomain` attribute to the [practice endpoint](accountancy_practice_api.md#get-details-of-accountancy-practice)

### 7 March

- Add support for new methods of calculating depreciation of capital assets via
[depreciation profiles](depreciation_profiles.md) when creating expenses,
bank transaction explanations, and bills.
- Deprecate `depreciation_schedule` for [expense](expenses.md) and
[bill/bill item](bills.md) requests and responses.
- Deprecate `asset_life_years` for [bank transaction explanation](bank_transaction_explanations.md) requests and responses.

## 2023

### 7 November

- Add [show a single transaction](transactions.md) endpoint
- Add the following attributes to [show transactions](transactions.md)endpoints:
    - `url`
    - `category_name`

### 23 October

- Add [Corporation Tax Returns](corporation_tax_returns.md) endpoints
- Add [Final Accounts Reports](final_accounts_reports.md) endpoints
- Add [Self Assessment Returns](self_assessment_returns.md) endpoints
- Add [VAT Returns](vat_returns.md) endpoints
- Add payment details to [Payroll](payroll.md) endpoints

### 9 October

- Add [balance sheet report](balance_sheet.md) endpoint
- Add [show transactions report](transactions.md) endpoint
- Support specifying date ranges and annual accounting period when requesting a [profit and loss report](profit_and_loss.md)
- Add endpoint to [retrieve data about accountancy practice](accountancy_practice_api.md#get-details-of-accountancy-practice)

### 25 September

- Add `account_owner` information to the [accountancy practice clients endpoint](accountancy_practice_api.md#list-clients) response

## 2022

### 9 November

- Add new endpoints for fetching information about [Hire Purchases](hire_purchases.md).
- Add new `is_paid_by_hire_purchase` parameter to endpoints in [Bills](bills.md).

### 14 October

- Add support for creating, updating and deleting [Price List Items](price_list_items.md).

### 12 October

- Add new `attachments` parameter for send_email endpoints in [Invoices](invoices.md#email-an-invoice), [Credit Notes](credit_notes.md#email-a-credit-note) and [Estimates](estimates/.md#email-an-estimate).

### 6 October

- Require [bill item URL](bills.md#bill-item-attributes) when [updating existing single-item bills](bills.md#update-a-bill), matching multi-item bill behaviour.

### 11 August

- Add stock attributes for [Expenses](expenses.md).

### 3 August

- Add support for `UkUnincorporatedLandlord` companies. This includes a new [Properties](properties.md) endpoint and a new `property` attribute for [Bank Transaction Explanations](bank_transaction_explanations.md), [Bills](bills.md), [Credit Notes](credit_notes.md), [Expenses](expenses.md), [Invoices](invoices.md) and [Journal Sets](journal_sets.md).

### 28 June

- Add support for zero to multiple [bill items](bills.md#bill-item-attributes) for bills on the production FreeAgent API.

### 23 May

- Add support for zero to multiple [bill items](bills.md#bill-item-attributes) for bills, only on the [FreeAgent Sandbox](quick_start.md). [Read announcement](https://api-discuss.freeagent.com/t/multi-item-bills-now-available-for-sandbox-testing/2879).

## 2021

### 8 December

- Add endpoints for reading, updating and deleting [default additional text](invoices.md#default-additional-text) for invoices and estimates.

### 2 November

- Remove bill item fields (e.g. `total_value`, sales tax) from the default [Bills](bills.md) endpoint response.
- Add new `nested_bill_items` parameter to include the `bill_items` details in [Bills](bills.md#list-all-bills-with-nested-bill-items) endpoint responses.
- Require the [`bill_items`](bills.md#bill-item-attributes) nested attribute when creating or updating via the [Bills](bills.md) endpoint.
- Obsolete: the `FreeAgent-Features: bill_items` header is no longer needed on the [Bills](bills.md) endpoint as the new schema is default.

### 12 October

- Add read-only [Payroll Profiles](payroll_profiles.md) endpoint.

## 2020

### 17 December

- Add endpoints for creating, updating and deleting [accounting categories](categories.md) and
[capital asset types](capital_asset_types.md).

### 27 July

- Allow retrieval of capital asset history, including capital allowance calculations. See
[Capital Assets](capital_assets.md) for details.

### 22 July

- Add [Credit Note Reconciliations](credit_note_reconciliations.md) endpoints

### 28 May

- Allow creation of journal entries in capital asset categories that require a sub-code (601-607). This can be done
by adding a URL to the relevant [capital asset type](capital_asset_types.md) to your request. See
[Journal Entry Attributes](journal_sets.md#journal-entry-attributes) documentation for more details.

### 23 April

- Add new sub_accounts parameter to the [Categories](categories.md) endpoint.

### 30 April

- Add a filter to the [/clients](accountancy_practice_api.md#list-clients) endpoint in the [Accountancy Practice API](accountancy_practice_api.md)

### 3 February

- Add [Credit Note](credit_notes.md) endpoints

## 2018

### 8 May

- Add endpoint for company [Business Categories](company.md#list-all-business-categories)

### 8 March

- Add read-only payroll profile to users

## 2017

### 2 October

- Updated [documentation](sales_tax.md#ec-vat-moss) around EC VAT MOSS

### 20 September

- Add endpoint for [Construction Industry Scheme bands](cis_bands.md)
- Add `cis_enabled` to companies
- Add the following CIS attributes to invoices:
    - `cis_rate`
    - `cis_deduction_rate`
    - `cis_deduction`
    - `cis_deduction_suffered`

### 12 June

- Add `payment_methods` flags to invoices
- Add `direct_debit_mandate_state` and `direct_debit_mandate` to contacts
- Add `direct_debit` resource to invoices (create only)

### 22 May

- Add `updated_since`filter to following endpoints:
    - /v2/invoices
    - /v2/bills
    - /v2/timeslips
    - /v2/estimates
    - /v2/expenses
    - /v2/bank_transactions
    - /v2/bank*transaction*explanations

### 9 May

- Add `status` attribute to bank accounts

### 4 May: Additional filters for bank transactions

- Add `explained` and `marked_for_review` views to bank transactions list
- Add latest statement upload filter for bank transactions

### 4 Apr

- Add `open` and `overdue` view filters for invoices

### 7 Mar

- Add support for reading RTI payroll data

## 2016

### 3 Oct

- Add support for viewing, starting and stopping running timers on timeslips
- Allow timeslips to be filtered by state (all/unbilled/running)

### 28 Sep: Changes to Users endpoint

- Brings the API behaviour in line with the FreeAgent web application
- It is no longer possible to set a password for a newly created user
- It is no longer possible to change a password for an existing user
- Invitations can be sent to users allowing them to choose their own password

### 22 Sep: Expenses link to rebilled invoice

- If an expense has been rebilled on an invoice, a link to that invoice now appears when requesting expense details.

### 20 Sep: NIN / UTR

- Add National Insurance Number and Unique Tax Reference attributes to users

### 19 Sep: Price List Item support

- We now support listing of single and all Price List Items

### 31 Mar: Email Addresses

- Clients can now fetch a list of verified email addresses available for sending invoices

### 28 Mar: Sales Tax Period support

- Support for listing, creating, and updating sales tax periods for US and Universal companies

## 2014

### 1st Jul: Journal Sets resource added

- `journal_sets` endpoint and documentation added
- `stock_items` endpoint and documentation added

### 24 Jun: Ordering Support

- Sorting options for listing of invoices and contacts added

### 3 Feb: API v1 No Longer Supported

- API v1 documentation removed

## 2013

### 30th Jan: Trial Balance API

- Retrieve Trial Balance data via our API

### 17th Jan: Documentation Updates

- Correction made to the sending invoice emails JSON example.

## 2012

### 20th Nov: Sales Tax Documentation Updates

- Sales tax documentation improved.
- `notes` and `comments` fields added to the Estimate and Invoice documentation.

### 2nd Nov: Documentation Updates

- Documentation examples corrected to show `url` field instead of `path`.
- Maximum attachment size updated.
- Sales tax documentation for expenses improved.