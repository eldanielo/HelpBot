﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace HelpBot
{
    [Template(TemplateUsage.NotUnderstood, "Ich habe \"{0}\" nicht verstanden", "Versuchen Sie es erneut, Ich habe \"{0}\" nicht verstanden.")]
    [Serializable]
    public class Diebstahlsanzeige
    {
        public static IForm<Diebstahlsanzeige> BuildForm()
        {

    
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
            public String Name { get; set; }
   
         
            public Adresse Adresse { get; set; }

        }
        [Serializable]
        public class Adresse
        {
            [Prompt("Wo ist die Tat geschehen?")]
            public string Location { get; set; }
        }
        [Serializable]
        public class Tat
        {
            [Prompt("Wann ist die Tat geschehen? ")]
            [Template(TemplateUsage.Help, "Geben Sie das Datum in der Form 1.1.2016 ein")]
            public DateTime Tatzeit { get; set; }
            [Prompt("Was wurde gestohlen? ")]
            [Template(TemplateUsage.Help, "So genau wie möglich beschreiben und ungefähren Euro-Wert angeben!")]
            public string GestohlenesGut { get; set; }
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