using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Web.Security;
using System.Drawing.Drawing2D;
using Newtonsoft.Json.Linq;

//Classe do sistema Ploomes CRM
public class PloomesCRM : ApiController
{
    protected SqlConnection conn;

    //########## CONSTRUTOR ##########
    public PloomesCRM()
    {
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PloomesCRM"].ConnectionString);
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
            return o.ToString() == "" ? DBNull.Value : ((JToken)o).ToObject<object>();
        }
        return o != null && o.ToString() != "" ? o : DBNull.Value;
    }

    //########## GETQUERYSTRING ##########
    //Traz o valor de uma determinada querystring
    protected string GetQueryString(HttpControllerContext context, string keyType)
    {
        IEnumerable<KeyValuePair<string, string>> queryString = context.Request.GetQueryNameValuePairs();

        foreach (KeyValuePair<string, string> k in queryString)
        {
            if (k.Key == keyType)
            {
                return k.Value;
            }
        }
        return null;
    }

    //########## GETFILTROSDINAMICOS ##########
    //Extrai os filtros dinâmicos da query string para envio ao SQL Server no formato de DataTable
    protected DataTable GetFiltrosDinamicos(HttpControllerContext context)
    {
        DataTable tb = new DataTable();
        tb.Columns.Add("ID_Campo");
        tb.Columns.Add("Valor");

        IEnumerable<KeyValuePair<string, string>> queryString = context.Request.GetQueryNameValuePairs();

        foreach (KeyValuePair<string, string> k in queryString)
        {
            try
            {
                DataRow r = tb.NewRow();
                r["ID_Campo"] = int.Parse(k.Key);
                string valor = k.Value;
                if (valor.ToLower() == "true") valor = "1";
                else if (valor.ToLower() == "false") valor = "0";
                r["Valor"] = valor;
                tb.Rows.Add(r);
            }
            catch (Exception)
            {
            }
        }
        return tb;
    }

    //########## GETMARCADORES ##########
    //Extrai os marcadores da query string para envio ao SQL Server no formato de DataTable
    protected DataTable GetMarcadores(HttpControllerContext context)
    {
        DataTable tb = new DataTable();
        tb.Columns.Add("ID_Marcador");

        IEnumerable<KeyValuePair<string, string>> queryString = context.Request.GetQueryNameValuePairs();

        string marcadoresIdsCsv = GetQueryString(context, "marcadores");
        if(marcadoresIdsCsv != null && marcadoresIdsCsv != "")
        {
            marcadoresIdsCsv = marcadoresIdsCsv.TrimEnd(',');
            int[] marcadoresIds = Array.ConvertAll(marcadoresIdsCsv.Split(','), int.Parse);
            foreach (int marcadorId in marcadoresIds)
            {
                try
                {
                    DataRow r = tb.NewRow();
                    r["ID_Marcador"] = marcadorId;
                    tb.Rows.Add(r);
                }
                catch (Exception)
                {
                }
            }
        }

        return tb;
    }

 
    //########## SELECTEDFIELDS ##########
    //Exibe somente os campos especificados via query string para a lista de resultados em questão
    protected List<Dictionary<string, object>> SelectedFields(List<Dictionary<string, object>> dataSet, HttpControllerContext context)
    {
        List<string> fields = new List<string>();
        IEnumerable<KeyValuePair<string, string>> queryString = context.Request.GetQueryNameValuePairs();
        List<Dictionary<string, object>> newDataSet = new List<Dictionary<string, object>>();

        foreach (KeyValuePair<string, string> k in queryString)
        {
            if (k.Key == "fields")
            {
                fields = k.Value.Split(',').ToList<string>();
            }
        }

        for (int i = 0; i < dataSet.Count; i++)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            foreach (string field in fields)
            {
                if (dataSet[i].ContainsKey(field))
                    row.Add(field, dataSet[i][field]);
            }
            newDataSet.Add(row);
        }

        return newDataSet.Count > 0 && newDataSet[0].Keys.Count > 0 ? newDataSet : dataSet;
    }

    //IMAGEUPLOAD
    //Método básico para upload de imagens
    protected object ImageUpload(int id_clienteploomes, string pasta, int maxlength, double maxratio, int maxheight, int maxwidth)
    {
        HttpPostedFile file = HttpContext.Current.Request.Files[0];
        if (file.ContentType.StartsWith("image/") && file.ContentLength <= maxlength)
        {
            try
            {
                //###### ALTA RESOLUÇÃO
                // Create a bitmap of the content of the fileUpload control in memory
                Bitmap originalBMP = new Bitmap(file.InputStream);

                // Calculate the new image dimensions
                int origWidth = originalBMP.Width;
                int origHeight = originalBMP.Height;
                double sngRatio = (double)origWidth / (double)origHeight;

                // Create a new bitmap which will hold the previous resized bitmap
                Bitmap newBMP = new Bitmap(originalBMP, origWidth, origHeight);

                // Create a graphic based on the new bitmap
                Graphics oGraphics = Graphics.FromImage(newBMP);

                // Set the properties for the new graphic file
                oGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw the new graphic based on the resized bitmap

                oGraphics.DrawImage(originalBMP, 0, 0, origWidth, origHeight);
                Rectangle section = new Rectangle(new Point(0, 0), new Size(sngRatio > maxratio ? origHeight * (int)maxratio : origWidth, origHeight));
                oGraphics.DrawImage(newBMP, 0, 0, section, GraphicsUnit.Pixel);

                // Save the new graphic file to the server
                string img = FormsAuthentication.HashPasswordForStoringInConfigFile(file.FileName + DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString(), "SHA1");

                string caminho = "D:/Anexos/PloomesCRM/" + ConfigurationManager.AppSettings["Ambiente"] + "/" + id_clienteploomes + "/" + pasta + "/";

                //Cria a pasta do cliente caso não exista
                if (!Directory.Exists(caminho))
                    Directory.CreateDirectory(caminho);

                newBMP.Save(caminho + img + "_HD" + Path.GetExtension(file.FileName));

                //##### BAIXA RESOLUÇÃO
                int newHeight = maxheight;
                int newWidth = Convert.ToInt32(Math.Round((double)newHeight * sngRatio));

                // Create a new bitmap which will hold the previous resized bitmap
                newBMP = new Bitmap(originalBMP, newWidth, newHeight);

                // Create a graphic based on the new bitmap
                oGraphics = Graphics.FromImage(newBMP);

                // Set the properties for the new graphic file
                oGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw the new graphic based on the resized bitmap

                oGraphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                section = new Rectangle(new Point(0, 0), new Size(newWidth > maxwidth ? maxwidth : newWidth, newHeight));
                oGraphics.DrawImage(newBMP, 0, 0, section, GraphicsUnit.Pixel);

                newBMP.Save(caminho + img + Path.GetExtension(file.FileName));

                // Once finished with the bitmap objects, we deallocate them.
                originalBMP.Dispose();
                newBMP.Dispose();
                oGraphics.Dispose();

                HttpContext.Current.Response.Write(img + Path.GetExtension(file.FileName));

                return img + Path.GetExtension(file.FileName);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                return "err";
            }
        }
        else
        {
            HttpContext.Current.Response.Write("err");
            return "err";
        }
    }

    //FILEDOWNLOAD
    //Método básico para download de arquivos
    protected void FileDownload(string caminho, string arquivo)
    {
        if (File.Exists(@caminho + arquivo))
        {
            HttpContext.Current.Response.AddHeader("Content-Disposition", "filename=" + arquivo);
            HttpContext.Current.Response.AddHeader("Content-Type", "image/png");
            HttpContext.Current.Response.TransmitFile(@caminho + arquivo);
            HttpContext.Current.Response.End();
        }
    }

}