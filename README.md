# Sample for how to use appsettings.json + overrides to populate web.config/app.config AppSettings and ConnectionStrings

This sample application shows how to use configuration builders (introduced in .NET 4.7.1) and the
configuration system introduced in ASP.NET Core (`Microsoft.Extensions.Configuration`) to load
`appsettings.json` files and overriding per-environment files like `appsettings.Development.json`
to populate app settings and connection strings usually set in `web.config` (also works for console apps
and `App.config`).