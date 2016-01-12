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
        parametros.Add("id_lista", id);
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

     



    /* public List<Dictionary<string, object>> Post(string uk, string apvk, JObject cliente)
     {
         try
         {
             object ID_Usuario = Authenticate(uk, "uk");
             object ID_ClientePloomes = Authenticate(apvk, "apvk");
             Dictionary<string, object> parametros = new Dictionary<string, object>();
             parametros.Add("ID_Usuario", ID_Usuario);
             parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
             parametros.Add("ID_Tipo", ToDBNull(cliente, "ID_Tipo"));
             parametros.Add("ID_Empresa", ToDBNull(cliente, "Empresa", "ID_Empresa"));
             parametros.Add("ID_Responsavel", ToDBNull(cliente, "Responsavel", "ID_Responsavel"));
             parametros.Add("Nome", cliente["Cliente"].ToString());
             parametros.Add("CNPJ", ToDBNull(cliente, "CNPJ"));
             parametros.Add("ID_Status", ToDBNull(cliente, "Status", "ID_Status"));
             parametros.Add("ID_Classe", ToDBNull(cliente, "Classe", "ID_Classe"));
             parametros.Add("Email", ToDBNull(cliente, "Email"));
             parametros.Add("Site", ToDBNull(cliente, "Site"));

             parametros.Add("ID_Segmento", ToDBNull(cliente, "Segmento", "ID_Segmento"));
             parametros.Add("ID_Origem", ToDBNull(cliente, "Origem", "ID_Origem"));
             parametros.Add("ID_Relacao", ToDBNull(cliente, "Relacao", "ID_Relacao"));
             parametros.Add("ID_Cargo", ToDBNull(cliente, "Cargo", "ID_Cargo"));
             parametros.Add("ID_Departamento", ToDBNull(cliente, "Departamento", "ID_Departamento"));
             parametros.Add("ID_QtdFuncionarios", ToDBNull(cliente, "QtdFuncionarios", "ID_QtdFuncionarios"));
             parametros.Add("Facebook", ToDBNull(cliente, "Facebook"));
             parametros.Add("Obs", ToDBNull(cliente, "Obs"));
             parametros.Add("DataFundacao", ToDBNull(cliente, "DataFundacao"));
             parametros.Add("Skype", ToDBNull(cliente, "Skype"));

             parametros.Add("Endereco", ToDBNull(cliente, "Endereco"));
             parametros.Add("Complemento", ToDBNull(cliente, "Complemento"));
             parametros.Add("Bairro", ToDBNull(cliente, "Bairro"));
             parametros.Add("CEP", ToDBNull(cliente, "CEP"));
             parametros.Add("Recebe_EmailMkt", ToDBNull(cliente, "Recebe_EmailMkt"));

             List<Dictionary<string, object>> id = DBQuery("Cliente_Insert2", parametros, CommandType.StoredProcedure);

             if (id.Count > 0)
             {
                 if (cliente["Telefones"] != null)
                 {
                     foreach (JObject telefone in cliente["Telefones"])
                     {
                         parametros = new Dictionary<string, object>();
                         parametros.Add("ID_Cliente", id[0]["ID_Cliente"]);
                         parametros.Add("Telefone", telefone["Telefone"].ToString());
                         parametros.Add("ID_Tipo", ToDBNull(telefone, "Tipo", "ID_Tipo"));
                         parametros.Add("ID_Pais", ToDBNull(telefone, "Pais", "ID_Pais"));

                         DBQuery("Cliente_Telefone_Insert", parametros, CommandType.StoredProcedure);
                     }
                     if (cliente["Campos"] != null && cliente["Campos"].ToString() != "")
                     {
                         CamposUpdate(ID_Usuario, ID_ClientePloomes, id[0]["ID_Cliente"], (JArray)cliente["Campos"]);
                     }
                 }
                 if (cliente["Marcadores"] != null)
                 {

                     JArray marcadores = (JArray)cliente["Marcadores"];
                     for (int i = 0; i < marcadores.Count; i++)
                     {
                         JObject marcador = (JObject)marcadores[i];
                         parametros = new Dictionary<string, object>();
                         parametros.Add("ID_Usuario", ID_Usuario);
                         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
                         parametros.Add("ID_Marcador", Convert.ToInt32(marcador["ID_Marcador"]));
                         parametros.Add("ID_Item", id[0]["ID_Cliente"]);
                         parametros.Add("ID_TipoItem", "1");
                         parametros.Add("Marcador", marcador["Marcador"].ToString());
                         DBQuery("Marcador_Item_Insert2", parametros, System.Data.CommandType.StoredProcedure);
                     }
                 }
             }

             return id.Count > 0 && id[0]["ID_Cliente"] != DBNull.Value ? Get(uk, apvk, null, null, null, null, null, id[0]["ID_Cliente"], null, null, null, null, null, null, null, null, null, true, true, true, true) : null;
         }
         catch (Exception)
         {
             return null;
         }
     }

     public List<Dictionary<string, object>> Put (int id, string uk, string apvk, JObject cliente)
     {
         object ID_Usuario = Authenticate(uk, "uk");
         object ID_ClientePloomes = Authenticate(apvk, "apvk");
         string sql = "Cliente_Update2";
         Dictionary<string, object> parametros = new Dictionary<string, object>();
         parametros.Add("ID_Usuario", ID_Usuario);
         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
         parametros.Add("ID_Cliente", id);
         parametros.Add("ID_Empresa", ToDBNull(cliente, "Cliente", "ID_Empresa"));
         parametros.Add("Nome", ToDBNull(cliente, "Nome"));
         parametros.Add("RazaoSocial", ToDBNull(cliente, "RazaoSocial"));
         parametros.Add("CNPJ", ToDBNull(cliente, "CNPJ"));
         parametros.Add("ID_Segmento", ToDBNull(cliente, "Segmento", "ID_Segmento"));
         parametros.Add("Segmento", ToDBNull(cliente, "Segmento", "Segmento"));
         parametros.Add("ID_Responsavel", ToDBNull(cliente, "Responsavel", "ID_Responsavel"));
         parametros.Add("ID_Relacao", ToDBNull(cliente, "Relacao", "ID_Relacao"));
         parametros.Add("ID_Origem", ToDBNull(cliente, "Origem", "ID_Origem"));
         parametros.Add("Origem", ToDBNull(cliente, "Origem", "Origem"));
         parametros.Add("ID_Cargo", ToDBNull(cliente, "Cargo", "ID_Cargo"));
         parametros.Add("Cargo", ToDBNull(cliente, "Cargo", "Cargo"));
         parametros.Add("ID_Departamento", ToDBNull(cliente, "Departamento", "ID_Departamento"));
         parametros.Add("Departamento", ToDBNull(cliente, "Departamento", "Departamento"));
         parametros.Add("DataFundacao", ToDBNull(cliente, "DataFundacao"));
         parametros.Add("ID_QtdFuncionarios", ToDBNull(cliente, "QtdFuncionarios", "ID_QtdFuncionarios"));
         parametros.Add("QtdFuncionarios", ToDBNull(cliente, "QtdFuncionarios", "QtdFuncionarios"));
         parametros.Add("Facebook", ToDBNull(cliente, "Facebook"));
         parametros.Add("Obs", ToDBNull(cliente, "Obs"));
         parametros.Add("Email", ToDBNull(cliente, "Email"));
         parametros.Add("Site", ToDBNull(cliente, "Site"));
         parametros.Add("Skype", ToDBNull(cliente, "Skype"));
         parametros.Add("ID_Cidade", ToDBNull(cliente, "Cidade", "ID_Cidade"));
         parametros.Add("Cidade", ToDBNull(cliente, "Cidade", "Cidade"));
         parametros.Add("ID_Pais", ToDBNull(cliente, "Pais", "ID_Pais"));
         parametros.Add("ID_UF", ToDBNull(cliente, "UF", "ID_UF"));
         parametros.Add("Endereco", ToDBNull(cliente, "Endereco"));
         parametros.Add("Complemento", ToDBNull(cliente, "Complemento"));
         parametros.Add("Bairro", ToDBNull(cliente, "Bairro"));
         parametros.Add("CEP", ToDBNull(cliente, "CEP"));
         parametros.Add("ID_Moeda", ToDBNull(cliente, "Moeda", "ID_Moeda"));
         object latitude = ToDBNull(cliente, "Localizacao", "Lat");
         if (latitude != DBNull.Value) latitude = Convert.ToDouble(latitude);
         object longitude = ToDBNull(cliente, "Localizacao", "Lng");
         if (longitude != DBNull.Value) longitude = Convert.ToDouble(longitude);
         parametros.Add("Latitude", latitude);
         parametros.Add("Longitude", longitude);
         parametros.Add("Recebe_EmailMkt", ToDBNull(cliente, "Recebe_EmailMkt"));

         DBQuery(sql, parametros, System.Data.CommandType.StoredProcedure);

         sql = "DELETE Marcador_Item FROM Marcador_Item INNER JOIN Marcador M ON Marcador_Item.ID_Marcador = M.ID INNER JOIN Vw_Cliente C ON Marcador_Item.ID_Item = C.ID INNER JOIN Usuario U ON C.ID_Usuario = U.ID AND C.Edita = 'True' AND C.Suspenso = 'False' AND C.ID = @ID_Cliente WHERE U.ID = ISNULL(@ID_Usuario, (SELECT ID_Criador FROM Ploomes_Cliente WHERE ID = @ID_ClientePloomes)) AND M.ID_TipoItem = 1";
         parametros = new Dictionary<string, object>();
         parametros.Add("ID_Usuario", ID_Usuario);
         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
         parametros.Add("ID_Cliente", id);
         DBQuery(sql, parametros, System.Data.CommandType.Text);

         sql = "DELETE Cliente_Telefone FROM Cliente_Telefone INNER JOIN Vw_Cliente C ON Cliente_Telefone.ID_Cliente = C.ID INNER JOIN Usuario U ON C.ID_Usuario = U.ID AND C.Edita = 'True' AND C.Suspenso = 'False' AND C.ID = @ID_Cliente WHERE U.ID = ISNULL(@ID_Usuario, (SELECT ID_Criador FROM Ploomes_Cliente WHERE ID = @ID_ClientePloomes))";
         parametros = new Dictionary<string, object>();
         parametros.Add("ID_Usuario", ID_Usuario);
         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
         parametros.Add("ID_Cliente", id);
         DBQuery(sql, parametros, System.Data.CommandType.Text);

         sql = "DELETE Cliente_Empresa FROM Cliente_Empresa INNER JOIN Vw_Cliente C ON Cliente_Empresa.ID_Cliente = C.ID INNER JOIN Usuario U ON C.ID_Usuario = U.ID AND C.Edita = 'True' AND C.Suspenso = 'False' AND C.ID = @ID_Cliente WHERE U.ID = ISNULL(@ID_Usuario, (SELECT ID_Criador FROM Ploomes_Cliente WHERE ID = @ID_ClientePloomes))";
         parametros = new Dictionary<string, object>();
         parametros.Add("ID_Usuario", ID_Usuario);
         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
         parametros.Add("ID_Cliente", id);
         DBQuery(sql, parametros, System.Data.CommandType.Text);

         if (cliente["Marcadores"] != null)
         {
             sql = "Marcador_Item_Insert2";
             JArray marcadores = (JArray)cliente["Marcadores"];
             for (int i = 0; i < marcadores.Count; i++)
             {
                 JObject marcador = (JObject)marcadores[i];
                 parametros = new Dictionary<string, object>();
                 parametros.Add("ID_Usuario", ID_Usuario);
                 parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
                 parametros.Add("ID_Marcador", Convert.ToInt32(marcador["ID_Marcador"]));
                 parametros.Add("ID_Item", id);
                 parametros.Add("ID_TipoItem", "1");
                 parametros.Add("Marcador", marcador["Marcador"].ToString());
                 DBQuery(sql, parametros, System.Data.CommandType.StoredProcedure);
             }
         }

         if (cliente["Telefones"] != null)
         {
             sql = "INSERT INTO Cliente_Telefone (ID_Cliente, Telefone, ID_Tipo, ID_Pais) SELECT C.ID, @Telefone, TT.ID, P.ID FROM Usuario U INNER JOIN Vw_Cliente C ON U.ID = C.ID_Usuario INNER JOIN Telefone_Tipo TT ON TT.ID = @ID_Tipo LEFT JOIN Cidade_Pais P ON P.ID = @ID_Pais WHERE U.ID = ISNULL(@ID_Usuario, (SELECT ID_Criador FROM Ploomes_Cliente WHERE ID = @ID_ClientePloomes)) AND @Telefone IS NOT NULL AND C.ID = @ID_Cliente AND C.Suspenso = 'False' AND C.Edita = 'True'";
             JArray telefones = (JArray)cliente["Telefones"];
             for (int i = 0; i < telefones.Count; i++)
             {
                 JObject telefone = (JObject)telefones[i];
                 parametros = new Dictionary<string, object>();
                 parametros.Add("ID_Usuario", ID_Usuario);
                 parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
                 parametros.Add("ID_Cliente", id);
                 parametros.Add("Telefone", ToDBNull(telefone, "Telefone"));
                 parametros.Add("ID_Tipo", ToDBNull(telefone, "Tipo", "ID_Tipo"));
                 parametros.Add("ID_Pais", ToDBNull(telefone, "Pais", "ID_Pais"));
                 DBQuery(sql, parametros, System.Data.CommandType.Text);
             }
         }

         if (cliente["Campos"] != null && cliente["Campos"].ToString() != "")
         {
             CamposUpdate(ID_Usuario, ID_ClientePloomes, id, (JArray)cliente["Campos"]);
         }

         return Get(uk, apvk, null, null, null, null, null, id, null, null, null, null, null, null, null, null, null, true, true, true, true);
     }

     public void Delete(int id, string uk, string apvk)
     {
         object ID_Usuario = Authenticate(uk, "uk");
         object ID_ClientePloomes = Authenticate(apvk, "apvk");
         Dictionary<string, object> parametros = new Dictionary<string, object>();
         parametros.Add("ID_Usuario", ID_Usuario);
         parametros.Add("ID_ClientePloomes", ID_ClientePloomes);
         parametros.Add("ID_Cliente", id);

         List<Dictionary<string, object>> resultado = DBQuery("Cliente_Delete", parametros, CommandType.StoredProcedure);

         if (resultado.Count == 0 || Convert.ToInt32(resultado[0]["Rows"]) == 0)
         {
             throw new Exception("Não excluiu");
         }

     }*/
}