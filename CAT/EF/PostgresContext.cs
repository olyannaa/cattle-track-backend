using System;
using System.Collections.Generic;
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

    public virtual DbSet<DailyAction> DailyActions { get; set; }

    public virtual DbSet<Research> Researches { get; set; }

    public virtual DbSet<GroupType> GroupTypes { get; set; }

    public virtual DbSet<GroupRaw> GroupsRaw { get; set; }
    public virtual DbSet<CowInseminationDTO> CowInseminations { get; set; }
    public virtual DbSet<Calving> Calvings { get; set; }
    public virtual DbSet<Weight> Weights { get; set; }
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
        modelBuilder.Entity<CowInseminationDTO>().HasNoKey().ToView(null);
        modelBuilder.Entity<CowInseminationDTO>(entity =>
        {
            entity.HasNoKey(); // Если это DTO без первичного ключа
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.CowId).HasColumnName("cow_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.InseminationType).HasColumnName("insemination_type");
            entity.Property(e => e.InseminationDate).HasColumnName("insemination_date");
            entity.Property(e => e.BullId).HasColumnName("bull_id");
        });
    OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public string? GetUserInfo(string login, string hashedPass)
    {
        return Database.SqlQuery<string>($"SELECT get_user_info({login},{hashedPass}) AS \"Value\"").SingleOrDefault();
    }

    public IEnumerable<IGrouping<Guid, AnimalCensus>> GetAnimalsWithIFByOrg(Guid organizationId, string? type = default, CensusSortInfoDTO? sort = default)
    {
        var query = Database.SqlQuery<AnimalCensus>($"SELECT * FROM get_animals_with_if_by_organization({organizationId})");

        if (type != null) query = query.Where(e => e.Type == type);

        if (sort is not null && sort.Active) query = query.Where(e => e.Status == "Активное");

        query = Sort(query, sort);

        return query.AsEnumerable().GroupBy(e => e.Id);
    }

    public IQueryable<ActiveAnimalDAL> GetAnimalsForActionsWithFilter(Guid organizationId, DailyAnimalsDTO dto)
    {
        var orgAnimals = Animals.Where(e => e.OrganizationId == organizationId);

        if (dto.Filter.IsActive ?? false)
            orgAnimals = orgAnimals.Where(e => e.Status == "Активное");

        if (dto.Filter.GroupId != null)
            orgAnimals = orgAnimals.Where(e => e.GroupId == dto.Filter.GroupId);

        if (dto.Filter.Type != null)
            orgAnimals = orgAnimals.Where(e => e.Type == dto.Filter.Type);

        if (dto.Filter.TagNumber != null)
            orgAnimals = orgAnimals.Where(e => e.TagNumber == dto.Filter.TagNumber);

        var field = dto.Filter.IdentificationField;
        if (field != null)
        {
            var animalIds = AnimalIdentifications.Where(e => e.FieldId == field.Id && e.Value == field.Value)
                                                .Select(e => e.AnimalId)
                                                .ToList();
            orgAnimals = orgAnimals.Where(e => animalIds.Contains(e.Id));
        }

        var result = orgAnimals.Include(e => e.Group)
                        .Select(e => new ActiveAnimalDAL
                        {
                            Id = e.Id,
                            TagNumber = e.TagNumber,
                            Type = e.Type,
                            Status = e.Status,
                            GroupId = e.GroupId,
                            GroupName = e.Group.Name
                        });

        return Sort(result, dto.SortInfo);
    }

    public IQueryable<dynamic>? GetDailyActionsWithPagination(Guid organizationId, string type,
        DailyActionsSortInfoDTO sort, int skip = default, int take = default)
    {
        return GetDailyActions(organizationId, type, sort)?.Skip(skip)?.Take(take);
    }

    public IQueryable<dynamic> GetDailyActions(Guid organizationId, string type, DailyActionsSortInfoDTO? sort = default)
    {
        IQueryable<dynamic> query;

        if (type == "Исследования")
            query = Database.SqlQuery<GetResearchDAL>($"SELECT * FROM get_research_by_organization({organizationId})");
        else
            query = Database.SqlQuery<GetActionsDAL>($"SELECT * FROM get_actions_by_organization_and_type({organizationId},{type})");

        query = Sort(query, sort);

        return query;
    }

    public int InsertDailyAction(Guid id, Guid animalId = default, string? actionType = default, string? actionSubtype = default,
        DateOnly? date = default, string? performedBy = default, string? result = default, string? medicine = default,
        string? dose = default, string? notes = default, DateOnly? nextActionDate = default, Guid? oldGroupId = default, Guid? newGroupId = default)
    {
        return Database.ExecuteSqlInterpolated($@"SELECT insert_daily_action({id},{animalId},{actionType},{actionSubtype},{date},
            {performedBy},{result},{medicine},{dose},{notes},{nextActionDate},{oldGroupId},{newGroupId})");
    }

    public int InsertResearch(Guid id, Guid orgId, Guid animalId = default, string? name = default, string? materialType = default,
        DateOnly? collectionDate = default, string? collectedBy = default, string? result = default, string? notes = default)
    {
        return Database.ExecuteSqlInterpolated($@"SELECT insert_research({id},{orgId},{animalId},{name},
            {materialType},{collectionDate},{collectedBy},{result},{notes})");
    }

    public int UpdateAnimal(Guid id, string? tag = default, string? type = default, string? breed = default, Guid? motherId = default,
        Guid? fatherId = default, string? status = default, Guid? groupId = default, string? origin = default, string? originLoc = default,
        DateOnly? birthDate = default, DateOnly? dateOfReceipt = default, DateOnly? dateOfDisposal = default, string? reasonOfDisposal = default,
        string? consumption = default, double? liveWeightAtDisposal = default, DateOnly? lastWeightDate = default,
        string? lastWeightWeight = default, string? identificationFieldName = default, string? identificationValue = default)
    {
        return Database.ExecuteSqlInterpolated($@"SELECT update_animal_data_with_if({id},{tag},{type},{breed},
            {motherId},{fatherId},{status},{groupId},{origin},{originLoc},{birthDate},{dateOfReceipt},{dateOfDisposal},
            {reasonOfDisposal},{consumption},{liveWeightAtDisposal},{lastWeightDate},{lastWeightWeight},{identificationFieldName},
            {identificationValue})");

    }

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
    public Guid InsertAnimalWithId(Animal animal)
    {
        Database.ExecuteSqlInterpolated($@"
        SELECT insert_animal_return_id(
            {animal.Id}, {animal.OrganizationId}, {animal.TagNumber},
            {animal.BirthDate}, {animal.Type},
            {animal.Breed}, {animal.MotherId}, {animal.FatherId}, {animal.Status},
            {animal.GroupId}, {animal.Origin}, {animal.OriginLocation})");

        return animal.Id;
    }
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

    public int GetAnimalsCountByOrganization(Guid organizationId, string type)
         => Database.SqlQuery<AnimalCensus>($"SELECT * FROM get_animals_by_org_and_type({organizationId}, {type})").Where(e => e.Status == "Активное").Count();


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

    public void InsertPregnancy(InsertPregnancyDTO pregnancy)
        => Database.ExecuteSqlInterpolated($@"
        SELECT insert_pregnancy(
            {pregnancy.CowId}, {pregnancy.Date}, {pregnancy.Status}, {pregnancy.ExpectedCalvingDate})");

    public void InsertCalving(CalvingDTO calving)
        => Database.ExecuteSqlInterpolated($@"
        SELECT insert_calving({calving.CowId}, {calving.Date}, {calving.Complication}, {calving.Type}, {calving.Veterinar},
            {calving.Treatments}, {calving.Pathology}, {calving.CalfId})");

    public int DeleteCalvingsByCow(Guid cowId)
        => Database.ExecuteSqlInterpolated($"SELECT delete_calvings_by_cow({cowId})");

    public int DeleteInseminationByCow(Guid cowId)
        => Database.ExecuteSqlInterpolated($"SELECT delete_insemination_by_cow({cowId})");

    public void DeletePregnancyByCow(Guid cowId)
        => Database.ExecuteSqlInterpolated($"SELECT delete_pregnancy_by_cow({cowId})");

    public IQueryable<CowInseminationDTO> GetPregnancyByOrganization(Guid organizationId)
        => CowInseminations
            .FromSqlRaw(@"SELECT * FROM get_pregnancy_by_organization({0})", organizationId);
    public Guid InsertCalving(InsertCalvingDTO dto, Guid calfId)
    {
        var connection = Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT insert_calving(@cowId, @date, @complication, @type, @veterinar, @treatments, @pathology, @calfId)";
        command.Parameters.Add(new NpgsqlParameter("cowId", dto.CowId));
        command.Parameters.Add(new NpgsqlParameter("date", dto.Date));
        command.Parameters.Add(new NpgsqlParameter("complication", dto.Complication));
        command.Parameters.Add(new NpgsqlParameter("type", dto.Type));
        command.Parameters.Add(new NpgsqlParameter("veterinar", dto.Veterinar ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("treatments", dto.Treatments ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("pathology", dto.Pathology ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("calfId", calfId));

        var newCalvingId = (Guid)command.ExecuteScalar();
        return newCalvingId;
    }

    public void InsertAnimalWeight(InsertAnimalWeightDTO dto)
        => Database.ExecuteSqlInterpolated($@"
            INSERT INTO weights (id, animal_id, date, weight, method, notes)
            VALUES ({dto.Id}, {dto.CalfId}, {dto.Date}, {dto.Weight}, {dto.Method}, {dto.Notes})");


    public int DeleteDailyAction(Guid actionId)
        => Database.ExecuteSqlInterpolated($@"SELECT delete_daily_action({actionId})");

    public int DeleteResearch(Guid researchId)
        => Database.ExecuteSqlInterpolated($@"SELECT delete_research({researchId})");

    public IQueryable<string?> GetIdentificationValues(Guid identificationId, Guid orgId, IdentificationValuesFilterDTO? filter = default)
    {
        var query = AnimalIdentifications.Include(e => e.Animal)
                                        .Where(e => e.Animal.OrganizationId == orgId)
                                        .Where(e => e.FieldId == identificationId);

        if (filter is not null)
        {
            if (filter.GroupId != null) query = query.Where(e => e.Animal.GroupId == filter.GroupId);
            if (filter.Type != null) query = query.Where(e => e.Animal.Type == filter.Type);
            if (filter.IsActive ?? false) query = query.Where(e => e.Animal.Status == "Активное");
        }
        return query.Select(e => e.Value).Where(e => e != String.Empty);
    }

    private IQueryable<T> Sort<T>(IQueryable<T> query, BaseSortInfoDTO? sort = default)
    {
        if (sort is not null && sort.Column is not null)
        {
            query = sort.Descending ? query.OrderByDescending(p => EntityFramework.Property<T>(p, sort.Column))
                                    : query.OrderBy(p => EntityFramework.Property<T>(p, sort.Column));
        }

        return query;
    }
}