<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ConfigSampleWebApp</title>
</head>
<body>
    <%

        foreach (var appSettingName in ConfigurationManager.AppSettings.AllKeys)
        {
            Response.Write($"Setting: {appSettingName}={ConfigurationManager.AppSettings[appSettingName]}<br/>");
        }

        foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
        {
            Response.Write("<br/>");
            Response.Write($"Connection string: {connectionString.Name}<br/>");
            Response.Write($"> connection string: {connectionString.ConnectionString}<br/>");
            Response.Write($"> provider: {connectionString.ProviderName}<br/>");
        }
    %>
</body>
</html>
