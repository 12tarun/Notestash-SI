//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotestashUserDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblDislike
    {
        public int Id { get; set; }
        public string DislikedNotes { get; set; }
        public int UserId { get; set; }
    
        public virtual tblUser tblUser { get; set; }
    }
}
