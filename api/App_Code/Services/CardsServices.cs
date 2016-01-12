using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

public class CardsService: ApiService
{
    public List<Dictionary<string, object>> Get(int? id = null, string nome = null, int? id_lista = null)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("id_card", id);
        parametros.Add("nome", nome);
        parametros.Add("id_lista", id_lista);

        List<Dictionary<string, object>> cards = DBQuery("Card_Select", parametros, CommandType.StoredProcedure);

        return cards;
    }


}