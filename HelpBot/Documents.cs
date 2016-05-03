using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpBot
{
    public class Document
    {

        public List<String> names;
        public Dictionary<string, int> attributes;
        public string url;
        public string info; 

        public static List<Document> documents()
        {
            List<Document> docs = new List<Document>() {
            new Document()
            {
                names = new List<string> { "führerschein", "drivers license", "drivers", "license", "driver" },
                attributes = new Dictionary<string, int> {
                 {"name", 2 },
                { "firstname",3 },
                { "birthday", 4},
                {  "date", 5},
                { "valid", 5},
                {  "authority", 6},
                {"number",  7},
                },
                url =  "https://upload.wikimedia.org/wikipedia/commons/3/3f/A_Licence_2013_Front.jpg",
                info = "https://www.help.gv.at/Portal.Node/hlpd/public/content/4/Seite.040000.html"
            },
            new Document()
            {
                names = new List<string> { "geburtsurkunde", "birth"},
                  attributes = new Dictionary<string, int>{ { "not found", -1 } },
                url = "http://jump4u.at/Dokumente/Geburtsurkunde.jpg",
                info = "https://www.help.gv.at/Portal.Node/hlpd/public/content/8/Seite.085100.html"
            },
                 new Document()
            {
                names = new List<string> { "sterbeurkunde", "death"},
                           attributes = new Dictionary<string, int>(),
                url = "https://de.wikipedia.org/wiki/Sterbeurkunde#/media/File:Sterbeurkunde_muster.jpg",
                info ="https://www.help.gv.at/Portal.Node/hlpd/public/content/3/Seite.030000.html"
            },
                         new Document()
            {
                names = new List<string> { "reisepass", "travel drocument", "passport", "pass" },
                           attributes = new Dictionary<string, int>(),
                url = "http://blog.diakonie.at/sites/default/files/blog/bild/reisepass_anonym_0.jpg",
                info ="https://www.help.gv.at/Portal.Node/hlpd/public/content/2/Seite.020000.html"
            },
        new Document()
            {
                names = new List<string> { "personalausweis", "identiy card", "id", "identity"},
                           attributes = new Dictionary<string, int>(),
                url = "http://www.lustenau.at/website/uploads/images/01/660x/personalausweis_austria.jpg?v=1",
                info="https://www.help.gv.at/Portal.Node/hlpd/public/content/3/Seite.030000.html"
            },
                
        };
            return docs;

        }
    }





}