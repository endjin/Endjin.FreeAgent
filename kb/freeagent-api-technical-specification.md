# FreeAgent API Technical Specification

## Table of Contents
1. [API Overview](#api-overview)
2. [Authentication & Security](#authentication--security)
3. [API Versioning](#api-versioning)
4. [Common Patterns](#common-patterns)
5. [Financial Resources](#financial-resources)
6. [Banking Resources](#banking-resources)
7. [Billing Resources](#billing-resources)
8. [Expense Management](#expense-management)
9. [Project & Time Tracking](#project--time-tracking)
10. [Contacts & Company](#contacts--company)
11. [Tax & Compliance](#tax--compliance)
12. [Asset Management](#asset-management)
13. [Payroll](#payroll)
14. [Supporting Resources](#supporting-resources)
15. [Sample Data for Testing](#sample-data-for-testing)
16. [Implementation Guidelines](#implementation-guidelines)

---

## API Overview

### Base URLs
- **Production**: `https://api.freeagent.com/v2`
- **Sandbox**: `https://api.sandbox.freeagent.com/v2`

### Rate Limits
- **Per Minute**: 120 requests
- **Per Hour**: 3,600 requests
- Headers returned: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

### Response Formats
- **JSON** (default): `Accept: application/json`
- **XML**: `Accept: application/xml`

### Pagination
- Default page size: 100
- Maximum page size: 100
- Parameters: `?page={n}&per_page={n}`
- Response headers: `Link` (with rel="first", "last", "next", "prev")

### HTTP Status Codes
- `200 OK` - Successful GET/PUT
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Invalid request
- `401 Unauthorized` - Invalid/missing authentication
- `403 Forbidden` - Access denied
- `404 Not Found` - Resource not found
- `422 Unprocessable Entity` - Validation errors
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
  "errors": {
    "error": {
      "message": "Error description",
      "code": "ERROR_CODE"
    }
  }
}
```

---

## Authentication & Security

### OAuth 2.0 Implementation

#### Authorization Flow
1. **Authorization Request**
   ```
   GET https://api.freeagent.com/v2/approve_app
   ?response_type=code
   &client_id={client_id}
   &redirect_uri={redirect_uri}
   &state={state}
   ```

2. **Token Exchange**
   ```http
   POST /v2/token_endpoint
   Content-Type: application/x-www-form-urlencoded

   grant_type=authorization_code
   &code={authorization_code}
   &client_id={client_id}
   &client_secret={client_secret}
   &redirect_uri={redirect_uri}
   ```

3. **Token Response**
   ```json
   {
     "access_token": "bearer_token_string",
     "token_type": "bearer",
     "expires_in": 604800,
     "refresh_token": "refresh_token_string"
   }
   ```

4. **Token Refresh**
   ```http
   POST /v2/token_endpoint
   Content-Type: application/x-www-form-urlencoded

   grant_type=refresh_token
   &refresh_token={refresh_token}
   &client_id={client_id}
   &client_secret={client_secret}
   ```

### Client Secret Rotation

Zero-downtime rotation process:
1. Generate new secret (both old and new valid for 2 hours)
2. Update application to use new secret
3. Old secret expires after 2 hours

```http
POST /v2/client_secret_rotation
Authorization: Bearer {access_token}
```

Response:
```json
{
  "client_secret": {
    "secret": "new_secret_string",
    "expires_at": "2024-01-01T14:00:00Z"
  }
}
```

---

## API Versioning

### Date-Based Versioning Strategy
- Version specified via `X-FreeAgent-Version` header
- Format: `YYYY-MM-DD`
- Default: Current date
- Backwards compatibility maintained for 6 months

Example:
```http
GET /v2/invoices
X-FreeAgent-Version: 2024-01-15
```

---

## Common Patterns

### Date Formats
- ISO 8601: `YYYY-MM-DD` for dates
- ISO 8601 with timezone: `YYYY-MM-DDTHH:MM:SSZ` for timestamps

### Filtering
Standard filter parameters:
- `from_date` / `to_date` - Date range filtering
- `updated_since` - Changes since timestamp
- `view` - Predefined filters (e.g., `view=recent`)
- `sort` - Sort order (e.g., `sort=-created_at`)

### Nested Resources
Access pattern: `/parent_resource/{id}/child_resource`
Example: `/contacts/123/invoices`

---

## Financial Resources

### Balance Sheet

#### Get Balance Sheet
```http
GET /accounting/balance_sheet/summary
```

Query Parameters:
- `date` (required) - Balance sheet date

Response:
```json
{
  "balance_sheet_summary": {
    "assets": {
      "current": {
        "bank_accounts": 50000.00,
        "accounts_receivable": 15000.00,
        "total_current_assets": 65000.00
      },
      "fixed": {
        "tangible_assets": 25000.00,
        "depreciation": -5000.00,
        "total_fixed_assets": 20000.00
      },
      "total_assets": 85000.00
    },
    "liabilities": {
      "current": {
        "accounts_payable": 8000.00,
        "vat_payable": 3000.00,
        "total_current_liabilities": 11000.00
      },
      "long_term": {
        "loans": 20000.00,
        "total_long_term_liabilities": 20000.00
      },
      "total_liabilities": 31000.00
    },
    "equity": {
      "share_capital": 10000.00,
      "retained_earnings": 44000.00,
      "total_equity": 54000.00
    },
    "total_liabilities_and_equity": 85000.00
  }
}
```

### Profit & Loss

#### Get Profit & Loss Report
```http
GET /accounting/profit_and_loss/summary
```

Query Parameters:
- `from_date` (required)
- `to_date` (required)

Response:
```json
{
  "profit_and_loss_summary": {
    "income": {
      "turnover": 125000.00,
      "other_income": 2000.00,
      "total_income": 127000.00
    },
    "expenses": {
      "cost_of_sales": 45000.00,
      "administrative_expenses": 30000.00,
      "total_expenses": 75000.00
    },
    "profit": {
      "gross_profit": 82000.00,
      "operating_profit": 52000.00,
      "profit_before_tax": 52000.00,
      "tax": 10000.00,
      "net_profit": 42000.00
    }
  }
}
```

### Trial Balance

#### Get Trial Balance
```http
GET /accounting/trial_balance/summary
```

Query Parameters:
- `date` (required)

Response:
```json
{
  "trial_balance_summary": {
    "entries": [
      {
        "nominal_code": "001",
        "name": "Bank Account",
        "debit_total": 50000.00,
        "credit_total": 0.00,
        "balance": 50000.00
      },
      {
        "nominal_code": "400",
        "name": "Sales",
        "debit_total": 0.00,
        "credit_total": 125000.00,
        "balance": -125000.00
      }
    ],
    "total_debits": 175000.00,
    "total_credits": 175000.00
  }
}
```

### Cash Flow

#### Get Cash Flow Statement
```http
GET /accounting/cash_flow
```

Query Parameters:
- `from_date` (required)
- `to_date` (required)

Response:
```json
{
  "cash_flow": {
    "operating_activities": {
      "receipts_from_customers": 120000.00,
      "payments_to_suppliers": -45000.00,
      "payments_to_employees": -30000.00,
      "net_cash_from_operations": 45000.00
    },
    "investing_activities": {
      "purchase_of_assets": -10000.00,
      "proceeds_from_asset_sales": 2000.00,
      "net_cash_from_investing": -8000.00
    },
    "financing_activities": {
      "loan_proceeds": 20000.00,
      "loan_repayments": -5000.00,
      "dividends_paid": -10000.00,
      "net_cash_from_financing": 5000.00
    },
    "net_change_in_cash": 42000.00,
    "opening_cash_balance": 8000.00,
    "closing_cash_balance": 50000.00
  }
}
```

---

## Banking Resources

### Bank Accounts

#### List Bank Accounts
```http
GET /bank_accounts
```

Query Parameters:
- `view` - Filter: `all`, `active`, `hidden`

#### Get Bank Account
```http
GET /bank_accounts/{id}
```

#### Create Bank Account
```http
POST /bank_accounts
Content-Type: application/json

{
  "bank_account": {
    "type": "StandardBankAccount",
    "name": "Business Current Account",
    "nominal_code": "001",
    "account_number": "12345678",
    "sort_code": "123456",
    "secondary_sort_code": "654321",
    "iban": "GB29NWBK60161331926819",
    "bic": "NWBKGB2L",
    "opening_balance": 1000.00,
    "bank_name": "Example Bank",
    "currency": "GBP",
    "is_primary": true,
    "status": "active"
  }
}
```

Response:
```json
{
  "bank_account": {
    "url": "https://api.freeagent.com/v2/bank_accounts/123",
    "id": 123,
    "type": "StandardBankAccount",
    "name": "Business Current Account",
    "currency": "GBP",
    "current_balance": 1000.00,
    "status": "active",
    "is_primary": true,
    "created_at": "2024-01-01T10:00:00Z",
    "updated_at": "2024-01-01T10:00:00Z"
  }
}
```

### Bank Transactions

#### List Bank Transactions
```http
GET /bank_transactions
```

Query Parameters:
- `bank_account` - Bank account URL
- `from_date` / `to_date` - Date range
- `view` - `all`, `explained`, `unexplained`, `imported`, `manual`

#### Get Bank Transaction
```http
GET /bank_transactions/{id}
```

#### Create Manual Bank Transaction
```http
POST /bank_transactions
Content-Type: application/json

{
  "bank_transaction": {
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/123",
    "dated_on": "2024-01-15",
    "description": "Payment from Client ABC",
    "amount": 1200.00,
    "is_manual": true
  }
}
```

#### Upload Bank Statement
```http
POST /bank_transactions/statement
Content-Type: application/json

{
  "statement": {
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/123",
    "statement": "base64_encoded_ofx_or_csv_data",
    "file_type": "ofx"
  }
}
```

### Bank Transaction Explanations

#### Create Explanation
```http
POST /bank_transaction_explanations
Content-Type: application/json

{
  "bank_transaction_explanation": {
    "bank_transaction": "https://api.freeagent.com/v2/bank_transactions/456",
    "dated_on": "2024-01-15",
    "category": "https://api.freeagent.com/v2/categories/001",
    "gross_value": 1200.00,
    "description": "Invoice payment",
    "attachment": "https://api.freeagent.com/v2/attachments/789",
    "sales_tax_rate": 20.0,
    "manual_sales_tax_amount": 200.00
  }
}
```

#### Delete Explanation
```http
DELETE /bank_transaction_explanations/{id}
```

### Bank Feeds (Beta)

#### Setup Bank Feed
```http
POST /bank_feeds
Content-Type: application/json

{
  "bank_feed": {
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/123",
    "provider": "open_banking",
    "authorization_code": "auth_code_from_provider"
  }
}
```

---

## Billing Resources

### Invoices

#### List Invoices
```http
GET /invoices
```

Query Parameters:
- `view` - `all`, `recent`, `open`, `overdue`, `draft`, `scheduled`
- `contact` - Filter by contact URL
- `project` - Filter by project URL
- `from_date` / `to_date` - Date range

#### Get Invoice
```http
GET /invoices/{id}
```

#### Create Invoice
```http
POST /invoices
Content-Type: application/json

{
  "invoice": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "dated_on": "2024-01-15",
    "due_on": "2024-02-15",
    "reference": "INV-001",
    "currency": "GBP",
    "exchange_rate": 1.0,
    "payment_terms_in_days": 30,
    "invoice_items": [
      {
        "description": "Consulting Services",
        "item_type": "Hours",
        "quantity": 10,
        "price": 150.00,
        "sales_tax_rate": 20.0,
        "category": "https://api.freeagent.com/v2/categories/001"
      },
      {
        "description": "Project Materials",
        "item_type": "Products",
        "quantity": 1,
        "price": 500.00,
        "sales_tax_rate": 20.0
      }
    ],
    "payment_methods": {
      "bank_account": {
        "bank_name": "Example Bank",
        "account_number": "12345678",
        "sort_code": "123456",
        "iban": "GB29NWBK60161331926819",
        "bic": "NWBKGB2L"
      },
      "paypal_email": "payments@example.com"
    }
  }
}
```

Response:
```json
{
  "invoice": {
    "url": "https://api.freeagent.com/v2/invoices/456",
    "id": 456,
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "reference": "INV-001",
    "dated_on": "2024-01-15",
    "due_on": "2024-02-15",
    "status": "Draft",
    "currency": "GBP",
    "net_value": 2000.00,
    "total_value": 2400.00,
    "paid_value": 0.00,
    "due_value": 2400.00,
    "created_at": "2024-01-15T10:00:00Z",
    "updated_at": "2024-01-15T10:00:00Z",
    "invoice_items": [
      {
        "url": "https://api.freeagent.com/v2/invoice_items/789",
        "position": 1,
        "description": "Consulting Services",
        "item_type": "Hours",
        "quantity": 10.0,
        "price": 150.00,
        "sales_tax_rate": 20.0,
        "subtotal": 1500.00,
        "sales_tax_amount": 300.00,
        "total": 1800.00
      }
    ]
  }
}
```

#### Update Invoice Status

**Send Invoice**
```http
PUT /invoices/{id}/send_email
Content-Type: application/json

{
  "invoice": {
    "email": {
      "to": "client@example.com",
      "subject": "Invoice INV-001",
      "body": "Please find attached invoice INV-001.",
      "cc": "accounts@example.com",
      "bcc": "records@example.com"
    }
  }
}
```

**Mark as Sent**
```http
PUT /invoices/{id}/mark_as_sent
```

**Mark as Paid**
```http
PUT /invoices/{id}/mark_as_paid
Content-Type: application/json

{
  "invoice": {
    "paid_on": "2024-02-01",
    "paid_into_bank_account": "https://api.freeagent.com/v2/bank_accounts/123"
  }
}
```

### Credit Notes

#### List Credit Notes
```http
GET /credit_notes
```

Query Parameters:
- `view` - `all`, `draft`, `sent`, `refunded`
- `contact` - Filter by contact URL

#### Create Credit Note
```http
POST /credit_notes
Content-Type: application/json

{
  "credit_note": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "dated_on": "2024-01-20",
    "reference": "CN-001",
    "invoice": "https://api.freeagent.com/v2/invoices/456",
    "currency": "GBP",
    "exchange_rate": 1.0,
    "credit_note_items": [
      {
        "description": "Refund for returned items",
        "quantity": 2,
        "price": 100.00,
        "sales_tax_rate": 20.0
      }
    ],
    "reason": "Items returned"
  }
}
```

#### Send Credit Note
```http
PUT /credit_notes/{id}/send_email
Content-Type: application/json

{
  "credit_note": {
    "email": {
      "to": "client@example.com",
      "subject": "Credit Note CN-001",
      "body": "Please find attached credit note CN-001."
    }
  }
}
```

#### Mark as Refunded
```http
PUT /credit_notes/{id}/mark_as_refunded
Content-Type: application/json

{
  "credit_note": {
    "refunded_on": "2024-01-25",
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/123"
  }
}
```

### Bills

#### List Bills
```http
GET /bills
```

Query Parameters:
- `view` - `all`, `open`, `overdue`, `paid`, `recurring`
- `from_date` / `to_date`

#### Create Bill
```http
POST /bills
Content-Type: application/json

{
  "bill": {
    "contact": "https://api.freeagent.com/v2/contacts/789",
    "dated_on": "2024-01-10",
    "due_on": "2024-02-10",
    "reference": "BILL-001",
    "category": "https://api.freeagent.com/v2/categories/250",
    "total_value": 600.00,
    "sales_tax_rate": 20.0,
    "manual_sales_tax_amount": 100.00,
    "project": "https://api.freeagent.com/v2/projects/111",
    "rebill_type": "manual",
    "rebill_factor": 1.5,
    "comments": "Office supplies for Q1"
  }
}
```

#### Mark Bill as Paid
```http
PUT /bills/{id}/mark_as_paid
Content-Type: application/json

{
  "bill": {
    "paid_on": "2024-02-05",
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/123"
  }
}
```

### Estimates

#### List Estimates
```http
GET /estimates
```

Query Parameters:
- `view` - `all`, `recent`, `draft`, `sent`, `approved`, `rejected`, `expired`

#### Create Estimate
```http
POST /estimates
Content-Type: application/json

{
  "estimate": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "dated_on": "2024-01-01",
    "expires_on": "2024-01-31",
    "reference": "EST-001",
    "currency": "GBP",
    "estimate_type": "Estimate",
    "estimate_items": [
      {
        "description": "Website Development",
        "item_type": "Services",
        "quantity": 1,
        "price": 5000.00,
        "sales_tax_rate": 20.0
      }
    ],
    "notes": "Valid for 30 days"
  }
}
```

#### Send Estimate
```http
PUT /estimates/{id}/send_email
Content-Type: application/json

{
  "estimate": {
    "email": {
      "to": "prospect@example.com",
      "subject": "Estimate EST-001",
      "body": "Please find our estimate attached."
    }
  }
}
```

#### Convert to Invoice
```http
POST /estimates/{id}/convert_to_invoice
```

Response returns the newly created invoice.

---

## Expense Management

### Expenses

#### List Expenses
```http
GET /expenses
```

Query Parameters:
- `view` - `all`, `recent`, `recurring`, `repeat`
- `user` - Filter by user URL
- `from_date` / `to_date`
- `updated_since`

#### Create Expense
```http
POST /expenses
Content-Type: application/json

{
  "expense": {
    "user": "https://api.freeagent.com/v2/users/456",
    "category": "https://api.freeagent.com/v2/categories/285",
    "dated_on": "2024-01-15",
    "currency": "GBP",
    "gross_value": 120.00,
    "sales_tax_rate": 20.0,
    "manual_sales_tax_amount": 20.00,
    "description": "Train ticket to client meeting",
    "project": "https://api.freeagent.com/v2/projects/789",
    "rebill_type": "rebill",
    "rebill_factor": 1.0,
    "receipt_reference": "RCPT-001",
    "mileage": 50,
    "vehicle_type": "car",
    "engine_type": "petrol",
    "engine_size": "1401_to_2000cc",
    "reclaim_mileage_rate": 0.45,
    "attachment": "https://api.freeagent.com/v2/attachments/123"
  }
}
```

Response:
```json
{
  "expense": {
    "url": "https://api.freeagent.com/v2/expenses/999",
    "id": 999,
    "user": "https://api.freeagent.com/v2/users/456",
    "category": "https://api.freeagent.com/v2/categories/285",
    "dated_on": "2024-01-15",
    "currency": "GBP",
    "gross_value": 120.00,
    "sales_tax_rate": 20.0,
    "net_value": 100.00,
    "sales_tax_value": 20.00,
    "description": "Train ticket to client meeting",
    "project": "https://api.freeagent.com/v2/projects/789",
    "rebill_type": "rebill",
    "rebill_status": "pending",
    "created_at": "2024-01-15T14:00:00Z",
    "updated_at": "2024-01-15T14:00:00Z"
  }
}
```

#### Rebill Expense
```http
PUT /expenses/{id}/rebill
Content-Type: application/json

{
  "expense": {
    "rebilled_on": "2024-01-20",
    "invoice": "https://api.freeagent.com/v2/invoices/888"
  }
}
```

---

## Project & Time Tracking

### Projects

#### List Projects
```http
GET /projects
```

Query Parameters:
- `view` - `all`, `active`, `completed`, `cancelled`, `hidden`
- `contact` - Filter by contact URL

#### Create Project
```http
POST /projects
Content-Type: application/json

{
  "project": {
    "name": "Website Redesign",
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "status": "Active",
    "currency": "GBP",
    "budget": 10000.00,
    "budget_units": "monetary",
    "uses_project_invoice_sequence": true,
    "hours_per_day": 8.0,
    "starts_on": "2024-01-01",
    "ends_on": "2024-03-31",
    "billing_basis": "time",
    "hourly_billing_rate": 150.00,
    "project_invoice_sequence_prefix": "WEB"
  }
}
```

Response:
```json
{
  "project": {
    "url": "https://api.freeagent.com/v2/projects/333",
    "id": 333,
    "name": "Website Redesign",
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "status": "Active",
    "currency": "GBP",
    "budget": 10000.00,
    "budget_units": "monetary",
    "budget_used": 0.00,
    "total_invoiced": 0.00,
    "total_to_invoice": 0.00,
    "created_at": "2024-01-01T09:00:00Z",
    "updated_at": "2024-01-01T09:00:00Z"
  }
}
```

### Tasks

#### List Tasks
```http
GET /tasks
```

Query Parameters:
- `project` - Filter by project URL
- `is_billable` - `true` or `false`

#### Create Task
```http
POST /tasks
Content-Type: application/json

{
  "task": {
    "project": "https://api.freeagent.com/v2/projects/333",
    "name": "Frontend Development",
    "is_billable": true,
    "billing_rate": 150.00,
    "billing_period": "hour",
    "status": "active"
  }
}
```

### Timeslips

#### List Timeslips
```http
GET /timeslips
```

Query Parameters:
- `user` - Filter by user URL
- `task` - Filter by task URL
- `project` - Filter by project URL
- `from_date` / `to_date`
- `updated_since`

#### Create Timeslip
```http
POST /timeslips
Content-Type: application/json

{
  "timeslip": {
    "user": "https://api.freeagent.com/v2/users/456",
    "task": "https://api.freeagent.com/v2/tasks/777",
    "project": "https://api.freeagent.com/v2/projects/333",
    "dated_on": "2024-01-15",
    "hours": 7.5,
    "comment": "Implemented responsive navigation"
  }
}
```

#### Start Timer
```http
POST /timeslips/timer
Content-Type: application/json

{
  "timeslip": {
    "user": "https://api.freeagent.com/v2/users/456",
    "task": "https://api.freeagent.com/v2/tasks/777"
  }
}
```

Returns:
```json
{
  "timeslip": {
    "url": "https://api.freeagent.com/v2/timeslips/888",
    "id": 888,
    "timer_started_at": "2024-01-15T09:00:00Z",
    "timer_running": true
  }
}
```

#### Stop Timer
```http
PUT /timeslips/{id}/stop_timer
```

---

## Contacts & Company

### Contacts

#### List Contacts
```http
GET /contacts
```

Query Parameters:
- `view` - `all`, `active`, `hidden`, `clients`, `suppliers`
- `updated_since`
- `sort` - Field to sort by

#### Create Contact
```http
POST /contacts
Content-Type: application/json

{
  "contact": {
    "organisation_name": "ABC Corporation",
    "first_name": "John",
    "last_name": "Smith",
    "email": "john.smith@abc.com",
    "phone_number": "+44 20 1234 5678",
    "mobile": "+44 7700 900123",
    "address1": "123 Business Street",
    "address2": "Suite 456",
    "town": "London",
    "region": "Greater London",
    "postcode": "EC1A 1BB",
    "country": "United Kingdom",
    "contact_name_on_invoices": true,
    "locale": "en",
    "account_balance": 0.00,
    "status": "active",
    "sales_tax_registration_number": "GB123456789",
    "uses_contact_invoice_sequence": true,
    "contact_invoice_sequence_prefix": "ABC",
    "charging_method": "time",
    "default_payment_terms_in_days": 30
  }
}
```

Response:
```json
{
  "contact": {
    "url": "https://api.freeagent.com/v2/contacts/222",
    "id": 222,
    "organisation_name": "ABC Corporation",
    "first_name": "John",
    "last_name": "Smith",
    "email": "john.smith@abc.com",
    "status": "active",
    "created_at": "2024-01-01T10:00:00Z",
    "updated_at": "2024-01-01T10:00:00Z"
  }
}
```

### Company

#### Get Company Details
```http
GET /company
```

Response:
```json
{
  "company": {
    "url": "https://api.freeagent.com/v2/company",
    "name": "Example Ltd",
    "subdomain": "example",
    "type": "UkLimitedCompany",
    "currency": "GBP",
    "mileage_units": "miles",
    "company_start_date": "2020-01-01",
    "first_accounting_year_end": "2020-12-31",
    "company_registration_number": "12345678",
    "sales_tax_registration_status": "Registered",
    "sales_tax_registration_number": "GB123456789",
    "sales_tax_rates": [
      {
        "rate": 20.0,
        "description": "Standard"
      },
      {
        "rate": 5.0,
        "description": "Reduced"
      },
      {
        "rate": 0.0,
        "description": "Zero"
      }
    ],
    "supports_auto_sales_tax_on_purchases": true,
    "created_at": "2020-01-01T09:00:00Z",
    "updated_at": "2024-01-15T14:30:00Z"
  }
}
```

#### Update Company
```http
PUT /company
Content-Type: application/json

{
  "company": {
    "name": "Example Limited",
    "sales_tax_registration_number": "GB987654321",
    "ec_vat_reporting_enabled": true
  }
}
```

### Users

#### List Users
```http
GET /users
```

#### Get Current User
```http
GET /users/me
```

Response:
```json
{
  "user": {
    "url": "https://api.freeagent.com/v2/users/456",
    "id": 456,
    "first_name": "Alice",
    "last_name": "Admin",
    "email": "alice@example.com",
    "role": "Admin",
    "permission_level": 7,
    "opening_mileage": 0.0,
    "created_at": "2020-01-01T09:00:00Z",
    "updated_at": "2024-01-01T10:00:00Z"
  }
}
```

Permission Levels:
- 0: No Access
- 1: Time Tracking
- 2: Expenses & Estimates
- 3: Invoicing, Expenses & Estimates
- 4: Bills
- 5: Banking
- 6: Tax, Payroll & Accounting
- 7: Full (Admins & Accountants)
- 8: Director

---

## Tax & Compliance

### VAT Returns

#### List VAT Returns
```http
GET /vat_returns
```

#### Get VAT Return
```http
GET /vat_returns/{id}
```

Response:
```json
{
  "vat_return": {
    "url": "https://api.freeagent.com/v2/vat_returns/101",
    "id": 101,
    "period_starts_on": "2024-01-01",
    "period_ends_on": "2024-03-31",
    "frequency": "quarterly",
    "status": "draft",
    "box1_vat_due_on_sales": 25000.00,
    "box2_vat_due_on_acquisitions": 500.00,
    "box3_total_vat_due": 25500.00,
    "box4_vat_reclaimed": 8000.00,
    "box5_net_vat_due": 17500.00,
    "box6_total_sales_ex_vat": 125000,
    "box7_total_purchases_ex_vat": 40000,
    "box8_total_supplies_ex_vat": 10000,
    "box9_total_acquisitions_ex_vat": 2500,
    "created_at": "2024-04-01T09:00:00Z",
    "updated_at": "2024-04-01T09:00:00Z"
  }
}
```

#### Mark as Filed
```http
PUT /vat_returns/{id}/mark_as_filed
Content-Type: application/json

{
  "vat_return": {
    "filed_on": "2024-04-15",
    "filed_online": true,
    "hmrc_reference": "123456789012"
  }
}
```

### Corporation Tax Returns

#### List Corporation Tax Returns
```http
GET /corporation_tax_returns
```

Response:
```json
{
  "corporation_tax_returns": [
    {
      "url": "https://api.freeagent.com/v2/corporation_tax_returns/201",
      "id": 201,
      "period_starts_on": "2023-01-01",
      "period_ends_on": "2023-12-31",
      "amount_payable": 15000.00,
      "status": "filed",
      "filed_on": "2024-10-01",
      "payment_due_on": "2024-10-01",
      "payment_status": "paid",
      "paid_on": "2024-09-28",
      "created_at": "2024-01-01T09:00:00Z",
      "updated_at": "2024-10-01T10:00:00Z"
    }
  ]
}
```

### Self Assessment Returns

#### List Self Assessment Returns
```http
GET /self_assessment_returns
```

Query Parameters:
- `user` - Filter by user URL

Response:
```json
{
  "self_assessment_returns": [
    {
      "url": "https://api.freeagent.com/v2/self_assessment_returns/301",
      "id": 301,
      "user": "https://api.freeagent.com/v2/users/456",
      "tax_year": "2023-24",
      "status": "filed",
      "filed_on": "2024-01-31",
      "amount_payable": 5000.00,
      "payment_due_on": "2024-01-31",
      "payment_status": "paid",
      "paid_on": "2024-01-30",
      "created_at": "2024-01-01T09:00:00Z",
      "updated_at": "2024-01-31T16:00:00Z"
    }
  ]
}
```

### Sales Tax

#### Get Sales Tax Rates
```http
GET /sales_tax_rates
```

Response:
```json
{
  "sales_tax_rates": [
    {
      "rate": 20.0,
      "description": "Standard",
      "valid_from": "2011-01-04",
      "valid_to": null
    },
    {
      "rate": 5.0,
      "description": "Reduced",
      "valid_from": "2011-01-04",
      "valid_to": null
    },
    {
      "rate": 0.0,
      "description": "Zero",
      "valid_from": "2011-01-04",
      "valid_to": null
    }
  ]
}
```

---

## Asset Management

### Capital Assets

#### List Capital Assets
```http
GET /capital_assets
```

Query Parameters:
- `view` - `all`, `active`, `disposed`

#### Create Capital Asset
```http
POST /capital_assets
Content-Type: application/json

{
  "capital_asset": {
    "description": "MacBook Pro 16\"",
    "capital_asset_type": "https://api.freeagent.com/v2/capital_asset_types/501",
    "purchased_on": "2024-01-15",
    "purchased_price": 2500.00,
    "located_at": "Main Office",
    "asset_life_years": 3,
    "depreciation_method": "straight_line",
    "residual_value": 500.00
  }
}
```

Response:
```json
{
  "capital_asset": {
    "url": "https://api.freeagent.com/v2/capital_assets/601",
    "id": 601,
    "description": "MacBook Pro 16\"",
    "capital_asset_type": "https://api.freeagent.com/v2/capital_asset_types/501",
    "purchased_on": "2024-01-15",
    "purchased_price": 2500.00,
    "asset_life_years": 3,
    "depreciation_method": "straight_line",
    "residual_value": 500.00,
    "current_book_value": 2500.00,
    "accumulated_depreciation": 0.00,
    "disposed_on": null,
    "created_at": "2024-01-15T11:00:00Z",
    "updated_at": "2024-01-15T11:00:00Z"
  }
}
```

#### Dispose of Asset
```http
PUT /capital_assets/{id}/dispose
Content-Type: application/json

{
  "capital_asset": {
    "disposed_on": "2024-12-31",
    "disposal_proceeds": 800.00
  }
}
```

### Capital Asset Types

#### List Capital Asset Types
```http
GET /capital_asset_types
```

Response:
```json
{
  "capital_asset_types": [
    {
      "url": "https://api.freeagent.com/v2/capital_asset_types/501",
      "id": 501,
      "description": "Computer Equipment",
      "nominal_code": "0050"
    },
    {
      "url": "https://api.freeagent.com/v2/capital_asset_types/502",
      "id": 502,
      "description": "Office Furniture",
      "nominal_code": "0051"
    }
  ]
}
```

### Depreciation Profiles

#### List Depreciation Profiles
```http
GET /depreciation_profiles
```

Response:
```json
{
  "depreciation_profiles": [
    {
      "url": "https://api.freeagent.com/v2/depreciation_profiles/701",
      "id": 701,
      "name": "Computer Equipment - 3 Years",
      "method": "straight_line",
      "period_years": 3,
      "residual_percentage": 20.0
    },
    {
      "url": "https://api.freeagent.com/v2/depreciation_profiles/702",
      "id": 702,
      "name": "Vehicles - Reducing Balance",
      "method": "reducing_balance",
      "annual_percentage": 25.0
    }
  ]
}
```

---

## Payroll

### Payroll (UK Only)

#### List Payrolls
```http
GET /payrolls
```

Query Parameters:
- `from_date` / `to_date`

Response:
```json
{
  "payrolls": [
    {
      "url": "https://api.freeagent.com/v2/payrolls/801",
      "id": 801,
      "dated_on": "2024-01-31",
      "period_start": "2024-01-01",
      "period_end": "2024-01-31",
      "total_gross": 15000.00,
      "total_net": 11500.00,
      "total_income_tax": 2000.00,
      "total_employee_nic": 1000.00,
      "total_employer_nic": 1500.00,
      "total_employee_pension": 500.00,
      "total_employer_pension": 750.00,
      "payslips": [
        {
          "url": "https://api.freeagent.com/v2/payslips/901",
          "user": "https://api.freeagent.com/v2/users/456",
          "gross_salary": 5000.00,
          "net_salary": 3800.00,
          "income_tax": 700.00,
          "employee_nic": 350.00,
          "employee_pension": 150.00
        }
      ],
      "created_at": "2024-01-31T09:00:00Z",
      "updated_at": "2024-01-31T09:00:00Z"
    }
  ]
}
```

### Payroll Profiles

#### List Payroll Profiles
```http
GET /payroll_profiles
```

Response:
```json
{
  "payroll_profiles": [
    {
      "url": "https://api.freeagent.com/v2/payroll_profiles/1001",
      "id": 1001,
      "user": "https://api.freeagent.com/v2/users/456",
      "employee_id": "EMP001",
      "tax_code": "1257L",
      "ni_letter": "A",
      "annual_salary": 60000.00,
      "pension_scheme": "workplace",
      "pension_contribution_percentage": 3.0,
      "student_loan_type": "plan_2",
      "created_at": "2024-01-01T09:00:00Z",
      "updated_at": "2024-01-01T09:00:00Z"
    }
  ]
}
```

---

## Supporting Resources

### Categories

#### List Categories
```http
GET /categories
```

Query Parameters:
- `type` - Filter by category type

Response:
```json
{
  "categories": [
    {
      "url": "https://api.freeagent.com/v2/categories/001",
      "nominal_code": "001",
      "description": "Sales",
      "category_type": "Income",
      "tax_reporting_name": "Sales"
    },
    {
      "url": "https://api.freeagent.com/v2/categories/250",
      "nominal_code": "250",
      "description": "Advertising",
      "category_type": "Administrative Expenses",
      "tax_reporting_name": "Advertising and Promotion"
    }
  ]
}
```

### Attachments

#### Upload Attachment
```http
POST /attachments
Content-Type: multipart/form-data

{
  "attachment": {
    "file": "binary_file_data",
    "filename": "receipt.pdf",
    "content_type": "application/pdf",
    "description": "Office supplies receipt"
  }
}
```

Response:
```json
{
  "attachment": {
    "url": "https://api.freeagent.com/v2/attachments/1101",
    "id": 1101,
    "filename": "receipt.pdf",
    "content_type": "application/pdf",
    "size": 102400,
    "description": "Office supplies receipt",
    "created_at": "2024-01-15T10:00:00Z"
  }
}
```

#### Delete Attachment
```http
DELETE /attachments/{id}
```

### Notes

#### List Notes
```http
GET /notes
```

Query Parameters:
- `contact` - Filter by contact URL
- `project` - Filter by project URL

#### Create Note
```http
POST /notes
Content-Type: application/json

{
  "note": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "project": "https://api.freeagent.com/v2/projects/333",
    "note": "Discussed project timeline and budget adjustments",
    "author": "https://api.freeagent.com/v2/users/456"
  }
}
```

### Recurring Invoices

#### List Recurring Invoices
```http
GET /recurring_invoices
```

#### Create Recurring Invoice
```http
POST /recurring_invoices
Content-Type: application/json

{
  "recurring_invoice": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "frequency": "Monthly",
    "recurring_start_date": "2024-02-01",
    "recurring_end_date": "2024-12-31",
    "days_before_due_date": 7,
    "reference": "REC-001",
    "invoice_items": [
      {
        "description": "Monthly Retainer",
        "item_type": "Services",
        "quantity": 1,
        "price": 1000.00,
        "sales_tax_rate": 20.0
      }
    ]
  }
}
```

Frequency Options:
- `Weekly`
- `Two_Weekly`
- `Four_Weekly`
- `Monthly`
- `Two_Monthly`
- `Quarterly`
- `Biannually`
- `Annually`

### Journal Sets

#### Create Journal Entry
```http
POST /journal_sets
Content-Type: application/json

{
  "journal_set": {
    "dated_on": "2024-01-31",
    "description": "Year-end adjustments",
    "journal_entries": [
      {
        "debit_value": 1000.00,
        "credit_value": 0.00,
        "category": "https://api.freeagent.com/v2/categories/001"
      },
      {
        "debit_value": 0.00,
        "credit_value": 1000.00,
        "category": "https://api.freeagent.com/v2/categories/002"
      }
    ]
  }
}
```

### Currencies

#### List Currencies
```http
GET /currencies
```

Response:
```json
{
  "currencies": [
    {
      "code": "USD",
      "name": "United States Dollar",
      "symbol": "$"
    },
    {
      "code": "EUR",
      "name": "Euro",
      "symbol": "€"
    },
    {
      "code": "GBP",
      "name": "British Pound",
      "symbol": "£"
    }
  ]
}
```

---

## Sample Data for Testing

### Invoice Creation Sample
```json
{
  "invoice": {
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "dated_on": "2024-01-15",
    "due_on": "2024-02-15",
    "reference": "INV-001",
    "currency": "GBP",
    "invoice_items": [
      {
        "description": "Consulting Services - January 2024",
        "item_type": "Services",
        "quantity": 80,
        "price": 125.00,
        "sales_tax_rate": 20.0
      }
    ]
  }
}
```

### Expense Creation Sample
```json
{
  "expense": {
    "user": "https://api.freeagent.com/v2/users/456",
    "category": "https://api.freeagent.com/v2/categories/285",
    "dated_on": "2024-01-15",
    "currency": "GBP",
    "gross_value": 54.00,
    "sales_tax_rate": 20.0,
    "description": "Client lunch meeting",
    "project": "https://api.freeagent.com/v2/projects/789"
  }
}
```

### Bank Transaction Explanation Sample
```json
{
  "bank_transaction_explanation": {
    "bank_transaction": "https://api.freeagent.com/v2/bank_transactions/456",
    "dated_on": "2024-01-15",
    "category": "https://api.freeagent.com/v2/categories/001",
    "gross_value": 12000.00,
    "description": "Invoice INV-001 payment",
    "sales_tax_rate": 20.0
  }
}
```

### Contact Creation Sample
```json
{
  "contact": {
    "organisation_name": "Tech Innovations Ltd",
    "first_name": "Sarah",
    "last_name": "Johnson",
    "email": "sarah@techinnovations.com",
    "phone_number": "+44 20 7123 4567",
    "address1": "100 Tech Street",
    "town": "London",
    "postcode": "EC2A 1AA",
    "country": "United Kingdom",
    "status": "active",
    "default_payment_terms_in_days": 30
  }
}
```

### Project Creation Sample
```json
{
  "project": {
    "name": "Mobile App Development",
    "contact": "https://api.freeagent.com/v2/contacts/123",
    "status": "Active",
    "currency": "GBP",
    "budget": 25000.00,
    "budget_units": "monetary",
    "starts_on": "2024-02-01",
    "ends_on": "2024-05-31",
    "billing_basis": "time_and_materials",
    "hourly_billing_rate": 150.00
  }
}
```

### Timeslip Creation Sample
```json
{
  "timeslip": {
    "user": "https://api.freeagent.com/v2/users/456",
    "task": "https://api.freeagent.com/v2/tasks/777",
    "project": "https://api.freeagent.com/v2/projects/333",
    "dated_on": "2024-01-15",
    "hours": 6.5,
    "comment": "Backend API implementation and testing"
  }
}
```

---

## Implementation Guidelines

### Best Practices

1. **Authentication**
   - Always use HTTPS
   - Store tokens securely
   - Implement token refresh before expiry
   - Use state parameter for CSRF protection

2. **Error Handling**
   - Implement exponential backoff for rate limits
   - Parse error responses for detailed information
   - Log all API interactions for debugging
   - Handle network timeouts gracefully

3. **Performance**
   - Use pagination for large datasets
   - Cache frequently accessed data
   - Batch operations where possible
   - Use `updated_since` for incremental syncs

4. **Data Integrity**
   - Validate data before submission
   - Handle currency precision correctly (2 decimal places)
   - Preserve URL references between resources
   - Implement idempotency for critical operations

5. **Testing**
   - Use sandbox environment for development
   - Create comprehensive test suites
   - Mock API responses for unit tests
   - Test error scenarios and edge cases

### Common Workflows

#### Invoice Lifecycle
1. Create draft invoice
2. Add invoice items
3. Send to client (email or mark as sent)
4. Track payment status
5. Mark as paid when payment received
6. Generate credit note if needed

#### Expense Management Flow
1. Create expense with receipt
2. Attach supporting documents
3. Submit for approval (if required)
4. Assign to project for rebilling
5. Include in invoice if rebillable
6. Track reimbursement status

#### Bank Reconciliation Process
1. Import bank statement
2. Match transactions automatically
3. Create explanations for unmatched items
4. Categorize transactions
5. Attach receipts/invoices
6. Complete reconciliation

#### Project Billing Workflow
1. Create project with budget
2. Set up tasks and billing rates
3. Track time via timeslips
4. Review unbilled time
5. Generate project invoice
6. Monitor budget vs actual

### Security Considerations

1. **API Keys**
   - Never expose client secrets in code
   - Rotate secrets regularly
   - Use environment variables
   - Implement secure storage

2. **Data Protection**
   - Encrypt sensitive data at rest
   - Use secure connections (TLS 1.2+)
   - Implement access controls
   - Audit API usage

3. **Compliance**
   - Follow GDPR requirements
   - Implement data retention policies
   - Allow data export/deletion
   - Maintain audit trails

---

## Conclusion

This technical specification provides comprehensive documentation for implementing a FreeAgent API client. The API offers robust functionality for accounting, invoicing, expense management, and financial reporting with proper authentication, error handling, and data validation mechanisms.

Key implementation priorities:
1. OAuth 2.0 authentication with token management
2. Core financial operations (invoicing, expenses, banking)
3. Comprehensive error handling and retry logic
4. Efficient data synchronization strategies
5. Thorough testing with provided sample data

The API's RESTful design, consistent patterns, and detailed documentation make it suitable for building reliable financial management integrations.