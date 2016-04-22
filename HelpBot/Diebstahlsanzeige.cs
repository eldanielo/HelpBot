using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace HelpBot
{
    [Template(TemplateUsage.NotUnderstood, "Ich habe \"{0}\" nicht verstqanden", "Versuchen Sie es erneut, Ich habe \"{0}\" nicht verstanden.")]
    [Serializable]
    public class Diebstahlsanzeige
    {
        public static IForm<Diebstahlsanzeige> BuildForm()
        {

            CompletionDelegate<Diebstahlsanzeige> processOrder = async (context, state) =>
            {

                await context.PostAsync("Ihre Anzeige wurde aufgenommen und die Ermittlungen haben begonnen");
            };
            return new FormBuilder<Diebstahlsanzeige>()
                    .Message("Um Ihren Fall bestmöglich behandeln zu können geben Sie uns bitte folgende Informationen")
                    .Build();
        }
        public Person person;
        public Tat tat;
       
        [Serializable]
        public class Person
        {
            [Prompt("Was ist Ihr {&}? ")]
            public String Vorname { get; set; }
            [Prompt("Was ist Ihr {&}? ")]
            public String Nachname { get; set; }
            public enum geschlecht { Männlich, Weiblich };
            [Prompt("Was ist Ihr {&}? {||}")]
            public geschlecht? Geschlecht { get; set; }
            [Numeric(0,double.MaxValue)]    
            [Prompt("Was ist Ihre {&}? ")]
            public string Telefonnummerr { get; set; }

            public Adresse Adresse { get; set; }

        }
        [Serializable]
        public class Adresse
        {
            [Prompt("In welcher Straße ist die Tat geschehen ")]
            public string Straße { get; set; }
            [Optional]
            [Numeric(0,double.MaxValue)]
            [Prompt("Was ist die Türnummer? ")]
            public string Türnummer { get; set; }
            [Prompt("In welcher {&} ist die Tat geschehen? ")]
            public string Stadt { get; set; }
            [Numeric(0, double.MaxValue)]
            [Prompt("Was ist die Postleitzahl? ")]
            public string Zip { get; set; }
        }
        [Serializable]
        public class Tat
        {
            [Prompt("Wann ist die Tat geschehen? ")]
            [Template(TemplateUsage.Help, "Geben Sie das Datum in der Form 1.1.2016 ein")]
            public DateTime Tatzeit { get; set; }
            [Prompt("Was wurde gestohlen? ")]
            public string GestohlenesGut { get; set; }
            [Optional]
            [Prompt("Beschreiben Sie den Tatort. ")]
            public string Bemerkung_Tatort { get; set; }
            [Optional]
            [Template(TemplateUsage.Help, "Erinnern Sie sich an Details? Wie viele Täter? Sahen Sie ein Fahrzeug?")]
            [Prompt("Beschreiben Sie die Tat. ")]
            public string Bemerkung_Tat { get; set; }
            [Optional]
            [Prompt("Beschreiben Sie den Täter ")]
            [Template(TemplateUsage.Help, "Erinnern Sie sich an irgendwelche Merkmale des Täters? Größe, Herkunft, Haarfarbe, Tattoos, etc..")]
            public string Bemerkung_Täter { get; set; }
            [Optional]
            [Prompt("Gibt es (&)?")]
            public List<Person> Zeugen { get; set; }
        }

    }
  
}