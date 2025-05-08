using System.Collections;
using System.Xml;

namespace NetEntreprisesTauxAT.App.Helpers;

public static class ExceptionHelper
{
    public static bool Save(this Exception ex, string fileName)
    {
        try
        {
            File.WriteAllText(fileName, ex.GetLog());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static string GetLog(this Exception ex)
    {
        return GetXmlString(DateTime.Now, ex.Message, xw => WriteException(xw, "exception", ex));
    }

    static string GetXmlString(DateTime d, string message, Action<XmlWriter> action)
    {
        StringWriter sw = new StringWriter();
        using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true }))
        {
            xw.WriteStartElement("Log");
            xw.WriteAttributeString("date", d.ToString("s"));
            xw.WriteElementString("Message", message);
            action(xw);
            xw.WriteEndElement();
        }

        return sw.ToString();
    }

    static void WriteException(XmlWriter writer, string name, Exception exception)
    {
        writer.WriteStartElement(name);
        writer.WriteElementString("Message", exception.Message);
        writer.WriteElementString("Type", exception.GetType().Name);

        WriteData(writer, exception.Data);

        writer.WriteStartElement("StackTrace");
        writer.WriteCData(exception.StackTrace);
        writer.WriteEndElement();

        if (exception.InnerException != null)
            WriteException(writer, "InnerException", exception.InnerException);
        writer.WriteEndElement();
    }

    static void WriteData(XmlWriter writer, IDictionary values)
    {
        writer.WriteStartElement("Datas");
        foreach (DictionaryEntry data in values)
        {
            writer.WriteStartElement("Data");
            writer.WriteAttributeString("key", data.Key as string ?? "NoKey");
            writer.WriteCData(Convert.ToString(data.Value) ?? "NoValue");
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
}