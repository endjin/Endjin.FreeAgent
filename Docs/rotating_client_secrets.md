# Rotating Client Secrets

With the client secret rotation feature, you can add a new secret to your OAuth client configuration, migrate to the new secret while the old secret is still usable, and disable the old secret afterwards. This can be useful when the client secret has been inadvertently disclosed or leaked, but doing this regularly also constitutes good security practice. Thanks to this feature, you can rotate secrets without downtime.

Treat your OAuth client credentials with extreme care, as they allow anyone who has them to use your app's identity to gain access to user information. Store your OAuth client information in a secure place and protect it, especially your client secret, just as you would a password. Where possible, use a secret manager to store client credentials. You must never commit client credentials into publicly available code repositories. We highly recommend that you avoid committing them to any code repository.

## Generating a new secret

You can generate a new secret for your application without revoking the old one, so your application will be able to authenticate using either the old or new secret value.

1. Click on the "Generate new secret" button and confirm.
2. Take note of the new secret value listed in the "OAuth secrets" table. This value will only be shown once.
3. Update your application to use the new secret value.
4. Revoke the old secret when it is no longer in use, or immediately if it has been compromised.

Note that you can only have a maximum of 2 active secrets for an application. If you already have 2 active secrets, you must revoke one before you can generate another.

## Revoking a secret

1. Find the secret you want to revoke in the "OAuth secrets" table. Verify this is the correct one by checking the "Last Used" timestamp.
2. Click "Revoke" and confirm.
3. The secret will be revoked. API requests using the revoked secret will fail.