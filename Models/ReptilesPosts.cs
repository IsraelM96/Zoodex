//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zoodex.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReptilesPosts
    {
        public int PKReptilePostID { get; set; }
        public int FKReptileID { get; set; }
        public int FKPostID { get; set; }
        public int FKUpdatedBy { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public bool Deleted { get; set; }
    
        public virtual Posts Posts { get; set; }
        public virtual Reptiles Reptiles { get; set; }
    }
}
