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
                url = "http://helpgv.azurewebsites.net/a.jpg"
            },
            new Document()
            {
                names = new List<string> { "geburtsurkunde", "birth"},
                  attributes = new Dictionary<string, int>{ { "not found", -1 } },
                url = "http://jump4u.at/Dokumente/Geburtsurkunde.jpg"
            },
                 new Document()
            {
                names = new List<string> { "sterbeurkunde", "death"},
                           attributes = new Dictionary<string, int>(),
                url = "https://de.wikipedia.org/wiki/Sterbeurkunde#/media/File:Sterbeurkunde_muster.jpg"
            },
                         new Document()
            {
                names = new List<string> { "resiepass", "travel drocument", "passport", "pass" },
                           attributes = new Dictionary<string, int>(),
                url = "http://blog.diakonie.at/sites/default/files/blog/bild/reisepass_anonym_0.jpg"
            },
        new Document()
            {
                names = new List<string> { "personalausweis", "identiy card", "id", "identity"},
                           attributes = new Dictionary<string, int>(),
                url = "http://www.lustenau.at/website/uploads/images/01/660x/personalausweis_austria.jpg?v=1"
            },
                
        };
            return docs;

        }
    }





}