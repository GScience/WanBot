using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.EssentialPermission
{
    /// <summary>
    /// 权限异常
    /// </summary>
    public class PermissionException : Exception
    {
        public string RequirePermission { get; }
        public PermissionException(string requirePermission):base($"Require permission {requirePermission}")
        {
            RequirePermission = requirePermission;
        }
    }
}
