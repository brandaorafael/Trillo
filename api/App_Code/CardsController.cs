using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

/// <summary>
/// Summary description for Class1
/// </summary>
public class CardsController: PloomesCRM
{
    private CardsService service;
    public CardsController()
    {
        service = new CardsService();
    }

    [ActionName("")]
    public object Get(int? id = null, int? id_lista = null, string nome = null)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, SelectedFields(service.Get(id, nome, id_lista), ControllerContext));
        }
        catch (Exception)
        {
            Dictionary<string, object> erro = new Dictionary<string, object>();
            erro.Add("success", false);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, erro);
        }
    }
    
}