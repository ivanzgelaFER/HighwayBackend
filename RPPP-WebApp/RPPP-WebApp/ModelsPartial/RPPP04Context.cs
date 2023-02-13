using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RPPP_WebApp.Models
{
    public partial class RPPP04Context
    {
        /*
     public IQueryable<ObjektOdrzavanje> MDObjektOdrzavanje() =>
      FromExpression(() => MDObjektOdrzavanje());

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObjektOdrzavanje>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.HasDbFunction(typeof(RPPP04Context).GetMethod(nameof(MDObjektOdrzavanje), new[] { typeof(int) }))
                        .HasName("fn_NajveceKupnje");

        }
        */
    }
}