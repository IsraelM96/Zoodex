using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zoodex.Controllers;
using Zoodex.Models;

namespace Zoodex.Functions
{
    public class RoleHandler
    {
        public static int GetRoleId(int? Id)
        {
            using (Zoodex_Entities Context = new Zoodex_Entities())
            {
                return (from a in Context.UsersRoles
                        join b in Context.Users
                        on a.FKUserID equals b.PKUserID
                        join c in Context.Roles
                        on a.FKRoleID equals c.PKRoleID
                        where a.Deleted == false &&
                              b.Deleted == false &&
                              c.Deleted == false &&
                              b.PKUserID == Id
                        select c.PKRoleID).FirstOrDefault();
            }
        }
        public static string GetRole(int? Id)
        {
            using (Zoodex_Entities Context = new Zoodex_Entities())
            {
                return (from a in Context.UsersRoles
                        join b in Context.Users
                        on a.FKUserID equals b.PKUserID
                        join c in Context.Roles
                        on a.FKRoleID equals c.PKRoleID
                        where a.Deleted == false &&
                              b.Deleted == false &&
                              c.Deleted == false &&
                              b.PKUserID == Id
                        select c.RoleName).FirstOrDefault();
            }
        }
    }
}