using Simulware.Barcode;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Web;

class BarcodeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string tipo = context.Request.QueryString["Tipo"];
        string idUser = context.Request.QueryString["Id_User"];
        string idClasse = context.Request.QueryString["Id_Classe"];
        string label = context.Request.QueryString["Label"];

        var data = new Input();
        data.WriteOnDb(Convert.ToInt32(tipo), Convert.ToInt32(idUser), Convert.ToInt32(idClasse), label);
        /*var dmImage = FinalDm(urlContent);
        context.Response.ContentType = "image/png";
        dmImage.Save(context.Response.OutputStream, ImageFormat.Png);*/
    }

    public bool IsReusable { get; }

    public Bitmap FinalDm(string data)
    {
        Signature s = new Signature();
        string hexString = "Test";
        try
        {
            byte[] sign = s.CreateSignature(data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in sign)
            {
                sb.Append(b.ToString("X2"));
            }

            hexString = sb.ToString();
            Console.WriteLine("Signature len:" + sign.Length);
            Console.WriteLine("Signature dump " + hexString);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.ToString());
        }
        Simulware.Barcode.DataMatrix dm = new Simulware.Barcode.DataMatrix();
        return dm.GenMatrix(hexString);
    }
    
}
