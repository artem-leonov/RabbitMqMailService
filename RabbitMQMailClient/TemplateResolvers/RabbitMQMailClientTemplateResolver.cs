﻿using System;
using System.IO;
using System.Web;
using RazorEngine.Templating;

namespace RabbitMQMailClient.TemplateResolvers
{
    public class RabbitMQMailClientTemplateResolver: ITemplateResolver
    {
        public string Resolve(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var path = HttpContext.Current.Server.MapPath(name);
            return File.ReadAllText(path, System.Text.Encoding.Default);
        }
    }
}