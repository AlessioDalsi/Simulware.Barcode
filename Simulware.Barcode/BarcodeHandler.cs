using Simulware.Barcode;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using Newtonsoft.Json;

class BarcodeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var data = new DBComm();
            JsonSerializer serializer = new JsonSerializer();
            string requestName = context.Request.Url.LocalPath;
            switch (requestName)
            {
                case "/Barcode/newSerial":
                    string tipo = context.Request.QueryString["Tipo"];
                    string idUser = context.Request.QueryString["IdUser"];
                    string idClasse = context.Request.QueryString["IdClasse"];
                    string label = context.Request.QueryString["Label"];

                    data.WriteOnDb(tipo, Convert.ToInt32(idUser), Convert.ToInt32(idClasse), label);
                    var jsoncontent = new
                    {
                        StatusCode = context.Response.StatusCode,
                        StatusMessage = "OK",
                        Serial = data.ReadFromDb(tipo, Convert.ToInt32(idUser), Convert.ToInt32(idClasse), label)
                    };
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(jsoncontent));
                    break;

                case "/Barcode/GetDM":
                    string serial = context.Request.QueryString["Serial"];
                    var Row = data.ReadFromDb(Convert.ToInt32(serial));
                    byte[] labelBytes = Encoding.BigEndianUnicode.GetBytes(Row.Item1.PadRight(150));
                    byte[] corsoBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Row.Item2));
                    byte[] userBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Row.Item3));
                    byte[] timeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Row.Item4));
                    byte[] serialBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Row.Item5));
                    byte[] typeBytes = Encoding.BigEndianUnicode.GetBytes(Row.Item6.PadRight(150));

                    byte[] content = MergeArrays(labelBytes, corsoBytes, userBytes, timeBytes, serialBytes, typeBytes);

                    var dmImage = FinalDm(content);
                    context.Response.ContentType = "image/png";
                    dmImage.Save(context.Response.OutputStream, ImageFormat.Png);
                    ////per le stringhe
                    //Encoding.UTF8.GetBytes()
                    ////per gli int
                    //BitConverter.GetBytes()
                    ////trasformazione in big endian
                    //IPAddress.HostToNetworkOrder()
                    context.Response.Write(content);
                    break;

                case "/Barcode/ReportData":
                    string serialRd = context.Request.QueryString["Serial"];
                    var rowRd = data.ReadFromDb(Convert.ToInt32(serialRd));
                    var jsonContentRd = new
                    {
                        label = rowRd.Item1,
                        corso = rowRd.Item2,
                        user = rowRd.Item3,
                        time = rowRd.Item4,
                        serial = rowRd.Item5,
                        type = rowRd.Item6
                    };
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(jsonContentRd));
                    break;

                default:
                    if (requestName != "/Barcode/GetDM" || requestName != "/Barcode/GetDM" || requestName!="/Barcode/ReportData")
                    {
                        context.Response.Write("Errore metodi richiamabili: <br>");
                        context.Response.Write("newSerial?Tipo=stringa&IdUser=intero&IdClasse=intero&Label=stringa <br>");
                        context.Response.Write("GetDM?Serial=intero <br>");
                        context.Response.Write("/Barcode/ReportData?Serial=intero");
                    }

                    break;
            }
        }
        catch (Exception)
        {
            context.Response.Write("Errore: " + context.Response.StatusCode);
        }

    }

    public bool IsReusable { get; }

    public byte[] MergeArrays(byte[] array1, byte[] array2, byte[] array3, byte[] array4, byte[] array5, byte[] array6)
    {
        byte[] ret = new byte[array1.Length + array2.Length + array3.Length + array4.Length + array5.Length + array6.Length];
        Buffer.BlockCopy(array1, 0, ret, 0, array1.Length);
        Buffer.BlockCopy(array2, 0, ret, array1.Length, array2.Length);
        Buffer.BlockCopy(array3, 0, ret, array1.Length + array2.Length, array3.Length);
        Buffer.BlockCopy(array4, 0, ret, array1.Length + array2.Length + array3.Length, array4.Length);
        Buffer.BlockCopy(array5, 0, ret, array1.Length + array2.Length + array3.Length + array4.Length, array5.Length);
        Buffer.BlockCopy(array6, 0, ret, array1.Length + array2.Length + array3.Length + array4.Length + array5.Length, array6.Length);
        return ret;
    }

    public Bitmap FinalDm(byte[] data)
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
