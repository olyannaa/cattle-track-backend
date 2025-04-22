using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CAT.Controllers.DTO;
using CAT.EF.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.Postgres;
namespace CAT.EF;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<AnimalIdentification> AnimalIdentifications { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<IdentificationField> IdentificationFields { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermission> RolesPermissions { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Insemination> Inseminations { get; set; }

    public virtual DbSet<GroupType> GroupTypes { get; set; }
    public virtual DbSet<GroupRaw> GroupsRaw { get; set; }
    public virtual DbSet<Calving> Calvings { get; set; }
    public virtual DbSet<Pregnancy> Pregnancies { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("animals_pkey");

            entity.ToTable("animals");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BirthDate)
                .HasMaxLength(50)
                .HasColumnName("birth_date");
            entity.Property(e => e.Breed).HasColumnName("breed");
            entity.Property(e => e.FatherId).HasColumnName("father_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.MotherId).HasColumnName("mother_id");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Origin).HasColumnName("origin");
            entity.Property(e => e.OriginLocation).HasColumnName("origin_location");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TagNumber).HasColumnName("tag_number");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Group).WithMany(p => p.Animals)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("animals_group_id_fkey");

            entity.HasOne(d => d.Organization).WithMany(p => p.Animals)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("animals_organization_id_fkey");
        });

        modelBuilder.Entity<AnimalIdentification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("animal_identification_pkey");

            entity.ToTable("animal_identification");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AnimalId).HasColumnName("animal_id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Animal).WithMany(p => p.AnimalIdentifications)
                .HasForeignKey(d => d.AnimalId)
                .HasConstraintName("animal_identification_animal_id_fkey");

            entity.HasOne(d => d.Field).WithMany(p => p.AnimalIdentifications)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("animal_identification_field_id_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Organization).WithMany(p => p.Groups)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("groups_organization_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Groups)
                 .HasForeignKey(d => d.TypeId)
                 .HasConstraintName("fk_group_type");
        });

        modelBuilder.Entity<IdentificationField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("identification_fields_pkey");

            entity.ToTable("identification_fields");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FieldName).HasColumnName("field_name");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");

            entity.HasOne(d => d.Organization).WithMany(p => p.IdentificationFields)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("identification_fields_organization_id_fkey");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("organizations_pkey");

            entity.ToTable("organizations");

            entity.HasIndex(e => e.Name, "organizations_name_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Permission1, "permissions_permission_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Permission1).HasColumnName("permission");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Role1, "roles_role_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Role1).HasColumnName("role");
        });

        modelBuilder.Entity<RolesPermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("roles_permissions");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany()
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_permissions_role_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("users_organization_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");
        });
        modelBuilder.Entity<GroupType>()
            .Property(e => e.Id)
            .HasColumnName("id");

        modelBuilder.Entity<GroupType>()
            .Property(e => e.Name)
            .HasColumnName("name");
        modelBuilder.Entity<GroupRaw>().HasNoKey().ToView(null);

        OnModelCreatingPartial(modelBuilder);
    }

    public string? GetUserInfo(string login, string hashedPass)
    {
        return Database.SqlQuery<string>($"SELECT get_user_info({login},{hashedPass}) AS \"Value\"").SingleOrDefault();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    public IQueryable<IdentificationInfoDTO>? GetOrgIdentifications(Guid org_id)
        => IdentificationFields.FromSqlRaw(@"SELECT * FROM get_identification_fields({0})", org_id)
                                .Select(x => new IdentificationInfoDTO { Id = x.Id, Name = x.FieldName });
    public IQueryable<Group>? GetOrgGroups(Guid org_id)
        => Groups.FromSqlRaw(@"SELECT * FROM get_groups({0})", org_id);
    public void InsertAnimal(Animal animal)
        => Database.ExecuteSqlInterpolated($@"SELECT insert_animal(
                                       {animal.Id}, {animal.OrganizationId}, {animal.TagNumber},
                                       {animal.BirthDate}, {animal.Type},
                                       {animal.Breed}, {animal.MotherId}, {animal.FatherId}, {animal.Status},
                                       {animal.GroupId}, {animal.Origin}, {animal.OriginLocation}

    public void InsertAnimalIdentification(Guid id, Guid fieldName, string fieldValue)
        => Database.ExecuteSqlInterpolated($@"SELECT insert_animal_identification({id}, {fieldName}, {fieldValue})");

    public void IfNetelInsertReproduction(Guid animalId, DateOnly? inseminationDate,
                                          DateOnly? expectedCalvingDate, string inseminationType,
                                          string spermBatch, string technician, string notes)
        => Database.ExecuteSqlInterpolated($@"SELECT if_netel_insert_insemination_and_pregnancy({animalId},
                                {inseminationDate}, {expectedCalvingDate}, {inseminationType},
                                {spermBatch}, {technician}, {notes}, {"Подлежит проверке"})");

    public void AddIdentificationField(string fieldName, Guid organizationId)
        => Database.ExecuteSqlInterpolated($@"SELECT add_identification_field({fieldName}, {organizationId})");
    public void DeleteIdentification(Guid identificationId)
        => Database.ExecuteSqlInterpolated($@"SELECT delete_identification_field({identificationId})");

    public void AddGroupType(Guid organizationId, string name)
        => Database.ExecuteSqlInterpolated($@"SELECT add_group_type({name}, {organizationId})");

    public IQueryable<GroupType> GetGroupTypes(Guid organizationId)
        => GroupTypes.FromSqlRaw(@"SELECT * FROM get_group_types_by_organization({0})", organizationId);

    public void DeleteGroupType(Guid typeId)
        => Database.ExecuteSqlInterpolated($@"SELECT delete_group_type({typeId})");

    public void AddGroup(Guid organizationId, string name, Guid? typeId, string? description = "", string? location = "")
        => Database.ExecuteSqlInterpolated($@"SELECT add_group({organizationId}, {name}, {typeId}, {description}, {location})");

    public IQueryable<GroupRaw> GetGroupsByOrganization(Guid organizationId)
       => GroupsRaw.FromSqlRaw(@"SELECT * FROM get_groups_by_organization({0})", organizationId);

    public void DeleteGroup(Guid groupId)
        => Database.ExecuteSqlInterpolated($@"SELECT delete_group({groupId})");

    public void EditGroup(Guid groupId, Guid organizationId, string groupName, Guid? typeId, string? description = "", string? location = "")
        => Database.ExecuteSqlInterpolated($@"SELECT update_group({groupId},{organizationId}, {groupName}, {typeId}, {description}, {location})");

}