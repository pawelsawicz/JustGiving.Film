using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Conventions;

namespace JustGiving.Film
{
    public class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            base.ConfigureConventions(conventions);
        }
    }
}