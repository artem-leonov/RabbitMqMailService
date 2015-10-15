using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace RabbitMQMailClient.TemplateBases
{
    public abstract class RabbitMQMailClientTemplateBase<T>: TemplateBase<T>
    {
        public string RenderPart(string templateName, object model = null)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Templates", templateName);
            return Razor.Parse(File.ReadAllText(path), model);
        }
    }
}