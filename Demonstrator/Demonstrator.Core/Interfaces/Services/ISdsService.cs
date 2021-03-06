﻿using Demonstrator.Models.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services
{
    public interface ISdsService
    {
        Task<IEnumerable<SdsViewModel>> GetAll();

        Task<IEnumerable<SdsViewModel>> GetAllFromSource();

        SdsViewModel GetFor(string asid);

        SdsViewModel GetFor(string odsCode, string interactionId = null);
    }
}
