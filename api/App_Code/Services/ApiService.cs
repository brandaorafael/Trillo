using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

public abstract class ApiService
{
    protected SqlConnection conn;
    private CultureInfo cult;

    public ApiService()
    {
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PloomesCRM"].ConnectionString);
        cult = CultureInfo.GetCultureInfo("pt-BR");
    }

    //########## DBQUERY ##########
    //Método para trazer resultados do banco de dados
    protected List<Dictionary<string, object>> DBQuery(string sql, Dictionary<string, object> parametros, System.Data.CommandType tipo)
    {
        List<Dictionary<string, object>> saidas = new List<Dictionary<string, object>>();

        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.CommandType = tipo;

        foreach (KeyValuePair<string, object> entry in parametros)
        {
            cmd.Parameters.AddWithValue(entry.Key, entry.Value);
        }

        conn.Open();

        SqlDataReader rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            Dictionary<string, object> saida = new Dictionary<string, object>();
            for (int i = 0; i < rd.FieldCount; i++)
            {
                saida.Add(rd.GetName(i), rd[i]);
            }
            saidas.Add(saida);
        }
        rd.Close();
        conn.Close();

        return saidas;
    }

    //########## DBQUERY - COM JSON OBJ ##########
    //Método para trazer resultados do banco de dados com mapeamento de objetos JSON
    protected List<Dictionary<string, object>> DBQuery(string sql, Dictionary<string, object> parametros, System.Data.CommandType tipo, Dictionary<string, string> objetos)
    {
        List<Dictionary<string, object>> saidas = new List<Dictionary<string, object>>();

        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.CommandType = tipo;

        foreach (KeyValuePair<string, object> entry in parametros)
        {
            cmd.Parameters.AddWithValue(entry.Key, entry.Value);
        }

        conn.Open();

        SqlDataReader rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            Dictionary<string, object> saida = new Dictionary<string, object>();
            for (int i = 0; i < rd.FieldCount; i++)
            {
                if (objetos.ContainsKey(rd.GetName(i)))
                {
                    if (saida.ContainsKey(objetos[rd.GetName(i)]))
                        (saida[objetos[rd.GetName(i)]] as Dictionary<string, object>).Add(rd.GetName(i), rd[i]);
                    else
                    {
                        Dictionary<string, object> item = new Dictionary<string, object>();
                        item.Add(rd.GetName(i), rd[i]);
                        saida.Add(objetos[rd.GetName(i)], item);
                    }
                }
                else
                    saida.Add(rd.GetName(i), rd[i]);
            }
            saidas.Add(saida);
        }
        rd.Close();
        conn.Close();

        return saidas;
    }

    //########## TODBNULL ##########
    //Transforma o objeto em DBNull caso seja nulo
    protected object ToDBNull(object o)
    {
        if (o is JToken)
        {
            return o.ToString() != "" ? ((JToken)o).ToObject<object>() : DBNull.Value;
        }
        return o != null && o.ToString() != "" ? o : DBNull.Value;
    }

    //########## TODBNULL ##########
    //Transforma o objeto em DBNull caso seja nulo
    protected object ToDBNull(object o, string child)
    {
        if (o is JObject)
        {
            return (o as JObject)[child] != null && (o as JObject)[child].ToString() != "" ? ((o as JObject)[child] as JToken).ToObject<object>() : DBNull.Value;
        }
        else if (o is Dictionary<string, object>)
        {
            Dictionary<string, object> a = o as Dictionary<string, object>;
            return a.ContainsKey(child) ? (a[child] != null ? a[child] : DBNull.Value) : DBNull.Value;
        }
        return o != null && o.ToString() != "" ? o : DBNull.Value;
    }

    //########## TODBNULL ##########
    //Transforma o objeto em DBNull caso seja nulo
    protected object ToDBNull(JObject o, string child, string grandchild)
    {
        return o != null ? o[child] != null && o[child] is JObject ? o[child][grandchild] != null ? o[child][grandchild].ToObject<object>() != null ? o[child][grandchild].ToObject<object>() : DBNull.Value : DBNull.Value : DBNull.Value : DBNull.Value;
    }

    //########## TODBNULL ##########
    //Transforma o objeto em DBNull caso seja nulo
    protected object ToDBNull(Dictionary<string, object> o, string child, string grandchild)
    {
        if (!o.ContainsKey(child)) return DBNull.Value;
        object childObj = o[child];
        if (childObj is Dictionary<string, object> && (childObj as Dictionary<string, object>).ContainsKey(grandchild))
        {
            return ToDBNull((childObj as Dictionary<string, object>), grandchild);
        }
        else if (childObj is JObject)
        {
            return ToDBNull(childObj, grandchild);
        }
        else return DBNull.Value;
    }

    //########## PAGINACAO ##########
    //Filtra os dados por paginação
    protected List<Dictionary<string, object>> Paginacao(List<Dictionary<string, object>> dados, string page_init, string page_end)
    {
        if (page_init != null || page_end != null)
        {
            int page_init_int, page_end_int;
            int.TryParse(page_init, out page_init_int);
            int.TryParse(page_end, out page_end_int);

            page_init_int = page_init_int == 0 || page_init_int > dados.Count ? 1 : page_init_int;
            page_end_int = page_end_int == 0 || page_end_int > dados.Count ? dados.Count : page_end_int;

            dados = dados.GetRange(page_init_int - 1, page_end_int - page_init_int + 1);
        }

        if (dados.Count > 0)
            dados[0].Add("TotalLinhas", dados.Count);

        return dados;
    }

    public bool FileDownload(string caminho, string arquivo, bool imagem)
    {
        bool exists;
        if (exists = File.Exists(@caminho + arquivo))
        {
            HttpContext.Current.Response.AddHeader("Content-Disposition", "filename=" + arquivo);
            if (imagem)
                HttpContext.Current.Response.AddHeader("Content-Type", "image/png");
            HttpContext.Current.Response.TransmitFile(@caminho + arquivo);
            HttpContext.Current.Response.End();
        }

        return exists;
    }

    public bool FileUpload(string caminho, string nomeBaseArquivo, HttpPostedFile arquivo)
    {
        if (arquivo.ContentLength > 10485760) return false;
        try
        {
            //Cria a pasta do cliente caso não exista
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            arquivo.SaveAs(caminho + nomeBaseArquivo + Path.GetExtension(arquivo.FileName));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}