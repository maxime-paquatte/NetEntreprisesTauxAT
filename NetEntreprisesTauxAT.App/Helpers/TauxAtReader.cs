using System.Xml.Linq;
using System.Xml.XPath;

namespace NetEntreprisesTauxAT.App.Helpers;

public static class TauxAtReader
{
    public const string TAUX_AT_TYPE = "atmp";
    
    public static IEnumerable<Data> GetData(string file)
    {
        var root = XElement.Load(file);
        var siren = root.XPathSelectElement("./declaration/declaration_identification/SIREN")?.Value;
        var etat = root.XPathSelectElement("./declaration/declaration_bilan/etat")?.Value;
        
        var infoCollective = root.XPathSelectElement($"./communication/information_collective[@type='{TAUX_AT_TYPE}']");
        if(infoCollective is null) yield break;
        
        foreach (var calculEl in infoCollective.Elements("calcul"))
        {
            foreach (var informationEl in calculEl.Elements("information"))
            {
                var data = new Data()
                {
                    FileName = Path.GetFileName(file)
                };
                data.Siren = siren;
                data.Etat = etat;
                data.DateCalcul = calculEl.Attribute("date_calcul")?.Value;
                data.CodeCtn = informationEl.XPathSelectElement("./donnees_declaration/rubriques/rubrique_string[@name='codeCtn']")?.Value;
                data.CodeRisque = informationEl.XPathSelectElement("./donnees_declaration/rubriques/rubrique_string[@name='codeRisque']")?.Value;
                data.TemoinBureau = informationEl.XPathSelectElement("./donnees_declaration/rubriques/rubrique_string[@name='temoinBureau']")?.Value;
                data.Section = informationEl.XPathSelectElement("./donnees_clefs/rubriques/rubrique_string[@name='section']")?.Value;
                data.DateDebutEffet = informationEl.XPathSelectElement("./donnees_clefs/rubriques/rubrique_date[@name='dateDebutEffet']")?.Value;
                data.Taux = informationEl.XPathSelectElement("./donnees_paie/taux")?.Value;

                yield return data;
            }
        }
    }
}

public class Data
{
    public required string FileName { get; set; }
    public string? Siren { get; set; }
    public string? Etat { get; set; }
    
    public string? CodeCtn { get; set; }
    public string? CodeRisque { get; set; }
    public string? TemoinBureau { get; set; }
    public string? Section { get; set; }
    public string? DateDebutEffet { get; set; }
    public string? DateCalcul { get; set; }
    public string? Taux { get; set; }
}


