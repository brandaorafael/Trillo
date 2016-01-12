<%@ Application Language="C#" %>

<script runat="server">
    void Application_Start()
    {
        System.Web.Http.GlobalConfiguration.Configure(PloomesCRMAPI.WebApiConfig.Register);
    }

    protected void Application_BeginRequest()
    {
        if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
        {
            Response.Flush();
        }
    }
</script>
