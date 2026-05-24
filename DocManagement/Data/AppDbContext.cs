using DocManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocManagement.Data
{
    public class AppDbContext : IdentityDbContext // 👈 burada dəyişiklik
    {

        // View üçün

        // Dynamic columns
        public DbSet<DocElement> DocElements { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppSetting> AppSettings { get; set; }


        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentListView> vw_DocumentList { get; set; }


        public DbSet<Zone> Zones { get; set; }
        public DbSet<Equipment> Equipments { get; set; }



        public DbSet<DocumentStatus> DocumentStatus { get; set; }

        public DbSet<StatusColor> StatusColors { get; set; }

        public DbSet<DocumentAuditViewModel> DocumentAuditView { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<DocumentItem> DocumentItems { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryType> InventoryTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔴 ÇOX VACİB – ƏVVƏL BASE
            base.OnModelCreating(modelBuilder);

            // 🔥 DOCUMENTS CƏDVƏLİNDƏ TRİGGER VAR DEYƏ EF-Ə DE
            modelBuilder.Entity<Document>()
                .ToTable("Documents", tb =>
                {
                    tb.HasTrigger("trg_Documents_Insert");
                    tb.HasTrigger("trg_Documents_Update");
                });

            modelBuilder.Entity<DocumentAuditViewModel>()
                .HasNoKey()
                .ToView("vw_X_Documents_Audit");

            modelBuilder.Entity<DocumentListView>()
                .HasNoKey()
                .ToView("vw_DocumentList");
        }
























    }
}
