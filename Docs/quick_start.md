# Quick start - using the Google OAuth 2.0 Playground

## Introduction

The [Google OAuth 2.0 Playground](https://code.google.com/oauthplayground/#step3&url=https%3A//&content_type=application/json&http_method=GET&useDefaultOauthCred=unchecked&oauthEndpointSelect=Custom&oauthAuthEndpointValue=https%3A//api.sandbox.freeagent.com/v2/approve_app&oauthTokenEndpointValue=https%3A//api.sandbox.freeagent.com/v2/token_endpoint&includeCredentials=unchecked "Google OAuth 2.0 Playground") enables developers to explore OAuth 2.0 compatible APIs without writing code.

To get set up to work with companies using FreeAgent, follow the steps below.

To get set up to work with accountancy practices and bookkeepers using FreeAgent, and allow them to access their client data, head over to the [Accountancy Practice API](accountancy_practice_api.md) docs and follow those steps.

To use the FreeAgent API for companies with the Google OAuth 2.0 Playground, you must do the following:

1. Sign into the company and complete the setup stages. If you don't do this, you will receive unexpected error messages when using the API.
2. Create an App at the [FreeAgent Developer Dashboard](/). Once created, take note of the *OAuth identifier* and *OAuth secret*.
3. Finally, it's time to get an Access Token which can then be used with the `OAuth Playground`, `curl` or your own apps during development. The following section explains exactly how to generate the Access and Refresh Tokens.

## Generate Access & Refresh tokens

This process can be broken into 3 steps.

### Configure the OAuth Playground

Go to the [Google OAuth 2.0 Playground](https://code.google.com/oauthplayground/#step3&url=https%3A//&content_type=application/json&http_method=GET&useDefaultOauthCred=unchecked&oauthEndpointSelect=Custom&oauthAuthEndpointValue=https%3A//api.sandbox.freeagent.com/v2/approve_app&oauthTokenEndpointValue=https%3A//api.sandbox.freeagent.com/v2/token_endpoint&includeCredentials=unchecked "Google OAuth 2.0 Playground")

The URL above sets up the Playground with FreeAgent's `sandbox` endpoints:

| Endpoint type                | Endpoint                                                                                                   |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------- |
| OAuth Authorization Endpoint | [https://api.sandbox.freeagent.com/v2/approve_app](https://api.sandbox.freeagent.com/v2/approve_app)       |
| OAuth Token Endpoint         | [https://api.sandbox.freeagent.com/v2/token_endpoint](https://api.sandbox.freeagent.com/v2/token_endpoint) |

Once the `OAuth 2.0 Playground` page has loaded, you will notice that the cog button at the top right is clicked and a dialog is already visible, allowing you to enter the OAuth Client ID and OAuth Client Secret from `Step 3` in the previous section.

To use the production API (instead of the sandbox one) change the two endpoints to reference
      **api.freeagent.com** instead of api.sandbox.freeagent.com

*Hint:* With the **Link** icon you can create a link to save these settings for future use and if you tick **Include OAuth credentials and OAuth tokens in the link** you won't have to enter the Client ID and Secret each time.

### Authorize API usage

On the left hand side now you will see `3 Steps`. Expand `Step 1`. Our goal is to Authorize API usage. Google OAuth Playground requires you to specify a scope before you can `Authorize APIs`. So go ahead and insert a scope name (any string should do) before hitting the now enabled `Authorize APIs` button.

The Playground will redirect you to the Sandbox where you will have to log in to the FreeAgent Sandbox account you created (`Step 1` of the previous section). Once logged in you can approve the Playground app.

Approving the app will return you to the Playground.

### Generate the API tokens

Click **Exchange Authorization Code for Tokens** to create access and refresh tokens which can be used to access the API.

You can then access the FreeAgent API for the Sandbox account you
authorised.  For example try:

```json
https://api.sandbox.freeagent.com/v2/company
```

which should produce:

```json
{"company":{"type":"UkLimitedCompany","currency":"GBP","mileage_units":"miles","company_start_date":"2010-07-01","sales_tax_registration_status":"Registered"}}

```

Now that you have an Access Token, you can also [use it with Curl](using_curl.md) or use it to test
out your own app with the FreeAgent API before you implement OAuth authentication.

Next up, [Introducing the FreeAgent API](introduction.md)