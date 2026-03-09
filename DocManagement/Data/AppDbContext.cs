using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DocManagement.Models;

namespace DocManagement.Data
{
    public class AppDbContext : IdentityDbContext // 👈 burada dəyişiklik
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        // 👇 MÜTLƏQ BU OLMALIDIR

        public DbSet<Zone> Zones { get; set; }
        public DbSet<Equipment> Equipments { get; set; }



        public DbSet<DocumentStatus> DocumentStatus { get; set; }

        public DbSet<StatusColor> StatusColors { get; set; }

        public DbSet<DocumentAuditViewModel> DocumentAuditView { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }



        public DbSet<DocumentListView> vw_DocumentList { get; set; }
        public DbSet<DocElement> DocElements { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔴 ÇOX VACİB – ƏVVƏL BASE
            base.OnModelCreating(modelBuilder);

            // 🔥 DOCUMENTS CƏDVƏLİNDƏ TRİGGER VAR DEYƏ EF-Ə DE
            modelBuilder.Entity<Document>()
                .ToTable(tb =>
                {
                    tb.HasTrigger("trg_Documents_Insert");
                    tb.HasTrigger("trg_Documents_Update");
                });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentAuditViewModel>()
                .HasNoKey()
                .ToView("vw_X_Documents_Audit");

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Document>()
            //    .HasOne(d => d.DocumentType)
            //    .WithMany(t => t.Documents)
            //    .HasForeignKey(d => d.DocumentTypeId)
            //    .OnDelete(DeleteBehavior.Restrict);




        }
























    }
}
