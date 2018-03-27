﻿using Simulware.Barcode;
using System;
using System.Drawing;
using System.Text;
using System.Web;
using Newtonsoft.Json;

class BarcodeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var data = new DBComm();
        JsonSerializer serializer = new JsonSerializer();
        string requestName = context.Request.Url.LocalPath;
        if (requestName == "/Barcode/newSerial")
        {
            string tipo = context.Request.QueryString["Tipo"];
            string idUser = context.Request.QueryString["IdUser"];
            string idClasse = context.Request.QueryString["IdClasse"];
            string label = context.Request.QueryString["Label"];
            
            data.WriteOnDb(tipo, Convert.ToInt32(idUser), Convert.ToInt32(idClasse), label);
            var jsoncontent = new
            { Serial = data.ReadFromDb(tipo, Convert.ToInt32(idUser), Convert.ToInt32(idClasse), label) };
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(jsoncontent));
        }
        else if (requestName == "/Barcode/GetDM")
        {
            string serial = context.Request.QueryString["Serial"];

            context.Response.Write(data.ReadFromDb(Convert.ToInt32(serial)));
        }

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
