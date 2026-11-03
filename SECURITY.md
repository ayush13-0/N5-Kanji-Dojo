# Security Policy

## Supported releases

Security reports should identify the exact platform, version, operating-system version and reproduction steps. Until a formal support schedule is established, treat the latest published release for each platform as the preferred version.

## Reporting a vulnerability

Please do not publish exploitable vulnerabilities in public issues. Contact the maintainer privately through the contact details on the developer's GitHub profile. Include:

- A clear description and potential impact.
- Affected platform and release version.
- Reproduction steps or a proof of concept that does not expose real user data.
- Any suggested mitigation.

Do not include passwords, private keys, signing keystores, signing passwords or personal data in a report.

## Security design claims and verification boundaries

The project handover describes the Android v1.6 package as having zero permissions and no `INTERNET` permission. It also describes the Windows and macOS applications as offline-first, with external social links opening in the system browser.

These are historical claims from the handover, not a fresh audit of the binaries in this repository. Before each release:

1. Inspect the Android manifest and permissions.
2. Inspect application code and dependencies for network access.
3. Verify Windows and macOS runtime behaviour and bundled dependencies.
4. Scan release artifacts using reputable security tooling.
5. Verify signatures and publish complete SHA-256 hashes.
6. Test installation, upgrade, uninstall and preservation of local study data.

## Signing-key protection

- Never commit `.jks`, `.keystore`, `.p12`, `.pfx`, private keys, certificate passwords or signing configuration containing secrets.
- Keep encrypted backups in a secure location.
- Restrict access to release credentials.
- If a signing key is exposed, assess whether the platform permits key rotation and publish a security advisory.

## Responsible disclosure

The maintainer will acknowledge valid reports and work toward a fix. Do not assume a particular response time until a service-level commitment is published.
