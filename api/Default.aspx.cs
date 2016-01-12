using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var config = GlobalConfiguration.Configuration.Services.GetApiExplorer();
        IApiExplorer apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();

        //foreach (ApiDescription api in apiExplorer.ApiDescriptions.OrderBy(api => api.RelativePath))
        //{
            //Response.Write("Path: " + api.RelativePath + "<br>");
            //Response.Write("HTTP method: " + api.HttpMethod + "<br>");
            //foreach (ApiParameterDescription parameter in api.ParameterDescriptions)
            //{
                //Response.Write("Parameter: " + parameter.Name + " - " + parameter.Source + "<br>");
            //}

            //Response.Write("<br><br>");
        //}
    }
}