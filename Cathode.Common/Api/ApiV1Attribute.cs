using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Cathode.Common.Api
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [MeansImplicitUse]
    public class ApiV1Attribute : ControllerAttribute, IApiBehaviorMetadata, IRouteTemplateProvider, IApiVersionProvider
    {
        public string? Name => null;

        public int? Order => null;

        public string Template { get; }

        public ApiVersionProviderOptions Options => ApiVersionProviderOptions.None;
        public IReadOnlyList<ApiVersion> Versions { get; }

        public ApiV1Attribute(string template)
        {
            Template = "api/v{version:apiVersion}/" + template;
            Versions = new[] {new ApiVersion(1, 0)};
        }
    }
}