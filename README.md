# Plugin.HttpClient

[![Auto build](https://github.com/DKorablin/Plugin.HttpClient/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.HttpClient/releases/latest)

Plugin.HttpClient is a dual-target (.NET Framework 3.5 / .NET 8) SAL plugin that provides:

* An HTTP/HTTPS test client (request editing, templating, auth, headers, cookies, certificates, proxy)
* A lightweight in-process mock/test HTTP server with routing to saved project items
* Project persistence (binary, XML, JSON) with template substitution for dynamic values
* Assembly endpoint import (reflection-based route discovery)
* Result logging (status, headers, body), formatting (JSON pretty print), and history tracking

Use it to explore APIs, build repeatable test suites, mock endpoints, and capture responses inside SAL host applications.