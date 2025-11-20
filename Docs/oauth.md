# OAuth 2.0

OAuth 2.0 allows FreeAgent users to authorise third party apps to
access their data without sharing their FreeAgent login details.
Instead, an app will have one access token and one refresh token for
each authorised FreeAgent user account.

The FreeAgent API implements [OAuth 2.0 Draft 22](http://tools.ietf.org/html/draft-ietf-oauth-v2-22).  It has also been
tested with the [Google OAuth 2.0 Playground](quick_start.md) which implements OAuth 2.0 Draft 10.

To use the sandbox API change the server in the examples below to:
      **https://api.sandbox.freeagent.com**

## How OAuth 2.0 works

*You can use the [Google OAuth 2.0 Playground](quick_start.md) toexamine each step of this process.*

The App developer registers a new app at the [FreeAgent Developer Dashboard](/) to obtain an OAuth Client ID and Secret.  The Client ID and Secret are unique to each App.

When a user initiates authorising an App to access their FreeAgent
account, the App sends the user to FreeAgent's OAuth Authorization Endpoint with the Client ID and some other parameters in the URL.  The User is prompted to log into FreeAgent and will be shown a screen
 allowing them to allow or deny access.  If the user allows
access, FreeAgent will redirect the User back to the App using the
Redirect URI which was either provided when the developer registered the
app or included in the URL which sent the user to the Authorization
Endpoint.  The call to the Redirect URI will include an authorization
token if the access request was approved.

There are various strategies which can be used to retrieve the
authorization token from this redirection.  For iOS Apps the most likely strategy is
to register a custom URL scheme and set a URL using this scheme as the Redirect
URI.  Web apps will redirect the user to a URL on their site.

Once the App has the Authorisation Token it must exchange this for
Access and Refresh Tokens.  This is done out of band.
An Authorization token expires after 15 minutes.

The App makes an HTTP Basic Auth request to the FreeAgent OAuth Token
Endpoint including the Client ID, Secret and the Authorisation Token
amongst other parameters.  In return the App will receive an Access
Token and a Refresh Token.

The Access Token is included in an HTTP header on each API request.  The Refresh Token is used to
request a new Access Token when the Access Token expires.

## The Authorisation Request

To link a user account to the app, the app must send the user to
FreeAgent's OAuth Authorization Endpoint `https://api.freeagent.com/v2/approve_app` with the following URL parameters:

- `client_id`   *(required) which is the Client ID obtained at FreeAgent DeveloperDashboard*
- `response_type=code` *(required)*
- `redirect_uri` *(required) must be url escaped and match with a redirect_uri registered in your .*
- `state` *(optional) arbitrary parameters supplied by the app developerto maintain their app's state*

An example is shown below:

```json
https://api.freeagent.com/v2/approve_app?redirect_uri=https%3A%2F%2Fcode.google.com%2Foauthplayground%2F&response_type=code&client_id=vb2z_Ds8I_QWEdfgWE&state=xyz
```

If approved, the app will redirect to the Redirect URI including the
Authorisation code and the optional state parameters:

```json
HTTP/1.1 302 Found
Location: https://client.example.com/cb?code=SplxlOBeZQQYbYS6WxSbIA&state=xyz
```

## The Access Token Request

The App must exchange the Authorisation Token for an Access Token and a
Refresh Token.  To do this, the app makes an HTTP Basic Auth POST to the FreeAgent Token Endpoint `https://api.freeagent.com/v2/token_endpoint` using the Client ID as the username and Client Secret as the
password and including the following in the POST body:

- `grant_type=authorization_code` *(required)*
- `code` *(required) the authorisation code received earlier*
- `redirect_uri` *(required only if the redirect URI was specified whenmaking the Authorisation request)*

For example:

```json
Content-Type: application/json

grant_type=authorization_code&code=SplxlOBeZQQYbYS6WxSbIA
&redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb

```

If successful, the server will return a JSON response containing the
access token and refresh token:

```json
{
 "access_token":"2YotnFZFEjr1zCsicMWpAA",
 "token_type":"bearer",
 "expires_in":3600,
 "refresh_token":"tGzv3JOkF0XG5Qx2TlKWIA",
  "refresh_token_expires_in":631151957
}
```

## Using the Access Token

On each API request, the access token must be presented in an HTTP header with the following format:

```json
Authorization: Bearer TOKEN
```

- One access token will be issued for each FreeAgent user which has authorised your application.
- You can query the API to find out which user belongs to the current token using the [Get Personal Profile User endpoint](users.md#get-personal-profile).
- An access token is currently valid for one hour but this may change in future.
- It is not necessary to refresh the access token before the old access token expires

## Refreshing the Access Token

Refresh tokens  may be used to retrieve a new access token.
This means it may be most convenient to refresh the access token on the next use after expiry.
Some client libraries will handle this automatically.

To refresh an access token, the app makes an HTTP Basic Auth POST to the FreeAgent Token Endpoint
`https://api.freeagent.com/v2/token_endpoint`
using the Client ID as the username and Client Secret as the
password and including the following in the POST body:

- `grant_type=refresh_token` *(required)*
- `refresh_token` *(required) the refresh token received earlier*

For example:

```json
Content-Type: application/json

grant_type=refresh_token&refresh_token=tGzv3JOkF0XG5Qx2TlKWIA

```

If successful, the server will return a JSON response containing the new access and refresh tokens:

```json
{
 "access_token":"2YotnFZFEjr1zCsicMWpAA",
 "token_type":"bearer",
 "expires_in":3600,
 "refresh_token":"txtvz3JOkF0XG5Qx2TlKWIA",
 "refresh_token_expires_in":631151957
}
```

## Client Libraries

- Google OAuth Client Library for Java, Python, .NET, Ruby, PHP,
Objective C
- Apache Amber
- OmniAuth Gem
- OAuth2 Gem
- PHP-OAuth2 https://github.com/adoy/PHP-OAuth2