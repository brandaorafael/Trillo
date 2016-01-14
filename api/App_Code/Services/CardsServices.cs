using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

public class CardsService: ApiService
{
    public List<Dictionary<string, object>> Get(int? id = null, string nome = null, int? id_lista = null)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", id);
        parametros.Add("nome", nome);
        parametros.Add("id_lista", id_lista);

        List<Dictionary<string, object>> cards = DBQuery("Card_Select", parametros, CommandType.StoredProcedure);

        return cards;
    }

    public List<Dictionary<string, object>> Post(JObject card)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("nome", ToDBNull(card, "nome"));
        parametros.Add("id_lista", ToDBNull(card, "id_lista"));

        List<Dictionary<string, object>> id = DBQuery("Card_Insert", parametros, CommandType.StoredProcedure);

        return id.Count > 0 && id[0]["id_card"] != DBNull.Value ? Get(int.Parse(id[0]["id_card"].ToString()), null, null) : null;
    }

    public void Delete(int id)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", id);

        List<Dictionary<string, object>> resultado = DBQuery("Card_Delete", parametros, CommandType.StoredProcedure);

        if (resultado.Count == 0 || Convert.ToInt32(resultado[0]["Rows"]) == 0)
        {
            throw new Exception("Não excluiu");
        }
    }

    public List<Dictionary<string, object>> Put(JObject card)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id", ToDBNull(card, "id"));
        parametros.Add("nome", ToDBNull(card, "nome"));
        parametros.Add("id_lista", ToDBNull(card, "id_lista"));
        parametros.Add("cor", ToDBNull(card, "cor"));

        List<Dictionary<string, object>> id = DBQuery("Card_Update", parametros, CommandType.StoredProcedure);

        return id.Count > 0 && id[0]["id_card"] != DBNull.Value ? Get(int.Parse(id[0]["id_card"].ToString()), null, null) : null;
    }
}