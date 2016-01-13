using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Data;
using Newtonsoft.Json.Linq;

public class ListasController : PloomesCRM
{
    private ListasService service;
    public ListasController()
    {
        service = new ListasService();
    }

    [ActionName("")]
    public object Get(int? id = null, string nome = null, bool card = true)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, SelectedFields(service.Get(id, nome, card), ControllerContext));
        }
        catch (Exception)
        {
            Dictionary<string, object> erro = new Dictionary<string, object>();
    erro.Add("success", false);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, erro);
        }
    }

    [ActionName("")]
    public object Post(JObject lista)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, SelectedFields(service.Post(lista), ControllerContext));
        }
        catch (Exception)
        {
            Dictionary<string, object> erro = new Dictionary<string, object>();
            erro.Add("success", false);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, erro);
        }
    }

    [ActionName("")]
    public object Delete(int id)
    {
        Dictionary<string, object> retorno = new Dictionary<string, object>();
        try
        {
            service.Delete(id);
            retorno.Add("success", true);
            return Request.CreateResponse(HttpStatusCode.OK, retorno);
        }
        catch (Exception)
        {
            retorno.Add("success", false);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, retorno);
        }
    }

    [ActionName("")]
    public object Put(JObject lista)
    {
        try
        {
            return Request.CreateResponse(HttpStatusCode.OK, SelectedFields(service.Put(lista), ControllerContext));
        }
        catch (Exception)
        {
            Dictionary<string, object> erro = new Dictionary<string, object>();
            erro.Add("success", false);
            return Request.CreateResponse(HttpStatusCode.InternalServerError, erro);
        }
    }
}
