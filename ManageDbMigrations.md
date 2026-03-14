### SqlServer Add Migration
Add-Migration MigrationName -Context SqlServerDbContext  -OutputDir MigrationsSqlServer

### Postgre Add Migration
Add-Migration MigrationName -Context PostgreSqlDbContext -OutputDir MigrationsPostgreSQL

### SqlServer Update Database
Update-Database -Context SqlServerDbContext

### Postgre Update Database
Update-Database -Context PostgreSqlDbContext

### Change environment variable and update database

# Environment variables: VPSProd, LocalProdProfile, DevProfile

$env:ASPNETCORE_ENVIRONMENT="VPSProd"
Update-Database -Context SqlServerDbContext