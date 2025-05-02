using CAT.Controllers.DTO;
using CAT.EF.DAL;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using EntityFramework = Microsoft.EntityFrameworkCore.EF;
using NpgsqlTypes;
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

            entity.HasOne(d => d.Organization).WithMany(p => p.Groups)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("groups_organization_id_fkey");
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

    public IQueryable<AnimalCensus> GetAnimalsByOrgAndType(Guid organizationId, string type, CensusSortInfoDTO? sort)
    {
        var query = Database.SqlQuery<AnimalCensus>($"SELECT * FROM get_animals_by_org_and_type({organizationId},{type})");

        if (sort is not null && sort.Active) query = query.Where(e => e.Status == "Активное");
        
        if (sort is not null && sort.Column is not null)
        {
                query = sort.Descending ? query.OrderByDescending(p => EntityFramework.Property<AnimalCensus>(p, sort.Column))
                                        : query.OrderBy(p => EntityFramework.Property<AnimalCensus>(p, sort.Column));
        }
        else
        {
            query = query.OrderBy(e => e.TagNumber);
        }
        return query;
    }

    public IQueryable<AnimalCensus> GetAnimalsWithPagintaion(Guid organizationId, string type, CensusSortInfoDTO? sortInfo, int skip = default, int take = default)
    {

        return GetAnimalsByOrgAndType(organizationId, type, sortInfo).Skip(skip).Take(take);
    }

    public int UpdateAnimal(Guid id, string? tag, string? type, Guid? groupId, DateOnly? birthDate, string? status)
    {
        return Database.ExecuteSql($"SELECT update_animal({id},{tag},{type},{groupId},{birthDate},{status})");
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
                                       )");

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
    public Guid InsertAnimalToDatabase(Guid org_id, AnimalCSVInfoDTO animal,
                    (DateOnly? birthDate, DateOnly? dateOfReceipt, DateOnly? dateOfDisposal,
                     DateOnly? lastWeightDate, double? lastWeightAtDisposal) parsedData,
                    Guid? motherId, Guid? fatherId, string originLocation)
    {


        var parameters = new[]
         {
                new NpgsqlParameter("@p_organization_id", org_id),
                new NpgsqlParameter("@p_tag_number", animal.TagNumber),
                new NpgsqlParameter("@p_birth_date", parsedData.birthDate ?? (object)DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Date },
                new NpgsqlParameter("@p_type", animal.Type ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_breed", animal.Breed ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_mother_id", motherId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_father_id", fatherId ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_status", animal.Status),
                new NpgsqlParameter("@p_group_id", DBNull.Value),
                new NpgsqlParameter("@p_origin", string.Empty),
                new NpgsqlParameter("@p_origin_location", originLocation ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_consumption", animal.Сonsumption ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_date_of_receipt", parsedData.dateOfReceipt ?? (object)DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Date },
                new NpgsqlParameter("@p_date_of_disposal", parsedData.dateOfDisposal ?? (object)DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Date },
                new NpgsqlParameter("@p_last_weight_weight", animal.LastWeightWeight ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_live_weight_at_disposal", parsedData.lastWeightAtDisposal ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_last_weigh_date", parsedData.lastWeightDate ?? (object)DBNull.Value) { NpgsqlDbType = NpgsqlDbType.Date },
                new NpgsqlParameter("@p_reason_of_disposal", animal.ReasonOfDisposal ?? (object)DBNull.Value)
            };

        Database.ExecuteSqlRaw(@"SELECT FROM insert_animal_from_csv(
                @p_organization_id, @p_tag_number, @p_birth_date, @p_type, 
                @p_breed, @p_mother_id, @p_father_id, @p_status, 
                @p_group_id, @p_origin, @p_origin_location, @p_consumption, 
                @p_date_of_receipt, @p_date_of_disposal, @p_last_weight_weight, 
                @p_live_weight_at_disposal, @p_last_weigh_date, @p_reason_of_disposal)", parameters);
        var createdAnimal = Animals
            .FirstOrDefault(a => a.OrganizationId == org_id && a.TagNumber == animal.TagNumber);

        return createdAnimal.Id;
    }

    public IQueryable<CowDTO> GetCowsByOrganization(Guid organizationId)
        => Animals.FromSqlRaw(@"SELECT * FROM get_cows_by_organization({0})", organizationId)
        .Select(a => new CowDTO
        {
            Id = a.Id,
            OrganizationId = a.OrganizationId,
            TagNumber = a.TagNumber ?? string.Empty,
            Type = a.Type,
            BirthDate = a.BirthDate,
            Status = a.Status
        });

    public IQueryable<BullDTO> GetBullsByOrganization(Guid organizationId) 
        => Animals.FromSqlRaw(@"SELECT * FROM get_bulls_by_organization({0})", organizationId)
        .Select(a => new BullDTO
        {
            Id = a.Id,
            OrganizationId = a.OrganizationId,
            TagNumber = a.TagNumber ?? string.Empty,
            Type = a.Type,
            BirthDate = a.BirthDate,
            Status = a.Status
        });
    public void InsertInsemination(InseminationDTO insemination)
        => Database.ExecuteSqlInterpolated($@"
        SELECT insert_insemination({insemination.CowId},{insemination.Date},{insemination.InseminationType}, {insemination.SpermBatch},
            {insemination.SpermManufacturer}, {insemination.BullId}, {insemination.EmbryoId}, {insemination.EmbryoManufacturer},
            {insemination.Technician}, {insemination.Notes})");

    public void InsertPregnancy(PregnancyDTO pregnancy)
        => Database.ExecuteSqlInterpolated($@"
        SELECT insert_pregnancy(
            {pregnancy.CowId}, {pregnancy.Date}, {pregnancy.Status}, {pregnancy.ExpectedCalvingDate})");
}