﻿using NRLS_API.Core.Enums;
using NRLS_API.Models.Core;
using System;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IJwtHelper
    {
        Response IsValid(string jwt, Tuple<JwtScopes, string> reqScope, DateTime? tokenIssued = null);
    }
}
