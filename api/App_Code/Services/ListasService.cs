using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

public class ListasService : ApiService
{
    public List<Dictionary<string, object>> Get(int? id = null, string nome = null, bool card = true)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", id);
        parametros.Add("nome", nome);

        List<Dictionary<string, object>> listas = DBQuery("Lista_Select", parametros, CommandType.StoredProcedure);

        CardsService cardService = new CardsService();

        if (card)
        {
            foreach (Dictionary<string, object> lista in listas)
            {
                lista.Add("cards", cardService.Get(null, null, (int)lista["id"]));
            }
        }

        return listas;
    }

    public List<Dictionary<string, object>> Post(JObject lista)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("nome", ToDBNull(lista, "nome"));

        List<Dictionary<string, object>> id = DBQuery("Lista_Insert", parametros, CommandType.StoredProcedure);

        return id.Count > 0 && id[0]["id_lista"] != DBNull.Value ? Get(int.Parse(id[0]["id_lista"].ToString()), null, true) : null;
    }

    public void Delete(int id)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", id);

        List<Dictionary<string, object>> resultado = DBQuery("Lista_Delete", parametros, CommandType.StoredProcedure);

        if (resultado.Count == 0 || Convert.ToInt32(resultado[0]["Rows"]) == 0)
        {
            throw new Exception("Não excluiu");
        }
    }

    public List<Dictionary<string, object>> Put(JObject lista)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", ToDBNull(lista, "id"));
        parametros.Add("nome", ToDBNull(lista, "nome"));

        List<Dictionary<string, object>> id = DBQuery("Lista_Update", parametros, CommandType.StoredProcedure);

        return id.Count > 0 && id[0]["id_lista"] != DBNull.Value ? Get(int.Parse(id[0]["id_lista"].ToString()), null, true) : null;
    }
}