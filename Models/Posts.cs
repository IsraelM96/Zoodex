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
    
    public partial class Posts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Posts()
        {
            this.ReptilesPosts = new HashSet<ReptilesPosts>();
        }
    
        public int PKPostID { get; set; }
        public Nullable<int> FKPicture { get; set; }
        public int FKUpdatedBy { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public bool Deleted { get; set; }
    
        public virtual Files Files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReptilesPosts> ReptilesPosts { get; set; }
    }
}