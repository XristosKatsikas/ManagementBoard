﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;

namespace BoardProject.API
{
    public static class ApiConvention
    {
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Get([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
                                ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object id)
        {
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void GetById([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
                                    ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object id)
        {
        }

        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Create([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any),
                                   ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object model)
        {
        }


        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Update([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
                                   ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object id, [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any),
                        ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object model)
        {
        }


        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Delete([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix),
                                   ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)]
            object id)
        {
        }
    }
}
